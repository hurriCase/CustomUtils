using System.Collections.Generic;
using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.UI.Theme.ThemeColors;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.Base
{
    [Resource(
        ResourcePaths.ThemeFullPath,
        ResourcePaths.GradientColorDatabaseAssetName,
        ResourcePaths.ThemeResourcePath
    )]
    internal sealed class GradientColorDatabase :
        ThemeColorDatabaseBase<GradientColorDatabase, ThemeGradientColor, Gradient>
    {
        [field: SerializeField, NonReorderable] public override List<ThemeGradientColor> Colors { get; protected set; }
    }
}