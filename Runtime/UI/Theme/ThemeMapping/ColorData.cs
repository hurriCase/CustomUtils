using System;
using CustomUtils.Runtime.UI.Theme.Base;
using UnityEngine;

namespace CustomUtils.Runtime.UI.Theme.ThemeMapping
{
    [Serializable]
    public struct ColorData : IEquatable<ColorData>
    {
        [field: SerializeField] public ColorType ColorType { get; set; }
        [field: SerializeField] public string ColorName { get; set; }

        public ColorData(ColorType colorType = ColorType.Solid, string colorName = null)
        {
            ColorType = colorType;
            ColorName = colorName ?? string.Empty;
        }

        public bool Equals(ColorData other) =>
            ColorType == other.ColorType && ColorName == other.ColorName;

        public override bool Equals(object obj) =>
            obj is ColorData other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine((int)ColorType, ColorName);

        public static bool operator ==(ColorData left, ColorData right) =>
            left.Equals(right);

        public static bool operator !=(ColorData left, ColorData right) =>
            left.Equals(right) is false;
    }
}