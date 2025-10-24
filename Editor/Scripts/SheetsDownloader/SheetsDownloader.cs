using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using CustomUtils.Runtime.Downloader;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace CustomUtils.Editor.Scripts.SheetsDownloader
{
    /// <summary>
    /// Handles downloading and managing Google Sheets data for a specified database type.
    /// Provides functionality to download sheets as CSV files and resolve sheet information
    /// from Google Sheets documents.
    /// </summary>
    /// <typeparam name="TDatabase">The type of sheet database that inherits from <see cref="SheetsDatabase{T, T}"/>
    /// .</typeparam>
    /// <typeparam name="TSheet">The type of sheets to use it for a database</typeparam>
    [UsedImplicitly]
    public sealed class SheetsDownloader<TDatabase, TSheet> where TDatabase : SheetsDatabase<TDatabase, TSheet>
        where TSheet : Sheet, new()
    {
        private readonly TDatabase _database;

        private const string UrlPattern = "https://docs.google.com/spreadsheets/d/{0}/export?format=csv&gid={1}";
        private const string SheetResolverUrl =
            "https://script.google.com/macros/s/" +
            "AKfycbycW2dsGZhc2xJh2Fs8yu9KUEqdM-ssOiK1AlES3crLqQa1lkDrI4mZgP7sJhmFlGAD/exec";

        /// <summary>
        /// Initializes a new instance of the <see cref="SheetsDownloader{T, T}"/> class.
        /// </summary>
        /// <param name="database">The database instance that contains sheet configuration
        /// and will store downloaded data.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="database"/> is null.</exception>
        [UsedImplicitly]
        public SheetsDownloader(TDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Resolves and updates the list of available sheets from a Google Sheets document.
        /// This method queries the Google Sheets document to get current sheet names and IDs,
        /// then updates the database's sheet collection accordingly.
        /// </summary>
        /// <returns>A <see cref="UniTask"/> representing the asynchronous resolve operation.</returns>
        /// <exception cref="Exception">Thrown when network errors occur, when the table is not found,
        /// when public read permission is not set, or when the response cannot be parsed.</exception>
        [UsedImplicitly]
        public async UniTask ResolveGoogleSheetsAsync()
        {
            var requestUrl = $"{SheetResolverUrl}?tableUrl={_database.TableId}";
            using var request = UnityWebRequest.Get(requestUrl);

            await request.SendWebRequest().ToUniTask();

            if (string.IsNullOrEmpty(request.error) is false)
            {
                Debug.LogError($"[SheetsDownloader::ResolveGoogleSheetsAsync] Network error: {request.error}");
                return;
            }

            ProcessResolveResponse(request);
        }

        /// <summary>
        /// Downloads all sheets that have changed since the last download operation.
        /// Only downloads sheets whose content length differs from the cached value.
        /// </summary>
        /// <returns>A <see cref="UniTask{DownloadResult}"/> representing the asynchronous download operation.
        /// The result contains information about how many sheets were downloaded and a status message.</returns>
        /// <exception cref="ArgumentException">Thrown when the database's TableId is null or empty.</exception>
        /// <exception cref="Exception">Thrown when network errors occur
        /// or when access to the Google Sheets document is denied.</exception>
        [UsedImplicitly]
        public async UniTask<DownloadResult> DownloadSheetsAsync()
        {
            Debug.Log($"[SheetsDownloader::DownloadSheetsAsync] Start downloading sheets ...");

            PrepareDownloadFolderIfNeeded();

            var changedCount = 0;
            var sheetsToDownload = new List<TSheet>();

            foreach (var sheet in _database.Sheets)
            {
                if (!sheet.TextAsset)
                {
                    sheetsToDownload.Add(sheet);
                    continue;
                }

                var contentLength = await GetSheetContentLengthAsync(sheet.Id);

                if (contentLength > 0 && sheet.HasChanged(contentLength))
                    sheetsToDownload.Add(sheet);
            }

            if (sheetsToDownload.Count == 0)
                return new DownloadResult(0, "All sheets are up to date!");

            foreach (var sheet in sheetsToDownload)
            {
                var result = await DownloadSingleSheetAsync(sheet);

                if (result.HasDownloads)
                    changedCount++;
            }

            var message = changedCount > 0
                ? $"{changedCount} changed sheets downloaded!"
                : "All sheets are up to date!";

            return new DownloadResult(changedCount, message);
        }

        /// <summary>
        /// Downloads a specific sheet regardless of whether it has changed.
        /// Forces download of the specified sheet and updates its content length.
        /// </summary>
        /// <param name="sheet">The sheet to download.</param>
        /// <returns>A <see cref="UniTask{DownloadResult}"/> representing the asynchronous download operation.
        /// The result contains information about the download and a status message.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sheet"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the database's TableId is null or empty.</exception>
        /// <exception cref="Exception">Thrown when network errors occur
        /// or when access to the Google Sheets document is denied.</exception>
        [UsedImplicitly]
        public async UniTask<DownloadResult> DownloadSingleSheetAsync(TSheet sheet)
        {
            if (sheet == null)
                return new DownloadResult(0, "Sheet is null");

            PrepareDownloadFolderIfNeeded();

            try
            {
                var url = ZString.Format(UrlPattern, _database.TableId, sheet.Id);

                using var request = UnityWebRequest.Get(url);
                await request.SendWebRequest().ToUniTask();

                var error = GetRequestError(request);
                if (string.IsNullOrEmpty(error) is false)
                {
                    var errorMessage = error.Contains("404") ? "Table Id is wrong!" : error;
                    return new DownloadResult(1, errorMessage);
                }

                var data = request.downloadHandler.data;
                await SaveSheetDataAsync(sheet, data);

                sheet.ContentLength = data.Length;

                return new DownloadResult(1, $"Sheet '{sheet.Name}' downloaded successfully!");
            }
            catch (Exception ex)
            {
                return new DownloadResult(0,
                    $"Failed to download sheet '{sheet.Name}': {ex.Message}");
            }
        }

        private void ProcessResolveResponse(UnityWebRequest request)
        {
            var error = ExtractInternalError(request);
            if (error != null)
            {
                Debug.LogError("[SheetsDownloader::ProcessResolveResponse] " +
                               $"Table not found or public read permission not set. With error: {error}");
                return;
            }

            var sheetsDict = JsonConvert.DeserializeObject<Dictionary<string, long>>(request.downloadHandler.text);

            if (sheetsDict == null)
            {
                Debug.LogError("[SheetsDownloader::ProcessResolveResponse] Failed to parse sheets response");
                return;
            }

            var existingSheets =
                _database.Sheets.ToDictionary(static sheet => sheet.Id, static s => s.ContentLength);

            _database.Sheets.Clear();
            foreach (var (sheetName, id) in sheetsDict)
            {
                var contentLength = existingSheets.GetValueOrDefault(id, 0);

                _database.Sheets.Add(new TSheet
                {
                    Id = id,
                    Name = sheetName,
                    ContentLength = contentLength
                });
            }
        }

        private async UniTask<long> GetSheetContentLengthAsync(long sheetId)
        {
            var url = ZString.Format(UrlPattern, _database.TableId, sheetId);

            using var request = UnityWebRequest.Head(url);
            await request.SendWebRequest().ToUniTask();

            if (string.IsNullOrEmpty(request.error) is false)
                return 0;

            return long.TryParse(request.GetResponseHeader("Content-Length"), out var length)
                ? length
                : 0;
        }

        private async UniTask SaveSheetDataAsync(TSheet sheet, byte[] data)
        {
            var path = Path.Combine(_database.GetDownloadPath(), $"{sheet.Name}.csv");
            await File.WriteAllBytesAsync(path, data);

            await UniTask.SwitchToMainThread();

            AssetDatabase.Refresh();
            sheet.TextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
        }

        private static string GetRequestError(UnityWebRequest request)
        {
            if (string.IsNullOrEmpty(request.error) is false)
                return request.error;

            return request.downloadHandler.text.Contains("signin/identifier")
                ? "It seems that access to this document is denied."
                : null;
        }

        private static string ExtractInternalError(UnityWebRequest request)
        {
            var matches =
                Regex.Matches(request.downloadHandler.text, @">(?<Message>.+?)<\/div>");

            if (matches.Count == 0 && request.downloadHandler.text.Contains("Google Script ERROR:") is false)
                return null;

            return matches.Count > 0
                ? matches[1].Groups["Message"].Value.Replace("quot;", string.Empty)
                : request.downloadHandler.text;
        }

        private void PrepareDownloadFolderIfNeeded()
        {
            var downloadPath = _database.GetDownloadPath();
            if (Directory.Exists(downloadPath) is false)
                Directory.CreateDirectory(downloadPath);
        }
    }
}