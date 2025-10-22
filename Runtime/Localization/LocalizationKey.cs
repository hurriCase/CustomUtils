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
        [SerializeField] private string _guid;
        [SerializeField] private string _key;
        [SerializeField] private string _tableName;

        internal string Guid => _guid;
        internal string Key => _key;
        internal string TableName => _tableName;

        internal bool IsValid => string.IsNullOrEmpty(_guid) is false;

        internal LocalizationKey(string guid, string key, string tableName)
        {
            _guid = guid;
            _key = key;
            _tableName = tableName;
        }

        internal static LocalizationKey Empty => new(string.Empty, string.Empty, string.Empty);

        public bool Equals(LocalizationKey other) => _guid == other._guid;
        public override bool Equals(object obj) => obj is LocalizationKey other && Equals(other);
        public override int GetHashCode() => _guid?.GetHashCode() ?? 0;
        public override string ToString() => string.IsNullOrEmpty(_key) ? "[Empty]" : _key;

        public static bool operator ==(LocalizationKey left, LocalizationKey right) => left.Equals(right);
        public static bool operator !=(LocalizationKey left, LocalizationKey right) => !left.Equals(right);
    }
}