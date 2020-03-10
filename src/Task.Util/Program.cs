// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Agent.Sdk;
using CommandLine;
using Microsoft.VisualStudio.Services.Agent;
using Microsoft.VisualStudio.Services.Agent.Util;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Task.Util
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            // We can't use the new SocketsHttpHandler for now for both Windows and Linux
            // On linux, Negotiate auth is not working if the TFS url is behind Https
            // On windows, Proxy is not working
            AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);
            using (HostContext context = new HostContext("Agent"))
            {
                return MainAsync(context, args).GetAwaiter().GetResult();
            }
        }

        public async static Task<int> MainAsync(IHostContext context, string[] args)
        {
            Tracing trace = context.GetTrace(nameof(Program));
            trace.Info($"Agent package {BuildConstants.AgentPackage.PackageName}.");
            trace.Info($"Running on {PlatformUtil.HostOS} ({PlatformUtil.HostArchitecture}).");
            trace.Info($"RuntimeInformation: {RuntimeInformation.OSDescription}.");
            var terminal = context.GetService<ITerminal>();

            try
            {
                trace.Info($"Version: {BuildConstants.AgentPackage.Version}");
                trace.Info($"Commit: {BuildConstants.Source.CommitHash}");
                trace.Info($"Culture: {CultureInfo.CurrentCulture.Name}");
                trace.Info($"UI Culture: {CultureInfo.CurrentUICulture.Name}");

                bool success = Parser.Default.ParseArguments<UploadCommand, GenerateCommand>(args)
                    .MapResult(
                      (UploadCommand opts) => RunUploadCommand(context, opts).Result,
                      (GenerateCommand opts) => RunGenerateCommand(context, opts),
                      errors => false
                    );

                if (!success)
                {
                    return Constants.Agent.ReturnCode.TerminatedError;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.ToString());

                trace.Error(e);
                return Constants.Agent.ReturnCode.RetryableError;
            }

            return Constants.Agent.ReturnCode.Success;
        }

        private static async Task<bool> RunUploadCommand(IHostContext context, UploadCommand uploadArgs)
        {
            // create a connection
            VssBasicCredential basicCred = new VssBasicCredential("VstsAgent", uploadArgs.PAT);
            VssCredentials credentials = new VssCredentials(null, basicCred, CredentialPromptType.DoNotPrompt);
            var connection = VssUtil.CreateConnection(new Uri(uploadArgs.Server), credentials);

            // Get path
            // XORing the results here because one of these should be not null, but not both.
            if (!(String.IsNullOrEmpty(uploadArgs.ZipPath) ^ String.IsNullOrEmpty(uploadArgs.FolderPath)))
            {
                throw new Exception("You must specify either zip-path or folder-path but not both.");
            }

            bool signZip = !String.IsNullOrEmpty(uploadArgs.SigningThumprint);
            bool isZip = !String.IsNullOrEmpty(uploadArgs.ZipPath);
            bool success;

            var uploader = new Uploader(context, connection);

            if (isZip)
            {
                if (!File.Exists(uploadArgs.ZipPath))
                {
                    throw new Exception($"The file specified by zip-path {uploadArgs.ZipPath} does not exist.");
                }

                if (signZip)
                {
                    throw new Exception($"You cannot use the sign-zip option if the files are already zipped.");
                }

                success = await uploader.Upload(new Guid(uploadArgs.TaskId), uploadArgs.ZipPath, uploadArgs.Overwrite, context.AgentShutdownToken);
            }
            else
            {
                if (!Directory.Exists(uploadArgs.FolderPath))
                {
                    throw new Exception($"The folder specified by folder-path {uploadArgs.FolderPath} does not exist.");
                }

                success = await uploader.ZipAndUpload(uploadArgs.FolderPath, uploadArgs.Overwrite, signZip, uploadArgs.SigningThumprint, context.AgentShutdownToken);
            }

            return success;
        }

        private static bool RunGenerateCommand(IHostContext context, GenerateCommand generateArgs)
        {
            var generator = new Generator(context);
            Console.WriteLine($"Generating a task into {generateArgs.FolderPath}");
            generator.GenerateTaskFiles(generateArgs.FolderPath, generateArgs.TaskId, generateArgs.TaskName, generateArgs.Overwrite, context.AgentShutdownToken);
            Console.WriteLine($"Done generating files.");
            Console.WriteLine($"Recommeneded next steps...");
            Console.WriteLine($"   cd {generateArgs.FolderPath}");
            Console.WriteLine($"   npm install");
            Console.WriteLine($"   tsc *.ts");
            Console.WriteLine();

            return true;
        }
    }
}
