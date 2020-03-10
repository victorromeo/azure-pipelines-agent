using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.VisualStudio.Services.Agent;
using Microsoft.VisualStudio.Services.Agent.Util;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;

namespace Task.Util
{
    [Verb(Constants.TaskUtil.CommandLine.Commands.Upload)]
    public class UploadCommand
    {
        [Option(Constants.TaskUtil.CommandLine.Args.Server, Required = true)]
        public string Server { get; set; }

        [Option(Constants.TaskUtil.CommandLine.Args.FolderPath, Group = "path")]
        public string FolderPath { get; set; }

        [Option(Constants.TaskUtil.CommandLine.Args.ZipPath, Group = "path")]
        public string ZipPath { get; set; }

        [Option(Constants.TaskUtil.CommandLine.Args.PAT, Required = true)]
        public string PAT { get; set; }

        [Option(Constants.TaskUtil.CommandLine.Args.SigningThumprint)]
        public string SigningThumprint { get; set; }

        [Option(Constants.TaskUtil.CommandLine.Args.TaskId)]
        public string TaskId { get; set; }

        [Option(Constants.TaskUtil.CommandLine.Flags.Overwrite)]
        public bool Overwrite { get; set; }
    }

    public class Uploader
    {
        public Uploader(IHostContext hostContext, VssConnection connection)
        {
            _hostContext = hostContext;
            _trace = _hostContext.GetTrace(nameof(Uploader));
            _connection = connection;
        }

        public async Task<bool> Upload(Guid taskId, string zipFile, bool overwrite, CancellationToken cancellationToken)
        {
            var taskServer = _hostContext.GetService<ITaskServer>();
            await taskServer.ConnectAsync(_connection);

            try
            {
                _trace.Info($"Starting task upload: {taskId} {zipFile}");
                using (FileStream fs = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize: _defaultFileStreamBufferSize, useAsync: true))
                {
                    await taskServer.UploadTaskZipAsync(taskId, fs, overwrite, cancellationToken);
                }
                return true;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {

                _trace.Info($"Task upload has been cancelled.");
                throw;
            }
        }

        public async Task<bool> ZipAndUpload(string taskFolderPath, bool overwrite, bool signZip, string fingerprint, CancellationToken cancellationToken)
        {
            if (Directory.Exists(taskFolderPath))
            {
                // Read the task.json file to get the name and id
                var task = GetTaskJsonAttributes(Path.Combine(taskFolderPath, Constants.Path.TaskJsonFile));

                var zipFile = taskFolderPath + ".zip";
                
                if (File.Exists(zipFile))
                {
                    File.Delete(zipFile);
                }
                
                ZipFile.CreateFromDirectory(taskFolderPath, zipFile);

                if (signZip)
                {
                    var nugetFile = taskFolderPath + ".nupkg";
                    String nugetPath = WhichUtil.Which("nuget", require: true);

                    if (File.Exists(nugetFile))
                    {
                        File.Delete(nugetFile);
                    }

                    File.Move(zipFile, nugetFile);

                    var processInvoker = _hostContext.GetService<IProcessInvoker>();
                    processInvoker.ErrorDataReceived += (Object sender, ProcessDataReceivedEventArgs args) => { Console.Error.WriteLine(args.Data); };
                    processInvoker.OutputDataReceived += (Object sender, ProcessDataReceivedEventArgs args) => { Console.WriteLine(args.Data); };
                    int exitCode = await processInvoker.ExecuteAsync(Directory.GetCurrentDirectory(), nugetPath, $"sign \"{nugetFile}\" -CertificateFingerprint {fingerprint}", new Dictionary<string, string>(), cancellationToken);

                    if (exitCode != 0)
                    {
                        throw new Exception("Unable to sign the task after zipping it. Task not uploaded.");
                    }

                    File.Move(nugetFile, zipFile);
                }

                return await Upload(task.Id, zipFile, overwrite, cancellationToken);
            }

            return false;
        }

        private TaskJson GetTaskJsonAttributes(string filePath)
        {
            _trace.Info($"Loading task definition '{filePath}'.");
            string json = File.ReadAllText(filePath);
            var task = JsonConvert.DeserializeObject<TaskJson>(json);
            return task;
        }

        private IHostContext _hostContext;
        private Tracing _trace;
        private VssConnection _connection;
        private int _defaultFileStreamBufferSize = 4096;
    }

    public sealed class TaskJson
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
    }
}
