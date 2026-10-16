using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ContactConversionWizard.Core.Import
{
    /// <summary>Normalizes birthdays to <c>YYYY-MM-DD</c> or, without year, <c>--MM-DD</c>.</summary>
    public static class BirthdayParser
    {
        private static readonly Regex[] Patterns =
        {
            new Regex(@"^(?<y>\d{4})-?(?<m>\d{2})-?(?<d>\d{2})$"),     // 1980-01-19, 19800119 (ISO 8601, vCard)
            new Regex(@"^--(?<m>\d{2})-?(?<d>\d{2})$"),                // --0119, --01-19 (vCard 4.0 without year)
            new Regex(@"^(?<d>\d{1,2})\.(?<m>\d{1,2})\.(?<y>\d{4})?$"), // 19.01.1980, 19.01. (German)
        };

        /// <summary>The year used when a format requires a year but none is known.</summary>
        public const int PlaceholderYear = 1900;

        /// <summary>Returns the normalized birthday, an empty string for empty input or <c>null</c> if the format is unknown.</summary>
        public static string Normalize(string value)
        {
            string text = (value ?? "").Trim();
            if (text == "") { return ""; }

            // drop a time part (1980-01-19T00:00:00Z)
            int time = text.IndexOf('T');
            if (time > 0) { text = text.Substring(0, time); }

            foreach (Regex pattern in Patterns)
            {
                Match match = pattern.Match(text);
                if (!match.Success) { continue; }

                int month = int.Parse(match.Groups["m"].Value, CultureInfo.InvariantCulture);
                int day = int.Parse(match.Groups["d"].Value, CultureInfo.InvariantCulture);
                bool hasYear = match.Groups["y"].Success && match.Groups["y"].Value != "";
                int year = hasYear ? int.Parse(match.Groups["y"].Value, CultureInfo.InvariantCulture) : 2000; // leap year for validation

                if (month < 1 || month > 12 || day < 1 || day > DateTime.DaysInMonth(year, month)) { return null; }
                return (hasYear ? year.ToString("0000", CultureInfo.InvariantCulture) : "-") + "-" + month.ToString("00", CultureInfo.InvariantCulture) + "-" + day.ToString("00", CultureInfo.InvariantCulture);
            }

            return null;
        }

        /// <summary>The birthday as date, using <see cref="PlaceholderYear"/> if the year is unknown.</summary>
        public static bool TryGetDate(string birthday, out DateTime date, out bool hasYear)
        {
            date = default(DateTime);
            hasYear = false;
            string normalized = Normalize(birthday);
            if (string.IsNullOrEmpty(normalized)) { return false; }

            hasYear = !normalized.StartsWith("--", StringComparison.Ordinal);
            string iso = hasYear ? normalized : PlaceholderYear.ToString(CultureInfo.InvariantCulture) + normalized.Substring(1);
            if (!hasYear && normalized == "--02-29") { iso = "1904-02-29"; } // the placeholder year 1900 has no 29 February
            return DateTime.TryParseExact(iso, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }
    }
}
