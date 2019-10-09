using System;
using System.Collections.Generic;
using System.IO;
using Agent.Sdk;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Pipelines = Microsoft.TeamFoundation.DistributedTask.Pipelines;

namespace Microsoft.VisualStudio.Services.Agent.Util
{
    public static class RepositoryUtil
    {
        public static bool HasMultipleCheckouts(Dictionary<string, string> jobSettings)
        {
            if (jobSettings != null && jobSettings.TryGetValue(WellKnownJobSettings.HasMultipleCheckouts, out string hasMultipleCheckoutsText))
            {
                return bool.TryParse(hasMultipleCheckoutsText, out bool hasMultipleCheckouts) && hasMultipleCheckouts;
            }

            return false;
        }

        public static string GetCloneDirectory(Pipelines.RepositoryResource repository)
        {
            ArgUtil.NotNull(repository, nameof(repository));

            string repoName =
                repository.Properties.Get<string>(RepositoryPropertyNames.Name) ??
                repository.Url?.AbsoluteUri ??
                repository.Alias;

            return GetCloneDirectory(repoName);
        }

        public static string GetCloneDirectory(string repoName)
        {
            ArgUtil.NotNullOrEmpty(repoName, nameof(repoName));

            int startPosition = repoName.TrimEnd('/').LastIndexOf('/');
            if (startPosition < 0)
            {
                startPosition = 0;
            }

            const string gitExtension = ".git";
            int endPosition = repoName.Length - 1;
            if (repoName.EndsWith(gitExtension, StringComparison.OrdinalIgnoreCase))
            {
                endPosition -= gitExtension.Length;
            }

            return repoName.Substring(startPosition, endPosition - startPosition + 1);
        }
    }
}