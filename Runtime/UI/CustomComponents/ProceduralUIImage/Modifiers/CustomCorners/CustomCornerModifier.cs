using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Attributes;
using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Modifiers.Base;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Modifiers.CustomCorners
{
    [ModifierID("Custom")]
    public sealed class CustomCornerModifier : CanvasScaleModifierBase
    {
        [field: SerializeField] public CornerRadiiData CornerRadii { get; private set; }

        public override Vector4 CalculateRadius(Rect imageRect)
        {
            var minSide = Mathf.Min(imageRect.width, imageRect.height);
            var maxAllowedRadius = minSide * 0.5f;
            var scaleFactor = rootCanvasProvider ? rootCanvasProvider.ScaleFactor : 1f;

            return new Vector4(
                Mathf.Min(CornerRadii.LeftTop * scaleFactor, maxAllowedRadius),
                Mathf.Min(CornerRadii.RightTop * scaleFactor, maxAllowedRadius),
                Mathf.Min(CornerRadii.RightBottom * scaleFactor, maxAllowedRadius),
                Mathf.Min(CornerRadii.LeftBottom * scaleFactor, maxAllowedRadius)
            );
        }
    }
}