using System;
using CustomUtils.Runtime.UI.GradientHelpers;
using CustomUtils.Runtime.UI.Theme.Base;
using CustomUtils.Runtime.UI.Theme.ColorModifiers.Base;
using TMPro;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ColorModifiers.GradientModifier
{
    [Serializable]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    [ColorModifier(ColorType.TextGradient)]
    internal sealed class TextGradientColorModifier : GradientModifierBase<TextGradientEffect, TextMeshProUGUI> { }
}