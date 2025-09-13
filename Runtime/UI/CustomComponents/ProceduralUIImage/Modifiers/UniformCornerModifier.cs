using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Attributes;
using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Modifiers.Base;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Modifiers
{
    [ModifierID("Uniform")]
    public sealed class UniformCornerModifier : CanvasScaleModifierBase
    {
        [field: SerializeField] public float Radius { get; private set; }

        public override Vector4 CalculateRadius(Rect imageRect)
        {
            var minSide = Mathf.Min(imageRect.width, imageRect.height);
            var maxAllowedRadius = minSide * 0.5f;
            var scaleFactor = rootCanvasProvider ? rootCanvasProvider.ScaleFactor : 1f;
            var scaledRadius = Radius * scaleFactor;
            var actualRadius = Mathf.Min(scaledRadius, maxAllowedRadius);

            return new Vector4(actualRadius, actualRadius, actualRadius, actualRadius);
        }
    }
}