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
    }
}