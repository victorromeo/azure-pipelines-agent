using Microsoft.VisualStudio.Services.Agent;
using Microsoft.VisualStudio.Services.Common;

namespace Agent.Cli.Fakes
{
    public class FakeCertificateManager : AgentService, IAgentCertificateManager
    {
        public bool SkipServerCertificateValidation { get; }
        public string CACertificateFile { get; }
        public string ClientCertificateFile { get; }
        public string ClientCertificatePrivateKeyFile { get; }
        public string ClientCertificateArchiveFile { get; }
        public string ClientCertificatePassword { get; }
        public IVssClientCertificateManager VssClientCertificateManager { get; }
    }
}