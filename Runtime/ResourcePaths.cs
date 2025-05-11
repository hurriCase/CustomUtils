namespace CustomUtils.Runtime
{
    internal static class ResourcePaths
    {
        internal const string CustomMenuPath = EditorPath + "CustomMenu/";
        internal const string CustomMenuSettingsFileName = CustomMenuPath + "CustomMenuSettings";

        internal const string LocalizationsFolderPath = "CustomLocalization/Localization";

        internal const string LocalizationSettingsResourcePath = "CustomLocalization";
        internal const string LocalizationSettingsFullPath = "Assets/Resources" + LocalizationSettingsResourcePath;
        internal const string LocalizationSettingsAssetName = "LocalizationSettings";

        internal const string DontDestroyOnLoadPath = "DontDestroyOnLoad";
        internal const string PrefabPrefix = "P_";

        private const string EditorPath = "Assets/Editor Default Resources/";
    }
}