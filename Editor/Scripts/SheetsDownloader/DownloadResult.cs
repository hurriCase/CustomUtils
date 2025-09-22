using JetBrains.Annotations;

namespace CustomUtils.Editor.Scripts.SheetsDownloader
{
    /// <summary>
    /// Represents the result of a sheets download operation, containing information about
    /// the number of sheets downloaded and a descriptive message.
    /// </summary>
    [UsedImplicitly]
    public readonly struct DownloadResult
    {
        /// <summary>
        /// Gets the number of sheets that were successfully downloaded during the operation.
        /// </summary>
        /// <value>The count of downloaded sheets. Returns 0 if no sheets were downloaded.</value>
        [UsedImplicitly]
        public int DownloadedCount { get; }

        /// <summary>
        /// Gets a descriptive message about the download operation result.
        /// </summary>
        /// <value>A human-readable message describing the outcome of the download operation.</value>
        [UsedImplicitly]
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadResult"/> struct.
        /// </summary>
        /// <param name="downloadedCount">The number of sheets that were downloaded.</param>
        /// <param name="message">A descriptive message about the download operation.</param>
        [UsedImplicitly]
        public DownloadResult(int downloadedCount, string message)
        {
            DownloadedCount = downloadedCount;
            Message = message;
        }

        /// <summary>
        /// Gets a value indicating whether any sheets were downloaded during the operation.
        /// </summary>
        /// <value><c>true</c> if one or more sheets were downloaded; otherwise, <c>false</c>.</value>
        [UsedImplicitly]
        public bool HasDownloads => DownloadedCount > 0;
    }
}