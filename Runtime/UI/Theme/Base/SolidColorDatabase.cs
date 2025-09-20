using System.Collections.Generic;
using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.UI.Theme.ThemeColors;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.Base
{
    [Resource(
        ResourcePaths.ThemeFullPath,
        ResourcePaths.SolidColorDatabaseAssetName,
        ResourcePaths.ThemeResourcePath
    )]
    internal sealed class SolidColorDatabase : ThemeColorDatabaseBase<SolidColorDatabase, ThemeSolidColor, Color>
    {
        [field: SerializeField, NonReorderable] public override List<ThemeSolidColor> Colors { get; protected set; }
    }
}