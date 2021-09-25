using System;
using Microsoft.VisualStudio.Services.Agent;

namespace Agent.Cli
{
    public class InjectionHostContext : HostContext
    {
        public InjectionHostContext(string hostType, string logFile = null) 
            : base(hostType, logFile)
        { }

        public void RegisterService<TInterface, TImplementation>()
            where TInterface : IAgentService
            where TImplementation : IAgentService, TInterface
        {
            ServiceTypes.TryAdd(typeof(TInterface), typeof(TImplementation));
        }
    }
}
