using UnityEngine.UIElements;

namespace CustomUtils.Editor.Scripts.SheetsDownloader
{
    internal readonly struct SheetsDownloaderElements
    {
        internal VisualElement TableIdContainer { get; }
        internal VisualElement SheetsContainer { get; }
        internal Button DownloadAllButton { get; }
        internal Button OpenGoogleButton { get; }

        internal SheetsDownloaderElements(VisualElement visualElement)
        {
            TableIdContainer = visualElement.Q<VisualElement>("TableIdContainer");
            SheetsContainer = visualElement.Q<VisualElement>("SheetsContainer");
            DownloadAllButton = visualElement.Q<Button>("DownloadAllButton");
            OpenGoogleButton = visualElement.Q<Button>("OpenGoogleButton");
        }
    }
}