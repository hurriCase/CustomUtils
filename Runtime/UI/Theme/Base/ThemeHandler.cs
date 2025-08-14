using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.CustomTypes.Singletons;
using R3;

namespace CustomUtils.Runtime.UI.Theme.Base
{
    [Resource(ResourcePaths.ThemeFullPath, ResourcePaths.ThemeHandlerAssetName, ResourcePaths.ThemeResourcePath)]
    internal sealed class ThemeHandler : SingletonScriptableObject<ThemeHandler>
    {
        internal ReactiveProperty<ThemeType> CurrentThemeType { get; } = new();
    }
}