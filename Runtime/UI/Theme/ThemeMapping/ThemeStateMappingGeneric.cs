using System;
using System.Collections.Generic;
using CustomUtils.Runtime.UI.Theme.Base;
using CustomUtils.Runtime.UI.Theme.ThemeColors;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Runtime.UI.Theme.ThemeMapping
{
    public abstract class ThemeStateMappingGeneric<TEnum> : ScriptableObject where TEnum : unmanaged, Enum
    {
        [field: SerializeField] internal List<StateColorMapping<TEnum>> StateMappings { get; private set; } = new();

        private ThemeColorDatabase ThemeColorDatabase => ThemeColorDatabase.Instance;
        private ThemeHandler ThemeHandler => ThemeHandler.Instance;

        public Color GetColorForState(TEnum state)
        {
            var mapping = GetMappingForState(state);
            if (mapping == null)
                return Color.white;

            return mapping.ColorType switch
            {
                ColorType.Solid => GetSolidColor(mapping.ColorName),
                ColorType.Shared => GetSharedColor(mapping.ColorName),
                _ => Color.white
            };
        }

        public string GetColorNameForState(TEnum state)
        {
            var mapping = GetMappingForState(state);
            return mapping?.ColorName;
        }

        public Gradient GetGradientForState(TEnum state)
        {
            var mapping = GetMappingForState(state);
            if (mapping?.ColorType != ColorType.Gradient)
                return null;

            if (ThemeColorDatabase.TryGetColorByName<ThemeGradientColor>(mapping.ColorName, out var gradientColor))
                return ThemeHandler.CurrentThemeType switch
                {
                    ThemeType.Light => gradientColor.LightThemeColor,
                    ThemeType.Dark => gradientColor.DarkThemeColor,
                    _ => null
                };

            return null;
        }

        public ColorType GetColorTypeForState(TEnum state)
        {
            var mapping = GetMappingForState(state);
            return mapping?.ColorType ?? ColorType.Solid;
        }

        private StateColorMapping<TEnum> GetMappingForState(TEnum state) =>
            StateMappings.AsValueEnumerable().FirstOrDefault(mapping =>
                EqualityComparer<TEnum>.Default.Equals(mapping.State, state));

        private Color GetSolidColor(string colorName)
        {
            if (ThemeColorDatabase.TryGetColorByName<ThemeSolidColor>(colorName, out var solidColor))
                return ThemeHandler.CurrentThemeType switch
                {
                    ThemeType.Light => solidColor.LightThemeColor,
                    ThemeType.Dark => solidColor.DarkThemeColor,
                    _ => Color.white
                };

            return Color.white;
        }

        private Color GetSharedColor(string colorName)
            => ThemeColorDatabase.TryGetColorByName<ThemeSharedColor>(colorName, out var sharedColor)
                ? sharedColor.Color
                : Color.white;
    }
}