using System;
using System.Collections.Generic;
using CustomUtils.Editor.CustomEditorUtilities;
using CustomUtils.Editor.Extensions;
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
    /// Provides common UI elements and download operations for sheets database management with manual add/remove capabilities.
    /// </summary>
    /// <typeparam name="TDatabase">The type of sheets database that inherits
    /// from <see cref="T:CustomUtils.Runtime.Downloader.SheetsDatabase`1" />.</typeparam>
    /// <typeparam name="TSheet">The type of sheets to use it for a database</typeparam>
    [UsedImplicitly]
    public abstract class SheetsDownloaderWindowBase<TDatabase, TSheet> : WindowBase
        where TDatabase : SheetsDatabase<TDatabase, TSheet>
        where TSheet : Sheet, new()
    {
        private const string TableUrlPattern = "https://docs.google.com/spreadsheets/d/{0}";

        private SheetsDownloader<TDatabase, TSheet> _sheetsDownloader;

        private bool _showSheetsList;
        private readonly List<bool> _showSheets = new();

        // Manual sheet addition fields
        private string _newSheetName = string.Empty;
        private string _newSheetId = string.Empty;

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
            _sheetsDownloader = new SheetsDownloader<TDatabase, TSheet>(Database);
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

            EditorVisualControls.DrawBoxWithFoldout("Sheets", ref _showSheetsList, DrawSheetsSection);

            DrawWarnings();
        }

        private void DrawSheetsSection()
        {
            DrawManualSheetAddition();

            EditorGUILayout.Space();

            DrawSheets();
        }

        private void DrawManualSheetAddition()
        {
            EditorGUILayout.LabelField("Add Sheet Manually", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            _newSheetName = EditorGUILayout.TextField("Sheet Name", _newSheetName);
            _newSheetId = EditorGUILayout.TextField("Sheet ID (GID)", _newSheetId);

            using var horizontalScope = EditorVisualControls.CreateHorizontalGroup();

            var canAdd = string.IsNullOrEmpty(_newSheetName) is false &&
                         string.IsNullOrEmpty(_newSheetId) is false &&
                         long.TryParse(_newSheetId, out _);

            using (new EditorGUI.DisabledScope(canAdd is false))
            {
                if (EditorVisualControls.Button("+ Add Sheet"))
                    AddManualSheet();
            }

            if (EditorVisualControls.Button("🔄 Resolve from Google"))
                ProcessResolveGoogleSheetsAsync().Forget();

            EditorGUI.indentLevel--;
        }

        private void AddManualSheet()
        {
            if (long.TryParse(_newSheetId, out var sheetId) is false)
            {
                EditorUtility.DisplayDialog("Error", "Invalid Sheet ID. Please enter a valid number.", "OK");
                return;
            }

            var existingSheet = Database.Sheets.AsValueEnumerable()
                .FirstOrDefault(sheet => sheet.Id == sheetId);

            if (existingSheet != null)
            {
                EditorUtility.DisplayDialog("Error",
                    $"Sheet with ID {sheetId} already exists: '{existingSheet.Name}'", "OK");
                return;
            }

            var newSheet = new TSheet
            {
                Name = _newSheetName.Trim(),
                Id = sheetId,
                ContentLength = 0,
                TextAsset = null
            };

            Database.Sheets.Add(newSheet);
            EditorUtility.SetDirty(Database);

            _newSheetName = string.Empty;
            _newSheetId = string.Empty;

            Debug.Log("[SheetsDownloaderWindowBase::AddManualSheet] " +
                      $"Added sheet '{newSheet.Name}' with ID {newSheet.Id}");
        }

        private void DrawSheets()
        {
            if (Database.Sheets.Count == 0)
            {
                EditorGUILayout.HelpBox("No sheets available. Add manually or resolve from Google Sheets.",
                    MessageType.Info);
                return;
            }

            var sheetsProperty = serializedObject.FindField(nameof(Database.Sheets));

            EditorGUILayout.LabelField($"Sheets ({Database.Sheets.Count})", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            for (var i = 0; i < Database.Sheets.Count; i++)
            {
                var index = i;
                EditorVisualControls.Foldout(Database.Sheets[i].Name, _showSheets, i,
                    () => DrawSheet(index, sheetsProperty));

                EditorGUILayout.Space();
            }

            EditorGUI.indentLevel--;
        }

        private void DrawSheet(int index, SerializedProperty sheetsProperty)
        {
            EditorGUI.indentLevel++;

            var sheet = Database.Sheets[index];
            var sheetProperty = sheetsProperty.GetArrayElementAtIndex(index);

            foreach (SerializedProperty childProperty in sheetProperty)
                EditorStateControls.PropertyField(childProperty);

            using var horizontalScope = EditorVisualControls.CreateHorizontalGroup();

            if (EditorVisualControls.Button("▼ Download"))
                ProcessDownloadSingleSheetAsync(sheet).Forget();

            if (EditorVisualControls.Button("✖ Remove"))
                RemoveSheet(index);

            EditorGUI.indentLevel--;
        }

        private void RemoveSheet(int index)
        {
            if (index < 0 || index >= Database.Sheets.Count)
                return;

            var sheet = Database.Sheets[index];
            var shouldRemove = EditorUtility.DisplayDialog("Remove Sheet",
                $"Are you sure you want to remove sheet '{sheet.Name}'?\n\nThis will not delete the downloaded CSV file.",
                "Remove", "Cancel");

            if (shouldRemove is false)
                return;

            Database.Sheets.RemoveAt(index);

            if (index < _showSheets.Count)
                _showSheets.RemoveAt(index);

            EditorUtility.SetDirty(Database);

            Debug.Log($"[SheetsDownloaderWindowBase::RemoveSheet] Removed sheet '{sheet.Name}'");
        }

        private async UniTaskVoid ProcessDownloadSingleSheetAsync(TSheet sheet)
        {
            if (string.IsNullOrEmpty(Database.TableId))
            {
                EditorUtility.DisplayDialog("Error", "Table Id is empty.", "OK");
                return;
            }

            try
            {
                var result = await _sheetsDownloader.DownloadSingleSheetAsync(sheet);

                EditorUtility.SetDirty(Database);

                if (result.HasDownloads)
                    OnSheetsDownloaded();

                Debug.Log($"[SheetsDownloaderWindowBase::ProcessDownloadSingleSheetAsync] {result.Message}");
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Error", $"Download failed: {ex.Message}", "OK");
                Debug.LogError($"[SheetsDownloaderWindowBase::ProcessDownloadSingleSheetAsync] {ex}");
            }
        }

        private async UniTaskVoid ProcessResolveGoogleSheetsAsync()
        {
            if (string.IsNullOrEmpty(Database.TableId))
            {
                EditorUtility.DisplayDialog("Error", "Table Id is empty.", "OK");
                return;
            }

            try
            {
                Debug.Log("[SheetsDownloaderWindowBase::ProcessResolveGoogleSheetsAsync] " +
                          "Resolving sheets from Google...");

                await _sheetsDownloader.ResolveGoogleSheetsAsync();

                EditorUtility.SetDirty(Database);

                Debug.Log($"[SheetsDownloaderWindowBase::ProcessResolveGoogleSheetsAsync] " +
                          $"Resolved {Database.Sheets.Count} sheets from Google Sheets");

                EditorUtility.DisplayDialog("Success",
                    $"Successfully resolved {Database.Sheets.Count} sheets from Google Sheets", "OK");
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Error", $"Resolve failed: {ex.Message}", "OK");
                Debug.LogError($"[SheetsDownloaderWindowBase::ProcessResolveGoogleSheetsAsync] {ex}");
            }
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

            if (EditorVisualControls.Button("▼ Download All Sheets"))
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
                Debug.LogError($"[SheetsDownloaderWindowBase::ProcessDownloadSheetsAsync] {ex}");
            }
        }

        private void DrawWarnings()
        {
            if (string.IsNullOrEmpty(Database.TableId))
                EditorVisualControls.WarningBox("Table Id is empty.");
            else if (Database.Sheets.Count == 0)
                EditorVisualControls.WarningBox("Sheets are empty. Add manually or click 'Resolve from Google'.");
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