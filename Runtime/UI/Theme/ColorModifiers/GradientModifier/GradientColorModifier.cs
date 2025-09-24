using CustomUtils.Runtime.UI.Theme.Base;
using CustomUtils.Runtime.UI.Theme.ImageVertexGradient;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ColorModifiers.GradientModifier
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [ColorModifier(ColorType.Gradient)]
    internal sealed class GradientColorModifier : GradientModifierBase
    {
        private void OnDestroy()
        {
            Graphic.ClearVertexGradient();
        }

        protected override void OnApplyColor(Gradient gradient)
        {
            Graphic.ApplyVertexGradient(gradient, CurrentGradientDirection.Value);
        }
    }
}