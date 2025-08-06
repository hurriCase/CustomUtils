using System;
using System.Collections;
using System.Collections.Generic;

namespace CustomUtils.Runtime.CustomTypes.Collections
{
    /// <summary>
    /// Provides an enumerator for iterating through a collection of tuples represented by key-value pairs,
    /// where the keys are enumeration values and the values are associated data types.
    /// Supports different enumeration modes, including optional skipping of the first enum element during enumeration.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type that represents the keys in the (key, value) pairs.</typeparam>
    /// <typeparam name="TValue">The type of the associated values in the (key, value) pairs.</typeparam>
    public struct TupleEnumerator<TEnum, TValue> : IEnumerator<(TEnum Key, Entry<TValue> Value)>
        where TEnum : unmanaged, Enum
    {
        private readonly EnumArray<TEnum, TValue> _enumArray;
        private readonly TEnum[] _enumValues;
        private readonly int _startIndex;
        private int _index;

        /// <summary>
        /// Initializes a new instance of the TupleEnumerator struct.
        /// </summary>
        /// <param name="enumArray">The EnumArray instance to enumerate over.</param>
        /// <param name="enumMode">The enumeration mode that determines iteration behavior.
        /// If set to SkipFirst, the enumeration will start from the second enum element.</param>
        internal TupleEnumerator(EnumArray<TEnum, TValue> enumArray, EnumMode enumMode = EnumMode.Default)
        {
            _enumArray = enumArray;
            _enumValues = (TEnum[])Enum.GetValues(typeof(TEnum));
            _startIndex = enumMode == EnumMode.SkipFirst ? 1 : 0;
            _index = _startIndex - 1;
        }

        /// <summary>
        /// Gets the current element in the collection as a (key, value) tuple.
        /// Provides access to the enumerator's current position.
        /// </summary>
        /// <value>A tuple containing the current enum key and its associated value.</value>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the enumerator is positioned before the first element or after the last element.
        /// </exception>
        public readonly (TEnum Key, Entry<TValue> Value) Current
        {
            get
            {
                if (_index < _startIndex || _index >= _enumValues.Length)
                    throw new InvalidOperationException("Enumerator is not positioned on a valid element.");
                return (_enumValues[_index], _enumArray.Entries[_index]);
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next element;
        /// false if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            _index++;
            return _index < _enumValues.Length && _index < _enumArray.Entries.Length;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// The initial position respects the configured enumeration mode.
        /// </summary>
        public void Reset() => _index = _startIndex - 1;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// For this struct enumerator, no cleanup is needed.
        /// </summary>
        public void Dispose()
        {
            // No cleanup needed for struct enumerator
        }

        /// <summary>
        /// Returns an enumerator that iterates through the items in the collection without creating new allocations.
        /// </summary>
        /// <returns>
        /// A <see cref="TupleEnumerator{TEnum, TValue}"/> that can be used to iterate through the (key, value) tuples of the enumeration.
        /// </returns>
        public readonly TupleEnumerator<TEnum, TValue> GetEnumerator() => this;

        /// <summary>
        /// Explicit interface implementation for non-generic Current property.
        /// This will box the current value when accessed through IEnumerator.
        /// </summary>
        /// <value>The current key-value tuple as an object, which may cause boxing.</value>
        readonly object IEnumerator.Current => Current;
    }
}