using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// ReSharper disable MemberCanBeInternal
namespace CustomUtils.Runtime.AssetLoader
{
    /// <summary>
    /// Specialized utility class for loading ScriptableObject resources.
    /// </summary>
    /// <typeparam name="TScriptableObject">The type of ScriptableObject to load.</typeparam>
    [UsedImplicitly]
    public static class ScriptableObjectLoader<TScriptableObject> where TScriptableObject : ScriptableObject
    {
        /// <summary>
        /// Loads a ScriptableObject resource using the standard ResourceLoader.
        /// </summary>
        /// <param name="path">Optional path to load from.</param>
        /// <returns>The loaded ScriptableObject or null if not found.</returns>
        [UsedImplicitly]
        public static TScriptableObject Load(string path = null) => ResourceLoader<TScriptableObject>.Load(path);

#if UNITY_EDITOR
        /// <summary>
        /// Loads a ScriptableObject from Editor Default Resources.
        /// </summary>
        /// <returns>The loaded ScriptableObject or null if not found.</returns>
        [UsedImplicitly]
        public static TScriptableObject LoadEditorResource()
        {
            var attribute = typeof(TScriptableObject).GetCustomAttribute<ResourceAttribute>();
            if (attribute is not { IsEditorResource: true })
            {
                Debug.LogWarning("[ScriptableObjectResourceLoader::LoadEditorResource] " +
                                 $"{typeof(TScriptableObject).Name} is not marked as an editor resource");
                return null;
            }

            var editorPath = attribute.GetEditorResourcePath();
            TScriptableObject resource = null;

            if (string.IsNullOrEmpty(editorPath) is false)
                resource = EditorGUIUtility.Load(editorPath) as TScriptableObject;

            if (resource)
                return resource;

            var assetPath = GetAssetPath(attribute);
            resource = AssetDatabase.LoadAssetAtPath<TScriptableObject>(assetPath);

            return resource;
        }

        /// <summary>
        /// Creates a new instance of the ScriptableObject and saves it to the appropriate path.
        /// </summary>
        /// <returns>The created ScriptableObject instance.</returns>
        [UsedImplicitly]
        public static TScriptableObject CreateAndSaveAsset()
        {
            var attribute = typeof(TScriptableObject).GetCustomAttribute<ResourceAttribute>();
            if (attribute == null)
            {
                Debug.LogWarning("[ScriptableObjectResourceLoader::CreateAndSaveAsset] " +
                                 $"{typeof(TScriptableObject).Name} missing ResourceAttribute");
                return null;
            }

            var assetPath = GetAssetPath(attribute);

            var existingAsset = AssetDatabase.LoadAssetAtPath<TScriptableObject>(assetPath);
            if (existingAsset)
                return existingAsset;

            var directory = Path.GetDirectoryName(assetPath);
            if (string.IsNullOrEmpty(directory) is false && Directory.Exists(directory) is false)
                Directory.CreateDirectory(directory);

            var resource = ScriptableObject.CreateInstance<TScriptableObject>();

            Debug.Log($"Creating asset at path: {assetPath}");

            AssetDatabase.CreateAsset(resource, assetPath);
            AssetDatabase.SaveAssets();

            ResourceLoader<TScriptableObject>.RemoveFromCache();

            return resource;
        }

        /// <summary>
        /// Clears the resource cache.
        /// </summary>
        [UsedImplicitly]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearCache() => ResourceLoader<TScriptableObject>.ClearCache();

        /// <summary>
        /// Removes the ScriptableObject from cache.
        /// </summary>
        [UsedImplicitly]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveFromCache() => ResourceLoader<TScriptableObject>.RemoveFromCache();

        /// <summary>
        /// Gets the full asset path for the ScriptableObject.
        /// </summary>
        /// <param name="attribute">The ResourceAttribute to get the path from.</param>
        /// <returns>The full asset path.</returns>
        private static string GetAssetPath(ResourceAttribute attribute)
        {
            var resourceFolderName = attribute.IsEditorResource ? "Editor Default Resources" : "Resources";

            return string.IsNullOrEmpty(attribute.FullPath)
                ? $"Assets/{resourceFolderName}/{attribute.Name}.asset"
                : $"Assets/{resourceFolderName}/{attribute.FullPath}/{attribute.Name}.asset";
        }
#endif
    }
}