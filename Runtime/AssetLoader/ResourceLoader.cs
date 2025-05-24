using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable MemberCanBeInternal
namespace CustomUtils.Runtime.AssetLoader
{
    /// <summary>
    /// Generic utility class for loading Unity resources.
    /// Simple, clean implementation without caching (Unity handles caching internally).
    /// </summary>
    /// <typeparam name="TResource">The type of resource to load, must inherit from UnityEngine.Object.</typeparam>
    [UsedImplicitly]
    public static class ResourceLoader<TResource> where TResource : Object
    {
        /// <summary>
        /// Loads a resource of type TResource from the specified path.
        /// If no path is provided, it will attempt to determine the path from ResourceAttribute.
        /// </summary>
        /// <param name="resourcePath">The path to load the resource from. If null, path is determined from ResourceAttribute.</param>
        /// <returns>The loaded resource or null if the resource could not be found.</returns>
        [UsedImplicitly]
        public static TResource Load(string resourcePath = null)
            => PathUtility.TryGetResourcePath<TResource>(ref resourcePath) is false
                ? null
                : Resources.Load<TResource>(resourcePath);

        /// <summary>
        /// Attempts to load a resource and returns whether the operation was successful.
        /// </summary>
        /// <param name="resource">When this method returns, contains the loaded resource if found; otherwise, the default value for the type.</param>
        /// <param name="path">The path to load the resource from. If null, path is determined from ResourceAttribute.</param>
        /// <returns>True if the resource was successfully loaded; otherwise, false.</returns>
        [UsedImplicitly]
        public static bool TryLoad(out TResource resource, string path = null)
        {
            resource = Load(path);
            return resource;
        }

        /// <summary>
        /// Loads all resources of type TResource from the specified path.
        /// </summary>
        /// <param name="path">The path to load the resources from.</param>
        /// <returns>An array of loaded resources or null if no resources were found.</returns>
        [UsedImplicitly]
        public static TResource[] LoadAll(string path)
        {
            var resources = Resources.LoadAll<TResource>(path);

            if (resources != null && resources.Length != 0)
                return resources;

            Debug.LogWarning($"[ResourceLoader::LoadAll] No resources found at path: {path}");
            return null;
        }

        /// <summary>
        /// Attempts to load all resources of type TResource from the specified path.
        /// </summary>
        /// <param name="path">The path to load the resources from.</param>
        /// <param name="resources">When this method returns, contains the loaded resources if found; otherwise, null.</param>
        /// <returns>True if resources were successfully loaded; otherwise, false.</returns>
        [UsedImplicitly]
        public static bool TryLoadAll(string path, out TResource[] resources)
            => (resources = LoadAll(path)) != null;

        /// <summary>
        /// Asynchronously loads a resource of type TResource from the specified path.
        /// If no path is provided, it will attempt to determine the path from ResourceAttribute.
        /// </summary>
        /// <param name="resourcePath">The path to load the resource from. If null, path is determined from ResourceAttribute.</param>
        /// <param name="cancellationToken">Optional cancellation token to stop loading</param>
        /// <returns>A UniTask that represents the asynchronous load operation.</returns>
        [UsedImplicitly]
        public static async UniTask<TResource> LoadAsync(string resourcePath = null,
            CancellationToken cancellationToken = default)
        {
            if (PathUtility.TryGetResourcePath<TResource>(ref resourcePath) is false)
                return null;

            var resourceRequest = Resources.LoadAsync<TResource>(resourcePath);

            await resourceRequest.ToUniTask(cancellationToken: cancellationToken);

            var resource = resourceRequest.asset as TResource;
            if (!resource)
                Debug.LogWarning($"[ResourceLoader::LoadAsync] Failed to load resource at path: {resourcePath}");

            return resource;
        }
    }
}