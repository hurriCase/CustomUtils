using System.Collections.Generic;
using CustomUtils.Runtime.UI.Theme.Databases;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ColorModifiers
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    internal sealed class SolidColorModifier : ColorModifierBase
    {
        internal override void ApplyColor()
        {
            if (SolidColorDatabase.Instance.TryGetColorByName(ColorName, out var color))
                Graphic.color = color;
        }

        internal override List<string> GetColorNames() => SolidColorDatabase.Instance.GetColorNames();
    }
}