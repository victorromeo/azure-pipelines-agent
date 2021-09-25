using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.Services.Agent;

namespace Agent.Cli.Fakes
{
    public class FakeVstsAgentWebProxy : AgentService, IVstsAgentWebProxy
    {
        private IHostContext _hc;

        public override void Initialize(IHostContext context)
        {
            _hc = context;
        }

        public string ProxyAddress { get; }
        public string ProxyUsername { get; }
        public string ProxyPassword { get; }
        public List<string> ProxyBypassList { get; }
        public IWebProxy WebProxy { get; }
    }
}