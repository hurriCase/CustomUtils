﻿using System;
using CustomUtils.Runtime.Attributes;
using CustomUtils.Runtime.UI.Theme.Base;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ThemeColors
{
    [Serializable]
    internal sealed class ThemeGradientColor : IThemeColor
    {
        [field: SerializeField, InspectorReadOnly] public string Name { get; private set; }
        [field: SerializeField, InspectorReadOnly] internal Gradient LightThemeColor { get; private set; }
        [field: SerializeField, InspectorReadOnly] internal Gradient DarkThemeColor { get; private set; }
    }
}