using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomUtils.Runtime.AssetLoader
{
    public static class ResourceLoader<T> where T : Object
    {
        private static readonly Dictionary<string, T> _resourceCache = new();
        private static readonly Dictionary<string, T[]> _resourceArrayCache = new();

        public static T Load(string path = null)
        {
            var cacheKey = GetCacheKey(path ?? GetPath());

            if (_resourceCache.TryGetValue(cacheKey, out var cached))
                return cached;

            var resource = Resources.Load<T>(path ?? GetPath());
            if (resource is null)
                Debug.LogWarning($"[ResourceLoader::Load] Failed to load resource at path: {path ?? GetPath()}");

            _resourceCache[cacheKey] = resource;
            return resource;
        }

        public static bool TryLoad(out T resource, string path = null)
            => resource = Load(path);

        public static T[] LoadAll(string path)
        {
            if (_resourceArrayCache.TryGetValue(path, out var cached))
                return cached;

            var resources = Resources.LoadAll<T>(path);
            if (resources == null || resources.Length == 0)
            {
                Debug.LogWarning($"[ResourceLoader::LoadAll] No resources found at path: {path}");
                return null;
            }

            _resourceArrayCache[path] = resources;
            return resources;
        }

        public static bool TryLoadAll(string path, out T[] resources)
            => (resources = LoadAll(path)) != null;

        public static void ClearCache()
        {
            _resourceCache.Clear();
            _resourceArrayCache.Clear();
        }

        public static void RemoveFromCache()
        {
            var cacheKey = GetCacheKey(GetPath());

            _resourceCache.Remove(cacheKey);
            _resourceArrayCache.Remove(cacheKey);
        }

        private static string GetCacheKey(string path) =>
            $"{typeof(T).Name}:{ValidatePath(path)}";

        private static string ValidatePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Debug.LogError($"[ResourceLoader::ValidatePath] Path cannot be null or empty");
                return null;
            }

            if (path.Contains("..") is false)
                return path.TrimStart('/');

            Debug.LogError($"[ResourceLoader::ValidatePath] Path cannot contain parent directory references");
            return null;
        }

        private static string GetPath()
        {
            var type = typeof(T);

            if (Attribute.GetCustomAttribute(type, typeof(ResourceAttribute)) is ResourceAttribute attribute)
                return attribute.TryGetFullResourcePath(out var fullResourcePath) ? fullResourcePath : typeof(T).Name;

            Debug.LogWarning($"[ResourceLoaderBase::Load] No ResourceAttribute found on type {type.Name}");
            return null;
        }
    }
}