using System;
using ContactConversionWizard.Core.Model;

namespace ContactConversionWizard.Core.Text
{
    /// <summary>
    /// Parses the keywords users can put into the notes of a contact:
    /// <c>VIP</c>, <c>PREFIX(...)</c>, <c>PREFER(home|work|mobile)</c>, <c>SPEEDDIAL(...)</c> and <c>CCW-IGNORE</c>.
    /// </summary>
    public static class NoteFlags
    {
        public const string IgnoreKeyword = "CCW-IGNORE";

        public static bool IsIgnored(string notes) => (notes ?? "").Contains(IgnoreKeyword);

        public static bool IsVip(string nickname, string notes, VipMode mode)
        {
            switch (mode)
            {
                case VipMode.Never: return false;
                case VipMode.IfNickname: return !string.IsNullOrEmpty(nickname);
                case VipMode.IfNotesContainVip: return (notes ?? "").Contains("VIP");
                default: throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unsupported VIP mode.");
            }
        }

        /// <summary>Returns the contents of <c>PREFIX(...)</c> or an empty string.</summary>
        public static string ParsePrefix(string notes) => GetKeywordArgument(notes, "PREFIX(") ?? "";

        /// <summary>Returns the type named in <c>PREFER(...)</c> or <see cref="PhoneType.None"/>.</summary>
        public static PhoneType ParsePrefer(string notes)
        {
            string value = GetKeywordArgument(notes, "PREFER(");
            switch (value?.ToLowerInvariant())
            {
                case "home": return PhoneType.Home;
                case "work": return PhoneType.Work;
                case "mobile": return PhoneType.Mobile;
                default: return PhoneType.None;
            }
        }

        /// <summary>
        /// Parses <c>SPEEDDIAL(...)</c>. Returns <c>NN</c>, <c>NN,VANITY</c>, <c>XX,VANITY</c> or an empty string if
        /// the keyword is missing or invalid.
        /// </summary>
        public static string ParseSpeedDial(string notes)
        {
            string value = GetKeywordArgument(notes, "SPEEDDIAL(");
            if (value == null) { return ""; }

            string[] parts = value.Split(',');
            if (parts.Length == 1)
            {
                if (int.TryParse(value, out int number))
                {
                    return number < 100 ? number.ToString("00") : "";
                }
                // a vanity code without a speed dial number
                return value.Length <= 8 ? "XX," + value : "";
            }

            if (parts.Length == 2 || parts.Length == 3)
            {
                if (parts[1].Length <= 8)
                {
                    if (int.TryParse(parts[0], out int number)) { return number.ToString("00") + "," + parts[1]; }
                    if (parts[0] == "XX") { return "XX," + parts[1]; }
                }
            }

            return "";
        }

        /// <summary>Applies all keywords found in <paramref name="notes"/> to <paramref name="contact"/>.</summary>
        public static void Apply(Contact contact, string nickname, string notes, VipMode vipMode)
        {
            contact.IsVip = IsVip(nickname, notes, vipMode);
            contact.SpeedDial = ParseSpeedDial(notes);
            contact.FritzPrefix = ParsePrefix(notes);

            PhoneType prefer = ParsePrefer(notes);
            if (prefer != PhoneType.None) { contact.ExplicitPreferred = prefer; }
        }

        private static string GetKeywordArgument(string notes, string keyword)
        {
            if (string.IsNullOrEmpty(notes)) { return null; }

            int start = notes.IndexOf(keyword, StringComparison.Ordinal);
            if (start < 0) { return null; }

            string rest = notes.Substring(start + keyword.Length);
            int end = rest.IndexOf(')');
            return end < 0 ? null : rest.Substring(0, end);
        }
    }
}
