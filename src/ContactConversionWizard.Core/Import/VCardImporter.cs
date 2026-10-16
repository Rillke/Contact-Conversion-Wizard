using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace ContactConversionWizard.Core.Import
{
    /// <summary>Reads contacts from vCard files (versions 2.1, 3.0 and 4.0, see RFC 2426 / RFC 6350).</summary>
    /// <remarks>
    /// The file is processed as bytes, so that every property can be decoded with its own <c>CHARSET</c>
    /// (e.g. <c>CHARSET=MACINTOSH</c> written by the macOS address book) and <c>ENCODING=QUOTED-PRINTABLE</c>
    /// (vCard 2.1 as written by many Android phones) works for any character set.
    /// </remarks>
    public sealed class VCardImporter
    {
        private readonly ImportOptions options;

        public VCardImporter(ImportOptions options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>Reads vCards from raw file content.</summary>
        /// <param name="data">The file content.</param>
        /// <param name="defaultEncoding">
        /// Encoding of properties without <c>CHARSET</c> parameter. <c>null</c> detects it: UTF-8 if the data is valid
        /// UTF-8, otherwise Windows-1252.
        /// </param>
        public ImportResult Import(byte[] data, Encoding defaultEncoding = null)
        {
            if (data == null) { throw new ArgumentNullException(nameof(data)); }

            data = ConvertUtf16ToUtf8(data, ref defaultEncoding);
            if (data.Length >= 3 && data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF)
            {
                data = data.Skip(3).ToArray();
                defaultEncoding = defaultEncoding ?? Encoding.UTF8;
            }

            return ImportBytes(data, defaultEncoding ?? DetectEncoding(data));
        }

        /// <summary>Reads vCards from already decoded text.</summary>
        public ImportResult Import(string text)
        {
            if (text == null) { throw new ArgumentNullException(nameof(text)); }
            return ImportBytes(Encoding.UTF8.GetBytes(text.TrimStart('﻿')), Encoding.UTF8);
        }

        /// <summary>UTF-8 if <paramref name="data"/> is valid UTF-8, otherwise Windows-1252 (a superset of ISO-8859-1).</summary>
        public static Encoding DetectEncoding(byte[] data)
        {
            try
            {
                new UTF8Encoding(false, true).GetString(data);
                return Encoding.UTF8;
            }
            catch (DecoderFallbackException)
            {
                return TextEncodings.Windows1252;
            }
        }

        private static byte[] ConvertUtf16ToUtf8(byte[] data, ref Encoding defaultEncoding)
        {
            Encoding utf16 = null;
            if (data.Length >= 2 && data[0] == 0xFF && data[1] == 0xFE) { utf16 = Encoding.Unicode; }
            else if (data.Length >= 2 && data[0] == 0xFE && data[1] == 0xFF) { utf16 = Encoding.BigEndianUnicode; }
            else if (defaultEncoding is UnicodeEncoding) { utf16 = defaultEncoding; }
            if (utf16 == null) { return data; }

            defaultEncoding = Encoding.UTF8;
            return Encoding.UTF8.GetBytes(utf16.GetString(data).TrimStart('﻿'));
        }

        private ImportResult ImportBytes(byte[] data, Encoding defaultEncoding)
        {
            var result = new ImportResult();
            var state = new CardState();

            // Latin-1 maps every byte to one char, so the structure can be parsed without knowing the encoding
            string raw = TextEncodings.Latin1.GetString(data);

            foreach (string rawLine in Unfold(raw))
            {
                try
                {
                    ProcessLine(rawLine, defaultEncoding, state, result);
                }
                catch (Exception ex)
                {
                    result.Warnings.Add(DecodeForMessage(rawLine, defaultEncoding) + Environment.NewLine + "==> " + ex.GetType().FullName + ": " + ex.Message);
                }
            }

            return result;
        }

        /// <summary>Splits the text into lines, joins folded lines and quoted-printable soft line breaks.</summary>
        internal static IEnumerable<string> Unfold(string text)
        {
            string normalized = text.Replace("\r\n", "\n").Replace('\r', '\n');
            var current = new StringBuilder();
            bool hasCurrent = false;

            foreach (string line in normalized.Split('\n'))
            {
                if (hasCurrent && line.Length > 0 && (line[0] == ' ' || line[0] == '\t'))
                {
                    current.Append(line, 1, line.Length - 1);
                    continue;
                }

                if (hasCurrent && IsQuotedPrintableContinued(current))
                {
                    current.Length--; // remove the soft line break "="
                    current.Append(line);
                    continue;
                }

                if (hasCurrent) { yield return current.ToString(); }
                current.Clear();
                current.Append(line);
                hasCurrent = true;
            }

            if (hasCurrent) { yield return current.ToString(); }
        }

        private static bool IsQuotedPrintableContinued(StringBuilder line)
        {
            if (line.Length == 0 || line[line.Length - 1] != '=') { return false; }
            string text = line.ToString();
            int colon = text.IndexOf(':');
            return colon > 0 && text.Substring(0, colon).ToUpperInvariant().Contains("QUOTED-PRINTABLE");
        }

        private void ProcessLine(string rawLine, Encoding defaultEncoding, CardState state, ImportResult result)
        {
            string line = rawLine;

            // group prefixes like "item1." (Apple) are ignored
            int colon = line.IndexOf(':');
            int dot = line.IndexOf('.');
            int firstSemicolon = line.IndexOf(';');
            if (dot > 0 && dot < colon && (firstSemicolon < 0 || dot < firstSemicolon))
            {
                line = line.Substring(dot + 1);
                colon = line.IndexOf(':');
            }

            if (line.Trim() == "") { return; }
            if (colon < 0) { throw new FormatException("Line contains no ':'."); }

            string name = line.Substring(0, colon);
            string parameters = "";
            int semicolon = name.IndexOf(';');
            if (semicolon >= 0)
            {
                parameters = name.Substring(semicolon + 1);
                name = name.Substring(0, semicolon);
            }
            name = name.Trim().ToUpperInvariant();

            string value = DecodeValue(line.Substring(colon + 1), parameters, defaultEncoding);

            // escaped characters should not disturb parsing
            value = value.Replace("\\:", ":").Replace("\\\\", "\\").Replace("\\;", ",").Replace("\\,", ",");

            switch (name)
            {
                case "BEGIN":
                    if (value.Trim().Equals("VCARD", StringComparison.OrdinalIgnoreCase)) { state.Reset(); }
                    break;
                case "END":
                    if (value.Trim().Equals("VCARD", StringComparison.OrdinalIgnoreCase)) { FinishCard(state, result); }
                    break;
                case "NOTE":
                    state.Notes = value;
                    break;
                case "N":
                    ParseName(value, state.Contact);
                    break;
                case "FN":
                    state.FullName = value.Trim();
                    break;
                case "NICKNAME":
                    state.Nickname = value.Trim();
                    break;
                case "ORG":
                    state.Contact.Company = value.TrimEnd(';').Replace(';', ' ').Trim();
                    break;
                case "TEL":
                    ParseTelephone(value.Trim(), parameters, state);
                    break;
                case "ADR":
                    ParseAddress(value, parameters, state);
                    break;
                case "EMAIL":
                    ParseEmail(value.Trim(), parameters, state);
                    break;
                case "PHOTO":
                    ParsePhoto(value, parameters, state.Contact);
                    break;
                case "BDAY":
                    ParseBirthday(value, parameters, state.Contact, result);
                    break;
            }
        }

        /// <summary>Decodes quoted-printable and the character set of a property value.</summary>
        private static string DecodeValue(string rawValue, string parameters, Encoding defaultEncoding)
        {
            Encoding encoding = defaultEncoding;
            bool quotedPrintable = false;

            foreach (string parameter in parameters.Split(';'))
            {
                string p = parameter.Trim();
                if (p.StartsWith("CHARSET=", StringComparison.OrdinalIgnoreCase))
                {
                    encoding = TextEncodings.GetEncoding(p.Substring("CHARSET=".Length).Trim('"')) ?? defaultEncoding;
                }
                else if (p.Equals("ENCODING=QUOTED-PRINTABLE", StringComparison.OrdinalIgnoreCase) || p.Equals("QUOTED-PRINTABLE", StringComparison.OrdinalIgnoreCase))
                {
                    quotedPrintable = true;
                }
            }

            byte[] bytes = quotedPrintable ? QuotedPrintable.Decode(rawValue) : TextEncodings.Latin1.GetBytes(rawValue);
            return encoding.GetString(bytes);
        }

        private static string DecodeForMessage(string rawLine, Encoding encoding)
        {
            return encoding.GetString(TextEncodings.Latin1.GetBytes(rawLine));
        }

        private static void ParseName(string value, Contact contact)
        {
            string[] parts = value.Split(';');
            contact.LastName = parts[0].Trim();
            contact.FirstName = parts.Length > 1 ? parts[1].Trim() : "";
        }

        private HashSet<string> ParseTypes(string parameters)
        {
            string types = parameters.ToLowerInvariant().Replace("type=", "");
            if (options.NextcloudVCardFix) { types = types.Replace("\"", ""); }
            var result = new HashSet<string>(types.Split(';', ',').Select(t => t.Trim()));

            // vCard 4.0: PREF=1 (the most preferred value)
            if (result.Contains("pref=1")) { result.Add("pref"); }
            return result;
        }

        private void ParseTelephone(string number, string parameters, CardState state)
        {
            Contact contact = state.Contact;
            if (number.StartsWith("tel:", StringComparison.OrdinalIgnoreCase)) { number = number.Substring(4); } // vCard 4.0 URI
            if (number == "") { return; }

            HashSet<string> types = ParseTypes(parameters);
            bool preferred = types.Contains("pref");
            bool fax = types.Contains("fax");

            // the last matching type wins
            PhoneType type = PhoneType.None;
            foreach (string t in types)
            {
                if (t == "home") { type = PhoneType.Home; }
                else if (t == "work") { type = PhoneType.Work; }
                else if (t == "cell" || t == "iphone" || t == "mobile") { type = PhoneType.Mobile; }
            }

            if (fax && type != PhoneType.Mobile)
            {
                // a fax without home/work type is treated according to the type preference
                bool home = type == PhoneType.Home || (type == PhoneType.None && options.TypePreference == TypePreference.Home);
                if (home && (preferred || contact.HomeFax == "")) { contact.HomeFax = number; }
                else if (!home && (preferred || contact.WorkFax == "")) { contact.WorkFax = number; }
                return;
            }

            if (type == PhoneType.None)
            {
                // TEL, TEL;VOICE, TEL;TYPE=OTHER, TEL;TYPE=MAIN ...: stored in a free field when the card is complete
                state.OtherNumbers.Add(number);
                return;
            }

            string current = ContactSlots.Get(contact, type);
            if (preferred || current == "")
            {
                if (current != "" && current != number) { state.OtherNumbers.Add(current); }
                ContactSlots.Set(contact, type, number);
                if (preferred) { contact.ExplicitPreferred = type; }
            }
            else if (current != number)
            {
                state.OtherNumbers.Add(number);
            }
        }

        private int RateType(HashSet<string> types, out bool isHomeOrWork)
        {
            bool home = types.Contains("home");
            bool work = types.Contains("work");
            isHomeOrWork = home || work;

            int rating = 0;
            if (options.TypePreference == TypePreference.Home && home) { rating += 1; }
            if (options.TypePreference == TypePreference.Work && work) { rating += 1; }
            if (types.Contains("pref")) { rating += 2; }
            return rating;
        }

        private void ParseAddress(string value, string parameters, CardState state)
        {
            int rating = RateType(ParseTypes(parameters), out bool isHomeOrWork);
            Contact contact = state.Contact;
            bool noAddressYet = contact.Street == "" && contact.Zip == "" && contact.City == "";

            // addresses without type are only used if there is no other one
            if (!isHomeOrWork && !noAddressYet) { return; }

            if (rating > state.AddressRating || noAddressYet)
            {
                VCardAddress address = VCardAddress.Parse(value);
                contact.Street = address.Street;
                contact.Zip = address.Zip;
                contact.City = address.City;
                state.AddressRating = isHomeOrWork ? rating : -1;
            }
        }

        private void ParseEmail(string value, string parameters, CardState state)
        {
            if (value.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase)) { value = value.Substring(7); }
            int rating = RateType(ParseTypes(parameters), out _);
            if (rating > state.EmailRating || state.Contact.Email == "")
            {
                state.Contact.Email = value;
                state.EmailRating = rating;
            }
        }

        private static void ParsePhoto(string value, string parameters, Contact contact)
        {
            string upperParameters = parameters.ToUpperInvariant();
            string encoded = null;

            if (upperParameters.Contains("BASE64") || upperParameters.Split(';').Contains("ENCODING=B"))
            {
                encoded = value;
            }
            else if (value.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && value.IndexOf(";base64,", StringComparison.OrdinalIgnoreCase) > 0)
            {
                // vCard 4.0 data URI
                encoded = value.Substring(value.IndexOf(";base64,", StringComparison.OrdinalIgnoreCase) + ";base64,".Length);
            }

            if (encoded != null)
            {
                contact.Photo = Convert.FromBase64String(encoded.Replace(" ", "").Replace("\t", ""));
            }
        }

        private static void ParseBirthday(string value, string parameters, Contact contact, ImportResult result)
        {
            string birthday = BirthdayParser.Normalize(value);
            if (birthday == null)
            {
                result.Warnings.Add("Unknown birthday format: BDAY:" + value);
                return;
            }

            // Apple marks birthdays without year with a placeholder year, e.g. BDAY;X-APPLE-OMIT-YEAR=1604:1604-01-19
            foreach (string parameter in parameters.Split(';'))
            {
                const string omitYear = "X-APPLE-OMIT-YEAR=";
                if (parameter.StartsWith(omitYear, StringComparison.OrdinalIgnoreCase) && birthday.StartsWith(parameter.Substring(omitYear.Length) + "-", StringComparison.Ordinal))
                {
                    birthday = "-" + birthday.Substring(4);
                }
            }

            contact.Birthday = birthday;
        }

        private void FinishCard(CardState state, ImportResult result)
        {
            Contact contact = state.Contact;
            NoteFlags.Apply(contact, state.Nickname, state.Notes, options.VipMode);

            foreach (string other in state.OtherNumbers)
            {
                if (!ContactSlots.PutIntoFreeSlot(contact, other, options.TypePreference, includeMobile: true))
                {
                    result.Warnings.Add((state.FullName != "" ? state.FullName : contact.LastName) + ": the number " + other + " was not imported, all number fields are in use.");
                }
            }

            // if FN equals the company, the N line only duplicates information
            if (contact.Company == state.FullName && state.FullName != "")
            {
                contact.FirstName = "";
                contact.LastName = "";
            }

            // vCards without N (e.g. jFritz 0.7.3.10)
            if (contact.FirstName == "" && contact.LastName == "" && contact.Company == "" && state.FullName != "")
            {
                contact.LastName = state.FullName;
            }

            contact.CombinedName = NameFormatter.GenerateFullName(contact.FirstName, contact.LastName, contact.Company, options.NameStyle);

            if (options.Accept(contact, state.Notes))
            {
                result.Contacts.Add(contact);
            }

            state.Reset();
        }

        private sealed class CardState
        {
            public Contact Contact { get; private set; } = new Contact();
            public string FullName { get; set; } = "";
            public string Notes { get; set; } = "";
            public string Nickname { get; set; } = "";
            public int AddressRating { get; set; }
            public int EmailRating { get; set; }
            public List<string> OtherNumbers { get; } = new List<string>();

            public void Reset()
            {
                Contact = new Contact();
                FullName = "";
                Notes = "";
                Nickname = "";
                AddressRating = 0;
                EmailRating = 0;
                OtherNumbers.Clear();
            }
        }
    }
}
