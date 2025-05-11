using System;

namespace CustomUtils.Editor.CustomMenu.MenuItems.MenuItems
{
    [Serializable]
    internal sealed class ScriptingSymbolMenuItem : BaseMenuItem<string>
    {
        internal string GetPrefsKey() => $"ScriptingSymbol_{MenuTarget}";
    }
}