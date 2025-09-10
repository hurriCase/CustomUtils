using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Attributes;
using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Modifiers.Base;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Modifiers
{
    [ModifierID("Uniform")]
    public sealed class UniformCornerModifier : CalculatableModifierBase
    {
        [field: SerializeField] public float CornerRadiusRatio { get; private set; }
        [field: SerializeField] public float DesiredRadius { get; private set; }

        public override Vector4 CalculateRadius(Rect imageRect)
        {
            var minSide = Mathf.Min(imageRect.width, imageRect.height);
            var actualRadius = minSide * CornerRadiusRatio;

            var maxAllowedRadius = minSide * 0.5f;
            actualRadius = Mathf.Min(actualRadius, maxAllowedRadius);

            return new Vector4(actualRadius, actualRadius, actualRadius, actualRadius);
        }

        internal override void ApplyRadiiFromDesired()
        {
            var rect = Graphic.rectTransform.rect;

            var minSize = Mathf.Min(rect.width, rect.height);
            var radius = DesiredRadius / minSize;
            CornerRadiusRatio = radius;
        }
    }
}