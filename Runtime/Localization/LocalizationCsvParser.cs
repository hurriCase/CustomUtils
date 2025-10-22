using System.Collections.Generic;
using System.Text.RegularExpressions;
using ZLinq;

namespace CustomUtils.Runtime.Localization
{
    internal static class LocalizationCsvParser
    {
        private const string QuotePlaceholder = "[_quote_]";
        private const string CommaPlaceholder = "[_comma_]";
        private const string NewlinePlaceholder = "[_newline_]";

        internal static List<string> ParseLines(string csvText)
        {
            csvText = NormalizeLineEndings(csvText);
            csvText = ProtectQuotedFields(csvText);
            csvText = ProcessAsianTextSpacing(csvText);

            return csvText.Split('\n')
                .AsValueEnumerable()
                .Where(static line => string.IsNullOrEmpty(line) is false)
                .ToList();
        }

        internal static List<string> ParseColumns(string line) =>
            line.Split(',')
                .AsValueEnumerable()
                .Select(static column => column.Trim()
                    .Replace(QuotePlaceholder, "\"")
                    .Replace(CommaPlaceholder, ",")
                    .Replace(NewlinePlaceholder, "\n"))
                .ToList();

        private static string NormalizeLineEndings(string text) =>
            text.Replace("\r\n", "\n").Replace("\"\"", QuotePlaceholder);

        private static string ProtectQuotedFields(string csvText)
        {
            var matches = Regex.Matches(csvText, "\"[\\s\\S]+?\"");
            foreach (Match match in matches)
            {
                csvText = csvText.Replace(match.Value,
                    match.Value.Replace("\"", string.Empty)
                        .Replace(",", CommaPlaceholder)
                        .Replace("\n", NewlinePlaceholder));
            }

            return csvText;
        }

        private static string ProcessAsianTextSpacing(string text) =>
            text.Replace("。", "。 ")
                .Replace("、", "、 ")
                .Replace("：", "： ")
                .Replace("！", "！ ")
                .Replace("（", " （")
                .Replace("）", "） ")
                .Trim();
    }
}