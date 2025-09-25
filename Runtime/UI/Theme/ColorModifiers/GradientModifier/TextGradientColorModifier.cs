using CustomUtils.Runtime.UI.GradientHelpers;
using CustomUtils.Runtime.UI.Theme.ColorModifiers.Base;
using TMPro;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ColorModifiers.GradientModifier
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    [ColorModifier(ColorType.TextGradient)]
    internal sealed class TextGradientColorModifier : GradientModifierBase<TextGradientEffect, TextMeshProUGUI> { }
}