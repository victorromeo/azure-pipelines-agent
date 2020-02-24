using CommandLine;
using Microsoft.VisualStudio.Services.Agent;
using System;
using System.Collections.Generic;
using System.Text;

namespace Agent.Listener.CommandLine
{
    [Verb(Constants.Agent.CommandLine.Commands.Remove)]
    public class RemoveAgent
    {
        [Option(Constants.Agent.CommandLine.Flags.Help)]
        public bool Help { get; set; }

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
