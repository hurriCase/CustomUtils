using System;
using CustomUtils.Runtime.Extensions.Gradient;
using CustomUtils.Runtime.UI.Theme.Base;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.Theme.Components
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Image))]
    internal sealed class ImageThemeComponent : BaseThemeComponent<Image>
    {
        private Material _originalMaterial;

        protected override void OnEnable()
        {
            base.OnEnable();

            _originalMaterial = _targetComponent.material;
        }

        protected override bool ShouldUpdateColor()
        {
            return ColorType switch
            {
                ColorType.Shared => _targetComponent.color != ThemeSharedColor.Color,
                ColorType.SolidColor => _targetComponent.color != GetCurrentSolidColor(),
                ColorType.Gradient => _targetComponent.CompareGradient(GetCurrentGradient()),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected override void ApplyColor()
        {
            switch (ColorType)
            {
                case ColorType.Shared:
                    _targetComponent.material = _originalMaterial;
                    _targetComponent.color = ThemeSharedColor.Color;
                    break;

                case ColorType.SolidColor:
                    _targetComponent.material = _originalMaterial;
                    _targetComponent.color = GetCurrentSolidColor();
                    break;

                case ColorType.Gradient:
                    _targetComponent.ApplyGradient(GetCurrentGradient());
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}