using System;
using CustomUtils.Editor.Scripts.CustomEditorUtilities;
using CustomUtils.Runtime.Downloader;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Editor.Scripts.SheetsDownloader
{
    /// <inheritdoc cref="WindowBase" />
    /// <summary>
    /// Abstract base class for Unity Editor windows that provide Google Sheets downloading functionality.
    /// Provides common UI elements and download operations for sheets database management with standard Unity array editor.
    /// </summary>
    /// <typeparam name="TDatabase">The type of sheets database that inherits
    /// from <see cref="T:CustomUtils.Runtime.Downloader.SheetsDatabase`1" />.</typeparam>
    /// <typeparam name="TSheet">The type of sheets to use it for a database</typeparam>
    [UsedImplicitly]
    public abstract class SheetsDownloaderWindowBase<TDatabase, TSheet> : WindowBase, ISheetsDownloaderWindow
        where TDatabase : SheetsDatabase<TDatabase, TSheet>
        where TSheet : Sheet, new()
    {
        private const string TableUrlPattern = "https://docs.google.com/spreadsheets/d/{0}";

        private SheetsDownloader<TDatabase, TSheet> _sheetsDownloader;

        /// <summary>
        /// Gets the database instance that contains sheet configuration and downloaded data.
        /// This property must be implemented by derived classes to provide the specific database instance.
        /// </summary>
        /// <value>The sheets database instance of type <typeparamref name="TDatabase"/>.</value>
        protected abstract TDatabase Database { get; }

        /// <inheritdoc />
        /// <summary>
        /// Initializes the window by setting up the serialized object and creating the sheets downloader instance.
        /// This method is called during window initialization and sets up the necessary components for operation.
        /// </summary>
        protected override void InitializeWindow()
        {
            serializedObject = new SerializedObject(Database);
            _sheetsDownloader = new SheetsDownloader<TDatabase, TSheet>(Database);
        }

        /// <summary>
        /// Draws the custom content specific to the derived window.
        /// This method must be implemented by derived classes to provide their specific UI elements
        /// and should typically call <see cref="DrawCommonSheetsSection"/> to include the standard sheets functionality.
        /// </summary>
        protected abstract void DrawCustomContent();

        /// <summary>
        /// Called after sheets have been successfully downloaded.
        /// </summary>
        protected virtual void OnSheetsDownloaded() { }

        /// <summary>
        /// Draws the common sheets management UI section including table ID field, download buttons,
        /// sheets list, and warning messages. This method should be called from derived classes
        /// in their custom content drawing methods.
        /// </summary>
        protected void DrawCommonSheetsSection()
        {
            PropertyField(nameof(Database.TableId));

            DrawDownloadButtons();

            EditorGUILayout.Space();

            PropertyField(nameof(Database.Sheets));

            DrawWarnings();
        }

        public void DownloadSheet(int sheetId)
        {
            foreach (var sheet in Database.Sheets)
            {
                if (sheet.Id != sheetId)
                    continue;

                ProcessDownloadSingleSheetAsync(sheet).Forget();
                return;
            }
        }

        private void DrawDownloadButtons()
        {
            using var horizontalScope = EditorVisualControls.CreateHorizontalGroup();

            if (EditorVisualControls.Button("▼ Download All Sheets"))
                ProcessDownloadSheetsAsync().Forget();

            if (EditorVisualControls.Button("🔄 Resolve from Google"))
                ProcessResolveGoogleSheetsAsync().Forget();

            if (EditorVisualControls.Button("❖ Open Google Sheets"))
                OpenGoogleSheet();
        }

        private async UniTaskVoid ProcessResolveGoogleSheetsAsync()
        {
            await ExecuteWithErrorHandling(async () =>
            {
                await _sheetsDownloader.ResolveGoogleSheetsAsync();

                Debug.Log("[SheetsDownloaderWindowBase::ProcessResolveGoogleSheetsAsync] " +
                          $"Resolved {Database.Sheets.Count} sheets from Google Sheets");

                EditorUtility.DisplayDialog("Success",
                    $"Successfully resolved {Database.Sheets.Count} sheets from Google Sheets", "OK");
            });
        }

        private async UniTaskVoid ProcessDownloadSingleSheetAsync(TSheet sheet)
        {
            await ExecuteWithErrorHandling(async () =>
            {
                Debug.Log($"[SheetsDownloaderWindowBase::ProcessDownloadSingleSheetAsync] Downloading {sheet.Name}...");

                var result = await _sheetsDownloader.DownloadSingleSheetAsync(sheet);

                if (result.HasDownloads)
                {
                    OnSheetsDownloaded();
                    Debug.Log($"[SheetsDownloaderWindowBase::ProcessDownloadSingleSheetAsync] {result.Message}");
                }
            });
        }

        private async UniTaskVoid ProcessDownloadSheetsAsync()
        {
            await ExecuteWithErrorHandling(async () =>
            {
                if (Database.Sheets.Count == 0)
                    await _sheetsDownloader.ResolveGoogleSheetsAsync();

                var result = await _sheetsDownloader.DownloadSheetsAsync();

                if (result.HasDownloads)
                {
                    OnSheetsDownloaded();
                    EditorUtility.DisplayDialog("Success", result.Message, "OK");
                }
            });
        }

        private async UniTask ExecuteWithErrorHandling(Func<UniTask> operation)
        {
            if (string.IsNullOrEmpty(Database.TableId))
            {
                EditorUtility.DisplayDialog("Error", "Table Id is empty.", "OK");
                return;
            }

            try
            {
                await operation();
                EditorUtility.SetDirty(Database);
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Error", $"Download failed: {ex.Message}", "OK");
                Debug.LogException(ex);
            }
        }

        private void OpenGoogleSheet()
        {
            if (string.IsNullOrEmpty(Database.TableId) is false)
            {
                Application.OpenURL(string.Format(TableUrlPattern, Database.TableId));
                return;
            }

            EditorUtility.DisplayDialog("Error", "Table Id is empty.", "OK");
        }

        private void DrawWarnings()
        {
            if (string.IsNullOrEmpty(Database.TableId))
                EditorVisualControls.WarningBox("Table Id is empty.");
            else if (Database.Sheets.Count == 0)
                EditorVisualControls.WarningBox("Sheets are empty. Add manually or click 'Resolve from Google'.");
            else if (Database.Sheets.AsValueEnumerable().Any(static sheet => !sheet.TextAsset))
                EditorVisualControls.WarningBox("Some sheets are not downloaded.");
        }

        /// <inheritdoc />
        /// <summary>
        /// Draws the main window content by updating the serialized object, calling the custom content drawing method,
        /// and applying any property modifications. This method handles the standard Unity Editor window update cycle.
        /// </summary>
        protected override void DrawWindowContent()
        {
            serializedObject.Update();

            DrawCustomContent();

            if (serializedObject.ApplyModifiedProperties())
                EditorUtility.SetDirty(Database);
        }
    }
}