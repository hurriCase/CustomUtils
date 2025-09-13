using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Attributes;
using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Modifiers.Base;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Modifiers.CustomCorners
{
    [ModifierID("Custom")]
    public sealed class CustomCornerModifier : ModifierBase
    {
        [field: SerializeField] public CornerRadiiData CornerRadii { get; private set; }

        public override Vector4 CalculateRadius(Rect imageRect)
        {
            var minSide = Mathf.Min(imageRect.width, imageRect.height);
            var maxAllowedRadius = minSide * 0.5f;

            return new Vector4(
                Mathf.Min(CornerRadii.LeftTop, maxAllowedRadius),
                Mathf.Min(CornerRadii.RightTop, maxAllowedRadius),
                Mathf.Min(CornerRadii.RightBottom, maxAllowedRadius),
                Mathf.Min(CornerRadii.LeftBottom, maxAllowedRadius)
            );
        }
    }
}