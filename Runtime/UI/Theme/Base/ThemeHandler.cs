using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.CustomTypes.Singletons;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.Base
{
    [Resource(ResourcePaths.ThemeFullPath, ResourcePaths.ThemeHandlerAssetName, ResourcePaths.ThemeResourcePath)]
    internal sealed class ThemeHandler : SingletonScriptableObject<ThemeHandler>
    {
        [field: SerializeField] internal ThemeType CurrentThemeType { get; set; }
    }
}