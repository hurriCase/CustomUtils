using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.Downloader;
using UnityEngine;

namespace CustomUtils.Runtime.Localization
{
    [Resource(
        ResourcePaths.LocalizationSettingsFullPath,
        ResourcePaths.LocalizationSettingsAssetName,
        ResourcePaths.LocalizationSettingsResourcesPath
    )]
    internal sealed class LocalizationDatabase : SheetsDatabase<LocalizationDatabase>
    {
        [field: SerializeField] internal SystemLanguage DefaultLanguage { get; private set; }
            = SystemLanguage.English;

        public override string GetDownloadPath() => ResourcePaths.LocalizationSheetsPath;
    }
}