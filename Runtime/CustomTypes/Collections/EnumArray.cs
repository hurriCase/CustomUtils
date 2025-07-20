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
    /// Supports different enumeration modes, including optional skipping of the first enum element.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to be used as keys for this structure. Must be an unmanaged, Enum type.</typeparam>
    /// <typeparam name="TValue">The type of values to be stored in the array.</typeparam>
    [Serializable, UsedImplicitly, MemoryPackable]
    public partial struct EnumArray<TEnum, TValue> : IEnumerable<TValue>
        where TEnum : unmanaged, Enum
    {
        [SerializeField] private TValue[] _values;
        [SerializeField] private EnumMode _enumMode;

        /// <summary>
        /// Gets the total number of elements in the array associated with the underlying enum type.
        /// </summary>
        [UsedImplicitly, MemoryPackIgnore]
        public int Length => Values.Length;

        /// <summary>
        /// Gets the enumeration mode that determines how elements are processed during iteration.
        /// </summary>
        [UsedImplicitly]
        public EnumMode EnumMode => _enumMode;

        /// <summary>
        /// Gets the array of values associated with the underlying enum type as keys.
        /// </summary>
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
        /// Initializes a new instance of the EnumArray with all elements set to the specified default value.
        /// </summary>
        /// <param name="defaultValue">The default value to assign to all elements in the array.</param>
        /// <param name="enumMode">The enumeration mode that determines iteration behavior. Defaults to EnumMode.Default.</param>
        [UsedImplicitly]
        public EnumArray(TValue defaultValue, EnumMode enumMode = EnumMode.Default)
        {
            var enumValues = (TEnum[])Enum.GetValues(typeof(TEnum));
            _values = new TValue[enumValues.Length];
            _enumMode = enumMode;
            for (var i = 0; i < _values.Length; i++)
                _values[i] = defaultValue;
        }

        /// <summary>
        /// Initializes a new instance of the EnumArray with default values for all elements.
        /// </summary>
        /// <param name="enumMode">The enumeration mode that determines iteration behavior.</param>
        [UsedImplicitly]
        public EnumArray(EnumMode enumMode)
        {
            var enumValues = (TEnum[])Enum.GetValues(typeof(TEnum));
            _values = new TValue[enumValues.Length];
            _enumMode = enumMode;
        }

        /// <summary>
        /// Initializes a new instance of the EnumArray with the specified values and enumeration mode.
        /// This constructor is used by MemoryPack for deserialization.
        /// </summary>
        /// <param name="values">The array of values to store.</param>
        /// <param name="enumMode">The enumeration mode that determines iteration behavior.</param>
        [MemoryPackConstructor]
        public EnumArray(TValue[] values, EnumMode enumMode)
        {
            _values = values;
            _enumMode = enumMode;
        }

        /// <summary>
        /// Indexer property that allows accessing or modifying values in the array using the associated enum type as the key.
        /// </summary>
        /// <param name="key">The enum key corresponding to the value in the array.</param>
        /// <returns>The value stored in the array at the position corresponding to the enum key.</returns>
        [UsedImplicitly]
        public TValue this[TEnum key]
        {
            get => Values[UnsafeEnumConverter<TEnum>.ToInt32(key)];
            set => Values[UnsafeEnumConverter<TEnum>.ToInt32(key)] = value;
        }

        /// <summary>
        /// Indexer property that allows accessing or modifying values in the array using the index of the associated enum value.
        /// </summary>
        /// <param name="index">The index corresponding to the value in the array.</param>
        /// <returns>The value stored in the array at the position corresponding to the index.</returns>
        [UsedImplicitly]
        public TValue this[int index]
        {
            get => Values[index];
            set => Values[index] = value;
        }

        /// <summary>
        /// Enumerates over (key, value) tuples like a dictionary without allocations.
        /// Respects the configured enumeration mode for iteration behavior.
        /// </summary>
        /// <returns>A struct enumerator that iterates through key-value pairs.</returns>
        [UsedImplicitly]
        public TupleEnumerator<TEnum, TValue> AsTuples() => new(this, _enumMode);

        /// <summary>
        /// Executes an action for each key-value pair in the array.
        /// Respects the configured enumeration mode - if set to SkipFirst, the first enum element will be skipped.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target object passed to the action.</typeparam>
        /// <param name="target">The target object to pass to the action method.</param>
        /// <param name="action">The action to execute for each key-value pair.</param>
        [UsedImplicitly]
        public void ForEach<TTarget>(TTarget target, Action<TTarget, TEnum, TValue> action)
        {
            var enumValues = (TEnum[])Enum.GetValues(typeof(TEnum));
            var startIndex = _enumMode == EnumMode.SkipFirst ? 1 : 0;

            for (var i = startIndex; i < enumValues.Length && i < Values.Length; i++)
                action(target, enumValues[i], Values[i]);
        }

        /// <summary>
        /// Returns a high-performance struct enumerator that iterates through the array of values.
        /// This method creates zero GC pressure and is optimized for Unity's performance requirements.
        /// Respects the configured enumeration mode for iteration behavior.
        /// </summary>
        /// <returns>A struct enumerator for the array of values.</returns>
        [UsedImplicitly]
        public Enumerator<TValue> GetEnumerator() => new(Values, _enumMode);

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
    }
}