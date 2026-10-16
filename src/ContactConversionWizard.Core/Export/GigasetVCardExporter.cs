using System.Collections.Generic;
using System.Text;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace ContactConversionWizard.Core.Export
{
    /// <summary>Simplified vCard 2.1 for Gigaset phones (tested with DX600A, names limited to 16 characters).</summary>
    public sealed class GigasetVCardExporter : ContactExporter
    {
        public const int MaxNameLength = 16;

        public override string FormatName => "Gigaset vCard";
        public override string DefaultFileName => "Simplified vCard Export for Gigaset";
        public override string FileExtension => "vcf";

        protected override ExportResult Export(IReadOnlyList<Contact> contacts, ExportContext context)
        {
            var entries = new EntryCollector(context);
            bool combinedNameWithCompany = NameFormatter.IncludesCompany(context.Options.NameStyle);

            foreach (Contact contact in contacts)
            {
                CleanNumbers numbers = context.Clean(contact);
                if (!numbers.HasVoiceNumber) { continue; }

                var sb = new StringBuilder();
                sb.Append("BEGIN:VCARD\r\n");
                sb.Append("VERSION:2.1\r\n");

                if ((combinedNameWithCompany && contact.Company != "") || (contact.LastName == "" && contact.FirstName == ""))
                {
                    sb.Append("N:").Append(Limit(contact.CombinedName)).Append(";\r\n");
                }
                else
                {
                    sb.Append("N:").Append(Limit(contact.LastName)).Append(';').Append(Limit(contact.FirstName)).Append("\r\n");
                }

                if (numbers.Home != "") { sb.Append("TEL;HOME:").Append(numbers.Home).Append("\r\n"); }
                if (numbers.Work != "") { sb.Append("TEL;WORK:").Append(numbers.Work).Append("\r\n"); }
                if (numbers.Mobile != "") { sb.Append("TEL;CELL:").Append(numbers.Mobile).Append("\r\n"); }

                string email = Escaping.VCard(contact.Email);
                if (email != "") { sb.Append("EMAIL:").Append(email).Append("\r\n"); }

                sb.Append("END:VCARD\r\n");
                entries.Add(contact.CombinedName, sb.ToString(), contact.CombinedName);
            }

            var document = new StringBuilder();
            entries.AppendTo(document);
            return new ExportResult(document.ToString(), TextEncodings.Utf8NoBom, entries.Count);
        }

        private static string Limit(string value) => Escaping.VCardGigaset(NameFormatter.LimitLength(value, MaxNameLength));
    }
}
