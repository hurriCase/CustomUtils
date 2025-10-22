using System;
using UnityEngine;

namespace CustomUtils.Runtime.Localization
{
    /// <summary>
    /// GUID-based localization key with human-readable identifier.
    /// The GUID ensures references remain valid even when key names change.
    /// </summary>
    [Serializable]
    public struct LocalizationKey : IEquatable<LocalizationKey>
    {
        [field: SerializeField] internal string Key { get; private set; }
        [field: SerializeField] internal string Guid { get; private set; }
        [field: SerializeField] internal string TableName { get; private set; }

        internal readonly bool IsValid => string.IsNullOrEmpty(Guid) is false;

        internal LocalizationKey(string guid, string key, string tableName)
        {
            Guid = guid;
            Key = key;
            TableName = tableName;
        }

        public bool Equals(LocalizationKey other) => Guid == other.Guid;
        public override bool Equals(object obj) => obj is LocalizationKey other && Equals(other);
        public readonly override int GetHashCode() => Guid?.GetHashCode() ?? 0;
        public readonly override string ToString() => string.IsNullOrEmpty(Key) ? "[Empty]" : Key;

        public static bool operator ==(LocalizationKey left, LocalizationKey right) => left.Equals(right);
        public static bool operator !=(LocalizationKey left, LocalizationKey right) => !left.Equals(right);
    }
}