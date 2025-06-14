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
    internal sealed class LocalizationSettings : SingletonScriptableObject<LocalizationSettings>
    {
        [field: SerializeField] internal string DefaultLanguage { get; private set; } = "English";
        [field: SerializeField] internal string TableId { get; set; }
        [field: SerializeField] internal List<Sheet> Sheets { get; set; } = new();
        [field: SerializeField] internal LanguageFontMapping[] FontMappings { get; private set; }
    }
}