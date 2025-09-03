using CustomUtils.Runtime.CustomBehaviours;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Scripts.Modifiers
{
    [UsedImplicitly]
    [DisallowMultipleComponent]
    public abstract class ProceduralImageModifier : GraphicBehaviour
    {
        [UsedImplicitly]
        public abstract Vector4 CalculateRadius(Rect imageRect);
    }
}