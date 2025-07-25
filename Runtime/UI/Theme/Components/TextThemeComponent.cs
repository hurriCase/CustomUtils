﻿using System;
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

            _originalFontMaterial = _targetComponent.fontSharedMaterial;
        }

        protected override bool ShouldUpdateColor() =>
            ColorType switch
            {
                ColorType.Gradient => !_targetComponent.CompareGradient(GetCurrentGradient()),
                ColorType.Shared => ThemeSharedColor != null && _targetComponent.color != ThemeSharedColor.Color,
                ColorType.Solid => _targetComponent.color != GetCurrentSolidColor(),
                _ => throw new ArgumentOutOfRangeException()
            };

        protected override void ApplyColor()
        {
            switch (ColorType)
            {
                case ColorType.Gradient:
                    var gradient = GetCurrentGradient();
                    if (gradient != null)
                        _targetComponent.ApplyGradient(gradient, GradientDirection);

                    break;

                case ColorType.Shared:
                    if (ThemeSharedColor != null)
                    {
                        _targetComponent.fontSharedMaterial = _originalFontMaterial
                            ? _originalFontMaterial
                            : _targetComponent.fontSharedMaterial;

                        _targetComponent.color = ThemeSharedColor.Color;
                    }

                    break;

                case ColorType.Solid:
                    _targetComponent.fontSharedMaterial = _originalFontMaterial
                        ? _originalFontMaterial
                        : _targetComponent.fontSharedMaterial;

                    _targetComponent.color = GetCurrentSolidColor();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}