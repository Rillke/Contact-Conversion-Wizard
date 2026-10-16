using System.Collections.Generic;
using System.Text;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace ContactConversionWizard.Core.Export
{
    /// <summary>Tab separated address book for AVM FRITZ!Adr / FRITZ!Fax (ISO-8859-1).</summary>
    public sealed class FritzAdrExporter : ContactExporter
    {
        public const int MaxNameLength = 31;
        public const string Header = "BEZCHNG\tFIRMA\tNAME\tVORNAME\tABTEILUNG\tSTRASSE\tPLZ\tORT\tKOMMENT\tTELEFON\tTELEFAX\tTRANSFER\tTERMINAL\tBENUTZER\tPASSWORT\tTRANSPROT\tTERMMODE\tNOTIZEN\tMOBILFON\tEMAIL\tHOMEPAGE\r\n";

        public override string FormatName => "Fritz!Adr";
        public override string DefaultFileName => "FritzFax Export";
        public override string FileExtension => "txt";

        protected override ExportResult Export(IReadOnlyList<Contact> contacts, ExportContext context)
        {
            var entries = new EntryCollector(context);

            foreach (Contact contact in contacts)
            {
                CleanNumbers numbers = context.Clean(contact);
                if (context.Options.OnlyWithFaxNumber && numbers.HomeFax == "" && numbers.WorkFax == "") { continue; }

                var lines = new List<Line>();
                if ((numbers.HomeFax != "" && numbers.WorkFax != "") || (numbers.Home != "" && numbers.Work != ""))
                {
                    // separate home and work entries
                    lines.Add(new Line(NameFormatter.LimitLength(contact.CombinedName, MaxNameLength, true, " H"), numbers.HomeFax, numbers.Home));
                    lines.Add(new Line(NameFormatter.LimitLength(contact.CombinedName, MaxNameLength, true, " W"), numbers.WorkFax, numbers.Work));
                }
                else
                {
                    string fax = numbers.WorkFax != "" ? numbers.WorkFax : numbers.HomeFax;
                    string phone = numbers.Work != "" ? numbers.Work : numbers.Home;
                    lines.Add(new Line(NameFormatter.LimitLength(contact.CombinedName, MaxNameLength), fax, phone));
                }

                // VIP and speed dial information is stored in the first entry only
                lines[0].Notes = (contact.IsVip ? "VIP " : "") + (contact.SpeedDial != "" ? "SPEEDDIAL(" + contact.SpeedDial + ")" : "");

                foreach (Line line in lines)
                {
                    string text = line.Name + "\t" + contact.Company + "\t" + contact.LastName + "\t" + contact.FirstName + "\t" + "\t"
                        + contact.Street + "\t" + contact.Zip + "\t" + contact.City + "\t" + "\t" + line.Phone + "\t" + line.Fax
                        + "\t\t\t\t\tA\t\t" + line.Notes + "\t" + numbers.Mobile + "\t" + contact.Email + "\t" + "\r\n";
                    entries.Add(line.Name, text, contact.CombinedName);
                }
            }

            var document = new StringBuilder(Header);
            entries.AppendTo(document);
            return new ExportResult(document.ToString(), TextEncodings.Latin1, entries.Count);
        }

        private sealed class Line
        {
            public Line(string name, string fax, string phone)
            {
                Name = name;
                Fax = fax;
                Phone = phone;
            }

            public string Name { get; }
            public string Fax { get; }
            public string Phone { get; }
            public string Notes { get; set; } = "";
        }
    }
}
