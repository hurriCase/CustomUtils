using JetBrains.Annotations;
using MemoryPack;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CustomUtils.Runtime.AddressableSystem
{
    /// <summary>
    /// Lightweight struct for caching sprite asset references by GUID.
    /// </summary>
    [MemoryPackable, UsedImplicitly]
    public readonly partial struct CachedSprite
    {
        /// <summary>
        /// Gets the Addressable asset GUID.
        /// </summary>
        [UsedImplicitly]
        public string AssetGUID { get; }

        /// <summary>
        /// Initializes a new CachedSprite with the specified GUID.
        /// </summary>
        /// <param name="assetGUID">The Addressable asset GUID.</param>
        [MemoryPackConstructor]
        [UsedImplicitly]
        public CachedSprite(string assetGUID) => AssetGUID = assetGUID;

        /// <summary>
        /// Initializes a new CachedSprite from an AssetReference.
        /// </summary>
        /// <param name="assetReference">The asset reference to cache.</param>
        [UsedImplicitly]
        public CachedSprite(AssetReference assetReference) => AssetGUID = assetReference.AssetGUID;

        /// <summary>
        /// Creates a CachedSprite from an asset path by resolving its GUID.
        /// </summary>
        /// <param name="assetPath">The asset path (e.g., "Assets/Sprites/icon.png").</param>
        [UsedImplicitly]
        public static CachedSprite FromPath(string assetPath)
        {
#if UNITY_EDITOR
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            return new CachedSprite(guid);
#else
            return default;
#endif
        }

        /// <summary>
        /// Gets whether the cached sprite has a valid GUID.
        /// </summary>
        [UsedImplicitly]
        public bool IsValid => string.IsNullOrEmpty(AssetGUID) is false;
    }
}