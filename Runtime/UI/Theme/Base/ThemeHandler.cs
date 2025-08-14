using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.CustomTypes.Singletons;
using JetBrains.Annotations;
using R3;

namespace CustomUtils.Runtime.UI.Theme.Base
{
    [Resource(ResourcePaths.ThemeFullPath, ResourcePaths.ThemeHandlerAssetName, ResourcePaths.ThemeResourcePath)]
    public sealed class ThemeHandler : SingletonScriptableObject<ThemeHandler>
    {
        [UsedImplicitly]
        public ReactiveProperty<ThemeType> CurrentThemeType { get; } = new(ThemeType.Light);
    }
}