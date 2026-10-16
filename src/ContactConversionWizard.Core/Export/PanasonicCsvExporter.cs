using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace ContactConversionWizard.Core.Export
{
    /// <summary>Phone book for Panasonic DECT phones (KX-TCD810/820 series, ISO-8859-1).</summary>
    public sealed class PanasonicCsvExporter : ContactExporter
    {
        public const int MaxNameLength = 16;

        public override string FormatName => "Panasonic";
        public override string DefaultFileName => "PanasonicExport";
        public override string FileExtension => "phb";

        protected override ExportResult Export(IReadOnlyList<Contact> contacts, ExportContext context)
        {
            var entries = new EntryCollector(context);

            foreach (Contact contact in contacts)
            {
                CleanNumbers numbers = context.Clean(contact);
                var list = new[] { numbers.Home, numbers.Work, numbers.Mobile }.Where(n => n != "").ToList();
                if (list.Count == 0) { continue; }

                string name = NameFormatter.LimitLength(contact.CombinedName, MaxNameLength);
                const string contactGroup = "1";
                entries.Add(name, "\"" + name + "\"," + contactGroup + "," + string.Join(",", list) + "\n", contact.CombinedName);
            }

            var document = new StringBuilder("; Panasonic Communications Co., Ltd. Phonebook DECT Version 2.00\r\n");
            entries.AppendTo(document);
            return new ExportResult(document.ToString(), TextEncodings.Latin1, entries.Count);
        }
    }
}
