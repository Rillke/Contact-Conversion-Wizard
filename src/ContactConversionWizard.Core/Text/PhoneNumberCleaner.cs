using System;
using System.Collections.Generic;
using System.Text;

namespace ContactConversionWizard.Core.Text
{
    /// <summary>Normalises phone numbers for export.</summary>
    public sealed class PhoneNumberCleaner
    {
        /// <summary>Country prefix value meaning "keep +XX prefixes intact".</summary>
        public const string KeepCountryPrefix = "keep";

        private readonly NumberCleanupOptions options;
        private readonly string countryPrefix;
        private readonly List<string> warnings = new List<string>();

        /// <param name="options">The characters to remove.</param>
        /// <param name="countryPrefix">The own country prefix such as <c>0049</c>, or <see cref="KeepCountryPrefix"/>.</param>
        public PhoneNumberCleaner(NumberCleanupOptions options, string countryPrefix)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.countryPrefix = countryPrefix ?? KeepCountryPrefix;
        }

        /// <summary>Warnings collected while cleaning (e.g. suspicious e-mail addresses used as numbers).</summary>
        public IReadOnlyList<string> Warnings => warnings;

        /// <summary>
        /// Removes unwanted characters, replaces the own country prefix by <c>0</c> and adds
        /// <paramref name="dialPrefix"/>. Values containing <c>@</c> (SIP addresses) are returned unchanged.
        /// </summary>
        public string Clean(string number, string dialPrefix = "")
        {
            number = number ?? "";
            dialPrefix = dialPrefix ?? "";

            if (number.Contains("@"))
            {
                if (number.Contains("<") || number.Contains(">"))
                {
                    warnings.Add("Warning: eMail '" + number + "' contains '<' and/or '>' characters. This is probably not a good idea!");
                }
                return number;
            }

            var buffer = new StringBuilder(number.Length);
            foreach (char c in number)
            {
                if (IsKept(c)) { buffer.Append(c); }
            }
            string result = buffer.ToString();

            if (countryPrefix != KeepCountryPrefix)
            {
                if (result.StartsWith("+", StringComparison.Ordinal)) { result = "00" + result.Substring(1); }
                if (countryPrefix != "" && result.StartsWith(countryPrefix, StringComparison.Ordinal))
                {
                    result = "0" + result.Substring(countryPrefix.Length);
                }
            }

            if (result != "")
            {
                result = dialPrefix + result;
                if (options.AddZeroPrefix) { result = "0" + result; }
            }

            return result;
        }

        private bool IsKept(char c)
        {
            if ((c >= '0' && c <= '9') || c == '+' || c == '*') { return true; }
            if (!options.RemoveBrackets && (c == '(' || c == ')')) { return true; }
            if (!options.RemoveHashKey && c == '#') { return true; }
            if (!options.RemoveHyphen && c == '-') { return true; }
            if (!options.RemoveXChar && c == 'x') { return true; }
            if (!options.RemoveSlash && c == '/') { return true; }
            if (!options.RemoveSpace && c == ' ') { return true; }
            if (!options.RemoveSquareBrackets && (c == '[' || c == ']')) { return true; }
            if (!options.RemoveLetters && ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))) { return true; }
            return false;
        }
    }
}
