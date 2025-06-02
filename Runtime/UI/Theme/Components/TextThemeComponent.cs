using System;
using CustomUtils.Runtime.Extensions.Gradient;
using CustomUtils.Runtime.UI.Theme.Base;
using TMPro;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.Components
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(TextMeshProUGUI))]
    internal sealed class TextThemeComponent : BaseThemeComponent<TextMeshProUGUI>
    {
        private Material _originalFontMaterial;

        protected override void OnEnable()
        {
            base.OnEnable();

            _originalFontMaterial = _targetComponent.fontSharedMaterial;
        }

        protected override bool ShouldUpdateColor()
        {
            switch (ColorType)
            {
                case ColorType.Gradient:
                    _targetComponent.CompareGradient(GetCurrentGradient());
                    return true;
                case ColorType.Shared:
                    return _targetComponent.color != ThemeSharedColor.Color;

                case ColorType.Solid:
                    return _targetComponent.color != GetCurrentSolidColor();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void ApplyColor()
        {
            switch (ColorType)
            {
                case ColorType.Gradient:
                    _targetComponent.ApplyGradient(GetCurrentGradient());
                    break;

                case ColorType.Shared:
                    _targetComponent.fontMaterial = _originalFontMaterial;
                    _targetComponent.color = ThemeSharedColor.Color;
                    break;

                case ColorType.Solid:
                    _targetComponent.fontMaterial = _originalFontMaterial;
                    _targetComponent.color = GetCurrentSolidColor();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}