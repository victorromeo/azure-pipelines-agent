using System;
using Agent.Cli.Options;
using CommandLine;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml;

namespace Agent.Cli
{
    public class Program
    {
        static void Main(string[] args)
        {
            var exitCode = Parser.Default.ParseArguments<ValidateOptions, RunOptions>(args)
                .MapResult(
                    (ValidateOptions opts) => Validate(opts),
                    (RunOptions opts) => Run(opts),
                    errs=> 1
                );

            Environment.ExitCode = exitCode;
        }

        public static int Validate(ValidateOptions opts)
        {
            var validationManager = new ValidationManager(opts);
            validationManager.Run();

            return 0;
        }

        public static int Run(RunOptions opts)
        {
            var pipelineManager = new PipelineManager(opts);
            pipelineManager.Run();
            return 0;
        }
    }
}
