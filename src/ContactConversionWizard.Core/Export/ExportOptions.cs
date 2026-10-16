using System;
using System.IO;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace ContactConversionWizard.Core.Export
{
    /// <summary>Settings that influence how contacts are written.</summary>
    public sealed class ExportOptions
    {
        /// <summary>Own country prefix like <c>0049</c>, or <see cref="PhoneNumberCleaner.KeepCountryPrefix"/>.</summary>
        public string CountryPrefix { get; set; } = PhoneNumberCleaner.KeepCountryPrefix;

        public NumberCleanupOptions Cleanup { get; set; } = new NumberCleanupOptions();

        /// <summary>Apply <c>PREFIX(...)</c> to all formats instead of only to the FRITZ!Box XML export.</summary>
        public bool ApplyPrefixToAllFormats { get; set; }

        public TypePreference TypePreference { get; set; } = TypePreference.Home;

        public NameStyle NameStyle { get; set; } = NameStyle.LastFirstBracketCompany;

        /// <summary>Export contact pictures (where the format supports them).</summary>
        public bool ExportPictures { get; set; }

        /// <summary>Path on the FRITZ!Box under which exported pictures will be stored (prefix of <c>imageURL</c>).</summary>
        public string PicturePath { get; set; } = "";

        /// <summary>FRITZ!Box XML: also export fax numbers.</summary>
        public bool IncludeFax { get; set; }

        /// <summary>vCard: only export contacts with a home, work or mobile number.</summary>
        public bool OnlyWithPhoneNumber { get; set; }

        /// <summary>
        /// Write all numbers of a contact, also several of the same type (e.g. two mobile numbers), in formats that
        /// support this (vCard, FRITZ!Box XML). Off by default; other formats are not affected.
        /// </summary>
        public bool MultipleNumbersPerType { get; set; }

        /// <summary>FRITZ!Adr: only export contacts with a fax number.</summary>
        public bool OnlyWithFaxNumber { get; set; }

        /// <summary>Generates file names of exported pictures. Defaults to random names.</summary>
        public Func<string> PictureFileNameGenerator { get; set; } = () => Path.ChangeExtension(Path.GetRandomFileName(), "jpg");
    }
}
