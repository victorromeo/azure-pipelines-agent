using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating;
using IFileProvider = Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Pipelines.Yaml.IFileProvider;

namespace Agent.Cli.Utils
{
    internal sealed class YamlFileProvider : IFileProvider
    {
        public Dictionary<String, String> FileContent => m_fileContent;

        public Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Pipelines.Yaml.FileData GetFile(String path)
        {
            return new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Pipelines.Yaml.FileData
            {
                Name = Path.GetFileName(path),
                Directory = Path.GetDirectoryName(path),
                Content = m_fileContent[path],
            };
        }

        public String ResolvePath(String defaultRoot, String path)
        {
            return Path.Combine(defaultRoot, path);
        }

        private readonly Dictionary<String, String> m_fileContent = new Dictionary<String, String>();
    }
}