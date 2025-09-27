using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CustomUtils.Runtime.Localization;
using Cysharp.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a property name to its backing field representation
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <returns>The backing field name format</returns>
        [UsedImplicitly]
        public static string ConvertToBackingField(this string propertyName) => $"<{propertyName}>k__BackingField";

        /// <summary>
        /// Converts a camelCase or PascalCase string to a spaced string.
        /// </summary>
        /// <param name="text">The camelCase or PascalCase string to convert.</param>
        /// <returns>A string with spaces between words.</returns>
        [UsedImplicitly]
        public static string ToSpacedWords(this string text) =>
            text.IsValid() ? Regex.Replace(text, "([a-z])([A-Z])", "$1 $2") : text;

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

        /// <summary>
        /// Determines whether the given string is invalid, meaning it is null or empty.
        /// </summary>
        /// <param name="str">The string to validate.</param>
        /// <returns>True if the string is null or empty; otherwise, false.</returns>
        [UsedImplicitly]
        public static bool IsInvalid(this string str) => string.IsNullOrEmpty(str);

        /// <summary>
        /// Splits a string by the specified delimiter and returns a list of trimmed string elements,
        /// excluding empty or whitespace-only elements.
        /// </summary>
        /// <param name="value">The string to split.</param>
        /// <param name="delimiter">The character used as a delimiter for splitting.</param>
        /// <returns>A list of trimmed string elements, with empty elements excluded.</returns>
        [UsedImplicitly]
        public static List<string> SplitToListTrimmed(this string value, char delimiter)
            => SplitToListTrimmed(value, delimiter, element => element);

        /// <summary>
        /// Splits a string by the specified delimiter, trims each element, and converts them to the specified type
        /// using the provided converter function. Empty or whitespace-only elements are excluded.
        /// </summary>
        /// <typeparam name="T">The type to convert each string element to.</typeparam>
        /// <param name="value">The string to split.</param>
        /// <param name="delimiter">The character used as a delimiter for splitting.</param>
        /// <param name="converter">A function that converts a trimmed string element to type T.</param>
        /// <returns>A list of converted elements of type T, with empty elements excluded.</returns>
        [UsedImplicitly]
        public static List<T> SplitToListTrimmed<T>(this string value, char delimiter, Func<string, T> converter)
        {
            if (value.IsInvalid())
                return new List<T>();

            var result = new List<T>();
            var startIndex = 0;

            for (var i = 0; i <= value.Length; i++)
            {
                if (i != value.Length && value[i] != delimiter)
                    continue;

                if (i > startIndex)
                {
                    var element = ExtractTrimmedElement(value, startIndex, i - 1);
                    if (element.IsValid() is false)
                    {
                        var convertedValue = converter(element);
                        result.Add(convertedValue);
                    }
                }

                startIndex = i + 1;
            }

            return result;
        }

        /// <summary>
        /// Extracts a trimmed substring from the specified range within a string,
        /// removing leading and trailing whitespace characters.
        /// </summary>
        /// <param name="value">The source string to extract from.</param>
        /// <param name="start">The starting index (inclusive) of the substring.</param>
        /// <param name="end">The ending index (inclusive) of the substring.</param>
        /// <returns>
        /// A trimmed substring from the specified range, or an empty string if the range
        /// contains only whitespace or if start index is greater than end index.
        /// </returns>
        [UsedImplicitly]
        public static string ExtractTrimmedElement(string value, int start, int end)
        {
            while (start <= end && char.IsWhiteSpace(value[start]))
            {
                start++;
            }

            while (end >= start && char.IsWhiteSpace(value[end]))
            {
                end--;
            }

            if (start > end)
                return string.Empty;

            using var builder = ZString.CreateStringBuilder(false);
            builder.Append(value, start, end - start + 1);
            return builder.ToString();
        }

        /// <summary>
        /// Attempts to retrieve the value of an environment variable.
        /// </summary>
        /// <param name="environmentVariableName">The name of the environment variable to retrieve.</param>
        /// <param name="value">When this method returns,
        /// contains the environment variable value if it exists and is valid; otherwise, null.</param>
        /// <param name="environmentVariableTarget">The target scope for the environment variable lookup.</param>
        /// <returns>True if the environment variable exists and has a valid value; otherwise, false.</returns>
        [UsedImplicitly]
        public static bool TryGetValueFromEnvironment(
            this string environmentVariableName,
            out string value,
            EnvironmentVariableTarget environmentVariableTarget = EnvironmentVariableTarget.Machine)
        {
            value = Environment.GetEnvironmentVariable(environmentVariableName, environmentVariableTarget);

            if (value.IsValid())
                return true;

            Debug.LogError("[StringExtensions::TryGetValueFromEnvironment] " +
                           $"Environment variable value for {environmentVariableName} wasn't found");
            return false;
        }
    }
}