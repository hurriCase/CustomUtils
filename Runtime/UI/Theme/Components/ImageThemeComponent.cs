using System;
using CustomUtils.Runtime.Extensions.GradientExtensions;
using CustomUtils.Runtime.UI.Theme.Base;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.Theme.Components
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public sealed class ImageThemeComponent : BaseThemeComponent<Image>
    {
        private Material _originalMaterial;

        protected override void OnEnable()
        {
            base.OnEnable();

            _originalMaterial = targetComponent.material;
        }

        protected override bool ShouldUpdateColor() =>
            ColorType switch
            {
                ColorType.Gradient => targetComponent.CompareGradient(GetCurrentGradient()),
                ColorType.Shared => ThemeSharedColor != null && targetComponent.color != ThemeSharedColor.Color,
                ColorType.Solid => targetComponent.color != GetCurrentSolidColor(),
                _ => throw new ArgumentOutOfRangeException()
            };

        protected override void OnApplyColor()
        {
            switch (ColorType)
            {
                case ColorType.Gradient:
                    targetComponent.ApplyGradient(GetCurrentGradient(), GradientDirection);
                    break;

                case ColorType.Shared:
                    targetComponent.material = _originalMaterial;
                    targetComponent.color = ThemeSharedColor.Color;
                    break;

                case ColorType.Solid:
                    targetComponent.material = _originalMaterial;
                    targetComponent.color = GetCurrentSolidColor();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}