using CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Helpers;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Modifiers.Base
{
    public abstract class CanvasScaleModifierBase : ModifierBase
    {
        protected Canvas RootCanvas => _rootCanvas
            ? _rootCanvas
            : _rootCanvas = RootCanvasProvider.Instance.RootCanvas;

        private Canvas _rootCanvas;

        public override Vector4 CalculateRadius(Rect imageRect)
            => !RootCanvas ? Vector4.zero : OnCalculateRadius(imageRect);

        protected abstract Vector4 OnCalculateRadius(Rect imageRect);
    }
}