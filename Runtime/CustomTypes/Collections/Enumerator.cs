using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CustomUtils.Runtime.CustomTypes.Collections
{
    /// <summary>
    /// Provides a high-performance struct-based enumerator for iterating over an array of elements.
    /// This enumerator is designed to minimize garbage collection overhead and is suitable for performance-critical scenarios.
    /// </summary>
    /// <typeparam name="TValue">The type of the elements in the array.</typeparam>
    [UsedImplicitly]
    public struct Enumerator<TValue> : IEnumerator<TValue>
    {
        private readonly TValue[] _array;
        private int _index;

        internal Enumerator(TValue[] array)
        {
            _array = array ?? throw new ArgumentNullException(nameof(array));
            _index = -1;
        }

        /// <summary>
        /// Gets the current element in the enumeration.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the enumerator is positioned before the first element or after the last element.
        /// </exception>
        public readonly TValue Current
        {
            get
            {
                if (_index < 0 || _index >= _array.Length)
                    throw new InvalidOperationException("Enumerator is not positioned on a valid element.");
                return _array[_index];
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element in the collection.
        /// </summary>
        /// <returns>true if the enumerator successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
        public bool MoveNext()
        {
            _index++;
            return _index < _array.Length;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            _index = -1;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// For this struct enumerator, no cleanup is needed.
        /// </summary>
        public void Dispose()
        {
            // No cleanup needed for struct enumerator
        }

        /// <summary>
        /// Explicit interface implementation for non-generic Current property.
        /// This will box the current value when accessed through IEnumerator.
        /// </summary>
        readonly object IEnumerator.Current => Current;
    }
}