using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.CustomTypes.Singletons;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Helpers
{
    [Resource(
        ResourcePaths.RootCanvasProviderFullPath,
        ResourcePaths.RootCanvasProviderAssetName,
        ResourcePaths.RootCanvasProviderResourcesPath
    )]
    internal sealed class RootCanvasProvider : SingletonScriptableObject<RootCanvasProvider>
    {
        [field: SerializeField] internal Canvas RootCanvas { get; private set; }
    }
}