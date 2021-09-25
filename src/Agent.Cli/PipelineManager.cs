using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Agent.Cli.Options;
using Agent.Cli.Utils;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Pipelines.Yaml;
using Microsoft.VisualStudio.Services.Agent.Worker;
using Microsoft.VisualStudio.Services.Agent.Worker.Handlers;

namespace Agent.Cli
{
    public class PipelineManager
    {
        private readonly RunOptions _opts;
        private readonly InjectionHostContext _hostContext;
        private readonly LocalPipelineParser m_pipelineParser;
        private readonly ParseOptions m_parseOptions;
        private readonly YamlFileProvider m_fileProvider;
        private readonly YamlTraceWriter m_traceWriter;

        public PipelineManager(RunOptions opts)
        {
            _opts = opts;

            var fileContent = FileUtils.LoadYaml(_opts.YamlPath);
            var fileName = FileUtils.GetName(_opts.YamlPath);
            var filePath = FileUtils.GetDirectory(_opts.YamlPath);

            _hostContext = new InjectionHostContext("windows");

            m_traceWriter = new YamlTraceWriter(_hostContext);
            m_fileProvider = new YamlFileProvider();

            m_fileProvider.FileContent[Path.Combine(filePath, fileName)] = fileContent;

            m_parseOptions = new ParseOptions()
            {
                MaxFiles = 10,
                MustacheEvaluationMaxResultLength = 512 * 1024, // 512k string length
                MustacheEvaluationTimeout = TimeSpan.FromSeconds(10),
                MustacheMaxDepth = 5,
            };

            m_pipelineParser = new LocalPipelineParser(
                m_traceWriter,
                m_fileProvider,
                m_parseOptions
            );
        }

        public void Run()
        {
            m_traceWriter.Info("Starting Pipeline");

            IWorker worker = new Worker();
            worker.Initialize(_hostContext);

            try
            {
                var fileName = FileUtils.GetName(_opts.YamlPath);
                var filePath = FileUtils.GetDirectory(_opts.YamlPath);

                var process = m_pipelineParser.Deserialize(filePath, fileName, null, CancellationToken.None);

                var stepHost = _hostContext.CreateService<IDefaultStepHost>();
                stepHost.Initialize(_hostContext);

                stepHost.ExecuteAsync(filePath, fileName, null, new Dictionary<string, string>(), true,
                    System.Text.Encoding.ASCII, true, true, CancellationToken.None);

                m_traceWriter.Info($"Passed validation");
            }
            catch (Exception ex)
            {
                m_traceWriter.Info($"Failed validation: {ex}");
            }


            worker.RunAsync("in", "out");
        }
    }
}