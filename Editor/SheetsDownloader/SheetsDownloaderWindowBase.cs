using System;
using CustomUtils.Editor.CustomEditorUtilities;
using CustomUtils.Runtime.Downloader;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Editor.SheetsDownloader
{
    /// <inheritdoc />
    /// <summary>
    /// Abstract base class for Unity Editor windows that provide Google Sheets downloading functionality.
    /// Provides common UI elements and download operations for sheets database management.
    /// </summary>
    /// <typeparam name="TDatabase">The type of sheets database that inherits
    /// from <see cref="T:CustomUtils.Runtime.Downloader.SheetsDatabase`1" />.</typeparam>
    [UsedImplicitly]
    public abstract class SheetsDownloaderWindowBase<TDatabase> : WindowBase
        where TDatabase : SheetsDatabase<TDatabase>
    {
        private const string TableUrlPattern = "https://docs.google.com/spreadsheets/d/{0}";

        private SheetsDownloader<TDatabase> _sheetsDownloader;

        /// <summary>
        /// Gets the database instance that contains sheet configuration and downloaded data.
        /// This property must be implemented by derived classes to provide the specific database instance.
        /// </summary>
        /// <value>The sheets database instance of type <typeparamref name="TDatabase"/>.</value>
        protected abstract TDatabase Database { get; }

        /// <summary>
        /// Called after sheets have been successfully downloaded.
        /// </summary>
        protected virtual void OnSheetsDownloaded() { }

        /// <inheritdoc />
        /// <summary>
        /// Initializes the window by setting up the serialized object and creating the sheets downloader instance.
        /// This method is called during window initialization and sets up the necessary components for operation.
        /// </summary>
        protected override void InitializeWindow()
        {
            serializedObject = new SerializedObject(Database);
            _sheetsDownloader = new SheetsDownloader<TDatabase>(Database);
        }

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

        /// <summary>
        /// Draws the custom content specific to the derived window.
        /// This method must be implemented by derived classes to provide their specific UI elements
        /// and should typically call <see cref="DrawCommonSheetsSection"/> to include the standard sheets functionality.
        /// </summary>
        protected abstract void DrawCustomContent();

        private void DrawDownloadButtons()
        {
            using var horizontalScope = EditorVisualControls.CreateHorizontalGroup();

            if (EditorVisualControls.Button("▼ Download Sheets"))
                ProcessDownloadSheetsAsync().Forget();

            if (EditorVisualControls.Button("❖ Open Google Sheets") is false)
                return;

            if (string.IsNullOrEmpty(Database.TableId) is false)
                Application.OpenURL(string.Format(TableUrlPattern, Database.TableId));
            else
                EditorUtility.DisplayDialog("Error", "Table Id is empty.", "OK");
        }

        private async UniTaskVoid ProcessDownloadSheetsAsync()
        {
            if (string.IsNullOrEmpty(Database.TableId))
            {
                EditorUtility.DisplayDialog("Error", "Table Id is empty.", "OK");
                return;
            }

            try
            {
                Debug.Log("[SheetsDownloaderWindowBase::ProcessDownloadSheetsAsync] " +
                          "Starting sheet download process...");

                if (Database.Sheets.Count == 0)
                    await _sheetsDownloader.ResolveGoogleSheetsAsync();

                var result = await _sheetsDownloader.DownloadSheetsAsync();

                EditorUtility.SetDirty(Database);

                if (result.HasDownloads)
                    OnSheetsDownloaded();

                Debug.Log($"[SheetsDownloaderWindowBase::ProcessDownloadSheetsAsync] {result.Message}");

                if (result.HasDownloads)
                    EditorUtility.DisplayDialog("Success", result.Message, "OK");
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Error", $"Download failed: {ex.Message}", "OK");
            }
        }

        private void DrawWarnings()
        {
            if (string.IsNullOrEmpty(Database.TableId))
                EditorVisualControls.WarningBox("Table Id is empty.");
            else if (Database.Sheets.Count == 0)
                EditorVisualControls.WarningBox("Sheets are empty. Click 'Download Sheets' to resolve.");
            else if (Database.Sheets.AsValueEnumerable().Any(sheet => !sheet.TextAsset))
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