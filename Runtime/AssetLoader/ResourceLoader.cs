using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable MemberCanBeInternal
namespace CustomUtils.Runtime.AssetLoader
{
    /// <summary>
    /// Generic utility class for loading Unity resources with automatic caching capabilities.
    /// </summary>
    /// <typeparam name="TResource">The type of resource to load, must inherit from UnityEngine.Object.</typeparam>
    public static class ResourceLoader<TResource> where TResource : Object
    {
        private static readonly Dictionary<string, TResource> _resourceCache = new();
        private static readonly Dictionary<string, TResource[]> _resourceArrayCache = new();

        /// <summary>
        /// Loads a resource of type TResource from the specified path with caching support.
        /// If no path is provided, it will attempt to determine the path from ResourceAttribute.
        /// </summary>
        /// <param name="path">The path to load the resource from. If null, path is determined from ResourceAttribute.</param>
        /// <returns>The loaded resource or null if the resource could not be found.</returns>
        public static TResource Load(string path = null)
        {
            var cacheKey = GetCacheKey(path ?? GetPath());

            if (_resourceCache.TryGetValue(cacheKey, out var cached))
                return cached;

            var resource = Resources.Load<TResource>(path ?? GetPath());
            if (!resource)
                Debug.LogWarning($"[ResourceLoader::Load] Failed to load resource at path: {path ?? GetPath()}");

            _resourceCache[cacheKey] = resource;
            return resource;
        }

        /// <summary>
        /// Attempts to load a resource and returns whether the operation was successful.
        /// </summary>
        /// <param name="resource">When this method returns, contains the loaded resource if found; otherwise, the default value for the type.</param>
        /// <param name="path">The path to load the resource from. If null, path is determined from ResourceAttribute.</param>
        /// <returns>True if the resource was successfully loaded; otherwise, false.</returns>
        public static bool TryLoad(out TResource resource, string path = null) => resource = Load(path);

        /// <summary>
        /// Loads all resources of type TResource from the specified path with caching support.
        /// </summary>
        /// <param name="path">The path to load the resources from.</param>
        /// <returns>An array of loaded resources or null if no resources were found.</returns>
        public static TResource[] LoadAll(string path)
        {
            if (_resourceArrayCache.TryGetValue(path, out var cached))
                return cached;

            var resources = Resources.LoadAll<TResource>(path);
            if (resources == null || resources.Length == 0)
            {
                Debug.LogWarning($"[ResourceLoader::LoadAll] No resources found at path: {path}");
                return null;
            }

            _resourceArrayCache[path] = resources;
            return resources;
        }

        /// <summary>
        /// Attempts to load all resources of type TResource from the specified path.
        /// </summary>
        /// <param name="path">The path to load the resources from.</param>
        /// <param name="resources">When this method returns, contains the loaded resources if found; otherwise, null.</param>
        /// <returns>True if resources were successfully loaded; otherwise, false.</returns>
        public static bool TryLoadAll(string path, out TResource[] resources)
            => (resources = LoadAll(path)) != null;

        /// <summary>
        /// Asynchronously loads a resource of type TResource from the specified path with caching support.
        /// If no path is provided, it will attempt to determine the path from ResourceAttribute.
        /// </summary>
        /// <param name="path">The path to load the resource from. If null, path is determined from ResourceAttribute.</param>
        /// <param name="cancellationToken"> Optional cancellation token to stop loading</param>
        /// <returns>A UniTask that represents the asynchronous load operation.
        /// The result contains the loaded resource or null if not found.</returns>
        public static async UniTask<TResource> LoadAsync(string path = null,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = GetCacheKey(path ?? GetPath());

            if (_resourceCache.TryGetValue(cacheKey, out var cached))
                return cached;

            var resourceRequest = Resources.LoadAsync<TResource>(path ?? GetPath());

            await resourceRequest.ToUniTask(cancellationToken: cancellationToken);

            var resource = resourceRequest.asset as TResource;
            if (!resource)
                Debug.LogWarning($"[ResourceLoader::LoadAsync] Failed to load resource at path: {path ?? GetPath()}");

            _resourceCache[cacheKey] = resource;
            return resource;
        }

        /// <summary>
        /// Clears all cached resources, both individual and arrays.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearCache()
        {
            _resourceCache.Clear();
            _resourceArrayCache.Clear();
        }

        /// <summary>
        /// Removes the default resource (determined by ResourceAttribute) from both caches.
        /// </summary>
        public static void RemoveFromCache()
        {
            var cacheKey = GetCacheKey(GetPath());
            _resourceCache.Remove(cacheKey);
            _resourceArrayCache.Remove(cacheKey);

#if UNITY_EDITOR
            _resourceCache.Remove($"Editor:{cacheKey}");
#endif
        }

        /// <summary>
        /// Gets a cache key for the specified path.
        /// </summary>
        /// <param name="path">The resource path.</param>
        /// <returns>A cache key string.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetCacheKey(string path) => $"{typeof(TResource).Name}:{ValidatePath(path)}";

        /// <summary>
        /// Validates the specified path.
        /// </summary>
        /// <param name="path">The path to validate.</param>
        /// <returns>The validated path or null if invalid.</returns>
        private static string ValidatePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Debug.LogError("[ResourceLoader::ValidatePath] Path cannot be null or empty");
                return null;
            }

            if (path.Contains("..") is false)
                return path.TrimStart('/');

            Debug.LogError("[ResourceLoader::ValidatePath] Path cannot contain parent directory references");
            return null;
        }

        /// <summary>
        /// Gets the resource path based on the ResourceAttribute of TResource.
        /// </summary>
        /// <returns>The resource path or null if no ResourceAttribute is found.</returns>
        private static string GetPath()
        {
            var type = typeof(TResource);

            if (Attribute.GetCustomAttribute(type, typeof(ResourceAttribute)) is ResourceAttribute attribute)
                return attribute.TryGetFullResourcePath(out var fullResourcePath)
                    ? fullResourcePath
                    : typeof(TResource).Name;

            Debug.LogWarning($"[ResourceLoader::GetPath] No ResourceAttribute found on type {type.Name}");
            return null;
        }
    }
}