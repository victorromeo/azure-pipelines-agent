using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CommandLine;
using Microsoft.VisualStudio.Services.Agent;

namespace Task.Util
{
    [Verb(Constants.TaskUtil.CommandLine.Commands.Generate)]
    public class GenerateCommand
    {
        [Option(Constants.TaskUtil.CommandLine.Args.FolderPath, Required = true)]
        public string FolderPath { get; set; }

        [Option(Constants.TaskUtil.CommandLine.Args.TaskId)]
        public string TaskId { get; set; }

        [Option(Constants.TaskUtil.CommandLine.Args.TaskName)]
        public string TaskName { get; set; }

        [Option(Constants.TaskUtil.CommandLine.Flags.Overwrite)]
        public bool Overwrite { get; set; }
    }

    public class Generator
    {
        public Generator(IHostContext hostContext)
        {
            _hostContext = hostContext;
            _trace = _hostContext.GetTrace(nameof(Uploader));
        }

        public void GenerateTaskFiles(string folderPath, string taskId, string taskName, bool overwrite, object agentShutdownToken)
        {
            if (Directory.Exists(folderPath))
            {
                if (!overwrite)
                {
                    throw new Exception($"The folder {folderPath} already exists.");
                }

                Directory.Delete(folderPath, true);
            }

            Directory.CreateDirectory(folderPath);
            Directory.SetCurrentDirectory(folderPath);

            // Create task.json
            taskId = String.IsNullOrEmpty(taskId) ? Guid.NewGuid().ToString() : taskId;
            taskName = String.IsNullOrEmpty(taskName) ? "MyNewTask" : taskName;
            string taskJson = 
                "{\n" +
                "    \"$schema\": \"https://raw.githubusercontent.com/Microsoft/azure-pipelines-task-lib/master/tasks.schema.json\",\n" +
                $"    \"id\": \"{taskId}\",\n" +
                $"    \"name\": \"{taskName}\",\n" +
                $"    \"friendlyName\": \"{taskName}\",\n" +
                "    \"description\": \"\",\n" +
                "    \"helpMarkDown\": \"\",\n" +
                "    \"category\": \"Utility\",\n" +
                "    \"author\": \"\",\n" +
                "    \"version\": {\n" +
                "        \"Major\": 0,\n" +
                "        \"Minor\": 1,\n" +
                "        \"Patch\": 0\n" +
                "    },\n" +
                $"    \"instanceNameFormat\": \"{taskName} $(samplestring)\",\n" +
                "    \"inputs\": [\n" +
                "        {\n" +
                "            \"name\": \"samplestring\",\n" +
                "            \"type\": \"string\",\n" +
                "            \"label\": \"Sample String\",\n" +
                "            \"defaultValue\": \"\",\n" +
                "            \"required\": true,\n" +
                "            \"helpMarkDown\": \"A sample string\"\n" +
                "        }\n" +
                "    ],\n" +
                "    \"execution\": {\n" +
                "        \"Node\": {\n" +
                "            \"target\": \"index.js\"\n" +
                "        }\n" +
                "    }\n" +
                "}\n";
            File.WriteAllText("task.json", taskJson);

            // Create package.json
            string packageJson =
                "{\n" +
                $"  \"name\": \"{taskName}\",\n" +
                "  \"version\": \"1.0.0\",\n" +
                "  \"description\": \"\",\n" +
                "  \"main\": \"index.js\",\n" +
                "  \"scripts\": {\n" +
                "    \"test\": \"test\"\n" +
                "  },\n" +
                "  \"author\": \"\",\n" +
                "  \"license\": \"ISC\",\n" +
                "  \"dependencies\": {\n" +
                "    \"azure-pipelines-task-lib\": \"^2.9.3\"\n" +
                "  },\n" +
                "  \"devDependencies\": {\n" +
                "    \"@types/node\": \"^13.7.7\",\n" +
                "    \"@types/q\": \"^1.5.2\"\n" +
                "  }\n" +
                "}\n";
            File.WriteAllText("package.json", packageJson);

            // Create the minimal typescript file
            string tsFile =
                "import tl = require('azure-pipelines-task-lib/task');\n" +
                "\n" +
                "function run() {\n" +
                "    try {\n" +
                "        const inputString: string | undefined = tl.getInput('samplestring', true);\n" +
                "        if (inputString == 'bad') {\n" +
                "            tl.setResult(tl.TaskResult.Failed, 'Bad input was given');\n" +
                "            return;\n" +
                "        }\n" +
                "        console.log('Hello', inputString);\n" +
                "    }\n" +
                "    catch (err) {\n" +
                "        tl.setResult(tl.TaskResult.Failed, err.message);\n" +
                "    }\n" +
                "};\n" +
                "\n" +
                "run();\n";
            File.WriteAllText("index.ts", tsFile);

            // Create nuspec file (for signing)
            string nuspecFile =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<package xmlns=\"http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd\">\n" +
                "    <metadata>\n" +
                "        <id>Task</id>\n" +
                "        <version>0.0.0</version>\n" +
                "        <description></description>\n" +
                "        <authors></authors>\n" +
                "    </metadata>\n" +
                "</package>\n";
            File.WriteAllText("task.nuspec", nuspecFile);
        }

        private IHostContext _hostContext;
        private Tracing _trace;
    }
}
