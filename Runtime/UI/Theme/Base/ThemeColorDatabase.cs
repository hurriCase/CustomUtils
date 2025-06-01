using System.Collections.Generic;
using System.Linq;
using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.CustomTypes.Singletons;
using CustomUtils.Runtime.UI.Theme.ThemeColors;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.Base
{
    [Resource(
        ResourcePaths.ThemeFullPath,
        ResourcePaths.ThemeColorDatabaseAssetName,
        ResourcePaths.ThemeResourcePath
    )]
    internal sealed class ThemeColorDatabase : SingletonScriptableObject<ThemeColorDatabase>
    {
        [field: SerializeField, NonReorderable] internal List<ThemeSolidColor> SolidColors { get; private set; }
        [field: SerializeField, NonReorderable] internal List<ThemeGradientColor> GradientColors { get; private set; }
        [field: SerializeField, NonReorderable] internal List<ThemeSharedColor> SharedColor { get; private set; }

        internal List<string> GetColorNames<TColor>() where TColor : IThemeColor
        {
            var colorList = GetColorList<TColor>();
            if (colorList == null || colorList.Count == 0)
                return null;

            return colorList.Select(color => color.Name).ToList();
        }

        internal bool TryGetColorByName<TColor>(string name, out TColor color) where TColor : IThemeColor
        {
            var colorList = GetColorList<TColor>();
            if (colorList == null || colorList.Count == 0)
            {
                color = default;
                return false;
            }

            color = colorList.Find(color => color.Name == name);
            return true;
        }

        private List<TColor> GetColorList<TColor>() where TColor : IThemeColor
        {
            return typeof(TColor) switch
            {
                var type when type == typeof(ThemeSolidColor) => SolidColors as List<TColor>,
                var type when type == typeof(ThemeGradientColor) => GradientColors as List<TColor>,
                var type when type == typeof(ThemeSharedColor) => SharedColor as List<TColor>,
                _ => null
            };
        }
    }
}