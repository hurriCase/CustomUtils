using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using CustomUtils.Runtime.Downloader;
using CustomUtils.Runtime.ResponseTypes;
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
        private const string UrlPattern = "https://docs.google.com/spreadsheets/d/{0}/export?format=csv&gid={1}";
        private const string SheetResolverUrl =
            "https://script.google.com/macros/s/" +
            "AKfycbycW2dsGZhc2xJh2Fs8yu9KUEqdM-ssOiK1AlES3crLqQa1lkDrI4mZgP7sJhmFlGAD/exec";

        private const string GoogleSignInIndicator = "signin/identifier";
        private const string ContentLengthHeader = "Content-Length";
        private const string CsvExtension = ".csv";
        private const string GoogleScriptErrorIndicator = "Google Script ERROR:";
        private const string ErrorMessagePattern = @">(?<Message>.+?)<\/div>";
        private const string QuotReplacement = "quot;";
        private const string MessageGroupName = "Message";
        private const string RequestUrlFormat = "{0}?tableUrl={1}";

        private const string AccessDeniedMessage = "It seems that access to this document is denied.";
        private const string TableIdWrongMessage = "Table Id is wrong!";
        private const string NetworkErrorFormat = "Network error: {0}";
        private const string AllSheetsUpToDateMessage = "All sheets are up to date!";
        private const string ChangedSheetsDownloadedFormat = "{0} changed sheets downloaded!";
        private const string SheetIsNullMessage = "Sheet is null";
        private const string Error404Indicator = "404";
        private const string SheetDownloadedSuccessFormat = "Sheet '{0}' downloaded successfully!";
        private const string SheetDownloadFailedFormat = "Failed to download sheet '{0}': {1}";
        private const string FailedToParseResponseMessage = "Failed to parse sheets response";
        private const string TableNotFoundOrNoPermissionFormat =
            "Table not found or public read permission not set: {0}";

        private readonly TDatabase _database;
        private readonly List<TSheet> _sheetsToDownload = new();

        private string RequestUrl => ZString.Format(RequestUrlFormat, SheetResolverUrl, _database.TableId);

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
        public async UniTask<Result> TryResolveGoogleSheetsAsync()
        {
            using var request = UnityWebRequest.Get(RequestUrl);

            await request.SendWebRequest().ToUniTask();

            return string.IsNullOrEmpty(request.error)
                ? ProcessResolveResponse(request)
                : Result.Invalid(ZString.Format(NetworkErrorFormat, request.error));
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
            Debug.Log("[SheetsDownloader::DownloadSheetsAsync] Start downloading sheets ...");

            PrepareDownloadFolderIfNeeded();

            await FillSheetsToDownloadAsync();

            if (_sheetsToDownload.Count == 0)
                return new DownloadResult(0, AllSheetsUpToDateMessage);

            var changedCount = 0;
            foreach (var sheet in _sheetsToDownload)
            {
                var result = await DownloadSingleSheetAsync(sheet);

                if (result.HasDownloads)
                    changedCount++;
            }

            var message = changedCount > 0
                ? ZString.Format(ChangedSheetsDownloadedFormat, changedCount)
                : AllSheetsUpToDateMessage;

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
                return new DownloadResult(0, SheetIsNullMessage);

            PrepareDownloadFolderIfNeeded();

            try
            {
                var url = ZString.Format(UrlPattern, _database.TableId, sheet.Id);

                using var request = UnityWebRequest.Get(url);
                await request.SendWebRequest().ToUniTask();

                var error = GetRequestError(request);
                if (string.IsNullOrEmpty(error) is false)
                {
                    var errorMessage = error.Contains(Error404Indicator) ? TableIdWrongMessage : error;
                    return new DownloadResult(1, errorMessage);
                }

                var data = request.downloadHandler.data;
                await SaveSheetDataAsync(sheet, data);

                sheet.ContentLength = data.Length;

                return new DownloadResult(1, ZString.Format(SheetDownloadedSuccessFormat, sheet.Name));
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return new DownloadResult(0, ZString.Format(SheetDownloadFailedFormat, sheet.Name, ex.Message));
            }
        }

        private async UniTask FillSheetsToDownloadAsync()
        {
            _sheetsToDownload.Clear();

            foreach (var sheet in _database.Sheets)
            {
                if (!sheet.TextAsset)
                {
                    _sheetsToDownload.Add(sheet);
                    continue;
                }

                var contentLength = await GetSheetContentLengthAsync(sheet.Id);

                if (contentLength > 0 && sheet.HasChanged(contentLength))
                    _sheetsToDownload.Add(sheet);
            }
        }

        private Result ProcessResolveResponse(UnityWebRequest request)
        {
            var error = ExtractInternalError(request);
            if (error != null)
                return Result.Invalid(ZString.Format(TableNotFoundOrNoPermissionFormat, error));

            var sheets = JsonConvert.DeserializeObject<Dictionary<string, long>>(request.downloadHandler.text);

            if (sheets == null)
                return Result.Invalid(FailedToParseResponseMessage);

            var existingSheets =
                _database.Sheets.ToDictionary(static sheet => sheet.Id, static sheet => sheet.ContentLength);

            _database.Sheets.Clear();
            foreach (var (sheetName, id) in sheets)
            {
                var contentLength = existingSheets.GetValueOrDefault(id, 0);

                _database.Sheets.Add(new TSheet
                {
                    Id = id,
                    Name = sheetName,
                    ContentLength = contentLength
                });
            }

            return Result.Valid();
        }

        private async UniTask<long> GetSheetContentLengthAsync(long sheetId)
        {
            var url = ZString.Format(UrlPattern, _database.TableId, sheetId);

            using var request = UnityWebRequest.Head(url);
            await request.SendWebRequest().ToUniTask();

            if (string.IsNullOrEmpty(request.error) is false)
                return 0;

            return long.TryParse(request.GetResponseHeader(ContentLengthHeader), out var length)
                ? length
                : 0;
        }

        private async UniTask SaveSheetDataAsync(TSheet sheet, byte[] data)
        {
            var path = Path.Combine(_database.GetDownloadPath(), ZString.Concat(sheet.Name, CsvExtension));
            await File.WriteAllBytesAsync(path, data);

            await UniTask.SwitchToMainThread();

            AssetDatabase.Refresh();
            sheet.TextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
        }

        private static string GetRequestError(UnityWebRequest request)
        {
            if (string.IsNullOrEmpty(request.error) is false)
                return request.error;

            return request.downloadHandler.text.Contains(GoogleSignInIndicator)
                ? AccessDeniedMessage
                : null;
        }

        private static string ExtractInternalError(UnityWebRequest request)
        {
            var matches =
                Regex.Matches(request.downloadHandler.text, ErrorMessagePattern);

            if (matches.Count == 0 && request.downloadHandler.text.Contains(GoogleScriptErrorIndicator) is false)
                return null;

            return matches.Count > 0
                ? matches[1].Groups[MessageGroupName].Value.Replace(QuotReplacement, string.Empty)
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