using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Scripts.Attributes;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Scripts.Modifiers
{
    [ModifierID("Round")]
    public sealed class RoundModifier : ProceduralImageModifier
    {
        public override Vector4 CalculateRadius(Rect imageRect)
        {
            var radius = Mathf.Min(imageRect.width, imageRect.height) * 0.5f;
            return new Vector4(radius, radius, radius, radius);
        }
    }
}