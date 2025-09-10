using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Attributes;
using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Modifiers.Base;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Modifiers
{
    [ModifierID("Rounded")]
    public sealed class RoundedCornerModifier : ModifierBase
    {
        public override Vector4 CalculateRadius(Rect imageRect)
        {
            var radius = Mathf.Min(imageRect.width, imageRect.height) * 0.5f;
            return new Vector4(radius, radius, radius, radius);
        }
    }
}