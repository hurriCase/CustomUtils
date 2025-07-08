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
    /// Supports optional skipping of the first enum element during enumeration.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to be used as keys for this structure. Must be an unmanaged, Enum type.</typeparam>
    /// <typeparam name="TValue">The type of values to be stored in the array.</typeparam>
    [Serializable, UsedImplicitly, MemoryPackable]
    public partial struct EnumArray<TEnum, TValue> : IEnumerable<TValue>
        where TEnum : unmanaged, Enum
    {
        [SerializeField] private TValue[] _values;
        [SerializeField] private bool _skipFirst;

        /// <summary>
        /// Gets the total number of elements in the array associated with the underlying enum type.
        /// </summary>
        [UsedImplicitly, MemoryPackIgnore]
        public int Length => Values.Length;

        /// <summary>
        /// Gets a value indicating whether the first enum element should be skipped during enumeration.
        /// </summary>
        [UsedImplicitly]
        public bool SkipFirst => _skipFirst;

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
        /// Initializes a new instance of the EnumArray struct with all elements set to the specified default value.
        /// </summary>
        /// <param name="defaultValue">The default value to assign to all elements in the array.</param>
        /// <param name="skipFirst">If true, the first enum element will be skipped during enumeration operations.</param>
        [UsedImplicitly]
        public EnumArray(TValue defaultValue, bool skipFirst = false)
        {
            var enumValues = (TEnum[])Enum.GetValues(typeof(TEnum));
            _values = new TValue[enumValues.Length];
            _skipFirst = skipFirst;
            for (var i = 0; i < _values.Length; i++)
                _values[i] = defaultValue;
        }

        /// <summary>
        /// Initializes a new instance of the EnumArray struct without setting default values.
        /// </summary>
        /// <param name="skipFirst">If true, the first enum element will be skipped during enumeration operations.</param>
        [UsedImplicitly]
        public EnumArray(bool skipFirst)
        {
            var enumValues = (TEnum[])Enum.GetValues(typeof(TEnum));
            _values = new TValue[enumValues.Length];
            _skipFirst = skipFirst;
        }

        /// <summary>
        /// Initializes a new instance of the EnumArray struct with the specified array of values.
        /// This constructor is used by MemoryPack for deserialization.
        /// </summary>
        /// <param name="values">The array of values to associate with the enum keys.</param>
        /// <param name="skipFirst">If true, the first enum element will be skipped during enumeration operations.</param>
        [MemoryPackConstructor]
        public EnumArray(TValue[] values, bool skipFirst)
        {
            _values = values;
            _skipFirst = skipFirst;
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
        /// Enumerates over (key, value) tuples like a dictionary without allocations.
        /// </summary>
        /// <returns>A struct enumerator that iterates through key-value pairs.</returns>
        [UsedImplicitly]
        public TupleEnumerator<TEnum, TValue> AsTuples() => new(this, _skipFirst);

        /// <summary>
        /// Executes an action for each key-value pair in the array.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target object passed to the action.</typeparam>
        /// <param name="target">The target object to pass to the action method.</param>
        /// <param name="action">The action to execute for each key-value pair.</param>
        [UsedImplicitly]
        public void ForEach<TTarget>(TTarget target, Action<TTarget, TEnum, TValue> action)
        {
            var enumValues = (TEnum[])Enum.GetValues(typeof(TEnum));
            var startIndex = _skipFirst ? 1 : 0;

            for (var i = startIndex; i < enumValues.Length && i < Values.Length; i++)
                action(target, enumValues[i], Values[i]);
        }

        /// <summary>
        /// Returns a high-performance struct enumerator that iterates through the array of values.
        /// This method creates zero GC pressure and is optimized for Unity's performance requirements.
        /// </summary>
        /// <returns>A struct enumerator for the array of values.</returns>
        [UsedImplicitly]
        public Enumerator<TValue> GetEnumerator() => new(Values, _skipFirst);

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