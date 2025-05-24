using System;
using System.IO;
using System.Security.Cryptography;
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

        /// <summary>
        /// Attempts to calculate an MD5 hash for the specified file's content.
        /// </summary>
        /// <param name="filePath">The path to the file to hash.</param>
        /// <param name="hash">When this method returns, contains the calculated MD5 hash as a lowercase hex string, or an empty string if the file doesn't exist.</param>
        /// <returns>True if the hash was calculated successfully; otherwise, false.</returns>
        /// <remarks>
        /// The hash is returned as a lowercase hexadecimal string without dashes.
        /// </remarks>
        public static bool TryGetFileContentHash(this string filePath, out string hash)
        {
            hash = string.Empty;
            if (File.Exists(filePath) is false)
                return false;

            using var md5 = MD5.Create();

            using var stream = File.OpenRead(filePath);

            var hashBytes = md5.ComputeHash(stream);
            hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            return true;
        }

        /// <summary>
        /// Attempts to generate a version string for a file based on its last write time and content hash.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="version">When this method returns, contains a version string in the format "yyyyMMdd-[first 8 chars of hash]", or an empty string if the file doesn't exist.</param>
        /// <returns>True if the version was generated successfully; otherwise, false.</returns>
        /// <remarks>
        /// The version combines the file's last modification date with the first 8 characters of its content hash.
        /// </remarks>
        public static bool TryGetFileVersion(this string filePath, out string version)
        {
            version = string.Empty;
            if (filePath.TryGetFileContentHash(out var hash) is false)
                return false;

            var fileInfo = new FileInfo(filePath);

            version = $"{fileInfo.LastWriteTime:yyyyMMdd}-{hash[..8]}";
            return true;
        }
    }
}