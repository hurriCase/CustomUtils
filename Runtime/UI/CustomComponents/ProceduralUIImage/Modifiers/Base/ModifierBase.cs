using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.UI.CustomComponents.ProceduralUIImage.Modifiers.Base
{
    [UsedImplicitly]
    public abstract class ModifierBase : MonoBehaviour
    {
        [UsedImplicitly]
        public abstract Vector4 CalculateRadius(Rect imageRect);
    }
}