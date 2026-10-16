using System.Collections.Generic;

namespace ContactConversionWizard.Core.Model
{
    /// <summary>
    /// A contact as it is held by the wizard between import and export.
    /// All string properties are never <c>null</c>; assigning <c>null</c> stores an empty string.
    /// </summary>
    public sealed class Contact
    {
        private string firstName = "";
        private string lastName = "";
        private string company = "";
        private string home = "";
        private string work = "";
        private string mobile = "";
        private string homeFax = "";
        private string workFax = "";
        private string combinedName = "";
        private string fritzPrefix = "";
        private string speedDial = "";
        private string street = "";
        private string zip = "";
        private string city = "";
        private string email = "";
        private string birthday = "";

        public string FirstName { get => firstName; set => firstName = value ?? ""; }
        public string LastName { get => lastName; set => lastName = value ?? ""; }
        public string Company { get => company; set => company = value ?? ""; }
        public string Home { get => home; set => home = value ?? ""; }
        public string Work { get => work; set => work = value ?? ""; }
        public string Mobile { get => mobile; set => mobile = value ?? ""; }
        public string HomeFax { get => homeFax; set => homeFax = value ?? ""; }
        public string WorkFax { get => workFax; set => workFax = value ?? ""; }

        /// <summary>The single display name used by most phone book formats and as unique key.</summary>
        public string CombinedName { get => combinedName; set => combinedName = value ?? ""; }

        /// <summary>Dial prefix (e.g. <c>#31#</c>) taken from the <c>PREFIX(...)</c> keyword in the notes.</summary>
        public string FritzPrefix { get => fritzPrefix; set => fritzPrefix = value ?? ""; }

        /// <summary>Speed dial in the form <c>NN</c>, <c>NN,VANITY</c> or <c>XX,VANITY</c>.</summary>
        public string SpeedDial { get => speedDial; set => speedDial = value ?? ""; }

        public string Street { get => street; set => street = value ?? ""; }
        public string Zip { get => zip; set => zip = value ?? ""; }
        public string City { get => city; set => city = value ?? ""; }
        public string Email { get => email; set => email = value ?? ""; }

        /// <summary>Birthday as <c>YYYY-MM-DD</c>, or <c>--MM-DD</c> if the year is unknown, or empty.</summary>
        public string Birthday { get => birthday; set => birthday = value ?? ""; }

        public bool IsVip { get; set; }

        /// <summary>JPEG data of the contact picture or <c>null</c>.</summary>
        public byte[] Photo { get; set; }

        /// <summary>The preferred number type as explicitly set by the source, or <see cref="PhoneType.None"/>.</summary>
        public PhoneType ExplicitPreferred { get; set; }

        /// <summary>
        /// The preferred number type: the explicitly set one, otherwise the first of home, work, mobile that is set.
        /// </summary>
        public PhoneType Preferred
        {
            get
            {
                if (ExplicitPreferred != PhoneType.None) { return ExplicitPreferred; }
                if (Home != "") { return PhoneType.Home; }
                if (Work != "") { return PhoneType.Work; }
                if (Mobile != "") { return PhoneType.Mobile; }
                return PhoneType.None;
            }
        }

        /// <summary>True if at least one phone or fax number is set.</summary>
        public bool HasAnyNumber => Home != "" || Work != "" || Mobile != "" || HomeFax != "" || WorkFax != "";

        /// <summary>
        /// All numbers of the contact with their real type, in the order of the source, as far as the source provides
        /// several numbers per type (FRITZ!Box XML, vCard). Empty for other sources.
        /// </summary>
        /// <remarks>
        /// The single number fields (<see cref="Home"/>, <see cref="Work"/>, ...) are filled as before: additional numbers
        /// are put into free fields of another type. Formats that can hold several numbers of one type use this list
        /// instead when <c>ExportOptions.MultipleNumbersPerType</c> is set.
        /// </remarks>
        public List<TypedNumber> Numbers { get; private set; } = new List<TypedNumber>();

        /// <summary>Adds a number to <see cref="Numbers"/> unless the same number of the same kind is already there.</summary>
        public void AddNumber(NumberKind kind, string number, bool preferred = false, string sourceType = "")
        {
            if (string.IsNullOrEmpty(number)) { return; }
            if (Numbers.Exists(n => n.Kind == kind && n.Number == number)) { return; }
            Numbers.Add(new TypedNumber(kind, number, preferred, sourceType));
        }

        public Contact Clone()
        {
            var copy = (Contact)MemberwiseClone();
            copy.Photo = Photo == null ? null : (byte[])Photo.Clone();
            copy.Numbers = new List<TypedNumber>(Numbers);
            return copy;
        }

        public override string ToString() => CombinedName;
    }
}
