using System.Collections.Generic;
using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.CustomTypes.Singletons;
using UnityEngine;

namespace CustomUtils.Runtime.UI.ImagePixelPerUnit
{
    [Resource(
        ResourcePaths.ImagePixelPerUnitFullPath,
        ResourcePaths.ImagePixelPerUnitDatabaseAssetName,
        ResourcePaths.ImagePixelPerUnitResourcePath
    )]
    internal sealed class PixelPerUnitDatabase : SingletonScriptableObject<PixelPerUnitDatabase>
    {
        [field: SerializeField] internal List<float> CornerSizes { get; private set; }
    }
}