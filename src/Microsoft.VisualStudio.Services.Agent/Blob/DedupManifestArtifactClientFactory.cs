// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using BuildXL.Cache.ContentStore.FileSystem;
using BuildXL.Cache.ContentStore.Interfaces.Logging;
using BuildXL.Cache.ContentStore.Interfaces.Sessions;
using BuildXL.Cache.ContentStore.Stores;
using BuildXL.Cache.ContentStore.Interfaces.Time;
using Microsoft.VisualStudio.Services.ArtifactServices.App.Shared.Cache;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Cache;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.BlobStore.Common.Telemetry;
using System.Threading.Tasks.Dataflow;
using System.Collections.Generic;
using BuildXL.Cache.ContentStore.Interfaces.Stores;
using BuildXL.Cache.ContentStore.Interfaces.FileSystem;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Agent.Blob
{
    [ServiceLocator(Default = typeof(DedupManifestArtifactClientFactory))]
    public interface IDedupManifestArtifactClientFactory
    {
        Task<(DedupManifestArtifactClient client, BlobStoreClientTelemetry telemetry)> CreateDedupManifestClientAsync(
            bool verbose,
            Action<string> traceOutput,
            VssConnection connection,
            CancellationToken cancellationToken);

            
        Task<(DedupStoreClient client, BlobStoreClientTelemetryTfs telemetry)> CreateDedupClientAsync(
            bool verbose,
            Action<string> traceOutput,
            VssConnection connection,
            CancellationToken cancellationToken);
    }

    public class DedupManifestArtifactClientFactory : IDedupManifestArtifactClientFactory
    {
        public static readonly DedupManifestArtifactClientFactory Instance = new DedupManifestArtifactClientFactory();

        private DedupManifestArtifactClientFactory()
        {
        }

        public async Task<(DedupManifestArtifactClient client, BlobStoreClientTelemetry telemetry)> CreateDedupManifestClientAsync(
            bool verbose,
            Action<string> traceOutput,
            VssConnection connection,
            CancellationToken cancellationToken)
        {
            const int maxRetries = 5;
            var tracer = CreateArtifactsTracer(verbose, traceOutput);
            IDedupStoreHttpClient dedupStoreHttpClient = await AsyncHttpRetryHelper.InvokeAsync(
                () =>
                {
                    ArtifactHttpClientFactory factory = new ArtifactHttpClientFactory(
                        connection.Credentials,
                        TimeSpan.FromSeconds(50),
                        tracer,
                        cancellationToken);

                    // this is actually a hidden network call to the location service:
                     return Task.FromResult(factory.CreateVssHttpClient<IDedupStoreHttpClient, DedupStoreHttpClient>(connection.GetClient<DedupStoreHttpClient>().BaseAddress));

                },
                maxRetries: maxRetries,
                tracer: tracer,
                canRetryDelegate: e => true,
                context: nameof(CreateDedupManifestClientAsync),
                cancellationToken: cancellationToken,
                continueOnCapturedContext: false);

            string cachePath = Environment.GetEnvironmentVariable("ADO_AGENT_ARTIFACT_CACHE_PATH");
            if (!string.IsNullOrEmpty(cachePath))
            {
                var cacheContext = new CacheContext(verbose ? Severity.Diagnostic : Severity.Warning);
                cacheContext.SetRootPath(cachePath);
                cacheContext.SetAppTraceSource(tracer);
                var cacheService = cacheContext.CreateCache();
                var cache = cacheService.GetCache(cachePath).Cache;
                dedupStoreHttpClient = new DedupStoreHttpClientWithCache(dedupStoreHttpClient, cache.ContentSession, cacheService.Context.Logger, cacheChunks: true, cacheNodes: true);
            }

            var telemetry = new BlobStoreClientTelemetry(tracer, dedupStoreHttpClient.BaseAddress);
            var client = new DedupStoreClientWithDataport(dedupStoreHttpClient, 192); // TODO
            return (new DedupManifestArtifactClient(telemetry, client, tracer), telemetry);
        }

        public class CacheContext : CacheContextBase
        {
            public CacheContext(CacheContext context) : base(context)
            {
            }

            public CacheContext(Severity minLogSeverity, IEnumerable<string> configPaths = null, string rootPath = null, bool useCacheAsService = false, string cacheName = null, int? maxCacheConnections = null, uint? sizeInMegabytes = null, FileRealizationMethod realizationMethod = FileRealizationMethod.HardLink, FileAccessMode localFileAccessMode = FileAccessMode.ReadOnly, int? cacheServiceGrpcPort = null)
             : base(minLogSeverity, configPaths, rootPath, useCacheAsService, cacheName, maxCacheConnections, sizeInMegabytes, realizationMethod, localFileAccessMode, cacheServiceGrpcPort)
            {
            }

            public override CacheContextBase Clone()
            {
                return new CacheContext(this);
            }

            protected override LocalCacheServiceBase CreateLocalCache(CacheContextBase context)
            {
                return new LocalCacheService(this);
            }
        }

        public class LocalCacheService : LocalCacheServiceBase
        {
            public LocalCacheService(CacheContextBase context) : base(context)
            {
            }

            protected override VolumeWideCacheBase CreateVolumeWideCache(CacheContextBase context)
            {
                return new VolumeWideCache(context);
            }
        }

        public class VolumeWideCache : VolumeWideCacheBase
        {
            internal VolumeWideCache(CacheContextBase context) : base(context)
            {
            }

            protected override IContentStore ConstructContentStore(CacheContextBase context)
            {

                IAbsFileSystem fileSystem = new PassThroughFileSystem(context.Logger);

                try
                {
                    if (context.UseCacheAsService)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {

                        // In process cache client

                        this.CacheRootPath = new AbsolutePath(Path.GetFullPath(context.RootPath));

                        // Start with defaults.
                        var cacheMaxSizeInMegabytes = DefaultCacheSizeInMegabytes;
                        bool? denyWriteAttributesOnContent = null;

                        if (context.ConfigPaths.Any())
                        {
                            if (context.ConfigPaths.Count() > 1)
                            {
                                context.Logger.Warning("Only parsing the first cache configuration file ({0}) of {1}.", context.ConfigPaths.First(), context.ConfigPaths.Count());
                            }

                            var fullPath = Path.GetFullPath(context.ConfigPaths.First());
                            var configPath = new AbsolutePath(fullPath);
                            var oldCacheConfiguration = ReadOldCacheConfiguration(fileSystem, configPath);

                            uint? configSizeInMegabytes = oldCacheConfiguration.Content?.FileSystem?.FileSystemL1?.SizeInMegabytes;
                            if (configSizeInMegabytes.HasValue)
                            {
                                cacheMaxSizeInMegabytes = configSizeInMegabytes.Value;
                            }

                            denyWriteAttributesOnContent =
                                oldCacheConfiguration.Content?.FileSystem?.FileSystemL1?.DenyWriteAttributesOnContent;
                        }

                        if (context.MaxSizeInMegabytes.HasValue)
                        {
                            cacheMaxSizeInMegabytes = context.MaxSizeInMegabytes.Value;
                        }

                        var denyWriteAttributesOnContentSetting = denyWriteAttributesOnContent.HasValue
                            ? denyWriteAttributesOnContent.Value
                                ? DenyWriteAttributesOnContentSetting.Enable
                                : DenyWriteAttributesOnContentSetting.Disable
                            // By default, ensure hardlinks work on new versions of Windows (Server 2019 version 18660+) and patched versions of Windows (Server 2016 with KB4540670)
                            : DenyWriteAttributesOnContentSetting.Disable;

                        return new FileSystemContentStore(
                            fileSystem,
                            SystemClock.Instance,
                            this.CacheRootPath,
                            new ConfigurationModel(
                                new ContentStoreConfiguration(new MaxSizeQuota($"{cacheMaxSizeInMegabytes}MB"),
                                    denyWriteAttributesOnContent: denyWriteAttributesOnContentSetting),
                                ConfigurationSelection.RequireAndUseInProcessConfiguration,
                                MissingConfigurationFileOption.DoNotWrite));
                    }
                }
                finally
                {
                    fileSystem?.Dispose();
                }
            }

            protected override Task PostAllAndComplete<T>(ITargetBlock<T> actionBlock, IEnumerable<T> inputs)
            {
                return BuildXL.Utilities.Collections.TargetBlockExtensions.PostAllAndComplete(actionBlock, inputs);
            }
        }

        public async Task<(DedupStoreClient client, BlobStoreClientTelemetryTfs telemetry)> CreateDedupClientAsync(
            bool verbose,
            Action<string> traceOutput,
            VssConnection connection,
            CancellationToken cancellationToken)
        {
            const int maxRetries = 5;
            var tracer = CreateArtifactsTracer(verbose, traceOutput);
            var dedupStoreHttpClient = await AsyncHttpRetryHelper.InvokeAsync(
                () =>
                {
                    ArtifactHttpClientFactory factory = new ArtifactHttpClientFactory(
                        connection.Credentials,
                        TimeSpan.FromSeconds(50),
                        tracer,
                        cancellationToken);

                    // this is actually a hidden network call to the location service:
                     return Task.FromResult(factory.CreateVssHttpClient<IDedupStoreHttpClient, DedupStoreHttpClient>(connection.GetClient<DedupStoreHttpClient>().BaseAddress));
                },
                maxRetries: maxRetries,
                tracer: tracer,
                canRetryDelegate: e => true,
                context: nameof(CreateDedupManifestClientAsync),
                cancellationToken: cancellationToken,
                continueOnCapturedContext: false);

            var telemetry = new BlobStoreClientTelemetryTfs(tracer, dedupStoreHttpClient.BaseAddress, connection);
            var client = new DedupStoreClient(dedupStoreHttpClient, 192); // TODO
            return (client, telemetry);
        }

        public static IAppTraceSource CreateArtifactsTracer(bool verbose, Action<string> traceOutput)
        {
            return new CallbackAppTraceSource(
                str => traceOutput(str),
                verbose
                    ? System.Diagnostics.SourceLevels.Verbose
                    : System.Diagnostics.SourceLevels.Information,
                includeSeverityLevel: verbose);
        }
    }
}