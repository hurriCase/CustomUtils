using System;
using CustomUtils.Runtime.UI.Theme.Base;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ThemeColors
{
    [Serializable]
    public sealed class ThemeSharedColor : IThemeColor
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] internal Color Color { get; private set; }
    }
}