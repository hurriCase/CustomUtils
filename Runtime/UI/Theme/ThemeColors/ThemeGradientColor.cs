using System;
using CustomUtils.Runtime.UI.Theme.Base;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ThemeColors
{
    [Serializable]
    internal sealed class ThemeGradientColor : IThemeColor
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] internal Gradient LightThemeColor { get; private set; }
        [field: SerializeField] internal Gradient DarkThemeColor { get; private set; }
    }
}