using System;
using System.Collections.Generic;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace ContactConversionWizard.Core.Import
{
    /// <summary>The contact fields a column of a generic CSV file can be assigned to.</summary>
    public enum CsvField
    {
        Ignore = -1,
        LastName = 0,
        FirstName = 1,
        Company = 2,
        Home = 3,
        Work = 4,
        Mobile = 5,
        HomeFax = 6,
        WorkFax = 7,
        Street = 8,
        Zip = 9,
        City = 10,
        Email = 11,
        Notes = 12,
        Birthday = 13,
    }

    /// <summary>Turns the rows of a generic CSV file into contacts using a user defined column assignment.</summary>
    public sealed class CsvContactMapper
    {
        /// <summary>Display names of the assignable fields, index = <see cref="CsvField"/> + 1.</summary>
        public static readonly IReadOnlyList<string> FieldNames = new[]
        {
            "(ignore)", "Lastname", "Firstname", "Company", "Home", "Work", "Mobile", "HomeFax", "WorkFax", "Street", "ZIP Code", "City", "eMail", "Notes", "Birthday",
        };

        private readonly ImportOptions options;

        public CsvContactMapper(ImportOptions options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public ImportResult Map(IEnumerable<string[]> rows, IReadOnlyList<CsvField> assignment)
        {
            if (rows == null) { throw new ArgumentNullException(nameof(rows)); }
            if (assignment == null) { throw new ArgumentNullException(nameof(assignment)); }

            var result = new ImportResult();
            foreach (string[] row in rows)
            {
                var contact = new Contact();
                string notes = "";
                for (int column = 0; column < assignment.Count && column < row.Length; column++)
                {
                    if (assignment[column] == CsvField.Notes) { notes = row[column]; }
                    Assign(contact, assignment[column], row[column], result);
                }

                contact.CombinedName = NameFormatter.GenerateFullName(contact.FirstName, contact.LastName, contact.Company, options.NameStyle);
                if (options.Accept(contact, notes))
                {
                    result.Contacts.Add(contact);
                }
            }
            return result;
        }

        private void Assign(Contact contact, CsvField field, string value, ImportResult result)
        {
            switch (field)
            {
                case CsvField.Ignore: break;
                case CsvField.LastName: contact.LastName = value; break;
                case CsvField.FirstName: contact.FirstName = value; break;
                case CsvField.Company: contact.Company = value; break;
                case CsvField.Home: contact.Home = value; break;
                case CsvField.Work: contact.Work = value; break;
                case CsvField.Mobile: contact.Mobile = value; break;
                case CsvField.HomeFax: contact.HomeFax = value; break;
                case CsvField.WorkFax: contact.WorkFax = value; break;
                case CsvField.Street: contact.Street = value; break;
                case CsvField.Zip: contact.Zip = value; break;
                case CsvField.City: contact.City = value; break;
                case CsvField.Email: contact.Email = value; break;
                case CsvField.Notes: NoteFlags.Apply(contact, "", value, options.VipMode); break;
                case CsvField.Birthday:
                    string birthday = BirthdayParser.Normalize(value);
                    if (birthday == null) { result.Warnings.Add("Unknown birthday format: " + value); }
                    else { contact.Birthday = birthday; }
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(field), field, "Unsupported CSV field.");
            }
        }
    }
}
