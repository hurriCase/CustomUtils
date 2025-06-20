﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using CustomUtils.Editor.CustomEditorUtilities;
using CustomUtils.Runtime;
using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.Helpers;
using CustomUtils.Runtime.Localization;
using Cysharp.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using ZLinq;

namespace CustomUtils.Editor.Localization
{
    internal sealed class LocalizationSettingsWindow : WindowBase
    {
        private static SerializedObject _serializedObject;
        private Vector2 _scrollPosition;

        private static LocalizationDatabase LocalizationDatabase => LocalizationDatabase.Instance;

        private static CancellationTokenSource _cancellationTokenSource;

        private static DateTime _lastRequestTime;

        private const string UrlPattern = "https://docs.google.com/spreadsheets/d/{0}/export?format=csv&gid={1}";

        [MenuItem(MenuItemNames.LocalizationMenuName)]
        internal static void ShowWindow()
        {
            GetWindow<LocalizationSettingsWindow>(nameof(LocalizationSettingsWindow).ToSpacedWords());
        }

        protected override void InitializeWindow()
        {
            serializedObject = new SerializedObject(LocalizationDatabase);
        }

        protected override void DrawWindowContent()
        {
            serializedObject.Update();

            using var scrollScope = EditorVisualControls.CreateScrollView(ref _scrollPosition);

            PropertyField(nameof(LocalizationDatabase.TableId));
            PropertyField(nameof(LocalizationDatabase.Sheets));
            PropertyField(nameof(LocalizationDatabase.FontMappings));

            DisplayButtons();
            DisplayWarnings();

            if (serializedObject.ApplyModifiedProperties())
                EditorUtility.SetDirty(LocalizationDatabase);
        }

        private void DisplayButtons()
        {
            if (EditorVisualControls.Button("▼ Download Sheets"))
                ProcessDownloadSheetsAsync().Forget();

            if (EditorVisualControls.Button("❖ Open Google Sheets"))
                Application.OpenURL(string.Format(SpreedSettings.TableUrlPattern, LocalizationDatabase.TableId));
        }

        private async UniTaskVoid ProcessDownloadSheetsAsync()
        {
            if (string.IsNullOrEmpty(LocalizationDatabase.TableId))
            {
                EditorUtility.DisplayDialog("Error", "Table Id is empty.", "OK");
                return;
            }

            CancellationSourceHelper.SetNewCancellationTokenSource(ref _cancellationTokenSource);

            try
            {
                await ResolveGoogleSheetsInternalAsync();
                await DownloadGoogleSheetsInternalAsync();
                EditorUtility.SetDirty(LocalizationDatabase);
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Error", $"Resolve failed: {ex.Message}", "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                CancellationSourceHelper.CancelAndDisposeCancellationTokenSource(ref _cancellationTokenSource);
            }
        }

        private void DisplayWarnings()
        {
            if (string.IsNullOrEmpty(LocalizationDatabase.TableId))
                EditorVisualControls.WarningBox("Table Id is empty.");
            else if (LocalizationDatabase.Sheets.Count == 0)
                EditorVisualControls.WarningBox("Sheets are empty.");
            else if (LocalizationDatabase.Sheets.AsValueEnumerable().Any(sheet => !sheet.TextAsset))
                EditorVisualControls.WarningBox("Sheets are not downloaded.");
        }

        private async UniTask DownloadGoogleSheetsInternalAsync()
        {
            PrepareDownloadFolder();

            await ProcessSheetsDownload(_cancellationTokenSource.Token);

            EditorUtility.DisplayDialog("Message",
                $"{LocalizationDatabase.Sheets.Count} localization sheets downloaded!", "OK");
        }

        private async UniTask ResolveGoogleSheetsInternalAsync()
        {
            EditorUtility.DisplayProgressBar("Resolving sheets...", "Executing Google App Script...", 0.5f);

            var requestUrl = $"{SpreedSettings.SheetResolverUrl}?tableUrl={LocalizationDatabase.TableId}";
            using var request = UnityWebRequest.Get(requestUrl);

            await request.SendWebRequest().ToUniTask(cancellationToken: _cancellationTokenSource.Token);

            if (string.IsNullOrEmpty(request.error) is false)
                throw new Exception($"Network error: {request.error}");

            ProcessResolveResponse(request);
        }

