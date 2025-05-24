// Put in the runtime assembly due to SingletonScriptableObject

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using ZLinq;
using Object = UnityEngine.Object;

namespace CustomUtils.Runtime.AssetLoader
{
    /// <summary>
    /// Generic utility class for loading Unity resources from multiple sources.
    /// Attempts to load resources from Editor Default Resources, standard Resources folders,
    /// and falls back to AssetDatabase searches when needed.
    /// </summary>
    /// <typeparam name="TResource">The type of resource to load, must inherit from UnityEngine.Object.</typeparam>
    [UsedImplicitly]
    public static class EditorLoader<TResource> where TResource : Object
    {
        /// <summary>
        /// Loads a resource of type TResource using multiple loading strategies.
        /// First tries EditorGUIUtility.Load for Editor Default Resources,
        /// then tries Resources.Load for standard Resources folders,
        /// then tries AssetDatabase.LoadAssetAtPath with the provided full path,
        /// and finally searches the entire project for assets of the specified type.
        /// </summary>
        /// <param name="resourcePath">The resource path. If null, a path is determined from ResourceAttribute.</param>
        /// <param name="fullPath">The complete asset path including file extension for AssetDatabase loading.</param>
        /// <returns>The loaded resource or null if the resource could not be found.</returns>
        [UsedImplicitly]
        public static TResource Load(string resourcePath = null, string fullPath = null)
        {
            if (PathUtility.TryGetResourcePath<TResource>(ref resourcePath) is false)
                return null;

            var resource = EditorGUIUtility.Load(resourcePath) as TResource;

            if (resource || ResourceLoader<TResource>.TryLoad(out resource, resourcePath))
                return resource;

            if (string.IsNullOrEmpty(fullPath) is false)
            {
                resource = AssetDatabase.LoadAssetAtPath<TResource>(fullPath);

                if (resource)
                    return resource;
            }

            var guids = AssetDatabase.FindAssets($"t:{nameof(TResource)}");
            if (guids.Length <= 0)
                return null;

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<TResource>(path);
        }

        /// <summary>
        /// Attempts to load a resource using the same multi-strategy approach as Load().
        /// </summary>
        /// <param name="resource">When this method returns, contains the loaded resource if found; otherwise, the default value for the type.</param>
        /// <param name="resourcePath">The resource path without file extension. If null, a path is determined from ResourceAttribute.</param>
        /// <param name="fullPath">The complete asset path including file extension for AssetDatabase loading.</param>
        /// <returns>True if the resource was successfully loaded; otherwise, false.</returns>
        [UsedImplicitly]
        public static bool TryLoad(out TResource resource, string resourcePath = null, string fullPath = null)
        {
            resource = Load(resourcePath, fullPath);
            return resource;
        }

        /// <summary>
        /// Loads all resources of type TResource from the specified path.
        /// Searches in Editor Default Resources or use AssetDatabase to find assets in the specified directory.
        /// </summary>
        /// <param name="path">The path to load the resources from.</param>
        /// <returns>An array of loaded resources or null if no resources were found.</returns>
        [UsedImplicitly]
        public static TResource[] LoadAll(string path)
        {
            var resources = LoadAllFromEditorResources(path);

            if (resources != null && resources.Length != 0)
                return resources;

            Debug.LogWarning($"[EditorLoader::LoadAll] No editor resources found at path: {path}");
            return null;
        }

        /// <summary>
        /// Attempts to load all resources of type TResource from the specified path.
        /// </summary>
        /// <param name="resourcePath">The path to load the resources from.</param>
        /// <param name="resources">When this method returns, contains the loaded resources if found; otherwise, null.</param>
        /// <returns>True if resources were successfully loaded; otherwise, false.</returns>
        [UsedImplicitly]
        public static bool TryLoadAll(string resourcePath, out TResource[] resources)
            => (resources = LoadAll(resourcePath)) != null;

        /// <summary>
        /// Internal method to load all resources from either a specific directory path or Editor Default Resources.
        /// If fullPath exists as a directory, search within that directory.
        /// Otherwise, searches in Editor Default Resources for assets matching the resourcePath.
        /// </summary>
        /// <param name="resourcePath">The resource path to search for within Editor Default Resources.</param>
        /// <param name="fullPath">Optional full directory path to search within. If null, searches Editor Default Resources.</param>
        /// <returns>Array of loaded resources or null if none found.</returns>
        private static TResource[] LoadAllFromEditorResources(string resourcePath, string fullPath = null)
        {
            if (PathUtility.TryGetResourcePath<TResource>(ref resourcePath) is false)
                return null;

            var resources = new List<TResource>();

            if (Directory.Exists(fullPath))
            {
                var assetGuids = AssetDatabase.FindAssets($"t:{typeof(TResource).Name}", new[] { fullPath });

                resources.AddRange(assetGuids.AsValueEnumerable()
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<TResource>)
                    .Where(asset => asset)
                    .ToArray());
            }
            else
            {
                var searchPaths = new[] { "Assets/Editor Default Resources" };
                var assetGuids = AssetDatabase.FindAssets($"t:{typeof(TResource).Name}", searchPaths);

                resources.AddRange(assetGuids
                    .AsValueEnumerable()
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Where(assetPathFromGuid =>
                        assetPathFromGuid.Contains(resourcePath, StringComparison.OrdinalIgnoreCase))
                    .Select(AssetDatabase.LoadAssetAtPath<TResource>)
                    .Where(asset => asset)
                    .ToArray());
            }

            return resources.Count > 0 ? resources.ToArray() : null;
        }
    }
}
#endif