using System;
using CustomUtils.Runtime.Attributes;
using CustomUtils.Runtime.UI.Theme.Base;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ThemeMapping
{
    [Serializable]
    public struct ColorMapping : IEquatable<ColorMapping>
    {
        [field: SerializeField] public ColorType ColorType { get; set; }
        [field: SerializeField, ThemeColorName] public string ColorName { get; set; }

        public ColorMapping(ColorType colorType = ColorType.Solid, string colorName = null)
        {
            ColorType = colorType;
            ColorName = colorName ?? string.Empty;
        }

        public bool Equals(ColorMapping other) =>
            ColorType == other.ColorType && ColorName == other.ColorName;

        public override bool Equals(object obj) =>
            obj is ColorMapping other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine((int)ColorType, ColorName);

        public static bool operator ==(ColorMapping left, ColorMapping right) =>
            left.Equals(right);

        public static bool operator !=(ColorMapping left, ColorMapping right) =>
            left.Equals(right) is false;
    }
}