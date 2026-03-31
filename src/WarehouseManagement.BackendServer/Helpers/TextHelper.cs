using System.Text;
using System.Text.RegularExpressions;

namespace WarehouseManagement.BackendServer.Helpers
{
    public class TextHelper
    {
        private static readonly Regex DiacriticsRegex =
            new Regex(@"\p{IsCombiningDiacriticalMarks}+", RegexOptions.Compiled);

        private static readonly Regex SpecialCharRegex =
            new Regex(@"[^\p{L}\p{Nd}\s-]", RegexOptions.Compiled);

        private static readonly Regex MultiDashRegex =
            new Regex(@"-+", RegexOptions.Compiled);

        private static readonly Regex SpaceRegex =
            new Regex(@"\s+", RegexOptions.Compiled);

        public static string ToUnsignedString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = input.Trim();

            // Remove diacritics
            string normalized = input.Normalize(NormalizationForm.FormD);
            string withoutDiacritics = DiacriticsRegex
                .Replace(normalized, string.Empty)
                .Replace('đ', 'd')
                .Replace('Đ', 'D');

            // Remove special characters
            string cleaned = SpecialCharRegex.Replace(withoutDiacritics, " ");

            // Replace spaces with "-"
            string slug = SpaceRegex.Replace(cleaned, "-");

            // Replace multiple "-" with single "-"
            slug = MultiDashRegex.Replace(slug, "-");

            // Lowercase and trim leading/trailing "-"
            return slug.ToLower().Trim('-');
        }

        public static string NormalizeSpaces(string input)
        {
            return Regex.Replace(input.Trim(), @"\s+", " ");
        }

        public static string GenerateSku(string name, int id)
        {
            var slug = ToUnsignedString(name);
            return $"{slug}-{id}".ToUpper();
        }

        public static string RemoveSign4VietnameseString(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;

            string normalized = str.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char c in normalized)
            {
                System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC)
                .Replace('đ', 'd').Replace('Đ', 'D');
        }

        public static string Truncate(string value, int maxChars)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }

        public static string GenerateRandomCode(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}