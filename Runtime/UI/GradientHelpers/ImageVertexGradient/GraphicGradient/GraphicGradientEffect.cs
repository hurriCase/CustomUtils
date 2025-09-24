using CustomUtils.Runtime.Extensions;
using CustomUtils.Runtime.UI.GradientHelpers.ImageVertexGradient.Base;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUtils.Runtime.UI.GradientHelpers.ImageVertexGradient.GraphicGradient
{
    public sealed class GraphicGradientEffect : GradientEffectBase<Graphic>
    {
        protected override void ApplyGradient(
            Graphic graphic,
            Color startColor,
            Color endColor,
            GradientDirection direction)
        {
            var gradientEffect = graphic.GetOrAddComponent<VertexGradientEffect>();

            gradientEffect.SetGradient(startColor, endColor, direction);
            graphic.SetVerticesDirty();
        }

        public override void ClearGradient(Graphic graphic)
        {
            if (graphic.TryGetComponent<VertexGradientEffect>(out var gradientEffect) is false)
                return;

            gradientEffect.Destroy();
            graphic.SetVerticesDirty();
        }
    }
}