using CommandLine;

namespace Agent.Cli.Options
{
    public abstract class CommonOptions
    {
        [Option('f', "file", HelpText = "Yaml File Path" , Required = true)]
        public string YamlPath { get; set; }
    }
}