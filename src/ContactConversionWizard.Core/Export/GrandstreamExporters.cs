using System.Collections.Generic;
using System.Text;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace ContactConversionWizard.Core.Export
{
    /// <summary>Grandstream GXV phones (XML address book, UTF-8 without BOM).</summary>
    public sealed class GrandstreamGxvExporter : ContactExporter
    {
        public const int MaxNameLength = 32;
        public const int RecommendedMaxContacts = 500;

        public override string FormatName => "Grandstream GXV XML";
        public override string DefaultFileName => "gs_phonebook";
        public override string FileExtension => "xml";

        protected override ExportResult Export(IReadOnlyList<Contact> contacts, ExportContext context)
        {
            var entries = new EntryCollector(context);
            bool combinedNameWithCompany = NameFormatter.IncludesCompany(context.Options.NameStyle);

            foreach (Contact contact in contacts)
            {
                CleanNumbers numbers = context.Clean(contact);
                if (!numbers.HasVoiceNumber) { continue; }

                string name = Escaping.Xml(NameFormatter.LimitLength(contact.CombinedName, MaxNameLength));
                GrandstreamNames.Split(contact, combinedNameWithCompany, out string firstName, out string lastName);
                var sb = new StringBuilder("    <Contact>\n");
                sb.Append("        <FirstName>").Append(Escaping.Xml(NameFormatter.LimitLength(firstName, MaxNameLength))).Append("</FirstName>\n");
                sb.Append("        <LastName>").Append(Escaping.Xml(NameFormatter.LimitLength(lastName, MaxNameLength))).Append("</LastName>\n");

                AppendPhone(sb, "Home", numbers.Home);
                AppendPhone(sb, "Work", numbers.Work);
                AppendPhone(sb, "Mobile", numbers.Mobile);
                sb.Append("    </Contact>\n");

                entries.Add(name, sb.ToString(), contact.CombinedName);
            }

            var document = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<AddressBook>\n    <version>1</version>\n");
            entries.AppendTo(document);
            document.Append("</AddressBook>");

            var result = new ExportResult(document.ToString(), TextEncodings.Utf8NoBom, entries.Count);
            if (entries.Count > RecommendedMaxContacts)
            {
                result.Notices.Add("Warning: Over 500 contacts have been exported! This is not supported by the Grandstream GXV 3140. Proceed with care!");
            }
            return result;
        }

        private static void AppendPhone(StringBuilder sb, string type, string number)
        {
            if (number == "") { return; }
            sb.Append("        <Phone type=\"").Append(type).Append("\">\n")
              .Append("            <phonenumber>").Append(Escaping.Xml(number)).Append("</phonenumber>\n")
              .Append("            <accountindex>0</accountindex>\n")
              .Append("        </Phone>\n");
        }
    }

    /// <summary>How the separate first and last name fields of Grandstream phones are filled.</summary>
    internal static class GrandstreamNames
    {
        /// <summary>
        /// The combined name goes into the last name if the selected name style includes the company and the contact has
        /// one (or if the contact has no first and last name); otherwise first and last name are used.
        /// </summary>
        public static void Split(Contact contact, bool combinedNameWithCompany, out string firstName, out string lastName)
        {
            bool useCombined = (combinedNameWithCompany && contact.Company != "") || (contact.FirstName == "" && contact.LastName == "");
            firstName = useCombined ? "" : contact.FirstName;
            lastName = useCombined ? contact.CombinedName : contact.LastName;
        }
    }

    /// <summary>
    /// Grandstream GXP phones (XML address book, one entry per number, UTF-8 without BOM). First and last name are written
    /// to separate fields like for GXV phones; if a contact has more than one number, the last name gets the suffixes
    /// " H", " W" and " M".
    /// </summary>
    public sealed class GrandstreamGxpExporter : PerNumberCsvExporter
    {
        private bool combinedNameWithCompany;
        public override string FormatName => "Grandstream GXP XML";
        public override string DefaultFileName => "gs_phonebook";
        public override string FileExtension => "xml";
        protected override int MaxNameLength => 20;
        protected override string Header => "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<AddressBook>\n";
        protected override Encoding FileEncoding => TextEncodings.Utf8NoBom;

        protected override ExportResult Export(IReadOnlyList<Contact> contacts, ExportContext context)
        {
            combinedNameWithCompany = NameFormatter.IncludesCompany(context.Options.NameStyle);
            ExportResult body = base.Export(contacts, context);
            var result = new ExportResult(body.Content + "</AddressBook>\n", body.Encoding, body.EntryCount);
            result.Warnings.AddRange(body.Warnings);
            return result;
        }

        // contacts with the same last name are different entries
        protected override string GetKey(string name, PhoneType type, Contact contact)
        {
            GrandstreamNames.Split(contact, combinedNameWithCompany, out string firstName, out _);
            return name + "\u0001" + firstName;
        }

        protected override string GetNameBase(Contact contact)
        {
            GrandstreamNames.Split(contact, combinedNameWithCompany, out _, out string lastName);
            return lastName;
        }

        protected override string FormatLine(string name, string number, PhoneType type, Contact contact, string speedDialNumber)
        {
            GrandstreamNames.Split(contact, combinedNameWithCompany, out string firstName, out _);
            return "    <Contact>\n"
                 + "        <LastName>" + Escaping.Xml(name) + "</LastName>\n"
                 + "        <FirstName>" + Escaping.Xml(NameFormatter.LimitLength(firstName, MaxNameLength)) + "</FirstName>\n"
                 + "        <Phone>\n"
                 + "            <phonenumber>" + Escaping.Xml(number) + "</phonenumber>\n"
                 + "            <accountindex>0</accountindex>\n"
                 + "        </Phone>\n"
                 + "    </Contact>\n";
        }
    }
}
