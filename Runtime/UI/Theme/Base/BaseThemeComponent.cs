using System;
using CustomUtils.Runtime.Extensions.GradientExtensions;
using CustomUtils.Runtime.UI.Theme.ThemeColors;
using UnityEngine;
using Component = UnityEngine.Component;

namespace CustomUtils.Runtime.UI.Theme.Base
{
    [ExecuteInEditMode]
    internal abstract class BaseThemeComponent<T> : MonoBehaviour, IBaseThemeComponent where T : Component
    {
        [field: SerializeField] public ColorType ColorType { get; set; } = ColorType.Solid;
        [field: SerializeField] public GradientDirection GradientDirection { get; set; }
        [field: SerializeField] public string ThemeSharedColorName { get; set; }
        [field: SerializeField] public string ThemeSolidColorName { get; set; }
        [field: SerializeField] public string ThemeGradientColorName { get; set; }

        [SerializeField] protected T _targetComponent;

        public ThemeSharedColor ThemeSharedColor => ThemeColorDatabase
            .TryGetColorByName<ThemeSharedColor>(ThemeSharedColorName, out var color)
            ? color
            : null;

        public ThemeSolidColor ThemeSolidColor => ThemeColorDatabase
            .TryGetColorByName<ThemeSolidColor>(ThemeSolidColorName, out var color)
            ? color
            : null;

        public ThemeGradientColor ThemeGradientColor => ThemeColorDatabase
            .TryGetColorByName<ThemeGradientColor>(ThemeGradientColorName, out var color)
            ? color
            : null;

        private ThemeHandler ThemeHandler => ThemeHandler.Instance;
        private ThemeColorDatabase ThemeColorDatabase => ThemeColorDatabase.Instance;

        private ThemeSharedColor _previousSharedThemeColor;
        private ThemeSolidColor _previousSolidThemeColor;
        private ThemeGradientColor _previousGradientThemeColor;
        private ColorType _previousColorType;
        private ThemeType _previousThemeType;

        protected virtual void Reset()
        {
            OnEnable();
        }

        protected virtual void OnEnable()
        {
            _targetComponent = _targetComponent ? _targetComponent : GetComponent<T>();

            OnApplyColor();
        }

        public virtual void OnApplyColor()
        {
            bool colorChanged;

            switch (ColorType)
            {
                case ColorType.Gradient:
                    colorChanged = _previousGradientThemeColor != ThemeGradientColor;
                    if (colorChanged)
                        _previousGradientThemeColor = ThemeGradientColor;
                    break;
                case ColorType.Shared:
                    colorChanged = _previousSharedThemeColor != ThemeSharedColor;
                    if (colorChanged)
                        _previousSharedThemeColor = ThemeSharedColor;
                    break;

                case ColorType.Solid:
                    colorChanged = _previousSolidThemeColor != ThemeSolidColor;
                    if (colorChanged)
                        _previousSolidThemeColor = ThemeSolidColor;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!_targetComponent || (colorChanged is false && _previousThemeType == ThemeHandler.CurrentThemeType &&
                                      _previousColorType == ColorType && ShouldUpdateColor() is false))
                return;

            ApplyColor();

            _previousThemeType = ThemeHandler.CurrentThemeType;
            _previousColorType = ColorType;
        }

        protected abstract bool ShouldUpdateColor();

        protected abstract void ApplyColor();

        protected Gradient GetCurrentGradient()
        {
            if (ThemeGradientColor == null)
                return null;

            return ThemeHandler.CurrentThemeType switch
            {
                ThemeType.Light => ThemeGradientColor.LightThemeColor,
                ThemeType.Dark => ThemeGradientColor.DarkThemeColor,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected Color GetCurrentSolidColor()
        {
            if (ThemeSolidColor == null)
                return Color.white;

            return ThemeHandler.CurrentThemeType switch
            {
                ThemeType.Light => ThemeSolidColor.LightThemeColor,
                ThemeType.Dark => ThemeSolidColor.DarkThemeColor,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}