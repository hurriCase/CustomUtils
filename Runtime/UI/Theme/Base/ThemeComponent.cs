using System;
using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.UI.Theme.ThemeColors;
using CustomUtils.Runtime.UI.Theme.VertexGradient;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.Theme.Base
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Graphic))]
    public sealed class ThemeComponent : MonoBehaviour
    {
        [field: SerializeField] public SerializableReactiveProperty<ColorType> CurrentColorType { get; set; } = new();

        [field: SerializeField]
        public SerializableReactiveProperty<GradientDirection> CurrentGradientDirection { get; set; } = new();

        [field: SerializeField] public string ThemeSharedColorName { get; set; }
        [field: SerializeField] public string ThemeSolidColorName { get; set; }
        [field: SerializeField] public string ThemeGradientColorName { get; set; }

        [SerializeField] private Graphic _targetGraphic;

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

        private void Reset()
        {
            OnEnable();
        }

        private void OnEnable()
        {
            _targetGraphic = _targetGraphic ? _targetGraphic : GetComponent<Graphic>();

            ApplyColor();
        }

        private void Awake()
        {
            ThemeHandler.Instance.CurrentThemeType.SubscribeAndRegister(this, self => self.ApplyColor());
            CurrentColorType.SubscribeAndRegister(this, self => self.ApplyColor());
            CurrentGradientDirection.SubscribeAndRegister(this, self => self.ApplyColor());
        }

        public void ApplyColor()
        {
            if (!_targetGraphic)
                return;

            OnApplyColor();
        }

        private void OnApplyColor()
        {
            switch (CurrentColorType.Value)
            {
                case ColorType.Gradient:
                    if (TryGetCurrentGradient(out var gradient))
                        _targetGraphic.ApplyVertexGradient(gradient, CurrentGradientDirection.Value);
                    break;

                case ColorType.Shared:
                    if (ThemeSharedColor != null)
                    {
                        _targetGraphic.ClearVertexGradient();
                        _targetGraphic.color = ThemeSharedColor.Color;
                    }

                    break;

                case ColorType.Solid:
                    _targetGraphic.ClearVertexGradient();
                    _targetGraphic.color = GetCurrentSolidColor();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool TryGetCurrentGradient(out Gradient gradient)
        {
            gradient = ThemeHandler.CurrentThemeType.Value switch
            {
                ThemeType.Light => ThemeGradientColor.LightThemeColor,
                ThemeType.Dark => ThemeGradientColor.DarkThemeColor,
                _ => throw new ArgumentOutOfRangeException()
            };

            return gradient != null;
        }

        private Color GetCurrentSolidColor()
        {
            if (ThemeSolidColor == null)
                return Color.white;

            return ThemeHandler.CurrentThemeType.Value switch
            {
                ThemeType.Light => ThemeSolidColor.LightThemeColor,
                ThemeType.Dark => ThemeSolidColor.DarkThemeColor,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}