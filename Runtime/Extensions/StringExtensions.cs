using System;
using System.Text.RegularExpressions;
using CustomUtils.Runtime.Localization;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a property name to its backing field representation
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <returns>The backing field name format</returns>
        [UsedImplicitly]
        public static string ConvertToBackingField(this string propertyName)
            => $"<{propertyName}>k__BackingField";

        /// <summary>
        /// Converts a camelCase or PascalCase string to a spaced string.
        /// </summary>
        /// <param name="text">The camelCase or PascalCase string to convert.</param>
        /// <returns>A string with spaces between words.</returns>
        [UsedImplicitly]
        public static string ToSpacedWords(this string text) =>
            string.IsNullOrEmpty(text) ? text : Regex.Replace(text, "([a-z])([A-Z])", "$1 $2");

        /// <summary>
        /// Extracts the substring that appears after the first occurrence of the specified character.
        /// </summary>
        /// <param name="input">The input string to search within.</param>
        /// <param name="symbol">The character to search for as a delimiter.</param>
        /// <returns>
        /// The substring after the first occurrence of the specified character.
        /// Returns an empty string if the character is not found or if it's the last character in the string.
        /// </returns>
        [UsedImplicitly]
        public static string GetTextAfter(this string input, char symbol) =>
            input[(input.IndexOf(symbol) + 1)..];

        /// <summary>
        /// Extracts the substring that appears after the first occurrence of the specified delimiter string.
        /// </summary>
        /// <param name="input">The input string to search within.</param>
        /// <param name="delimiter">The string to search for as a delimiter.</param>
        /// <returns>
        /// The substring after the first occurrence of the specified delimiter.
        /// Returns an empty string if the delimiter is not found or if it appears at the end of the string.
        /// </returns>
        [UsedImplicitly]
        public static string GetTextAfter(this string input, string delimiter)
        {
            var index = input.IndexOf(delimiter, StringComparison.Ordinal);
            return index >= 0 ? input[(index + delimiter.Length)..] : string.Empty;
        }

        /// <summary>
        /// Gets the localized string for the specified localization key.
        /// </summary>
        /// <param name="key">The localization key to retrieve the translated text for.</param>
        /// <returns>The localized string corresponding to the key, or the key itself if no localization is found.</returns>
        [UsedImplicitly]
        public static string GetLocalization(this string key) => LocalizationController.Localize(key);

        /// <summary>
        /// Gets the localized string for the specified localization key.
        /// </summary>
        /// <param name="key">The localization key to retrieve the translated text for.</param>
        /// <param name="language">The specific language to use for localization.</param>
        /// <returns>The localized string corresponding to the key, or the key itself if no localization is found.</returns>
        [UsedImplicitly]
        public static string GetLocalization(this string key, SystemLanguage language)
            => LocalizationController.Localize(key, language);

        /// <summary>
        /// Determines whether the given string is valid, meaning it is not null or empty.
        /// </summary>
        /// <param name="str">The string to validate.</param>
        /// <returns>True if the string is not null or empty; otherwise, false.</returns>
        [UsedImplicitly]
        public static bool IsValid(this string str) => string.IsNullOrEmpty(str) is false;
    }
}