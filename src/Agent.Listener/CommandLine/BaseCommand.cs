using CommandLine;
using Microsoft.VisualStudio.Services.Agent;
using System;
using System.Collections.Generic;
using System.Text;

namespace Agent.Listener.CommandLine
{
    // Command can be run without a Verb
    public class BaseCommand
    {
        [Option(Constants.Agent.CommandLine.Flags.Commit)]
        public bool Commit { get; set; }

        [Option(Constants.Agent.CommandLine.Flags.Help)]
        public bool Help { get; set; }

        [Option(Constants.Agent.CommandLine.Flags.Once)]
        public bool RunOnce { get; set; }

        [Option(Constants.Agent.CommandLine.Flags.Version)]
        public bool Version { get; set; }
    }
}
