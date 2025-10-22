using System.Collections.Generic;
using System.Linq;
using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.CustomTypes.Singletons;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Runtime.UI.ImagePixelPerUnit
{
    [Resource(
        ResourcePaths.ImagePixelPerUnitFullPath,
        ResourcePaths.ImagePixelPerUnitDatabaseAssetName,
        ResourcePaths.ImagePixelPerUnitResourcePath
    )]
    internal sealed class PixelPerUnitDatabase : SingletonScriptableObject<PixelPerUnitDatabase>
    {
        [field: SerializeField] internal List<PixelPerUnitData> PixelPerUnitData { get; private set; }

        internal List<string> GetPixelPerUnitTypeNames() => PixelPerUnitData?.Select(static data => data.Name).ToList();

        internal PixelPerUnitData GetPixelPerUnitData(string pixelPerUnityTypeName)
            => PixelPerUnitData.AsValueEnumerable().FirstOrDefault(data => data.Name == pixelPerUnityTypeName);
    }
}