using System;
using UnityEngine;

namespace CustomUtils.Runtime.AnyType
{
    [Serializable]
    public struct AnyValue
    {
        public ValueType Type { get; set; }

        public bool BoolValue { get; set; }
        public int IntValue { get; set; }
        public float FloatValue { get; set; }
        public string StringValue { get; set; }
        public Vector3 Vector3Value { get; set; }

        public static implicit operator bool(AnyValue value) => value.ConvertValue<bool>();
        public static implicit operator int(AnyValue value) => value.ConvertValue<int>();
        public static implicit operator float(AnyValue value) => value.ConvertValue<float>();
        public static implicit operator string(AnyValue value) => value.ConvertValue<string>();
        public static implicit operator Vector3(AnyValue value) => value.ConvertValue<Vector3>();

        public T ConvertValue<T>()
        {
            if (typeof(T) == typeof(object)) return CastToObject<T>();
            return Type switch
            {
                ValueType.Int => AsInt<T>(IntValue),
                ValueType.Float => AsFloat<T>(FloatValue),
                ValueType.Bool => AsBool<T>(BoolValue),
                ValueType.String => (T)(object)StringValue,
                ValueType.Vector3 => AsVector3<T>(Vector3Value),
                _ => throw new InvalidCastException($"Cannot convert AnyValue of type {Type} to {typeof(T).Name}")
            };
        }

        private T AsBool<T>(bool value) => typeof(T) == typeof(bool) && value is T correctType ? correctType : default;
        private T AsInt<T>(int value) => typeof(T) == typeof(int) && value is T correctType ? correctType : default;
        private T AsFloat<T>(float value) => typeof(T) == typeof(float) && value is T correctType ? correctType : default;
        private T AsVector3<T>(Vector3 value) => typeof(T) == typeof(Vector3) && value is T correctType ? correctType : default;

        public static Type TypeOf(ValueType valueType)
        {
            return valueType switch
            {
                ValueType.Bool => typeof(bool),
                ValueType.Int => typeof(int),
                ValueType.Float => typeof(float),
                ValueType.String => typeof(string),
                ValueType.Vector3 => typeof(Vector3),
                _ => throw new NotSupportedException($"Unsupported ValueType: {valueType}")
            };
        }

        public static ValueType ValueTypeOf(Type type)
        {
            return type switch
            {
                _ when type == typeof(bool) => ValueType.Bool,
                _ when type == typeof(int) => ValueType.Int,
                _ when type == typeof(float) => ValueType.Float,
                _ when type == typeof(string) => ValueType.String,
                _ when type == typeof(Vector3) => ValueType.Vector3,
                _ => throw new NotSupportedException($"Unsupported type: {type}")
            };
        }

        private T CastToObject<T>()
        {
            return Type switch
            {
                ValueType.Int => (T)(object)IntValue,
                ValueType.Float => (T)(object)FloatValue,
                ValueType.Bool => (T)(object)BoolValue,
                ValueType.String => (T)(object)StringValue,
                ValueType.Vector3 => (T)(object)Vector3Value,
                _ => throw new InvalidCastException($"Cannot convert AnyValue of type {Type} to {typeof(T).Name}")
            };
        }
    }
}