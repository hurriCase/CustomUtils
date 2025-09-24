using CustomUtils.Runtime.UI.Theme.Base;
using TMPro;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ColorModifiers.GradientModifier
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    [ColorModifier(ColorType.TextGradient)]
    internal sealed class TextGradientColorModifier : GradientModifierBase
    {
        [field: SerializeField] internal TextMeshProUGUI Text { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();

            Text = GetComponent<TextMeshProUGUI>();
        }

        private void OnDestroy()
        {
            Text.enableVertexGradient = false;
        }

        protected override void OnApplyColor(Gradient gradient)
        {
            if (gradient.colorKeys.Length < 1)
            {
                Debug.LogError("[TextGradientColorModifier::ApplyTextMeshProGradient] " +
                               "Invalid gradient provided. Ensure it has at least one color key.");
                return;
            }

            var startColor = gradient.colorKeys[0].color;
            var endColor = gradient.colorKeys[^1].color;

            Text.colorGradient = CurrentGradientDirection.Value switch
            {
                ImageVertexGradient.GradientDirection.TopToBottom => new VertexGradient(startColor, startColor, endColor, endColor),
                ImageVertexGradient.GradientDirection.LeftToRight => new VertexGradient(startColor, endColor, endColor, startColor),
                ImageVertexGradient.GradientDirection.BottomToTop => new VertexGradient(endColor, endColor, startColor, startColor),
                ImageVertexGradient.GradientDirection.RightToLeft => new VertexGradient(endColor, startColor, startColor, endColor),
                _ => new VertexGradient(startColor)
            };

            Text.enableVertexGradient = CurrentGradientDirection.Value != ImageVertexGradient.GradientDirection.None;
        }
    }
}