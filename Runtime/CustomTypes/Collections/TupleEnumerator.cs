using System;
using System.Collections;
using System.Collections.Generic;

namespace CustomUtils.Runtime.CustomTypes.Collections
{
    /// <summary>
    /// Provides an enumerator for iterating through a collection of tuples represented by key-value pairs,
    /// where the keys are enumeration values and the values are associated data types.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type that represents the keys in the (key, value) pairs.</typeparam>
    /// <typeparam name="TValue">The type of the associated values in the (key, value) pairs.</typeparam>
    public struct TupleEnumerator<TEnum, TValue> : IEnumerator<(TEnum Key, TValue Value)>
        where TEnum : unmanaged, Enum
    {
        private readonly EnumArray<TEnum, TValue> _enumArray;
        private readonly TEnum[] _enumValues;
        private int _index;

        internal TupleEnumerator(EnumArray<TEnum, TValue> enumArray)
        {
            _enumArray = enumArray;
            _enumValues = (TEnum[])Enum.GetValues(typeof(TEnum));
            _index = -1;
        }

        /// <summary>
        /// Gets the current element in the collection as a (key, value) tuple.
        /// Provides access to the enumerator's current position.
        /// <exception cref="InvalidOperationException">
        /// Thrown when the enumerator is positioned before the first element or after the last element.
        /// </exception>
        /// </summary>
        public readonly (TEnum Key, TValue Value) Current
        {
            get
            {
                if (_index < 0 || _index >= _enumValues.Length)
                    throw new InvalidOperationException("Enumerator is not positioned on a valid element.");
                return (_enumValues[_index], _enumArray.Values[_index]);
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>True if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
        public bool MoveNext()
        {
            _index++;
            return _index < _enumValues.Length && _index < _enumArray.Values.Length;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset() => _index = -1;

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
        readonly object IEnumerator.Current => Current;
    }
}