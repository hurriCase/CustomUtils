using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using ZLinq;

// ReSharper disable UnusedMember.Global
namespace CustomUtils.Editor.CustomMenu.MenuItems.Helpers
{
    public static class ScriptingSymbolHandler
    {
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.delayCall += SyncAllSymbols;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static void SyncAllSymbols()
        {
            var settings = CustomMenuSettings.Instance;
            if (settings.ScriptingSymbols is null || settings.ScriptingSymbols.Length == 0)
                return;

            foreach (var symbol in settings.ScriptingSymbols)
                SyncSymbolWithPrefs(symbol.MenuTarget, symbol.GetPrefsKey());
        }

        public static void ToggleSymbol(string symbolName, string prefsKey)
        {
            var isEnabled = EditorPrefs.GetBool(prefsKey, false);
            isEnabled = isEnabled is false;
            EditorPrefs.SetBool(prefsKey, isEnabled);

            if (isEnabled)
            {
                AddDefineSymbol(symbolName);
                Debug.Log($"[ScriptingSymbolHandler] Symbol '{symbolName}' enabled");
            }
            else
            {
                RemoveDefineSymbol(symbolName);
                Debug.Log($"[ScriptingSymbolHandler] Symbol '{symbolName}' disabled");
            }
        }

        public static bool IsSymbolEnabled(string prefsKey, bool defaultValue = false) =>
            EditorPrefs.GetBool(prefsKey, defaultValue);

        private static void SyncSymbolWithPrefs(string symbolName, string prefsKey)
        {
            var isEnabled = EditorPrefs.GetBool(prefsKey, false);
            var currentBuildTarget =
                NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            var currentDefines = PlayerSettings.GetScriptingDefineSymbols(currentBuildTarget);
            var symbolDefined = currentDefines.Contains(symbolName);

            switch (isEnabled)
            {
                case true when symbolDefined is false:
                    AddDefineSymbol(symbolName);
                    break;

                case false when symbolDefined:
                    RemoveDefineSymbol(symbolName);
                    break;
            }
        }

        private static void AddDefineSymbol(string symbolToAdd)
        {
            if (string.IsNullOrEmpty(symbolToAdd))
                return;

            var currentBuildTarget = NamedBuildTarget.FromBuildTargetGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup);

            var currentDefines = PlayerSettings.GetScriptingDefineSymbols(currentBuildTarget);

            if (currentDefines.Contains(symbolToAdd))
                return;

            var updatedDefines = string.IsNullOrEmpty(currentDefines)
                ? symbolToAdd
                : currentDefines + ";" + symbolToAdd;

            PlayerSettings.SetScriptingDefineSymbols(currentBuildTarget, updatedDefines);
        }

        private static void RemoveDefineSymbol(string symbolToRemove)
        {
            if (string.IsNullOrEmpty(symbolToRemove))
                return;

            var currentBuildTarget = NamedBuildTarget.FromBuildTargetGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup);

            var currentDefines = PlayerSettings.GetScriptingDefineSymbols(currentBuildTarget);

            if (currentDefines.Contains(symbolToRemove) is false)
                return;

            var definesList = currentDefines.Split(';');

            var updatedDefines = string.Join(";", definesList.AsValueEnumerable()
                .Where(defineSymbol => defineSymbol != symbolToRemove)
                .ToArray());

            PlayerSettings.SetScriptingDefineSymbols(currentBuildTarget, updatedDefines);
        }
    }
}