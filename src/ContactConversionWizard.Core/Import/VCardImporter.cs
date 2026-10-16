using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace ContactConversionWizard.Core.Import
{
    /// <summary>Reads contacts from vCard files (versions 2.1, 3.0 and 4.0, see RFC 2426 / RFC 6350).</summary>
    public sealed class VCardImporter
    {
        private readonly ImportOptions options;

        public VCardImporter(ImportOptions options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public ImportResult Import(byte[] data, Encoding encoding)
        {
            if (data == null) { throw new ArgumentNullException(nameof(data)); }
            if (encoding == null) { throw new ArgumentNullException(nameof(encoding)); }
            return Import(encoding.GetString(data).TrimStart('﻿'));
        }

        public ImportResult Import(string text)
        {
            if (text == null) { throw new ArgumentNullException(nameof(text)); }

            var result = new ImportResult();
            var state = new CardState();

            foreach (string rawLine in Unfold(text))
            {
                try
                {
                    ProcessLine(rawLine, state, result);
                }
                catch (Exception ex)
                {
                    result.Warnings.Add(rawLine + Environment.NewLine + "==> " + ex.GetType().FullName + ": " + ex.Message);
                }
            }

            return result;
        }

        /// <summary>Splits the text into lines and joins folded continuation lines.</summary>
        internal static IEnumerable<string> Unfold(string text)
        {
            string normalized = text.Replace("\r\n", "\n").Replace('\r', '\n');
            var current = new StringBuilder();
            bool hasCurrent = false;

            foreach (string line in normalized.Split('\n'))
            {
                if (line.Length > 0 && (line[0] == ' ' || line[0] == '\t') && hasCurrent)
                {
                    current.Append(line, 1, line.Length - 1);
                    continue;
                }

                if (hasCurrent) { yield return current.ToString(); }
                current.Clear();
                current.Append(line);
                hasCurrent = true;
            }

            if (hasCurrent) { yield return current.ToString(); }
        }

        private void ProcessLine(string rawLine, CardState state, ImportResult result)
        {
            // escaped characters should not disturb parsing
            string line = rawLine.Replace("\\:", ":").Replace("\\\\", "\\").Replace("\\;", ",").Replace("\\,", ",");

            // group prefixes like "item1." (Apple) are ignored
            if (line.StartsWith("item", StringComparison.OrdinalIgnoreCase) && line.IndexOf('.') >= 0 && line.IndexOf('.') < line.IndexOf(':'))
            {
                line = line.Substring(line.IndexOf('.') + 1);
            }

            if (line.Trim() == "") { return; }

            int colon = line.IndexOf(':');
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
            string value = line.Substring(colon + 1);

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
                    ParseTelephone(value, parameters, state.Contact);
                    break;
                case "ADR":
                    ParseAddress(value, parameters, state);
                    break;
                case "EMAIL":
                    ParseEmail(value, parameters, state);
                    break;
                case "PHOTO":
                    ParsePhoto(value, parameters, state.Contact);
                    break;
            }
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
            return new HashSet<string>(types.Split(';', ',').Select(t => t.Trim()));
        }

        private void ParseTelephone(string number, string parameters, Contact contact)
        {
            HashSet<string> types = ParseTypes(parameters);
            bool preferred = types.Contains("pref");
            bool fax = types.Contains("fax");

            // the last matching type wins, like in the original parser
            PhoneType type = PhoneType.None;
            foreach (string t in types)
            {
                if (t == "home") { type = PhoneType.Home; }
                else if (t == "work") { type = PhoneType.Work; }
                else if (t == "cell") { type = PhoneType.Mobile; }
            }

            switch (type)
            {
                case PhoneType.Home when !fax:
                    if (preferred) { contact.Home = number; contact.ExplicitPreferred = PhoneType.Home; }
                    else if (contact.Home == "") { contact.Home = number; }
                    break;
                case PhoneType.Work when !fax:
                    if (preferred) { contact.Work = number; contact.ExplicitPreferred = PhoneType.Work; }
                    else if (contact.Work == "") { contact.Work = number; }
                    break;
                case PhoneType.Mobile:
                    if (preferred) { contact.Mobile = number; contact.ExplicitPreferred = PhoneType.Mobile; }
                    else if (contact.Mobile == "") { contact.Mobile = number; }
                    break;
                case PhoneType.Home:
                    if (preferred || contact.HomeFax == "") { contact.HomeFax = number; }
                    break;
                case PhoneType.Work:
                    if (preferred || contact.WorkFax == "") { contact.WorkFax = number; }
                    break;
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
            if (!isHomeOrWork) { return; }

            Contact contact = state.Contact;
            bool noAddressYet = contact.Street == "" && contact.Zip == "" && contact.City == "";
            if (rating > state.AddressRating || noAddressYet)
            {
                VCardAddress address = VCardAddress.Parse(value);
                contact.Street = address.Street;
                contact.Zip = address.Zip;
                contact.City = address.City;
                state.AddressRating = rating;
            }
        }

        private void ParseEmail(string value, string parameters, CardState state)
        {
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

        private void FinishCard(CardState state, ImportResult result)
        {
            Contact contact = state.Contact;
            NoteFlags.Apply(contact, state.Nickname, state.Notes, options.VipMode);

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

            if (contact.CombinedName != "" && contact.HasAnyNumber && !NoteFlags.IsIgnored(state.Notes))
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

            public void Reset()
            {
                Contact = new Contact();
                FullName = "";
                Notes = "";
                Nickname = "";
                AddressRating = 0;
                EmailRating = 0;
            }
        }
    }
}
