using System;
using System.IO;
using System.Threading;
using Agent.Cli.Options;
using Agent.Cli.Utils;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Pipelines.Yaml;

namespace Agent.Cli
{
    public class ValidationManager
    {
        private readonly ValidateOptions _opts;
        private readonly InjectionHostContext _hostContext;
        private readonly PipelineParser m_pipelineParser;
        private readonly ParseOptions m_parseOptions;
        private readonly YamlFileProvider m_fileProvider;
        private readonly YamlTraceWriter m_traceWriter;

        public ValidationManager(ValidateOptions opts)
        {
            _opts = opts;

            var fileContent = FileUtils.LoadYaml(_opts.YamlPath);
            var fileName = FileUtils.GetName(_opts.YamlPath);
            var filePath = FileUtils.GetDirectory(_opts.YamlPath);
            
            _hostContext = new InjectionHostContext("windows");

            m_traceWriter = new YamlTraceWriter(_hostContext);
            m_fileProvider = new YamlFileProvider();

            m_fileProvider.FileContent[Path.Combine(filePath,fileName)]= fileContent;

            m_parseOptions = new ParseOptions()
            {
                MaxFiles = 10,
                MustacheEvaluationMaxResultLength = 512 * 1024, // 512k string length
                MustacheEvaluationTimeout = TimeSpan.FromSeconds(10),
                MustacheMaxDepth = 5,
            };

            m_pipelineParser = new PipelineParser(
                m_traceWriter,
                m_fileProvider,
                m_parseOptions
            );
        }

        public void Run()
        {
            try
            {
                var fileName = FileUtils.GetName(_opts.YamlPath);
                var filePath = FileUtils.GetDirectory(_opts.YamlPath);

                m_pipelineParser.DeserializeAndSerialize(filePath, fileName, null, CancellationToken.None);
                m_traceWriter.Info($"Passed validation");
            }
            catch (Exception ex)
            {
                m_traceWriter.Info($"Failed validation: {ex}");
            }
        }
    }
}