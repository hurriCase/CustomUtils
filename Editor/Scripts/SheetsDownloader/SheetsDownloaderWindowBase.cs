using CustomUtils.Editor.Scripts.Extensions;
using CustomUtils.Runtime.Downloader;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEditor;
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

        protected VisualElement CustomContentSlot { get; private set; }
        protected abstract TDatabase Database { get; }

        private SheetsDownloader<TDatabase, TSheet> _sheetsDownloader;
        private SerializedObject _serializedObject;

        private SheetsDownloaderElements _elements;

        protected abstract void CreateCustomContent();
        protected virtual void OnSheetsDownloaded() { }

        public void DownloadSheet(long sheetId)
        {
            var sheet = Database.Sheets.FirstOrDefault(sheet => sheet.Id == sheetId);
            if (sheet != null)
                ProcessDownloadSingleSheetAsync(sheet).Forget();
        }

        protected void CreateGUI()
        {
            _baseLayout.CloneTree(rootVisualElement);
            _elements = new SheetsDownloaderElements(rootVisualElement);

            _sheetsDownloader = new SheetsDownloader<TDatabase, TSheet>(Database);

            _serializedObject = new SerializedObject(Database);

            _serializedObject.CreateProperty(nameof(Database.TableId), _elements.TableIdContainer);
            _serializedObject.CreateProperty(nameof(Database.Sheets), _elements.SheetsContainer);

            SetupButtons();

            CustomContentSlot = rootVisualElement.Q<VisualElement>("CustomContentSlot");

            CreateCustomContent();
        }

        private void SetupButtons()
        {
            _elements.DownloadAllButton.clicked += () => ProcessDownloadSheetsAsync().Forget();
            _elements.OpenGoogleButton.clicked += OpenGoogleSheet;
        }

        private async UniTaskVoid ProcessDownloadSingleSheetAsync(TSheet sheet)
        {
            var result = await _sheetsDownloader.DownloadSingleSheetAsync(sheet);
            result.DisplayMessage();

            if (result.IsValid)
                OnSheetsDownloaded();
        }

        private async UniTaskVoid ProcessDownloadSheetsAsync()
        {
            if (Database.Sheets.Count == 0)
            {
                var resolveResult = await _sheetsDownloader.TryResolveGoogleSheetsAsync();
                resolveResult.DisplayMessage();
            }

            var downloadResult = await _sheetsDownloader.DownloadSheetsAsync();

            if (downloadResult.IsValid is false)
                return;

            OnSheetsDownloaded();
            downloadResult.DisplayMessage();
        }

        private void OpenGoogleSheet()
        {
            Application.OpenURL(ZString.Format(SheetDownloaderConstants.TableUrlPattern, Database.TableId));
        }
    }
}