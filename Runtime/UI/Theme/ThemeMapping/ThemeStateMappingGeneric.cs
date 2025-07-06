using System;
using CustomUtils.Runtime.CustomTypes;
using CustomUtils.Runtime.UI.Theme.Base;
using CustomUtils.Runtime.UI.Theme.ThemeColors;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ThemeMapping
{
    /// <inheritdoc cref="ScriptableObject" />
    /// <summary>
    /// Abstract base class for mapping enum states to theme colors.
    /// Provides a unified way to associate enum values with colors from the theme database.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to map colors to. Must be an unmanaged enum type.</typeparam>
    public abstract class ThemeStateMappingGeneric<TEnum> : ScriptableObject where TEnum : unmanaged, Enum
    {
        [field: SerializeField] public EnumArray<TEnum, ColorMapping> StateMappings { get; private set; }

        private ThemeColorDatabase ThemeColorDatabase => ThemeColorDatabase.Instance;
        private ThemeHandler ThemeHandler => ThemeHandler.Instance;

        /// <summary>
        /// Gets the resolved color for the specified enum state.
        /// </summary>
        /// <param name="state">The enum state to get the color for</param>
        /// <returns>
        /// The resolved color from the theme database, or Color.white if no mapping exists.
        /// </returns>
        [UsedImplicitly]
        public Color GetColorForState(TEnum state)
        {
            var mapping = StateMappings[state];
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
        /// <returns>The color name or empty string if no mapping exists.</returns>
        [UsedImplicitly]
        public string GetColorNameForState(TEnum state) => StateMappings[state].ColorName;

        /// <summary>
        /// Gets the resolved gradient for the specified enum state.
        /// </summary>
        /// <param name="state">The enum state to get the gradient for</param>
        /// <returns>The resolved gradient or null if not found/applicable.</returns>
        [UsedImplicitly]
        public Gradient GetGradientForState(TEnum state)
        {
            var mapping = StateMappings[state];
            if (mapping.ColorType != ColorType.Gradient)
                return null;

            if (ThemeColorDatabase.TryGetColorByName<ThemeGradientColor>(mapping.ColorName, out var gradientColor))
            {
                return ThemeHandler.CurrentThemeType switch
                {
                    ThemeType.Light => gradientColor.LightThemeColor,
                    ThemeType.Dark => gradientColor.DarkThemeColor,
                    _ => null
                };
            }

            return null;
        }

        /// <summary>
        /// Gets the color type associated with the specified enum state.
        /// </summary>
        /// <param name="state">The enum state to get the color type for</param>
        /// <returns>The ColorType for the specified state.</returns>
        [UsedImplicitly]
        public ColorType GetColorTypeForState(TEnum state) => StateMappings[state].ColorType;

        /// <summary>
        /// Gets the mapping for the specified state.
        /// </summary>
        /// <param name="state">The enum state to get the mapping for</param>
        /// <returns>The color mapping for the specified state</returns>
        [UsedImplicitly]
        public ColorMapping GetMappingForState(TEnum state) => StateMappings[state];

        /// <summary>
        /// Configures a theme component to use the color mapping for the specified enum state.
        /// </summary>
        /// <param name="state">The enum state to configure the theme component for</param>
        /// <param name="themeComponent">The theme component to configure</param>
        [UsedImplicitly]
        public void SetComponentForState(TEnum state, IBaseThemeComponent themeComponent)
        {
            var mapping = StateMappings[state];

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

        /// <summary>
        /// Compares the content of this mapping with another mapping.
        /// This is what you probably want for detecting theme changes.
        /// </summary>
        [UsedImplicitly]
        public bool HasSameContentAs(ThemeStateMappingGeneric<TEnum> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            // Compare each mapping in the enum array
            var enumValues = (TEnum[])Enum.GetValues(typeof(TEnum));
            foreach (var enumValue in enumValues)
            {
                if (StateMappings[enumValue].Equals(other.StateMappings[enumValue]) is false)
                    return false;
            }

            return true;
        }

        private Color GetSolidColor(string colorName)
        {
            if (ThemeColorDatabase.TryGetColorByName<ThemeSolidColor>(colorName, out var solidColor))
            {
                return ThemeHandler.CurrentThemeType switch
                {
                    ThemeType.Light => solidColor.LightThemeColor,
                    ThemeType.Dark => solidColor.DarkThemeColor,
                    _ => Color.white
                };
            }

            return Color.white;
        }

        private Color GetSharedColor(string colorName) =>
            ThemeColorDatabase.TryGetColorByName<ThemeSharedColor>(colorName, out var sharedColor)
                ? sharedColor.Color
                : Color.white;
    }
}