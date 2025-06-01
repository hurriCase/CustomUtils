using CustomUtils.Runtime.CustomTypes.Singletons;
using UnityEngine;

namespace CustomUtils.Runtime.AssetLoader.DontDestroyLoader
{
    [Resource(
        ResourcePaths.AssetLoaderConfigFullPath,
        ResourcePaths.AssetLoaderConfigAssetName,
        ResourcePaths.AssetLoaderConfigResourcesPath
    )]
    internal sealed class AssetLoaderConfig : SingletonScriptableObject<AssetLoaderConfig>
    {
        [field: SerializeField] public string DontDestroyPath { get; private set; } = "DontDestroyOnLoad";
    }
}