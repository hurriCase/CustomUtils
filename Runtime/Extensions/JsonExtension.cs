﻿using System;
using System.Linq;
using Newtonsoft.Json;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
namespace CustomUtils.Runtime.Extensions
{
    public static class JsonExtension
    {
        /// <summary>
        /// Gets the JSON property name for an enum value, using either the JsonPropertyAttribute
        /// value or the enum value's string representation.
        /// </summary>
        /// <param name="enumValue">The enum value to get the JSON property name for.</param>
        public static string GetJsonPropertyName(this Enum enumValue)
        {
            var attribute = enumValue.GetType()
                .GetField(enumValue.ToString())
                ?.GetCustomAttributes(typeof(JsonPropertyAttribute), false)
                .FirstOrDefault() as JsonPropertyAttribute;

            return attribute?.PropertyName ?? enumValue.ToString();
        }
    }
}