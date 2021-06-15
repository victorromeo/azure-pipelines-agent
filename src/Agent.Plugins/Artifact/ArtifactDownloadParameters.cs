// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Agent.Plugins
{
    internal class ArtifactDownloadParameters
    {
        /// <remarks>
        /// Options on how to retrieve the build using the following parameters.
        /// </remarks>
        public BuildArtifactRetrievalOptions ProjectRetrievalOptions { get; set; }
        /// <remarks>
        /// Either project ID or project name need to be supplied.
        /// </remarks>
        public Guid ProjectId { get; set; }
        /// <remarks>
        /// Either project ID or project name need to be supplied.
        /// </remarks>
        public string ProjectName { get; set; }
        public int PipelineId { get; set; }
        public string ArtifactName { get; set; }
        public string TargetDirectory { get; set; }
        public string[] MinimatchFilters { get; set; }
        public bool MinimatchFilterWithArtifactName { get; set; }
        public bool IncludeArtifactNameInPath { get; set; }

        public int ParallelizationLimit { get; set; } = 8;
        public int RetryDownloadCount { get; set; } = 4;
        public bool CheckDownloadedFiles { get; set; } = false;
        // Show if DwonloadBuildArtifactsTaskV1 called download method.
        // We need it sine this task should use another minimatch options
        public bool BuildArtifactsTask { get; set; } = false;
    }

    internal enum BuildArtifactRetrievalOptions
    {
        RetrieveByProjectId,
        RetrieveByProjectName
    }
}
