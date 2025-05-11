using System.Text.RegularExpressions;
using UnityEditor;

// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedMember.Global
namespace CustomUtils.Editor.Extensions
{
    public static class StringExtensionsEditor
    {
        /// <summary>
        /// Creates a folder structure in the Unity project based on the provided path.
        /// </summary>
        /// <param name="path">The path where the folders should be created, using forward slashes (/) as separators.</param>
        public static void CreateFolder(this string path)
        {
            var folders = path.Split('/');
            var currentPath = folders[0];

            for (var i = 1; i < folders.Length; i++)
            {
                var parentPath = currentPath;
                currentPath = $"{currentPath}/{folders[i]}";

                if (AssetDatabase.IsValidFolder(currentPath) is false)
                    AssetDatabase.CreateFolder(parentPath, folders[i]);
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Converts a camelCase or PascalCase string to a spaced string.
        /// </summary>
        /// <param name="text">The camelCase or PascalCase string to convert.</param>
        /// <returns>A string with spaces between words.</returns>
        public static string ToSpacedWords(this string text) =>
            string.IsNullOrEmpty(text) ? text : Regex.Replace(text, "([a-z])([A-Z])", "$1 $2");

        /// <summary>
        /// Converts a property name to its backing field representation
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <returns>The backing field name format</returns>
        public static string ConvertToBackingField(this string propertyName)
            => $"<{propertyName}>k__BackingField";
    }
}