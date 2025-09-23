using System;
using CustomUtils.Runtime.UI.Theme.Base;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ThemeMapping
{
    [Serializable]
    public struct ColorData
    {
        [field: SerializeField] public ColorType ColorType { get; private set; }
        [field: SerializeField] public string ColorName { get; private set; }
    }
}