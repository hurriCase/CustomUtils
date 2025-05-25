using System;
using System.Text.RegularExpressions;

namespace CustomUtils.Runtime.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a camelCase or PascalCase string to a spaced string.
        /// </summary>
        /// <param name="text">The camelCase or PascalCase string to convert.</param>
        /// <returns>A string with spaces between words.</returns>
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
        public static string GetTextAfter(this string input, string delimiter)
        {
            var index = input.IndexOf(delimiter, StringComparison.Ordinal);
            return index >= 0 ? input[(index + delimiter.Length)..] : string.Empty;
        }
    }
}