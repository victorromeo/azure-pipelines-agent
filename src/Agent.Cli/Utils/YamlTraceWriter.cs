using System;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Pipelines.Yaml;
using Microsoft.VisualStudio.Services.Agent;

namespace Agent.Cli.Utils
{
    internal sealed class YamlTraceWriter : ITraceWriter
    {
        public YamlTraceWriter(IHostContext hostContext)
        {
            m_trace = hostContext.GetTrace(nameof(YamlTraceWriter));
        }

        public void Info(String format, params Object[] args)
        {
            m_trace.Info(format, args);
        }

        public void Verbose(String format, params Object[] args)
        {
            m_trace.Verbose(format, args);
        }


        private readonly Tracing m_trace;
    }
}