using System;
using System.Collections.Generic;
using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.CustomTypes.Singletons;
using CustomUtils.Runtime.UI.Theme.ThemeColors;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.Base
{
    [Resource("Assets/Client/Scriptables/Resources/UI/Theme", nameof(ThemeColorDatabase), "UI/Theme")]
    internal sealed class ThemeColorDatabase : SingletonScriptableObject<ThemeColorDatabase>
    {
        [field: SerializeField, NonReorderable] internal List<ThemeSolidColor> SolidColors { get; private set; }
        [field: SerializeField, NonReorderable] internal List<ThemeGradientColor> GradientColors { get; private set; }
        [field: SerializeField, NonReorderable] internal List<ThemeSharedColor> SharedColor { get; private set; }

        internal string[] GetColorNames<TColor>() where TColor : IThemeColor
        {
            var colorList = GetColorList<TColor>();
            if (colorList == null || colorList.Count == 0)
                return new[] { "No colors found" };

            var names = new string[colorList.Count];
            for (var i = 0; i < colorList.Count; i++)
                names[i] = colorList[i].Name;

            return names;
        }

        internal int GetColorIndexByName(string name, ColorType colorType) =>
            colorType switch
            {
                ColorType.Shared => SharedColor.FindIndex(color => color.Name == name),
                ColorType.SolidColor => SolidColors.FindIndex(color => color.Name == name),
                ColorType.Gradient => GradientColors.FindIndex(color => color.Name == name),
                _ => throw new ArgumentOutOfRangeException(nameof(colorType), colorType, null)
            };

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