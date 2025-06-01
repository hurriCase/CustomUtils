namespace CustomUtils.Runtime
{
    internal static class ResourcePaths
    {
        internal const string CustomMenuResourcePath = "CustomMenu";
        internal const string CustomMenuSettingsAssetName = "CustomMenuSettings";

        internal const string EditorThemeResourcePath = "EditorTheme";
        internal const string EditorThemeSettingsAssetName = "EditorThemeSettings";

        internal const string ThemHandlerResourcePath = "Theme";
        internal const string ThemeHandlerFullPath = ThemHandlerResourcePath + ThemeHandlerAssetName;
        internal const string ThemeHandlerAssetName = "ThemeHandler";

        internal const string LocalizationSettingsResourcesPath = "CustomLocalization";
        internal const string LocalizationSettingsFullPath = ResourcesPath + LocalizationSettingsResourcesPath;
        internal const string LocalizationSettingsAssetName = "LocalizationSettings";

        internal const string LocalizationsFolderPath = "CustomLocalization/Localization";

        internal const string LoggerSettingsResourcesPath = "Logger";
        internal const string LoggerSettingsFullPath = ResourcesPath + LoggerSettingsResourcesPath;
        internal const string LoggerSettingsAssetName = "LogCollectorSettings";

        internal const string DontDestroyOnLoadPath = "DontDestroyOnLoad";
        internal const string PrefabPrefix = "P_";

        private const string ResourcesPath = "Assets/Resources";
    }
}