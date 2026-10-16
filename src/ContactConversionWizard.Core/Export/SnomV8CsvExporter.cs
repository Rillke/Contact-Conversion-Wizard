using System.Collections.Generic;
using System.Text;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace ContactConversionWizard.Core.Export
{
    /// <summary>Snom phones with v8 firmware: CSV with master contacts and member numbers (UTF-8 without BOM).</summary>
    public sealed class SnomV8CsvExporter : ContactExporter
    {
        public const int MaxNameLength = 31;

        public override string FormatName => "Snom v8 CSV";
        public override string DefaultFileName => "tbook_v8";
        public override string FileExtension => "csv";

        protected override ExportResult Export(IReadOnlyList<Contact> contacts, ExportContext context)
        {
            var entries = new EntryCollector(context);
            int masterId = 100000;

            foreach (Contact contact in contacts)
            {
                CleanNumbers numbers = context.Clean(contact);
                if (!numbers.HasVoiceNumber) { continue; }

                string name = NameFormatter.LimitLength(contact.CombinedName, MaxNameLength);
                List<KeyValuePair<string, string>> phones = OrderNumbers(contact, numbers);
                string vip = contact.IsVip ? "vip" : "";
                var sb = new StringBuilder();

                if (phones.Count > 1)
                {
                    masterId++;
                    string master = masterId.ToString("D8");
                    sb.Append(Line(name, master, "MASTER", contact.FirstName, contact.LastName, contact.Email, phones[0].Key));
                    foreach (KeyValuePair<string, string> phone in phones)
                    {
                        sb.Append(Line("", phone.Value, vip, "Member_Alias", master, "", phone.Key));
                    }
                }
                else
                {
                    sb.Append(Line(name, phones[0].Value, vip, contact.FirstName, contact.LastName, contact.Email, phones[0].Key));
                }

                entries.Add(name, sb.ToString(), contact.CombinedName);
            }

            var document = new StringBuilder();
            entries.AppendTo(document);
            return new ExportResult(document.ToString(), TextEncodings.Utf8NoBom, entries.Count);
        }

        /// <summary>The non-empty numbers as (type, number), the preferred one first, then home, work, mobile.</summary>
        private static List<KeyValuePair<string, string>> OrderNumbers(Contact contact, CleanNumbers numbers)
        {
            var all = new List<KeyValuePair<PhoneType, string>>
            {
                new KeyValuePair<PhoneType, string>(PhoneType.Home, numbers.Home),
                new KeyValuePair<PhoneType, string>(PhoneType.Work, numbers.Work),
                new KeyValuePair<PhoneType, string>(PhoneType.Mobile, numbers.Mobile),
            };

            var result = new List<KeyValuePair<string, string>>();
            PhoneType preferred = contact.Preferred;
            foreach (KeyValuePair<PhoneType, string> entry in all)
            {
                if (entry.Key == preferred && entry.Value != "") { result.Add(Pair(entry)); }
            }
            foreach (KeyValuePair<PhoneType, string> entry in all)
            {
                if (entry.Key != preferred && entry.Value != "") { result.Add(Pair(entry)); }
            }
            return result;
        }

        private static KeyValuePair<string, string> Pair(KeyValuePair<PhoneType, string> entry)
            => new KeyValuePair<string, string>(entry.Key.ToString().ToLowerInvariant(), entry.Value);

        private static string Line(string name, string number, string relation, string firstName, string lastName, string email, string type)
        {
            const string sep = "\",\"";
            return "\"" + name + sep + number + sep + relation + sep + "" + sep + firstName + sep + lastName + sep + "" + sep + ""
                + sep + email + sep + "" + sep + "" + sep + "" + sep + "false" + sep + "" + sep + type + sep + "" + "\"\r\n";
        }
    }
}
