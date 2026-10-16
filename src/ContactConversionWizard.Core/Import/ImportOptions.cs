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
    }
}
