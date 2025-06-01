using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.CustomTypes.Singletons;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.Base
{
    [Resource(
        ResourcePaths.ThemeHandlerFullPath,
        ResourcePaths.ThemeHandlerAssetName,
        ResourcePaths.ThemHandlerResourcePath
    )]
    internal sealed class ThemeHandler : SingletonScriptableObject<ThemeHandler>
    {
        [field: SerializeField] internal ThemeType CurrentThemeType { get; set; }
    }
}