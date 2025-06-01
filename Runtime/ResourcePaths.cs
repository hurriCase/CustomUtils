namespace CustomUtils.Runtime
{
    internal static class ResourcePaths
    {
        internal const string CustomMenuResourcePath = "CustomMenu";
        internal const string CustomMenuSettingsAssetName = "CustomMenuSettings";

        internal const string EditorThemeResourcePath = "EditorTheme";
        internal const string EditorThemeSettingsAssetName = "EditorThemeSettings";

        internal const string ThemeFullPath = ResourcesPath + "/" + ThemeResourcePath;
        internal const string ThemeHandlerAssetName = "ThemeHandler";
        internal const string ThemeColorDatabaseAssetName = "ThemeColorDatabase";
        internal const string ThemeResourcePath = "Theme";

        internal const string LocalizationSettingsFullPath = ResourcesPath + "/" + LocalizationSettingsResourcesPath;
        internal const string LocalizationSettingsAssetName = "LocalizationSettings";
        internal const string LocalizationSettingsResourcesPath = "CustomLocalization";

        internal const string AssetLoaderConfigFullPath = ResourcesPath + "/" + AssetLoaderConfigResourcesPath;
        internal const string AssetLoaderConfigAssetName = "AssetLoaderConfig";
        internal const string AssetLoaderConfigResourcesPath = "AssetLoader";

        internal const string LocalizationsFolderPath = "CustomLocalization/Localization";

        internal const string LoggerSettingsResourcesPath = "Logger";
        internal const string LoggerSettingsFullPath = ResourcesPath + LoggerSettingsResourcesPath;
        internal const string LoggerSettingsAssetName = "LogCollectorSettings";

        private const string ResourcesPath = "Assets/Resources";
    }
}