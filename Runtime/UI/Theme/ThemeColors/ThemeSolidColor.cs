using System;
using CustomUtils.Runtime.CustomTypes.Collections;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ThemeColors
{
    [Serializable]
    internal sealed class ThemeSolidColor : IThemeColor<Color>
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public EnumArray<ThemeType, Color> Colors { get; private set; } =
            new(EnumMode.SkipFirst);
    }
}