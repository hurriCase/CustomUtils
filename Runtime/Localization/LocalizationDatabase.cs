using System.Collections.Generic;
using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.CustomTypes.Singletons;
using UnityEngine;

namespace CustomUtils.Runtime.Localization
{
    [Resource(
        ResourcePaths.LocalizationSettingsFullPath,
        ResourcePaths.LocalizationSettingsAssetName,
        ResourcePaths.LocalizationSettingsResourcesPath
    )]
    internal sealed class LocalizationDatabase : SingletonScriptableObject<LocalizationDatabase>
    {
        [field: SerializeField] internal SystemLanguage DefaultLanguage { get; private set; } = SystemLanguage.English;
        [field: SerializeField] internal string TableId { get; set; }
        [field: SerializeField] internal List<Sheet> Sheets { get; set; } = new();
    }
}