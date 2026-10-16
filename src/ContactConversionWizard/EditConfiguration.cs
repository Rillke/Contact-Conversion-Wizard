using System.Windows.Forms;

namespace Contact_Conversion_Wizard
{
    public partial class EditConfiguration : Form
    {
        private readonly AppSettings settings;

        public EditConfiguration(AppSettings settings)
        {
            this.settings = settings;
            InitializeComponent();

            OPT_hidecol.Checked = settings.HideEmptyColumns;
            OPT_adjustcol.Checked = settings.AdjustableColumns;
            OPT_prefixNONFB.Checked = settings.ApplyPrefixToAllFormats;
            OPT_importOther.Checked = settings.ImportOtherNumbers;
            OPT_DUPren.Checked = settings.RenameDuplicates;

            checkBox_OutlookEmptyNumber.Checked = settings.OutlookImportEmptyNumber;
            checkBox_OutlookEmptyName.Checked = settings.OutlookImportEmptyName;

            checkBox_cleanBrackets.Checked = !settings.Cleanup.RemoveBrackets;
            checkBox_cleanHashKey.Checked = !settings.Cleanup.RemoveHashKey;
            checkBox_cleanSlash.Checked = !settings.Cleanup.RemoveSlash;
            checkBox_cleanHyphen.Checked = !settings.Cleanup.RemoveHyphen;
            checkBox_cleanXchar.Checked = !settings.Cleanup.RemoveXChar;
            checkBox_cleanSpace.Checked = !settings.Cleanup.RemoveSpace;
            checkBox_cleanSquareBrackets.Checked = !settings.Cleanup.RemoveSquareBrackets;
            checkBox_cleanLetters.Checked = !settings.Cleanup.RemoveLetters;
            checkBox_cleanaddzeroprefix.Checked = settings.Cleanup.AddZeroPrefix;

            checkBox_checkVersion.Checked = settings.CheckForUpdates;
        }

        private void EditConfiguration_FormClosing(object sender, FormClosingEventArgs e)
        {
            settings.HideEmptyColumns = OPT_hidecol.Checked;
            settings.AdjustableColumns = OPT_adjustcol.Checked;
            settings.ApplyPrefixToAllFormats = OPT_prefixNONFB.Checked;
            settings.ImportOtherNumbers = OPT_importOther.Checked;
            settings.RenameDuplicates = OPT_DUPren.Checked;

            settings.OutlookImportEmptyNumber = checkBox_OutlookEmptyNumber.Checked;
            settings.OutlookImportEmptyName = checkBox_OutlookEmptyName.Checked;

            settings.NextcloudVCardFix = checkBox_nextcloud_vfc_fix.Checked;

            settings.Cleanup.RemoveBrackets = !checkBox_cleanBrackets.Checked;
            settings.Cleanup.RemoveHashKey = !checkBox_cleanHashKey.Checked;
            settings.Cleanup.RemoveSlash = !checkBox_cleanSlash.Checked;
            settings.Cleanup.RemoveHyphen = !checkBox_cleanHyphen.Checked;
            settings.Cleanup.RemoveXChar = !checkBox_cleanXchar.Checked;
            settings.Cleanup.RemoveSpace = !checkBox_cleanSpace.Checked;
            settings.Cleanup.RemoveSquareBrackets = !checkBox_cleanSquareBrackets.Checked;
            settings.Cleanup.RemoveLetters = !checkBox_cleanLetters.Checked;
            settings.Cleanup.AddZeroPrefix = checkBox_cleanaddzeroprefix.Checked;

            settings.CheckForUpdates = checkBox_checkVersion.Checked;
        }
    }
}
