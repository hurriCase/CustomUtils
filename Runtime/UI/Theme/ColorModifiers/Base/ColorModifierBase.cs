using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ColorModifiers.Base
{
    internal abstract class ColorModifierBase : MonoBehaviour
    {
        internal abstract void UpdateColor(string colorName);
    }
}