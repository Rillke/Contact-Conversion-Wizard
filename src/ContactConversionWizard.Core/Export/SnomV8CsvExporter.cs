using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace ContactConversionWizard.Core.Export
{
    /// <summary>
    /// Snom phones with v8 or newer firmware: CSV with master contacts and member numbers (UTF-8 without BOM).
    /// </summary>
    /// <remarks>
    /// See https://service.snom.com/display/wiki/Local+Directory#LocalDirectory-Import: a contact with several numbers is
    /// written as a <c>MASTER</c> line of type <c>sip</c> whose number is the first (preferred) number of the contact,
    /// followed by one <c>Member_Alias</c> line per number that refers to the master by that number.
    /// </remarks>
    public sealed class SnomV8CsvExporter : ContactExporter
    {
        public const int MaxNameLength = 31;

        public override string FormatName => "Snom v8 CSV";
        public override string DefaultFileName => "tbook_v8";
        public override string FileExtension => "csv";

        protected override ExportResult Export(IReadOnlyList<Contact> contacts, ExportContext context)
        {
            var entries = new EntryCollector(context);
            var masterKeys = new HashSet<string>();
            int fallbackMasterId = 100000;

            foreach (Contact contact in contacts)
            {
                CleanNumbers numbers = context.Clean(contact);
                if (!numbers.HasVoiceNumber) { continue; }

                string name = NameFormatter.LimitLength(contact.CombinedName, MaxNameLength);
                List<KeyValuePair<string, string>> phones = OrderNumbers(contact, numbers);
                var sb = new StringBuilder();

                if (phones.Count > 1)
                {
                    // the master is referenced by its number, which must therefore be unique
                    string master = phones.Select(p => p.Value).FirstOrDefault(n => !masterKeys.Contains(n));
                    if (master == null)
                    {
                        do { fallbackMasterId++; } while (masterKeys.Contains(fallbackMasterId.ToString("D8")));
                        master = fallbackMasterId.ToString("D8");
                        context.Warnings.Add(contact.CombinedName + ": all numbers are already used by other contacts, the contact gets the internal key " + master + ".");
                    }
                    masterKeys.Add(master);

                    sb.Append(Line(name, master, "MASTER", contact.FirstName, contact.LastName, contact.Email, "sip"));
                    string relation = contact.IsVip ? "vip" : "none";
                    foreach (KeyValuePair<string, string> phone in phones)
                    {
                        sb.Append(Line("", phone.Value, relation, "Member_Alias", master, "", phone.Key));
                    }
                }
                else
                {
                    sb.Append(Line(name, phones[0].Value, contact.IsVip ? "vip" : "", contact.FirstName, contact.LastName, contact.Email, phones[0].Key));
                }

                entries.Add(name, sb.ToString(), contact.CombinedName);
            }

            var document = new StringBuilder();
            entries.AppendTo(document);
            return new ExportResult(document.ToString(), TextEncodings.Utf8NoBom, entries.Count);
        }

        /// <summary>The non-empty numbers as (Snom type, number), the preferred one first, then home, work, mobile.</summary>
        private static List<KeyValuePair<string, string>> OrderNumbers(Contact contact, CleanNumbers numbers)
        {
            var all = new List<KeyValuePair<PhoneType, string>>
            {
                new KeyValuePair<PhoneType, string>(PhoneType.Home, numbers.Home),
                new KeyValuePair<PhoneType, string>(PhoneType.Work, numbers.Work),
                new KeyValuePair<PhoneType, string>(PhoneType.Mobile, numbers.Mobile),
            };

            PhoneType preferred = contact.Preferred;
            return all.Where(e => e.Key == preferred && e.Value != "")
                .Concat(all.Where(e => e.Key != preferred && e.Value != ""))
                .Select(e => new KeyValuePair<string, string>(SnomType(e.Key), e.Value))
                .ToList();
        }

        private static string SnomType(PhoneType type)
        {
            switch (type)
            {
                case PhoneType.Work: return "business";
                case PhoneType.Mobile: return "mobile";
                default: return "home";
            }
        }

        private static string Line(string name, string number, string relation, string firstName, string lastName, string email, string type)
        {
            string[] fields = { name, number, relation, "", firstName, lastName, "", "", email, "", "", "", "false", "", type, "" };
            return string.Join(",", fields.Select(Escaping.CsvQuoted)) + "\r\n";
        }
    }
}
