using CustomUtils.Runtime.UI.GradientHelpers.Base;
using TMPro;
using UnityEngine;

namespace CustomUtils.Runtime.UI.GradientHelpers
{
    public sealed class TextGradientEffect : GradientEffectBase<TextMeshProUGUI>
    {
        protected override void ApplyGradient(
            TextMeshProUGUI text,
            Color startColor,
            Color endColor,
            GradientDirection direction)
        {
            text.colorGradient = direction switch
            {
                GradientDirection.TopToBottom => new VertexGradient(startColor, startColor, endColor, endColor),
                GradientDirection.LeftToRight => new VertexGradient(startColor, endColor, endColor, startColor),
                GradientDirection.BottomToTop => new VertexGradient(endColor, endColor, startColor, startColor),
                GradientDirection.RightToLeft => new VertexGradient(endColor, startColor, startColor, endColor),
                _ => new VertexGradient(startColor)
            };

            text.enableVertexGradient = direction != GradientDirection.None;
        }

        public override void ClearGradient(TextMeshProUGUI text)
        {
            text.enableVertexGradient = false;
        }
    }
}