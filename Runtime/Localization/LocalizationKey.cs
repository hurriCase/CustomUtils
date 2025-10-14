using System;
using UnityEngine;

namespace CustomUtils.Runtime.Localization
{
    /// <summary>
    /// GUID-based reference to a localization key.
    /// </summary>
    [Serializable]
    public struct LocalizationKey : IEquatable<LocalizationKey>
    {
        [field: SerializeField, HideInInspector] internal string GUID { get; private set; }

        internal bool IsValid => string.IsNullOrEmpty(GUID) is false;

        internal LocalizationKey(string guid)
        {
            GUID = guid;
        }

        internal static LocalizationKey Generate() => new(Guid.NewGuid().ToString());

        internal static LocalizationKey Empty => new(string.Empty);

        public bool Equals(LocalizationKey other) => GUID == other.GUID;

        public override bool Equals(object obj) => obj is LocalizationKey other && Equals(other);

        public override int GetHashCode() => GUID?.GetHashCode() ?? 0;

        public static bool operator ==(LocalizationKey left, LocalizationKey right) => left.Equals(right);

        public static bool operator !=(LocalizationKey left, LocalizationKey right) => !left.Equals(right);
    }
}