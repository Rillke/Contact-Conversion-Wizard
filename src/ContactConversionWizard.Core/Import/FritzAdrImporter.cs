using System;
using System.IO;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace ContactConversionWizard.Core.Import
{
    /// <summary>Reads the tab separated export of AVM's FRITZ!Adr address book (ISO-8859-1, 21 columns).</summary>
    public sealed class FritzAdrImporter
    {
        public const int ColumnCount = 21;

        // column indices of the FRITZ!Adr format
        private const int Designation = 0, CompanyColumn = 1, NameColumn = 2, FirstNameColumn = 3, StreetColumn = 5,
            ZipColumn = 6, CityColumn = 7, CommentColumn = 8, PhoneColumn = 9, FaxColumn = 10, MobileColumn = 18, EmailColumn = 19;

        private readonly ImportOptions options;

        public FritzAdrImporter(ImportOptions options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public ImportResult Import(TextReader reader)
        {
            if (reader == null) { throw new ArgumentNullException(nameof(reader)); }

            var result = new ImportResult();
            string line;
            int lineNumber = -1;

            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                string[] columns = line.Split('\t');
                if (columns.Length != ColumnCount)
                {
                    result.Warnings.Add("Unable to parse line " + lineNumber + ", found only " + columns.Length + " TAB separated segments instead of 21:" + Environment.NewLine + line);
                    continue;
                }
                if (columns[Designation] == "BEZCHNG") { continue; } // header line

                Contact contact = ParseColumns(columns);
                if (options.Accept(contact, columns[CommentColumn]))
                {
                    result.Contacts.Add(contact);
                }
            }

            return result;
        }

        private Contact ParseColumns(string[] columns)
        {
            var contact = new Contact
            {
                CombinedName = columns[Designation],
                Company = columns[CompanyColumn],
                LastName = columns[NameColumn],
                FirstName = columns[FirstNameColumn],
                Email = columns[EmailColumn],
                Street = columns[StreetColumn],
                Zip = columns[ZipColumn],
                City = columns[CityColumn],
                Mobile = columns[MobileColumn],
            };

            NoteFlags.Apply(contact, "", columns[CommentColumn], options.VipMode);

            // the designation suffix written by the FRITZ!Adr export overrides the type preference
            TypePreference storeAs = options.TypePreference;
            if (contact.CombinedName.EndsWith(" (privat)", StringComparison.Ordinal)) { storeAs = TypePreference.Home; }
            if (contact.CombinedName.EndsWith(" (gesch.)", StringComparison.Ordinal)) { storeAs = TypePreference.Work; }

            if (storeAs == TypePreference.Home)
            {
                contact.Home = columns[PhoneColumn];
                contact.HomeFax = columns[FaxColumn];
            }
            else
            {
                contact.Work = columns[PhoneColumn];
                contact.WorkFax = columns[FaxColumn];
            }

            return contact;
        }
    }
}
