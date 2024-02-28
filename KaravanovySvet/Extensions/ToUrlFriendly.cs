using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace KaravanovySvet.Extensions
{
    public static class StringExtensions
    {
        public static string ToUrlFriendly(this string title)
        {
            if (string.IsNullOrEmpty(title)) return "";

            // Replace Czech characters with ASCII equivalents
            var sb = new StringBuilder();
            var normalizedString = title.Normalize(NormalizationForm.FormD);
            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            title = sb.ToString().Normalize(NormalizationForm.FormC);

            // Manual replacements for specific Czech characters not handled by normalization
            title = title.Replace("ě", "e")
                         .Replace("š", "s")
                         .Replace("č", "c")
                         .Replace("ř", "r")
                         .Replace("ž", "z")
                         .Replace("ý", "y")
                         .Replace("á", "a")
                         .Replace("í", "i")
                         .Replace("é", "e")
                         .Replace("ó", "o")
                         .Replace("ú", "u")
                         .Replace("ů", "u");

            // Lowercase the string
            title = title.ToLowerInvariant();

            // Replace invalid characters with hyphens
            title = Regex.Replace(title, @"[^a-z0-9\s-]", "-");

            // Convert multiple spaces/hyphens into one hyphen
            title = Regex.Replace(title, @"[\s-]+", "-").Trim();

            return title;
        }
    }
}