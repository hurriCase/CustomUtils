using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomUtils.Editor.CustomMenu.MenuItems.MenuItems.MethodExecution;
using UnityEditor;
using UnityEngine;

namespace CustomUtils.Editor.CustomMenu
{
    internal static class MenuManager
    {
        internal static void GenerateMenuItemsScriptFromSettings(CustomMenuSettings settings)
        {
            var scriptContent = GenerateMenuItemsScriptContentFromSettings(settings);

            if (string.IsNullOrWhiteSpace(scriptContent))
                return;

            WriteScriptFile(scriptContent);
        }

        private static void WriteScriptFile(string scriptContent)
        {
            var scriptPath = "Assets/Editor Default Resources/CustomMenu/Scripts/Editor/GeneratedMenuItems.cs";
            var directory = Path.GetDirectoryName(scriptPath);
            if (string.IsNullOrEmpty(directory) is false && Directory.Exists(directory) is false)
                Directory.CreateDirectory(directory);

            File.WriteAllText(scriptPath, scriptContent);
            AssetDatabase.Refresh();

            Debug.Log("Generated menu items script successfully at: " + scriptPath);
        }

        private static string GenerateMenuItemsScriptContentFromSettings(CustomMenuSettings settings)
        {
            var content = @"using CustomMenu.Editor.MenuItems.Helpers;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CustomMenu.Scripts.Editor
{
    internal static class GeneratedMenuItems
    {";

            var isFirstMenuItem = true;

            var usedMethodNames = new HashSet<string>();
            var usedMenuPaths = new HashSet<string>();

            // Generate Scene Menu Items
            if (settings.SceneMenuItems != null)
                foreach (var item in settings.SceneMenuItems)
                {
                    if (!item.MenuTarget)
                    {
                        Debug.LogError(
                            "[MenuManager::GenerateMenuItemsScriptContent] Scene Asset must be assigned to create Menu Items");
                        return string.Empty;
                    }

                    if (ValidateMenuPath(item.MenuPath) is false)
                        return string.Empty;

                    if (usedMenuPaths.Add(item.MenuPath) is false)
                    {
                        Debug.LogError($"[MenuManager] Duplicate menu path '{item.MenuPath}' " +
                                       $"for scene '{item.SceneName}'. Skipping this item.");
                        continue;
                    }

                    var baseMethodName = $"OpenScene{item.SceneName.Replace(" ", string.Empty)}";
                    var methodName = GetUniqueMethodName(baseMethodName, usedMethodNames);

                    if (isFirstMenuItem)
                        isFirstMenuItem = false;
                    else
                        content += "\n";

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

            // Generate Asset Menu Items
            if (settings.AssetMenuItems != null)
                foreach (var item in settings.AssetMenuItems.Where(assetMenuItem => assetMenuItem.MenuTarget))
                {
                    if (ValidateMenuPath(item.MenuPath) is false)
                        return string.Empty;

                    if (usedMenuPaths.Add(item.MenuPath) is false)
                    {
                        Debug.LogError($"[MenuManager] Duplicate menu path '{item.MenuPath}' " +
                                       $"for asset '{item.MenuTarget.name}'. Skipping this item.");
                        continue;
                    }

                    var baseMethodName = $"SelectAsset{item.MenuTarget.name.Replace(" ", "_")}";
                    var methodName = GetUniqueMethodName(baseMethodName, usedMethodNames);

                    var assetPath = AssetDatabase.GetAssetPath(item.MenuTarget);

                    if (isFirstMenuItem)
                        isFirstMenuItem = false;
                    else
                        content += "\n";

                    content += $@"
        [MenuItem(""{item.MenuPath}"", priority = {item.Priority})]
        private static void {methodName}()
        {{
            var asset = AssetDatabase.LoadAssetAtPath<Object>(""{assetPath}"");
            Selection.activeObject = asset;
        }}";
                }

            // Generate Custom Method Execution Menu Items
            if (settings.MethodExecutionItems != null)
                foreach (var item in settings.MethodExecutionItems)
                {
                    if (ValidateMenuPath(item.MenuPath) is false)
                        return string.Empty;

                    if (usedMenuPaths.Add(item.MenuPath) is false)
                    {
                        Debug.LogError($"[MenuManager] Duplicate menu path '{item.MenuPath}' " +
                                       $"for method '{item.MenuTarget}'. Skipping this item.");
                        continue;
                    }

                    if (isFirstMenuItem)
                        isFirstMenuItem = false;
                    else
                        content += "\n";

                    content += GenerateMethodExecutionContent(item, usedMethodNames);
                }

            // Generate Scripting Symbol Menu Items
            if (settings.ScriptingSymbols != null)
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

                    var baseMethodName = $"ToggleSymbol_{symbol.MenuTarget.Replace(" ", "_")}";
                    var methodName = GetUniqueMethodName(baseMethodName, usedMethodNames);
                    var validateMethodName = $"Validate{methodName}";

                    var prefsKey = symbol.GetPrefsKey();

                    if (isFirstMenuItem)
                        isFirstMenuItem = false;
                    else
                        content += "\n";

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

            content += @"
    }
}";

            return content;
        }

        private static string GenerateMethodExecutionContent(MethodExecutionMenuItem menuItem, HashSet<string> usedMethodNames)
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

        private static bool ValidateMenuPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("[MenuManager] Menu Path cannot be empty");
                return false;
            }

            if (path.Contains('/') is false)
            {
                Debug.LogError($"[MenuManager] Menu path '{path}' should contain a submenu " +
                               "(using forward slash to specify it, e.g. 'Tools/Custom')");
                return false;
            }

            if (path.EndsWith('/'))
            {
                Debug.LogError($"[MenuManager] Menu path '{path}' cannot end with a forward slash");
                return false;
            }

            if (path.StartsWith('/'))
            {
                Debug.LogError($"[MenuManager] Menu path '{path}' cannot start with a forward slash");
                return false;
            }

            if (path.Contains("//") is false)
                return true;

            Debug.LogError($"[MenuManager] Menu path '{path}' contains double slashes which would create empty menu items");
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