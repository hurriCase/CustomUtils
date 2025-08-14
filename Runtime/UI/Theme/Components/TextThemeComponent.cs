using System;
using CustomUtils.Runtime.Extensions.GradientExtensions;
using CustomUtils.Runtime.UI.Theme.Base;
using TMPro;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.Components
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class TextThemeComponent : BaseThemeComponent<TextMeshProUGUI>
    {
        private Material _originalFontMaterial;

        protected override void OnEnable()
        {
            base.OnEnable();

            _originalFontMaterial = targetComponent.fontSharedMaterial;
        }

        protected override bool ShouldUpdateColor() =>
            ColorType switch
            {
                ColorType.Gradient => !targetComponent.CompareGradient(GetCurrentGradient()),
                ColorType.Shared => ThemeSharedColor != null && targetComponent.color != ThemeSharedColor.Color,
                ColorType.Solid => targetComponent.color != GetCurrentSolidColor(),
                _ => throw new ArgumentOutOfRangeException()
            };

        protected override void OnApplyColor()
        {
            switch (ColorType)
            {
                case ColorType.Gradient:
                    var gradient = GetCurrentGradient();
                    if (gradient != null)
                        targetComponent.ApplyGradient(gradient, GradientDirection);

                    break;

                case ColorType.Shared:
                    if (ThemeSharedColor != null)
                    {
                        targetComponent.fontSharedMaterial = _originalFontMaterial
                            ? _originalFontMaterial
                            : targetComponent.fontSharedMaterial;

                        targetComponent.color = ThemeSharedColor.Color;
                    }

                    break;

                case ColorType.Solid:
                    targetComponent.fontSharedMaterial = _originalFontMaterial
                        ? _originalFontMaterial
                        : targetComponent.fontSharedMaterial;

                    targetComponent.color = GetCurrentSolidColor();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}