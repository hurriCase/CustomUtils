// EnumArray.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomUtils.Unsafe.CustomUtils.Unsafe;
using UnityEngine;

namespace CustomUtils.Runtime.CustomTypes
{
    [Serializable]
    public struct EnumArray<TEnum, TValue> : IEnumerable<TValue>
        where TEnum : unmanaged, Enum
    {
        [SerializeField] private TValue[] _values;

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

        public EnumArray(TValue defaultValue)
        {
            var enumValues = (TEnum[])Enum.GetValues(typeof(TEnum));
            _values = new TValue[enumValues.Length];
            for (var i = 0; i < _values.Length; i++)
                _values[i] = defaultValue;
        }

        public TValue this[TEnum key]
        {
            get => Values[UnsafeEnumConverter<TEnum>.ToInt32(key)];
            set => Values[UnsafeEnumConverter<TEnum>.ToInt32(key)] = value;
        }

        public int Length => Values.Length;

        public IEnumerator<TValue> GetEnumerator() => Values.AsEnumerable().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}