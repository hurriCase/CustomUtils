namespace CustomUtils.Editor
{
    internal static class MenuItemNames
    {
        internal const string FlatScriptMenuName = UtilsMenuName + "Flat Script";
        internal const string SpriteAlphaAdderMenuName = UtilsMenuName + "Sprite Alpha Adder";
        internal const string FixNPOTMenuName = UtilsMenuName + "Fix NPOT";
        internal const string AnalyzeUnusedAssemblyReferencesMenuName = UtilsMenuName + "Analyze Unused Assembly References";
        internal const string DirtyMakerMenuName = UtilsMenuName + "Dirty Maker";

        internal const string LocalizationMenuName = SettingsMenuName + "Localization";
        internal const string CustomMenuSettingsMenuName = SettingsMenuName + "Custom Menu Settings";
        internal const string LoggerSettingsMenuName = SettingsMenuName + "Logger Settings";

        private const string RootMenuName = "Tools/";
        private const string UtilsMenuName = RootMenuName + "Utils/";

        private const string SettingsMenuName = RootMenuName + "Settings/";
    }
}