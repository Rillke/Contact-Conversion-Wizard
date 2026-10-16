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
                var sb = new StringBuilder("    <Contact>\n");
                if (combinedNameWithCompany && contact.Company != "")
                {
                    sb.Append("        <FirstName></FirstName>\n");
                    sb.Append("        <LastName>").Append(name).Append("</LastName>\n");
                }
                else
                {
                    sb.Append("        <FirstName>").Append(Escaping.Xml(NameFormatter.LimitLength(contact.FirstName, MaxNameLength))).Append("</FirstName>\n");
                    sb.Append("        <LastName>").Append(Escaping.Xml(NameFormatter.LimitLength(contact.LastName, MaxNameLength))).Append("</LastName>\n");
                }

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
              .Append("            <phonenumber>").Append(number).Append("</phonenumber>\n")
              .Append("            <accountindex>0</accountindex>\n")
              .Append("        </Phone>\n");
        }
    }

    /// <summary>Grandstream GXP phones (XML address book, one entry per number, UTF-8 without BOM).</summary>
    public sealed class GrandstreamGxpExporter : PerNumberCsvExporter
    {
        public override string FormatName => "Grandstream GXP XML";
        public override string DefaultFileName => "gs_phonebook";
        public override string FileExtension => "xml";
        protected override int MaxNameLength => 20;
        protected override string Header => "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<AddressBook>\n";
        protected override Encoding FileEncoding => TextEncodings.Utf8NoBom;

        protected override ExportResult Export(IReadOnlyList<Contact> contacts, ExportContext context)
        {
            ExportResult body = base.Export(contacts, context);
            var result = new ExportResult(body.Content + "</AddressBook>\n", body.Encoding, body.EntryCount);
            result.Warnings.AddRange(body.Warnings);
            return result;
        }

        protected override string AdjustName(string name) => Escaping.Xml(name);

        protected override string FormatLine(string name, string number, PhoneType type, Contact contact, string speedDialNumber)
        {
            return "    <Contact>\n"
                 + "        <LastName>" + name + "</LastName>\n"
                 + "        <FirstName></FirstName>\n"
                 + "        <Phone>\n"
                 + "            <phonenumber>" + number + "</phonenumber>\n"
                 + "            <accountindex>0</accountindex>\n"
                 + "        </Phone>\n"
                 + "    </Contact>\n";
        }
    }
}
