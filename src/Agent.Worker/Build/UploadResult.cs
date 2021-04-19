// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.VisualStudio.Services.Agent.Util;
using Microsoft.VisualStudio.Services.BlobStore.Common.Telemetry;

namespace Microsoft.VisualStudio.Services.Agent.Worker.Build
{
    public class UploadResult
    {
        public UploadResult()
        {
            FailedFiles = new List<string>();
            TotalFileSizeUploaded = 0;
        }

        public UploadResult(List<string> failedFiles, long totalFileSizeUploaded)
        {
            FailedFiles = failedFiles;
            TotalFileSizeUploaded = totalFileSizeUploaded;
        }
        public List<string> FailedFiles { get; set; }

        public long TotalFileSizeUploaded { get; set; }

        // Blobstore data
        public long ChunksUploaded { get; private set; }
        public long CompressionBytesSaved { get; private set; }
        public long DedupUploadBytesSaved { get; private set; }
        public long LogicalContentBytesUploaded { get; private set; }
        public long PhysicalContentBytesUploaded { get; private set; }
        public long TotalNumberOfChunks { get; private set; }

        public void AddDedupStats(DedupUploadStatistics uploadStats)
        {
            this.ChunksUploaded += uploadStats.ChunksUploaded;
            this.CompressionBytesSaved += uploadStats.CompressionBytesSaved;
            this.DedupUploadBytesSaved += uploadStats.DedupUploadBytesSaved;
            this.LogicalContentBytesUploaded += uploadStats.LogicalContentBytesUploaded;
            this.PhysicalContentBytesUploaded += uploadStats.PhysicalContentBytesUploaded;
            this.TotalNumberOfChunks += uploadStats.TotalNumberOfChunks;
        }

        public DedupUploadStatistics GetDedupUploadStatistics()
        {
            return new DedupUploadStatistics(this.ChunksUploaded, this.CompressionBytesSaved, this.DedupUploadBytesSaved, this.LogicalContentBytesUploaded, this.PhysicalContentBytesUploaded, this.TotalNumberOfChunks);
        }

        public void AddUploadResult(UploadResult resultToAdd)
        {
            ArgUtil.NotNull(resultToAdd, nameof(resultToAdd));

            this.FailedFiles.AddRange(resultToAdd.FailedFiles);
            this.TotalFileSizeUploaded += resultToAdd.TotalFileSizeUploaded;

            this.ChunksUploaded += resultToAdd.ChunksUploaded;
            this.CompressionBytesSaved += resultToAdd.CompressionBytesSaved;
            this.DedupUploadBytesSaved += resultToAdd.DedupUploadBytesSaved;
            this.LogicalContentBytesUploaded += resultToAdd.LogicalContentBytesUploaded;
            this.PhysicalContentBytesUploaded += resultToAdd.PhysicalContentBytesUploaded;
            this.TotalNumberOfChunks += resultToAdd.TotalNumberOfChunks;
        }
    }
}
