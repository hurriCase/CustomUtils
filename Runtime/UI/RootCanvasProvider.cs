using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.CustomTypes.Singletons;
using UnityEngine;

namespace CustomUtils.Runtime.UI
{
    [Resource(
        ResourcePaths.RootCanvasProviderFullPath,
        ResourcePaths.RootCanvasProviderAssetName,
        ResourcePaths.RootCanvasProviderResourcesPath
    )]
    public sealed class RootCanvasProvider : SingletonScriptableObject<RootCanvasProvider>
    {
        [field: SerializeField] public Canvas RootCanvas { get; private set; }
    }
}