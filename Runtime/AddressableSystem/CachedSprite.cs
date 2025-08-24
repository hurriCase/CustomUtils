using JetBrains.Annotations;
using MemoryPack;
using UnityEngine.AddressableAssets;

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
        /// Gets whether the cached sprite has a valid GUID.
        /// </summary>
        [UsedImplicitly]
        public bool IsValid => string.IsNullOrEmpty(AssetGUID) is false;
    }
}