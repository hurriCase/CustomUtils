﻿namespace CustomUtils.Runtime
{
    internal static class ResourcePaths
    {
        internal const string CustomMenuResourcePath = "CustomMenu";
        internal const string CustomMenuSettingsAssetName = "CustomMenuSettings";

        internal const string ThemeFullPath = ResourcesPath + "/" + ThemeResourcePath;
        internal const string ThemeHandlerAssetName = "ThemeHandler";
        internal const string ThemeColorDatabaseAssetName = "ThemeColorDatabase";
        internal const string ThemeResourcePath = "Theme";

        internal const string ImagePixelPerUnitFullPath = ResourcesPath + "/" + ImagePixelPerUnitResourcePath;
        internal const string ImagePixelPerUnitDatabaseAssetName = "ImagePixelPerUnitDatabase";
        internal const string ImagePixelPerUnitResourcePath = "ImagePixelPerUnit";

        internal const string LocalizationSettingsFullPath = ResourcesPath + "/" + LocalizationSettingsResourcesPath;
        internal const string LocalizationSettingsAssetName = "LocalizationDatabase";
        internal const string LocalizationSettingsResourcesPath = "CustomLocalization";
        internal const string LocalizationSheetsPath =
            ResourcesPath + "/" + LocalizationSettingsResourcesPath + "/" + "Localization";

        internal const string AssetLoaderConfigFullPath = ResourcesPath + "/" + AssetLoaderConfigResourcesPath;
        internal const string AssetLoaderConfigAssetName = "AssetLoaderConfig";
        internal const string AssetLoaderConfigResourcesPath = "AssetLoader";

        internal const string RootCanvasProviderFullPath = ResourcesPath + "/" + RootCanvasProviderResourcesPath;
        internal const string RootCanvasProviderAssetName = "RootCanvasProvider";
        internal const string RootCanvasProviderResourcesPath = "RootCanvasProvider";

        internal const string ResourceReferencesFullPath = "Assets/CustomUtils/Resources";
        internal const string ResourceReferencesAssetName = "ResourceReferences";

        internal const string LoggerSettingsResourcesPath = "Logger";
        internal const string LoggerSettingsFullPath = ResourcesPath + LoggerSettingsResourcesPath;
        internal const string LoggerSettingsAssetName = "LogCollectorSettings";

        internal const string MappingsPath = "Mappings/";

        private const string ResourcesPath = "Assets/Resources";
    }
}