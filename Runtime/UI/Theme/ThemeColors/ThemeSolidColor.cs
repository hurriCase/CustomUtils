using System;
using CustomUtils.Runtime.UI.Theme.Base;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ThemeColors
{
    [Serializable]
    internal sealed class ThemeSolidColor : IThemeColor
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] internal Color LightThemeColor { get; private set; }
        [field: SerializeField] internal Color DarkThemeColor { get; private set; }
    }
}