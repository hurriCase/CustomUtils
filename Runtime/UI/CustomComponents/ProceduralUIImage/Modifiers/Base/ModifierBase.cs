using CustomUtils.Runtime.CustomBehaviours;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Modifiers.Base
{
    [UsedImplicitly]
    [DisallowMultipleComponent]
    public abstract class ModifierBase : GraphicBehaviour
    {
        [UsedImplicitly]
        public abstract Vector4 CalculateRadius(Rect imageRect);
    }
}