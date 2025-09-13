using CustomUtils.Runtime.UI.RootCanvas;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Modifiers.Base
{
    public abstract class CanvasScaleModifierBase : ModifierBase
    {
        [SerializeField] protected RootCanvasProvider rootCanvasProvider;
    }
}