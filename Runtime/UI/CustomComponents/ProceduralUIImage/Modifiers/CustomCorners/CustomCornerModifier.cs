using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Attributes;
using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Modifiers.Base;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Modifiers.CustomCorners
{
    [ModifierID("Custom")]
    public sealed class CustomCornerModifier : CalculatableModifierBase
    {
        [field: SerializeField] public CornerRadiiData DesiredCornerRadii { get; private set; }
        [field: SerializeField] public CornerRadiiData CornerRadiusRatios { get; private set; }

        public override Vector4 CalculateRadius(Rect imageRect)
        {
            var minSide = Mathf.Min(imageRect.width, imageRect.height);
            var maxAllowedRadius = minSide * 0.5f;

            return new Vector4(
                Mathf.Min(minSide * CornerRadiusRatios.LeftTop, maxAllowedRadius),
                Mathf.Min(minSide * CornerRadiusRatios.RightTop, maxAllowedRadius),
                Mathf.Min(minSide * CornerRadiusRatios.RightBottom, maxAllowedRadius),
                Mathf.Min(minSide * CornerRadiusRatios.LeftBottom, maxAllowedRadius)
            );
        }

        internal override void ApplyRadiiFromDesired()
        {
            var rectTransform = Graphic.rectTransform;

            var rect = rectTransform.rect;
            var minSize = Mathf.Min(rect.width, rect.height);
            var data = CornerRadiusRatios;
            data.LeftTop = DesiredCornerRadii.LeftTop / minSize;
            data.RightTop = DesiredCornerRadii.RightTop / minSize;
            data.RightBottom = DesiredCornerRadii.RightBottom / minSize;
            data.LeftBottom = DesiredCornerRadii.LeftBottom / minSize;
            CornerRadiusRatios = data;
        }
    }
}