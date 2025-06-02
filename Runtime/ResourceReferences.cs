using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.CustomTypes.Singletons;
using UnityEngine;

namespace CustomUtils.Runtime
{
    [Resource(ResourcePaths.ResourceReferencesFullPath, ResourcePaths.ResourceReferencesAssetName)]
    internal sealed class ResourceReferences : SingletonScriptableObject<ResourceReferences>
    {
        [field: SerializeField] internal Sprite SquareSprite { get; private set; }
    }
}