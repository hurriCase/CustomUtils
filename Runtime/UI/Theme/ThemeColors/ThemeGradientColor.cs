using System;
using CustomUtils.Runtime.CustomTypes.Collections;
using CustomUtils.Runtime.UI.Theme.Base;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ThemeColors
{
    [Serializable]
    public sealed class ThemeGradientColor : IThemeColor<Gradient>
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public EnumArray<ThemeType, Gradient> Colors { get; private set; } =
            new(EnumMode.SkipFirst);
    }
}