using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Attributes;
using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Modifiers.Base;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Modifiers
{
    [ModifierID("Canvas Scale")]
    public sealed class CanvasScaleCornerModifier : ModifierBase
    {
        [field: SerializeField] public float Radius { get; private set; }

        [SerializeField] private Canvas _canvasScaleReference;

        public override Vector4 CalculateRadius(Rect imageRect)
        {
            if (!_canvasScaleReference)
                return Vector4.zero;

            var minSide = Mathf.Min(imageRect.width, imageRect.height);

            var scaleFactor = _canvasScaleReference.scaleFactor;
            var scaledRadius = Radius * scaleFactor;

            var maxAllowedRadius = minSide * 0.5f;
            var actualRadius = Mathf.Min(scaledRadius, maxAllowedRadius);

            return new Vector4(actualRadius, actualRadius, actualRadius, actualRadius);
        }
    }
}