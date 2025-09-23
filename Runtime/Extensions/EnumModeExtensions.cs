using System;
using CustomUtils.Runtime.CustomTypes.Collections;
using JetBrains.Annotations;

namespace CustomUtils.Runtime.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="EnumMode"/>.
    /// </summary>
    [UsedImplicitly]
    public static class EnumModeExtensions
    {
        /// <summary>
        /// Gets enum values based on the specified enumeration mode.
        /// </summary>
        /// <typeparam name="TEnum">The enum type to retrieve values from.</typeparam>
        /// <param name="enumMode">The mode that determines filtering behavior.</param>
        /// <returns>Array of enum values. SkipFirst mode excludes the first enum value.</returns>
        [UsedImplicitly]
        public static TEnum[] GetEnumValues<TEnum>(this EnumMode enumMode)
            where TEnum : Enum
        {
            var enumValues = (TEnum[])Enum.GetValues(typeof(TEnum));
            var startIndex = enumMode == EnumMode.SkipFirst ? 1 : 0;
            return enumValues[startIndex..];
        }
    }
}