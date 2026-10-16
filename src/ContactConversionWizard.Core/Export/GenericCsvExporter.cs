using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace ContactConversionWizard.Core.Export
{
    /// <summary>
    /// A CSV file with all fields and a header row, independent of any device, e.g. for spreadsheets or as input for
    /// other tools. UTF-8 with BOM (so that Excel recognises the encoding), comma separated, all fields quoted.
    /// The column names match the field names of the generic CSV import, so the file can be read in again.
    /// </summary>
    public sealed class GenericCsvExporter : ContactExporter
    {
        public static readonly IReadOnlyList<string> Columns = new[]
        {
            "Combined Name", "Lastname", "Firstname", "Company", "Home", "Work", "Mobile", "Preferred", "HomeFax", "WorkFax",
            "Street", "ZIP Code", "City", "eMail", "Birthday", "VIP", "Speed Dial", "Notes",
        };

        public override string FormatName => "Generic CSV";
        public override string DefaultFileName => "contacts";
        public override string FileExtension => "csv";

        protected override ExportResult Export(IReadOnlyList<Contact> contacts, ExportContext context)
        {
            var entries = new EntryCollector(context);

            foreach (Contact contact in contacts)
            {
                CleanNumbers numbers = context.Clean(contact);
                if (context.Options.OnlyWithPhoneNumber && !numbers.HasVoiceNumber) { continue; }

                string notes = string.Join(" ", new[]
                {
                    contact.IsVip ? "VIP" : "",
                    contact.SpeedDial != "" ? "SPEEDDIAL(" + contact.SpeedDial + ")" : "",
                    contact.FritzPrefix != "" ? "PREFIX(" + contact.FritzPrefix + ")" : "",
                    contact.ExplicitPreferred != PhoneType.None ? "PREFER(" + contact.ExplicitPreferred.ToString().ToLowerInvariant() + ")" : "",
                }.Where(n => n != ""));

                string[] fields =
                {
                    contact.CombinedName, contact.LastName, contact.FirstName, contact.Company, numbers.Home, numbers.Work, numbers.Mobile,
                    contact.Preferred == PhoneType.None ? "" : contact.Preferred.ToString().ToLowerInvariant(),
                    numbers.HomeFax, numbers.WorkFax, contact.Street, contact.Zip, contact.City, contact.Email, contact.Birthday,
                    contact.IsVip ? "Yes" : "No", contact.SpeedDial, notes,
                };
                entries.Add(contact.CombinedName, Line(fields), contact.CombinedName);
            }

            var document = new StringBuilder(Line(Columns));
            entries.AppendTo(document);
            return new ExportResult(document.ToString(), TextEncodings.Utf8WithBom, entries.Count);
        }

        private static string Line(IEnumerable<string> fields) => string.Join(",", fields.Select(Escaping.CsvQuoted)) + "\r\n";
    }
}
