using System;
using CustomUtils.Runtime.Attributes;
using CustomUtils.Runtime.UI.Theme.Base;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ThemeMapping
{
    [Serializable]
    internal class StateColorMapping<TEnum> where TEnum : unmanaged, Enum
    {
        [field: SerializeField] public TEnum State { get; set; }
        [field: SerializeField] public ColorType ColorType { get; set; } = ColorType.Solid;
        [field: SerializeField, ThemeColorName] public string ColorName { get; set; }
    }
}