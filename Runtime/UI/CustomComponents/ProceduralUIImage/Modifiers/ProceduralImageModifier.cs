using CustomUtils.Runtime.CustomBehaviours;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Modifiers
{
    [UsedImplicitly]
    [DisallowMultipleComponent]
    public abstract class ProceduralImageModifier : GraphicBehaviour
    {
        [UsedImplicitly]
        public abstract Vector4 CalculateRadius(Rect imageRect);
    }
}