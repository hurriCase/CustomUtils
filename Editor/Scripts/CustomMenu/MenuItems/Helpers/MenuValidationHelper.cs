using System.Collections.Generic;
using UnityEngine;

namespace CustomUtils.Editor.Scripts.CustomMenu.MenuItems.Helpers
{
    internal static class MenuValidationHelper
    {
        internal static bool Validate<T>(T menuTarget, string menuPath, string itemName, string itemType,
            HashSet<string> usedMenuPaths) =>
            ValidateMenuTarget(menuTarget)
            && ValidateMenuPath(menuPath)
            && ValidateAndAddUniqueMenuPath(menuPath, itemName, itemType, usedMenuPaths);

        private static bool ValidateMenuTarget<T>(T menuTarget)
        {
            if (menuTarget != null && (menuTarget is not Object @object || @object))
                return true;

            Debug.LogError(
                "[MenuController::GenerateSceneMenuItems] Scene Asset must be assigned to create Menu Items");
            return false;
        }

        private static bool ValidateMenuPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("[MenuValidationHelper::ValidateMenuPath] Menu Path cannot be empty");
                return false;
            }

            if (!path.Contains('/'))
            {
                Debug.LogError(
                    $"[MenuValidationHelper::ValidateMenuPath] Menu path '{path}' should contain a submenu " +
                    "(using forward slash to specify it, e.g. 'Tools/Custom')");
                return false;
            }

            if (path.EndsWith('/'))
            {
                Debug.LogError(
                    $"[MenuValidationHelper::ValidateMenuPath] Menu path '{path}' cannot end with a forward slash");
                return false;
            }

            if (path.StartsWith('/'))
            {
                Debug.LogError(
                    $"[MenuValidationHelper::ValidateMenuPath] Menu path '{path}' cannot start with a forward slash");
                return false;
            }

            if (!path.Contains("//"))
                return true;

            Debug.LogError(
                $"[MenuValidationHelper::ValidateMenuPath] Menu path '{path}' contains double slashes which would create empty menu items");
            return false;
        }

        private static bool ValidateAndAddUniqueMenuPath(string menuPath, string itemName, string itemType,
            HashSet<string> usedMenuPaths)
        {
            if (usedMenuPaths.Add(menuPath))
                return true;

            Debug.LogError($"[MenuValidationHelper] Duplicate menu path '{menuPath}' " +
                           $"for {itemType} '{itemName}'.");
            return false;
        }

        internal static string GetUniqueMethodName(string baseName, HashSet<string> usedNames)
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

        internal static string SanitizeMethodName(string input)
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

            if (!string.IsNullOrEmpty(sanitized) && char.IsDigit(sanitized[0]))
                sanitized = "_" + sanitized;

            return string.IsNullOrEmpty(sanitized) ? "Method" : sanitized;
        }
    }
}