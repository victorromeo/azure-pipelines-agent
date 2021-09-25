using System;
using System.IO;

namespace Agent.Cli.Utils
{
    public class FileUtils
    {
        public static string LoadYaml(string path)
        {
            var yamlInfo = new FileInfo(path);
            if (yamlInfo.Exists && yamlInfo.Extension.ToLower() == ".yml")
            {
                using var reader = yamlInfo.OpenText();
                return reader.ReadToEnd();
            }

            throw new ArgumentException($"Unable to load '{path}'", nameof(path));
        }

        public static string GetName(string path)
        {
            var pathInfo = new FileInfo(path);
            return pathInfo.Name;
        }

        public static string GetFullName(string path)
        {
            var pathInfo = new FileInfo(path);
            return pathInfo.FullName;
        }

        public static string GetDirectory(string path)
        {
            var pathInfo = new FileInfo(path);
            return pathInfo.DirectoryName;
        }
    }
}


