using JetBrains.Annotations;

namespace CustomUtils.Runtime.ResponseTypes
{
    /// <summary>
    /// Result of a sheets download operation.
    /// </summary>
    [UsedImplicitly]
    public readonly struct DownloadResult
    {
        /// <summary>
        /// Number of sheets successfully downloaded.
        /// </summary>
        [UsedImplicitly]
        public int DownloadedCount { get; }

        /// <summary>
        /// Description of the download operation result.
        /// </summary>
        [UsedImplicitly]
        public string Message { get; }

        [UsedImplicitly]
        public DownloadResult(int downloadedCount, string message)
        {
            DownloadedCount = downloadedCount;
            Message = message;
        }

        /// <summary>
        /// True if any sheets were downloaded.
        /// </summary>
        [UsedImplicitly]
        public bool HasDownloads => DownloadedCount > 0;
    }
}