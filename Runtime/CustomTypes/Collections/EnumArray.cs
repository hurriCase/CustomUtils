using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using CustomUtils.Unsafe.Unsafe;
using JetBrains.Annotations;
using MemoryPack;
using UnityEngine;
using ZLinq;

namespace CustomUtils.Runtime.CustomTypes.Collections
{
    /// <summary>
    /// A generic struct that associates an array of values with an underlying enum type as keys.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to be used as keys for this structure.</typeparam>
    /// <typeparam name="TValue">The type of values to be stored in the array.</typeparam>
    [Serializable, UsedImplicitly, MemoryPackable]
    public partial struct EnumArray<TEnum, TValue> :
        IEnumerable<TValue>, IEquatable<EnumArray<TEnum, TValue>>,
        ISerializationCallbackReceiver
        where TEnum : unmanaged, Enum
    {
        /// <summary>
        /// Gets the array of entries associated with the underlying enum type as keys.
        /// </summary>
        [UsedImplicitly]
        [field: SerializeField] public Entry<TValue>[] Entries { get; private set; }

        /// <summary>
        /// Gets the total number of elements in the array associated with the underlying enum type.
        /// </summary>
        [UsedImplicitly, MemoryPackIgnore]
        public int Length => Entries?.Length ?? 0;

        internal static string EntriesPropertyName => nameof(Entries);

        [MemoryPackIgnore]
        private static int GetValuesCount =>
            Enum.GetValues(typeof(TEnum)).Cast<TEnum>()
                .Distinct()
                .Count();

        /// <summary>
        /// Initializes a new instance of the EnumArray with all elements set to the specified default value.
        /// </summary>
        /// <param name="defaultValue">The default value to assign to all elements in the array.</param>
        [UsedImplicitly]
        public EnumArray(TValue defaultValue)
        {
            Entries = new Entry<TValue>[GetValuesCount];

            for (var i = 0; i < Entries.Length; i++)
                Entries[i] = new Entry<TValue> { Value = defaultValue };
        }

        /// <summary>
        /// Initializes a new instance of the EnumArray
        /// with all elements set to values created by the specified factory method.
        /// </summary>
        /// <param name="factory">A factory method that creates default values for each array element.</param>
        [UsedImplicitly]
        public EnumArray(Func<TValue> factory)
        {
            Entries = new Entry<TValue>[GetValuesCount];

            for (var i = 0; i < Entries.Length; i++)
                Entries[i] = new Entry<TValue> { Value = factory() };
        }

        [MemoryPackConstructor, EditorBrowsable(EditorBrowsableState.Never)]
        public EnumArray(Entry<TValue>[] entries)
        {
            Entries = entries;
        }

        /// <summary>
        /// Indexer property that allows accessing or modifying values in the array
        /// using the associated enum type as the key.
        /// </summary>
        /// <param name="key">The enum key corresponding to the value in the array.</param>
        /// <returns>The value stored in the array at the position corresponding to the enum key.</returns>
        [UsedImplicitly]
        public TValue this[TEnum key]
        {
            get => Entries[UnsafeEnumConverter<TEnum>.ToInt32(key)].Value;
            set => Entries[UnsafeEnumConverter<TEnum>.ToInt32(key)].Value = value;
        }

        /// <summary>
        /// Indexer property that allows accessing or modifying values in the array
        /// using the index of the associated enum value.
        /// </summary>
        /// <param name="index">The index corresponding to the value in the array.</param>
        /// <returns>The value stored in the array at the position corresponding to the index.</returns>
        [UsedImplicitly]
        public TValue this[int index]
        {
            get => Entries[index].Value;
            set => Entries[index].Value = value;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnBeforeSerialize()
        {
            // No action needed
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnAfterDeserialize()
        {
            if (Entries != null && Entries.Length != 0)
                return;

            var count = GetValuesCount;
            Entries = new Entry<TValue>[count];

            for (var i = 0; i < count; i++)
                Entries[i] = new Entry<TValue>();
        }

        /// <summary>
        /// Enumerates over (key, value) tuples like a dictionary without allocations.
        /// </summary>
        /// <returns>A struct enumerator that iterates through key-value pairs.</returns>
        [UsedImplicitly]
        public TupleEnumerator<TEnum, TValue> AsTuples() => new(this);

        /// <summary>
        ///
        /// </summary>
        /// <returns>A struct enumerator for the array of values.</returns>
        [UsedImplicitly]
        public Enumerator<TValue> GetEnumerator() => new(Entries);

        /// <summary>
        /// Explicit interface implementation that boxes the struct enumerator only when needed.
        /// Use the non-generic GetEnumerator() for better performance.
        /// </summary>
        /// <returns>A boxed enumerator for the array of values.</returns>
        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Explicit interface implementation for non-generic enumeration.
        /// Use the non-generic GetEnumerator() for better performance.
        /// </summary>
        /// <returns>A boxed enumerator for the array of values.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Determines whether the specified EnumArray is equal to the current EnumArray.
        /// </summary>
        /// <param name="other">The EnumArray to compare with the current instance.</param>
        /// <returns>true if the specified EnumArray is equal to the current instance; otherwise, false.</returns>
        public readonly bool Equals(EnumArray<TEnum, TValue> other)
        {
            if (Entries == null && other.Entries == null)
                return true;

            if (Entries == null || other.Entries == null)
                return false;

            if (Entries.Length != other.Entries.Length)
                return false;

            for (var i = 0; i < Entries.Length; i++)
            {
                if (EqualityComparer<TValue>.Default.Equals(Entries[i].Value, other.Entries[i].Value) is false)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current EnumArray.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>true if the specified object is equal to the current instance; otherwise, false.</returns>
        public readonly override bool Equals(object obj) =>
            obj is EnumArray<TEnum, TValue> other && Equals(other);

        /// <summary>
        /// Returns the hash code for the current EnumArray instance.
        /// </summary>
        /// <returns>A hash code for the current instance.</returns>
        public readonly override int GetHashCode()
        {
            if (Entries == null)
                return 0;

            var hash = new HashCode();

            foreach (var entry in Entries)
                hash.Add(entry.Value);

            return hash.ToHashCode();
        }

        /// <summary>
        /// Determines whether two EnumArray instances are equal.
        /// </summary>
        public static bool operator ==(EnumArray<TEnum, TValue> left, EnumArray<TEnum, TValue> right) =>
            left.Equals(right);

        /// <summary>
        /// Determines whether two EnumArray instances are not equal.
        /// </summary>
        public static bool operator !=(EnumArray<TEnum, TValue> left, EnumArray<TEnum, TValue> right) =>
            left.Equals(right) is false;
    }
}