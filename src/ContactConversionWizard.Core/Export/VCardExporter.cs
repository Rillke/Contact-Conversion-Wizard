using System.Collections.Generic;
using System.Text;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace ContactConversionWizard.Core.Export
{
    /// <summary>vCard 3.0 file (UTF-8 without BOM).</summary>
    public sealed class VCardExporter : ContactExporter
    {
        public override string FormatName => "vCard";
        public override string DefaultFileName => "vCard Export";
        public override string FileExtension => "vcf";

        protected override ExportResult Export(IReadOnlyList<Contact> contacts, ExportContext context)
        {
            var entries = new EntryCollector(context);
            string addressType = context.Options.TypePreference == TypePreference.Home ? "HOME" : "WORK";

            foreach (Contact contact in contacts)
            {
                CleanNumbers numbers = context.Clean(contact);
                if (context.Options.OnlyWithPhoneNumber && !numbers.HasVoiceNumber) { continue; }

                string street = Escaping.VCard(contact.Street);
                string city = Escaping.VCard(contact.City);
                string zip = Escaping.VCard(contact.Zip);
                string email = Escaping.VCard(contact.Email);
                string company = Escaping.VCard(contact.Company);
                PhoneType preferred = contact.Preferred;

                var sb = new StringBuilder();
                sb.Append("BEGIN:VCARD\r\n");
                sb.Append("VERSION:3.0\r\n");
                sb.Append("N:").Append(Escaping.VCard(contact.LastName)).Append(';').Append(Escaping.VCard(contact.FirstName)).Append(";;;\r\n");
                sb.Append("FN:").Append(Escaping.VCard(contact.CombinedName)).Append("\r\n");
                if (company != "") { sb.Append("ORG:").Append(company).Append(";\r\n"); }

                AppendTel(sb, "HOME", numbers.Home, preferred == PhoneType.Home);
                AppendTel(sb, "WORK", numbers.Work, preferred == PhoneType.Work);
                AppendTel(sb, "CELL", numbers.Mobile, preferred == PhoneType.Mobile);
                if (numbers.HomeFax != "") { sb.Append("TEL;type=HOME;type=FAX:").Append(numbers.HomeFax).Append("\r\n"); }
                if (numbers.WorkFax != "") { sb.Append("TEL;type=WORK;type=FAX:").Append(numbers.WorkFax).Append("\r\n"); }

                if (!(street == "" && zip == "" && city == ""))
                {
                    sb.Append("ADR;type=").Append(addressType).Append(";type=pref:;;").Append(street).Append(';').Append(city).Append(";;").Append(zip).Append(";\r\n");
                }

                if (email != "") { sb.Append("EMAIL;type=INTERNET;type=pref:").Append(email).Append("\r\n"); }

                string notes = (contact.IsVip ? "VIP " : "") + (contact.SpeedDial != "" ? "SPEEDDIAL(" + contact.SpeedDial + ")" : "");
                if (notes != "") { sb.Append("NOTE:").Append(notes).Append("\r\n"); }

                sb.Append("END:VCARD\r\n");
                entries.Add(contact.CombinedName, sb.ToString(), contact.CombinedName);
            }

            var document = new StringBuilder();
            entries.AppendTo(document);
            return new ExportResult(document.ToString(), TextEncodings.Utf8NoBom, entries.Count);
        }

        private static void AppendTel(StringBuilder sb, string type, string number, bool preferred)
        {
            if (number == "") { return; }
            sb.Append("TEL;type=").Append(type).Append(preferred ? ";type=pref:" : ":").Append(number).Append("\r\n");
        }
    }
}
