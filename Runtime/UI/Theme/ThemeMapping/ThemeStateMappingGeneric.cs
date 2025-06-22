using System;
using System.Collections.Generic;
using CustomUtils.Runtime.UI.Theme.Base;
using CustomUtils.Runtime.UI.Theme.ThemeColors;
using JetBrains.Annotations;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Runtime.UI.Theme.ThemeMapping
{
    /// <inheritdoc />
    /// <summary>
    /// Abstract base class for mapping enum states to theme colors.
    /// Provides a unified way to associate enum values with colors from the theme database.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to map colors to. Must be an unmanaged enum type.</typeparam>
    public abstract class ThemeStateMappingGeneric<TEnum> : ScriptableObject where TEnum : unmanaged, Enum
    {
        [field: SerializeField] internal List<StateColorMapping<TEnum>> StateMappings { get; private set; } = new();

        private ThemeColorDatabase ThemeColorDatabase => ThemeColorDatabase.Instance;
        private ThemeHandler ThemeHandler => ThemeHandler.Instance;

        /// <summary>
        /// Gets the resolved color for the specified enum state.
        /// </summary>
        /// <param name="state">The enum state to get the color for</param>
        /// <returns>
        /// The resolved color from the theme database, or Color.white if no mapping exists or the color cannot be resolved.
        /// The returned color respects the current theme (Light/Dark) for solid colors.
        /// </returns>
        [UsedImplicitly]
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

        /// <summary>
        /// Gets the color name associated with the specified enum state.
        /// </summary>
        /// <param name="state">The enum state to get the color name for</param>
        /// <returns>
        /// The color name from the theme database, or null if no mapping exists for the specified state.
        /// </returns>
        [UsedImplicitly]
        public string GetColorNameForState(TEnum state)
        {
            var mapping = GetMappingForState(state);
            return mapping?.ColorName;
        }

        /// <summary>
        /// Gets the resolved gradient for the specified enum state.
        /// </summary>
        /// <param name="state">The enum state to get the gradient for</param>
        /// <returns>
        /// The resolved gradient from the theme database, or null if no mapping exists,
        /// the color type is not Gradient, or the gradient cannot be resolved.
        /// The returned gradient respects the current theme (Light/Dark).
        /// </returns>
        [UsedImplicitly]
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

        /// <summary>
        /// Gets the color type associated with the specified enum state.
        /// </summary>
        /// <param name="state">The enum state to get the color type for</param>
        /// <returns>
        /// The ColorType for the specified state, or ColorType.Solid if no mapping exists.
        /// </returns>
        [UsedImplicitly]
        public ColorType GetColorTypeForState(TEnum state)
        {
            var mapping = GetMappingForState(state);
            return mapping?.ColorType ?? ColorType.Solid;
        }

        /// <summary>
        /// Configures a theme component to use the color mapping for the specified enum state.
        /// This method sets the appropriate color name and type on the theme component and applies the color.
        /// </summary>
        /// <param name="state">The enum state to configure the theme component for</param>
        /// <param name="themeComponent">The theme component to configure</param>
        [UsedImplicitly]
        public void SetComponentForState(TEnum state, IBaseThemeComponent themeComponent)
        {
            var mapping = GetMappingForState(state);
            if (mapping == null)
                return;

            themeComponent.ColorType = mapping.ColorType;

            switch (mapping.ColorType)
            {
                case ColorType.Solid:
                    themeComponent.ThemeSolidColorName = mapping.ColorName;
                    break;

                case ColorType.Gradient:
                    themeComponent.ThemeGradientColorName = mapping.ColorName;
                    break;

                case ColorType.Shared:
                    themeComponent.ThemeSharedColorName = mapping.ColorName;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            themeComponent.OnApplyColor();
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