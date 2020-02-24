using CommandLine;
using Microsoft.VisualStudio.Services.Agent;
using System;
using System.Collections.Generic;
using System.Text;

namespace Agent.Listener.CommandLine
{
    // Default Non-Requried Verb
    [Verb(Constants.Agent.CommandLine.Commands.Run)]
    public class RunAgent : BaseCommand
    {
        [Option(Constants.Agent.CommandLine.Flags.Diagnostics)]
        public bool Diagnostics { get; set; }

    }
}
