using System.Text.RegularExpressions;
using JetBrains.Annotations;
using UnityEditor;

// ReSharper disable MemberCanBeInternal
namespace CustomUtils.Editor.Extensions
{
    public static class StringExtensionsEditor
    {
        /// <summary>
        /// Creates a folder structure in the Unity project based on the provided path.
        /// </summary>
        /// <param name="path">The path where the folders should be created, using forward slashes (/) as separators.</param>
        [UsedImplicitly]
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
        /// Converts a property name to its backing field representation
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <returns>The backing field name format</returns>
        public static string ConvertToBackingField(this string propertyName)
            => $"<{propertyName}>k__BackingField";
    }
}