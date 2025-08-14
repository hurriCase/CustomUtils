using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.CustomTypes.Singletons;
using R3;

namespace CustomUtils.Runtime.UI.Theme.Base
{
    [Resource(ResourcePaths.ThemeFullPath, ResourcePaths.ThemeHandlerAssetName, ResourcePaths.ThemeResourcePath)]
    public sealed class ThemeHandler : SingletonScriptableObject<ThemeHandler>
    {
        public ReactiveProperty<ThemeType> CurrentThemeType { get; } = new();
    }
}