using CommandLine;
using Microsoft.VisualStudio.Services.Agent;

namespace Agent.Listener.CommandLine
{
    [Verb(Constants.Agent.CommandLine.Commands.Remove)]
    public class RemoveAgent : BaseCommand
    {
        [Option(Constants.Agent.CommandLine.Args.Auth)]
        public string Auth { get; set; }

        [Option(Constants.Agent.CommandLine.Flags.LaunchBrowser)]
        public bool LaunchBrowser { get; set; }

        [Option(Constants.Agent.CommandLine.Args.Password)]
        public string Password { get; set; }

        [Option(Constants.Agent.CommandLine.Args.Token)]
        public string Token { get; set; }

        [Option(Constants.Agent.CommandLine.Flags.Unattended)]
        public bool Unattended { get; set; }

        [Option(Constants.Agent.CommandLine.Args.UserName)]
        public string UserName { get; set; }
    }
}