        private void PrepareDownloadFolder()
        {
            if (Directory.Exists(ResourcePaths.LocalizationSheetsPath) is false)
                Directory.CreateDirectory(ResourcePaths.LocalizationSheetsPath);

            var files = Directory.GetFiles(ResourcePaths.LocalizationSheetsPath, "*.csv");
            foreach (var file in files)
                File.Delete(file);
        }

        private async UniTask ProcessSheetsDownload(CancellationToken cancellationToken)
        {
            for (var i = 0; i < LocalizationDatabase.Sheets.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var sheet = LocalizationDatabase.Sheets[i];
                var progress = (float)(i + 1) / LocalizationDatabase.Sheets.Count;

                var progressText =
                    $"[{(int)(100 * progress)}%] [{i + 1}/{LocalizationDatabase.Sheets.Count}] Downloading {sheet.Name}...";

                if (EditorUtility.DisplayCancelableProgressBar("Downloading sheets...", progressText, progress))
                    cancellationToken.ThrowIfCancellationRequested();

                await DownloadSingleSheet(sheet, i, cancellationToken);
            }

            LocalizationController.ReadLocalizationData();
        }

        private async UniTask DownloadSingleSheet(Sheet sheet, int index, CancellationToken cancellationToken)
        {
            var url = string.Format(UrlPattern, LocalizationDatabase.TableId, sheet.Id);

            using var request = UnityWebRequest.Get(url);
            await request.SendWebRequest().ToUniTask(cancellationToken: cancellationToken);

            var error = GetRequestError(request);
            if (string.IsNullOrEmpty(error))
            {
                await SaveSheetData(sheet, index, request.downloadHandler.data);
                return;
            }

            var errorMessage = error.Contains("404") ? "Table Id is wrong!" : error;
            throw new Exception(errorMessage);
        }

        private string GetRequestError(UnityWebRequest request)
        {
            if (string.IsNullOrEmpty(request.error) is false)
                return request.error;

            return request.downloadHandler.text.Contains("signin/identifier")
                ? "It seems that access to this document is denied."
                : null;
        }

        private async UniTask SaveSheetData(Sheet sheet, int index, byte[] data)
        {
            var path = Path.Combine(ResourcePaths.LocalizationSheetsPath, $"{sheet.Name}.csv");

            await File.WriteAllBytesAsync(path, data);

            await UniTask.SwitchToMainThread();

            AssetDatabase.Refresh();
            LocalizationDatabase.Sheets[index].TextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            EditorUtility.SetDirty(this);

            Debug.Log("[LocalizationSettingsWindow::SaveSheetData] " +
                      $"Sheet {sheet.Name} ({sheet.Id}) saved to {path}");
        }

        private void ProcessResolveResponse(UnityWebRequest request)
        {
            var error = ExtractInternalError(request);
            if (error != null)
            {
                EditorUtility.DisplayDialog("Error",
                    "Table not found or public read permission not set.", "OK");
                return;
            }

            var sheetsDict = JsonConvert.DeserializeObject<Dictionary<string, long>>(request.downloadHandler.text);
            if (sheetsDict == null)
                throw new Exception("Failed to parse sheets response");

            LocalizationDatabase.Sheets.Clear();
            foreach (var (sheetName, id) in sheetsDict)
            {
                LocalizationDatabase.Sheets.Add(new Sheet
                {
                    Id = id,
                    Name = sheetName
                });
            }
        }

        private static string ExtractInternalError(UnityWebRequest request)
        {
            var matches = Regex.Matches(request.downloadHandler.text, @">(?<Message>.+?)<\/div>");

            if (matches.Count == 0 && request.downloadHandler.text.Contains("Google Script ERROR:") is false)
                return null;

            return matches.Count > 0
                ? matches[1].Groups["Message"].Value.Replace("quot;", string.Empty)
                : request.downloadHandler.text;
        }
    }
}