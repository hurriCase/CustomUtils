using System;
using System.Collections;
using System.Collections.Generic;
using CustomUtils.Unsafe.CustomUtils.Unsafe;
using JetBrains.Annotations;
using MemoryPack;
using UnityEngine;

namespace CustomUtils.Runtime.CustomTypes.Collections
{
    /// <summary>
    /// A generic struct that associates an array of values with an underlying enum type as keys.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to be used as keys for this structure. Must be an unmanaged, Enum type.</typeparam>
    /// <typeparam name="TValue">The type of values to be stored in the array.</typeparam>
    [Serializable, UsedImplicitly, MemoryPackable]
    public partial struct EnumArray<TEnum, TValue> : IEnumerable<TValue>
        where TEnum : unmanaged, Enum
    {
        [SerializeField] private TValue[] _values;

        /// <summary>
        /// Gets the array of values associated with the underlying enum type as keys.
        /// </summary>
        /// <remarks>
        /// If the internal array is not initialized, it is created with a size equal to the number
        /// of elements in the enum type. This ensures that the array length always matches the
        /// number of keys in the enum type.
        /// </remarks>
        [UsedImplicitly]
        public TValue[] Values
        {
            get
            {
                if (_values != null)
                    return _values;

                var enumValues = (TEnum[])Enum.GetValues(typeof(TEnum));
                _values = new TValue[enumValues.Length];

                return _values;
            }
        }

        /// <summary>
        /// A generic structure that associates an array of values with an enum type as keys.
        /// </summary>
        /// <typeparam name="TEnum">The enumeration type used as keys. Must be unmanaged and of Enum type.</typeparam>
        /// <typeparam name="TValue">The type of values stored in the array.</typeparam>
        /// <remarks>
        /// Provides convenient and type-safe access to a fixed-size array structured around the keys of an enum.
        /// </remarks>
        [UsedImplicitly]
        public EnumArray(TValue defaultValue)
        {
            var enumValues = (TEnum[])Enum.GetValues(typeof(TEnum));
            _values = new TValue[enumValues.Length];
            for (var i = 0; i < _values.Length; i++)
                _values[i] = defaultValue;
        }

        [MemoryPackConstructor]
        public EnumArray(TValue[] values)
        {
            _values = values;
        }

        /// <summary>
        /// Indexer property that allows accessing or modifying values in the array using the associated enum type as the key.
        /// </summary>
        /// <param name="key">The enum key corresponding to the value in the array.</param>
        /// <returns>The value stored in the array at the position corresponding to the enum key.</returns>
        /// <remarks>
        /// This indexer leverages the underlying integer representation of the enum to retrieve or update the value
        /// in the array. It ensures type safety by preventing invalid keys, aligning with the enum's defined range.
        /// </remarks>
        [UsedImplicitly]
        public TValue this[TEnum key]
        {
            get => Values[UnsafeEnumConverter<TEnum>.ToInt32(key)];
            set => Values[UnsafeEnumConverter<TEnum>.ToInt32(key)] = value;
        }

        /// <summary>
        /// Enumerates over (key, value) tuples like a dictionary without allocations
        /// </summary>
        [UsedImplicitly]
        public TupleEnumerator<TEnum, TValue> AsTuples() => new(this);

        /// <summary>
        /// Executes an action for each key-value pair
        /// </summary>
        [UsedImplicitly]
        public void ForEach<TTarget>(TTarget target, Action<TTarget, TEnum, TValue> action)
        {
            var enumValues = (TEnum[])Enum.GetValues(typeof(TEnum));
            for (var i = 0; i < enumValues.Length && i < Values.Length; i++)
                action(target, enumValues[i], Values[i]);
        }

        /// <summary>
        /// Gets the total number of elements in the array associated with the underlying enum type.
        /// </summary>
        /// <remarks>
        /// The length corresponds to the number of elements in the enum type, ensuring that the
        /// structure maintains a fixed size based on the enum's key count.
        /// </remarks>
        [UsedImplicitly]
        public int Length => Values.Length;

        /// <summary>
        /// Returns a high-performance struct enumerator that iterates through the array of values.
        /// This method creates zero GC pressure and is optimized for Unity's performance requirements.
        /// </summary>
        /// <returns>A struct enumerator for the array of values.</returns>
        [UsedImplicitly]
        public Enumerator<TValue> GetEnumerator() => new(Values);

        /// <summary>
        /// Explicit interface implementation that boxes the struct enumerator only when needed.
        /// Use the non-generic GetEnumerator() for better performance.
        /// </summary>
        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Explicit interface implementation for non-generic enumeration.
        /// Use the non-generic GetEnumerator() for better performance.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}