using System;
using CustomUtils.Runtime.Downloader;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomUtils.Editor.Scripts.SheetsDownloader
{
    [UsedImplicitly]
    public abstract class SheetsDownloaderWindowBase<TDatabase, TSheet> : EditorWindow, ISheetsDownloaderWindow
        where TDatabase : SheetsDatabase<TDatabase, TSheet>
        where TSheet : Sheet, new()
    {
        [SerializeField] private VisualTreeAsset _baseLayout;
        [SerializeField] private VisualTreeAsset _sheetItemLayout;

        private const string TableUrlPattern = "https://docs.google.com/spreadsheets/d/{0}";

        private SheetsDownloader<TDatabase, TSheet> _sheetsDownloader;
        private TextField _tableIdField;
        private ListView _sheetsList;
        private VisualElement _warningBox;
        private Label _warningText;

        protected VisualElement CustomContentSlot { get; private set; }
        protected abstract TDatabase Database { get; }

        protected void CreateGUI()
        {
            _baseLayout.CloneTree(rootVisualElement);

            _sheetsDownloader = new SheetsDownloader<TDatabase, TSheet>(Database);

            SetupBaseElements();
            SetupSheetsList();
            SetupButtons();

            CustomContentSlot = rootVisualElement.Q<VisualElement>("CustomContentSlot");

            CreateCustomContent();
            UpdateWarnings();
        }

        protected abstract void CreateCustomContent();
        protected virtual void OnSheetsDownloaded() { }

        private void SetupBaseElements()
        {
            _tableIdField = rootVisualElement.Q<TextField>("TableId");
            _tableIdField.value = Database.TableId;
            _tableIdField.RegisterValueChangedCallback(evt =>
            {
                Database.TableId = evt.newValue;
                EditorUtility.SetDirty(Database);
                UpdateWarnings();
            });

            _warningBox = rootVisualElement.Q<VisualElement>("WarningBox");
            _warningText = rootVisualElement.Q<Label>("WarningText");
        }

        private void SetupSheetsList()
        {
            _sheetsList = rootVisualElement.Q<ListView>("SheetsList");
            _sheetsList.itemsSource = Database.Sheets;

            _sheetsList.makeItem = () => _sheetItemLayout.CloneTree();
            _sheetsList.bindItem = BindSheetItem;

            _sheetsList.Rebuild();
        }

        private void BindSheetItem(VisualElement element, int index)
        {
            if (index < 0 || index >= Database.Sheets.Count)
                return;

            var sheet = Database.Sheets[index];

            var nameLabel = element.Q<Label>("SheetName");
            var idLabel = element.Q<Label>("SheetId");
            var textAssetField = element.Q<ObjectField>("TextAssetField");
            var downloadButton = element.Q<Button>("DownloadButton");

            nameLabel.text = sheet.Name;
            idLabel.text = $"ID: {sheet.Id}";

            textAssetField.objectType = typeof(TextAsset);
            textAssetField.value = sheet.TextAsset;
            textAssetField.SetEnabled(false);

            downloadButton.clicked += () => DownloadSheet(sheet.Id);
        }

        private void SetupButtons()
        {
            var downloadAllButton = rootVisualElement.Q<Button>("DownloadAllButton");
            downloadAllButton.clicked += () => ProcessDownloadSheetsAsync().Forget();

            var resolveButton = rootVisualElement.Q<Button>("ResolveButton");
            resolveButton.clicked += () => ProcessResolveGoogleSheetsAsync().Forget();

            var openGoogleButton = rootVisualElement.Q<Button>("OpenGoogleButton");
            openGoogleButton.clicked += OpenGoogleSheet;
        }

        public void DownloadSheet(long sheetId)
        {
            var sheet = Database.Sheets.FirstOrDefault(sheet => sheet.Id == sheetId);
            if (sheet != null)
                ProcessDownloadSingleSheetAsync(sheet).Forget();
        }

        private async UniTaskVoid ProcessResolveGoogleSheetsAsync()
        {
            await ExecuteWithErrorHandling(async () =>
            {
                await _sheetsDownloader.ResolveGoogleSheetsAsync();

                _sheetsList.itemsSource = Database.Sheets;
                _sheetsList.Rebuild();
                UpdateWarnings();

                EditorUtility.DisplayDialog("Success",
                    $"Successfully resolved {Database.Sheets.Count} sheets from Google Sheets", "OK");
            });
        }

        private async UniTaskVoid ProcessDownloadSingleSheetAsync(TSheet sheet)
        {
            await ExecuteWithErrorHandling(async () =>
            {
                Debug.Log($"[{GetType().Name}] Downloading {sheet.Name}...");

                var result = await _sheetsDownloader.DownloadSingleSheetAsync(sheet);

                if (result.HasDownloads)
                {
                    OnSheetsDownloaded();
                    Debug.Log($"[{GetType().Name}] {result.Message}");
                    _sheetsList.Rebuild();
                    UpdateWarnings();
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
                    _sheetsList.Rebuild();
                    UpdateWarnings();
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

        private void UpdateWarnings()
        {
            string warningMessage = null;

            if (string.IsNullOrEmpty(Database.TableId))
                warningMessage = "Table Id is empty.";
            else if (Database.Sheets.Count == 0)
                warningMessage = "Sheets are empty. Add manually or click 'Resolve from Google'.";
            else if (Database.Sheets.Any(static sheet => !sheet.TextAsset))
                warningMessage = "Some sheets are not downloaded.";

            if (string.IsNullOrEmpty(warningMessage))
                _warningBox.AddToClassList("hidden");
            else
            {
                _warningBox.RemoveFromClassList("hidden");
                _warningText.text = warningMessage;
            }
        }
    }
}