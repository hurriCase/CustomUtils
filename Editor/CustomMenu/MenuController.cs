using System;
using System.Collections.Generic;
using System.IO;
using CustomUtils.Editor.CustomMenu.MenuItems.MenuItems;
using CustomUtils.Editor.CustomMenu.MenuItems.MenuItems.MethodExecution;
using UnityEditor;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Editor.CustomMenu
{
    internal static class MenuController
    {
        private const string ScriptPath =
            "Assets/Editor Default Resources/CustomMenu/Scripts/Editor/GeneratedMenuItems.cs";

        internal static void GenerateMenuItemsScriptFromSettings(CustomMenuSettings settings)
        {
            var scriptContent = GenerateMenuItemsScriptContentFromSettings(settings);

            if (string.IsNullOrWhiteSpace(scriptContent))
                return;

            WriteScriptFile(scriptContent);
        }

        private static void WriteScriptFile(string scriptContent)
        {
            var directory = Path.GetDirectoryName(ScriptPath);
            if (string.IsNullOrEmpty(directory) is false && Directory.Exists(directory) is false)
                Directory.CreateDirectory(directory);

            File.WriteAllText(ScriptPath, scriptContent);
            AssetDatabase.Refresh();

            Debug.Log("Generated menu items script successfully at: " + ScriptPath);
        }

        private static string GenerateMenuItemsScriptContentFromSettings(CustomMenuSettings settings)
        {
            var content = @"using CustomUtils.Editor.CustomMenu.MenuItems.Helpers;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor_Default_Resources.CustomMenu.Scripts.Editor
{
    internal static class GeneratedMenuItems
    {";

            var isFirstMenuItem = true;
            var usedMethodNames = new HashSet<string>();
            var usedMenuPaths = new HashSet<string>();

            if (GenerateSceneMenuItems(settings, ref content, ref isFirstMenuItem, usedMethodNames,
                    usedMenuPaths) is false ||
                GenerateAssetMenuItems(settings, ref content, ref isFirstMenuItem, usedMethodNames,
                    usedMenuPaths) is false ||
                GeneratePrefabMenuItems(settings, ref content, ref isFirstMenuItem, usedMethodNames,
                    usedMenuPaths) is false ||
                GenerateMethodExecutionMenuItems(settings, ref content, ref isFirstMenuItem, usedMethodNames,
                    usedMenuPaths) is false ||
                GenerateScriptingSymbolMenuItems(settings, ref content, ref isFirstMenuItem, usedMethodNames,
                    usedMenuPaths) is false)
                return string.Empty;

            content += @"
    }
}";

            return content;
        }

        private static bool GenerateSceneMenuItems(
            CustomMenuSettings settings,
            ref string content,
            ref bool isFirstMenuItem,
            HashSet<string> usedMethodNames,
            HashSet<string> usedMenuPaths)
        {
            if (settings.SceneMenuItems == null)
                return true;

            foreach (var item in settings.SceneMenuItems)
            {
                if (!item.MenuTarget)
                {
                    Debug.LogError(
                        "[MenuManager::GenerateMenuItemsScriptContent] Scene Asset must be assigned to create Menu Items");
                    return false;
                }

                if (ValidateMenuPath(item.MenuPath) is false)
                    return false;

                if (usedMenuPaths.Add(item.MenuPath) is false)
                {
                    Debug.LogError($"[MenuManager] Duplicate menu path '{item.MenuPath}' " +
                                   $"for scene '{item.SceneName}'. Skipping this item.");
                    continue;
                }

                var baseMethodName = $"OpenScene{SanitizeMethodName(item.SceneName)}";
                var methodName = GetUniqueMethodName(baseMethodName, usedMethodNames);

                AddMethodSeparator(ref content, ref isFirstMenuItem);

                content += $@"
        [MenuItem(""{item.MenuPath}"", priority = {item.Priority})]
        private static void {methodName}()
        {{
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() is false)
                return;

            var scenePath = ""{item.ScenePath}"";
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }}";
            }

            return true;
        }

        private static bool GenerateAssetMenuItems(
            CustomMenuSettings settings,
            ref string content,
            ref bool isFirstMenuItem,
            HashSet<string> usedMethodNames,
            HashSet<string> usedMenuPaths)
        {
            if (settings.AssetMenuItems == null)
                return true;

            foreach (var item in settings.AssetMenuItems.AsValueEnumerable()
                         .Where(assetMenuItem => assetMenuItem.MenuTarget))
            {
                if (ValidateMenuPath(item.MenuPath) is false)
                    return false;

                if (usedMenuPaths.Add(item.MenuPath) is false)
                {
                    Debug.LogError($"[MenuManager] Duplicate menu path '{item.MenuPath}' " +
                                   $"for asset '{item.MenuTarget.name}'. Skipping this item.");
                    continue;
                }

                var baseMethodName = $"SelectAsset{SanitizeMethodName(item.MenuTarget.name)}";
                var methodName = GetUniqueMethodName(baseMethodName, usedMethodNames);
                var assetPath = AssetDatabase.GetAssetPath(item.MenuTarget);

                AddMethodSeparator(ref content, ref isFirstMenuItem);

                content += $@"
        [MenuItem(""{item.MenuPath}"", priority = {item.Priority})]
        private static void {methodName}()
        {{
            var asset = AssetDatabase.LoadAssetAtPath<Object>(""{assetPath}"");
            Selection.activeObject = asset;
        }}";
            }

            return true;
        }

        private static bool GeneratePrefabMenuItems(
            CustomMenuSettings settings,
            ref string content,
            ref bool isFirstMenuItem,
            HashSet<string> usedMethodNames,
            HashSet<string> usedMenuPaths)
        {
            if (settings.PrefabMenuItems == null)
                return true;

            foreach (var item in settings.PrefabMenuItems.AsValueEnumerable()
                         .Where(prefabMenuItem => prefabMenuItem.MenuTarget))
            {
                if (ValidateMenuPath(item.MenuPath) is false)
                    return false;

                if (usedMenuPaths.Add(item.MenuPath) is false)
                {
                    Debug.LogError($"[MenuManager] Duplicate menu path '{item.MenuPath}' " +
                                   $"for prefab '{item.MenuTarget.name}'. Skipping this item.");
                    continue;
                }

                var baseMethodName = $"Create{SanitizeMethodName(item.MenuTarget.name)}";
                var methodName = GetUniqueMethodName(baseMethodName, usedMethodNames);
                var prefabPath = AssetDatabase.GetAssetPath(item.MenuTarget);

                AddMethodSeparator(ref content, ref isFirstMenuItem);

                content += GeneratePrefabMenuItemContent(item, methodName, prefabPath);
            }

            return true;
        }

        private static bool GenerateMethodExecutionMenuItems(
            CustomMenuSettings settings,
            ref string content,
            ref bool isFirstMenuItem,
            HashSet<string> usedMethodNames,
            HashSet<string> usedMenuPaths)
        {
            if (settings.MethodExecutionItems == null)
                return true;

            foreach (var item in settings.MethodExecutionItems)
            {
                if (ValidateMenuPath(item.MenuPath) is false)
                    return false;

                if (usedMenuPaths.Add(item.MenuPath) is false)
                {
                    Debug.LogError($"[MenuManager] Duplicate menu path '{item.MenuPath}' " +
                                   $"for method '{item.MenuTarget}'. Skipping this item.");
                    continue;
                }

                AddMethodSeparator(ref content, ref isFirstMenuItem);

                content += GenerateMethodExecutionContent(item, usedMethodNames);
            }

            return true;
        }

        private static bool GenerateScriptingSymbolMenuItems(
            CustomMenuSettings settings,
            ref string content,
            ref bool isFirstMenuItem,
            HashSet<string> usedMethodNames,
            HashSet<string> usedMenuPaths)
        {
            if (settings.ScriptingSymbols == null)
                return true;

            foreach (var symbol in settings.ScriptingSymbols)
            {
                if (string.IsNullOrEmpty(symbol.MenuTarget))
                {
                    Debug.LogError("[MenuManager] Symbol name cannot be empty");
                    continue;
                }

                if (ValidateMenuPath(symbol.MenuPath) is false)
                    continue;

                if (usedMenuPaths.Add(symbol.MenuPath) is false)
                {
                    Debug.LogError($"[MenuManager] Duplicate menu path '{symbol.MenuPath}' " +
                                   $"for symbol '{symbol.MenuTarget}'. Skipping this item.");
                    continue;
                }

                var baseMethodName = $"ToggleSymbol_{SanitizeMethodName(symbol.MenuTarget)}";
                var methodName = GetUniqueMethodName(baseMethodName, usedMethodNames);
                var validateMethodName = $"Validate{methodName}";
                var prefsKey = symbol.GetPrefsKey();

                AddMethodSeparator(ref content, ref isFirstMenuItem);

                content += $@"
        [MenuItem(""{symbol.MenuPath}"", priority = {symbol.Priority})]
        private static void {methodName}()
        {{
            ScriptingSymbolHandler.ToggleSymbol(""{symbol.MenuTarget}"", ""{prefsKey}"");
        }}

        [MenuItem(""{symbol.MenuPath}"", true)]
        private static bool {validateMethodName}()
        {{
            Menu.SetChecked(""{symbol.MenuPath}"", ScriptingSymbolHandler.IsSymbolEnabled(""{prefsKey}"", false));
            return true;
        }}";
            }

            return true;
        }

        private static void AddMethodSeparator(ref string content, ref bool isFirstMenuItem)
        {
            if (isFirstMenuItem)
                isFirstMenuItem = false;
            else
                content += "\n";
        }

        private static string GeneratePrefabMenuItemContent(
            PrefabMenuItem item,
            string methodName,
            string prefabPath)
        {
            var content = $@"
        [MenuItem(""{item.MenuPath}"", priority = {item.Priority})]
        private static void {methodName}(MenuCommand menuCommand)
        {{
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(""{prefabPath}"");

            if (!prefab)
            {{
                Debug.LogError(""Prefab not found at path: {prefabPath}"");
                return;
            }}

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            if (!instance)
            {{
                Debug.LogError(""Failed to instantiate prefab"");
                return;
            }}

            GameObjectUtility.SetParentAndAlign(instance, menuCommand.context as GameObject);

            Undo.RegisterCreatedObjectUndo(instance, ""Create "" + instance.name);

            Selection.activeObject = instance;
        }}";

            return content;
        }

        private static string GenerateMethodExecutionContent(MethodExecutionMenuItem menuItem,
            HashSet<string> usedMethodNames)
        {
            var baseMethodName = menuItem.MenuTarget.ToString();
            var methodName = GetUniqueMethodName(baseMethodName, usedMethodNames);

            return menuItem.MenuTarget switch
            {
                MethodExecutionType.DeleteAllPlayerPrefs => $@"
        [MenuItem(""{menuItem.MenuPath}"", priority = {menuItem.Priority})]
        private static void {methodName}()
        {{
            PlayerPrefs.DeleteAll();
            Debug.Log(""All PlayerPrefs deleted."");
        }}",

                MethodExecutionType.ToggleDefaultSceneAutoLoad => $@"
        [MenuItem(""{menuItem.MenuPath}"", priority = {menuItem.Priority})]
        private static void {methodName}()
        {{
            DefaultSceneLoader.ToggleAutoLoad();
        }}

        [MenuItem(""{menuItem.MenuPath}"", true)]
        private static bool Validate{methodName}()
        {{
            Menu.SetChecked(""{menuItem.MenuPath}"", DefaultSceneLoader.IsDefaultSceneSet());
            return true;
        }}",

                _ => throw new ArgumentOutOfRangeException(nameof(menuItem.MenuTarget), menuItem.MenuTarget, null)
            };
        }

        private static string SanitizeMethodName(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "Empty";

            var sanitized = input
                .Replace(" ", "_")
                .Replace(".", "_")
                .Replace("-", "_")
                .Replace("+", "Plus")
                .Replace("&", "And")
                .Replace("@", "At")
                .Replace("#", "Hash")
                .Replace("$", "Dollar")
                .Replace("%", "Percent")
                .Replace("^", "Caret")
                .Replace("*", "Star")
                .Replace("(", "_")
                .Replace(")", "_")
                .Replace("[", "_")
                .Replace("]", "_")
                .Replace("{", "_")
                .Replace("}", "_")
                .Replace("|", "_")
                .Replace("\\", "_")
                .Replace("/", "_")
                .Replace("?", "_")
                .Replace("<", "_")
                .Replace(">", "_")
                .Replace(",", "_")
                .Replace(";", "_")
                .Replace(":", "_")
                .Replace("'", "_")
                .Replace("\"", "_")
                .Replace("!", "_")
                .Replace("~", "_")
                .Replace("`", "_")
                .Replace("=", "_");

            while (sanitized.Contains("__"))
            {
                sanitized = sanitized.Replace("__", "_");
            }

            sanitized = sanitized.Trim('_');

            if (string.IsNullOrEmpty(sanitized) is false && char.IsDigit(sanitized[0]))
                sanitized = "_" + sanitized;

            return string.IsNullOrEmpty(sanitized) ? "Method" : sanitized;
        }

        private static bool ValidateMenuPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("[MenuController::ValidateMenuPath] Menu Path cannot be empty");
                return false;
            }

            if (path.Contains('/') is false)
            {
                Debug.LogError($"[MenuController::ValidateMenuPath] Menu path '{path}' should contain a submenu " +
                               "(using forward slash to specify it, e.g. 'Tools/Custom')");
                return false;
            }

            if (path.EndsWith('/'))
            {
                Debug.LogError("[MenuController::ValidateMenuPath] " +
                               $"Menu path '{path}' cannot end with a forward slash");
                return false;
            }

            if (path.StartsWith('/'))
            {
                Debug.LogError("[MenuController::ValidateMenuPath] " +
                               $"Menu path '{path}' cannot start with a forward slash");
                return false;
            }

            if (path.Contains("//") is false)
                return true;

            Debug.LogError("[MenuController::ValidateMenuPath] " +
                           $"Menu path '{path}' contains double slashes which would create empty menu items");
            return false;
        }

        private static string GetUniqueMethodName(string baseName, HashSet<string> usedNames)
        {
            var methodName = baseName;
            var suffix = 1;

            while (usedNames.Contains(methodName))
            {
                methodName = $"{baseName}_{suffix}";
                suffix++;
            }

            usedNames.Add(methodName);
            return methodName;
        }
    }
}