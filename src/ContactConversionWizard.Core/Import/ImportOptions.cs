using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace ContactConversionWizard.Core.Import
{
    /// <summary>Settings that influence how contacts are read.</summary>
    public sealed class ImportOptions
    {
        public NameStyle NameStyle { get; set; } = NameStyle.LastFirstBracketCompany;

        public TypePreference TypePreference { get; set; } = TypePreference.Home;

        public VipMode VipMode { get; set; } = VipMode.Never;

        /// <summary>Remove quotes around TEL types (<c>TYPE="cell"</c>) as written by Nextcloud.</summary>
        public bool NextcloudVCardFix { get; set; }

        /// <summary>Also import contacts that have no phone or fax number.</summary>
        public bool ImportWithoutNumber { get; set; }

        /// <summary>Also import contacts that have no name (they are named after their e-mail address or first number).</summary>
        public bool ImportWithoutName { get; set; }

        /// <summary>
        /// Decides whether a read contact is imported and, for contacts without name, generates a name.
        /// Contacts whose notes contain <c>CCW-IGNORE</c> are never imported.
        /// </summary>
        public bool Accept(Contact contact, string notes = "")
        {
            if (NoteFlags.IsIgnored(notes)) { return false; }
            if (!contact.HasAnyNumber && !ImportWithoutNumber) { return false; }

            if (contact.CombinedName == "")
            {
                if (!ImportWithoutName) { return false; }
                contact.CombinedName = FallbackName(contact);
            }
            return contact.CombinedName != "";
        }

        private static string FallbackName(Contact contact)
        {
            foreach (string candidate in new[] { contact.Email, contact.Mobile, contact.Home, contact.Work, contact.HomeFax, contact.WorkFax })
            {
                if (candidate != "") { return candidate; }
            }
            return "(no name)";
        }
    }
}
