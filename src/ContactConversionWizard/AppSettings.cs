using System;
using System.IO;
using ContactConversionWizard.Core.Config;
using ContactConversionWizard.Core.Text;

namespace Contact_Conversion_Wizard
{
    /// <summary>The persisted options of the wizard (stored in <c>%APPDATA%\ContactConversionWizard\CCW.config</c>).</summary>
    public sealed class AppSettings
    {
        public static readonly string SettingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ContactConversionWizard");

        public static string SettingsFile => Path.Combine(SettingsFolder, "CCW.config");

        public bool HideEmptyColumns { get; set; }
        public bool AdjustableColumns { get; set; }
        public bool ApplyPrefixToAllFormats { get; set; }
        public bool ImportOtherNumbers { get; set; } = true;
        public bool RenameDuplicates { get; set; }
        public bool CheckForUpdates { get; set; } = true;

        /// <summary>Import contacts without phone number (all sources; stored under the old Outlook specific key).</summary>
        public bool ImportWithoutNumber { get; set; }

        /// <summary>Import contacts without name (all sources; stored under the old Outlook specific key).</summary>
        public bool ImportWithoutName { get; set; }

        public bool NextcloudVCardFix { get; set; }

        public NumberCleanupOptions Cleanup { get; } = new NumberCleanupOptions();

        /// <summary>Selections of the combo boxes of the main window, stored as their display text.</summary>
        public string NameStyle { get; set; }
        public string TypePreference { get; set; }
        public string OutlookImport { get; set; }
        public string Vip { get; set; }
        public string CountryPrefix { get; set; }
        public string PictureExport { get; set; }
        public string FritzPicturePath { get; set; }

        /// <summary>True if the loaded file still contains the clear text Google password of older versions.</summary>
        public bool ContainsLegacyGooglePassword { get; private set; }

        public static AppSettings Load(string path)
        {
            ConfigFile config = ConfigFile.Load(path);
            var settings = new AppSettings();

            settings.HideEmptyColumns = config.GetBool("cfg_hideemptycols", settings.HideEmptyColumns);
            settings.AdjustableColumns = config.GetBool("cfg_adjustablecols", settings.AdjustableColumns);
            settings.ApplyPrefixToAllFormats = config.GetBool("cfg_prefixNONFB", settings.ApplyPrefixToAllFormats);
            settings.ImportOtherNumbers = config.GetBool("cfg_importOther", settings.ImportOtherNumbers);
            settings.RenameDuplicates = config.GetBool("cfg_DUPren", settings.RenameDuplicates);
            settings.ImportWithoutNumber = config.GetBool("outlook_empty_number", settings.ImportWithoutNumber);
            settings.ImportWithoutName = config.GetBool("outlook_empty_name", settings.ImportWithoutName);
            settings.NextcloudVCardFix = config.GetBool("nextcloud_vcf_fix", settings.NextcloudVCardFix);
            settings.CheckForUpdates = config.GetBool("cfg_checkVersion", settings.CheckForUpdates);

            NumberCleanupOptions c = settings.Cleanup;
            c.RemoveBrackets = config.GetBool("clean_brackets", c.RemoveBrackets);
            c.RemoveHashKey = config.GetBool("clean_hashkey", c.RemoveHashKey);
            c.RemoveSlash = config.GetBool("clean_slash", c.RemoveSlash);
            c.RemoveHyphen = config.GetBool("clean_hyphen", c.RemoveHyphen);
            c.RemoveXChar = config.GetBool("clean_xchar", c.RemoveXChar);
            c.RemoveSpace = config.GetBool("clean_space", c.RemoveSpace);
            c.RemoveSquareBrackets = config.GetBool("clean_squarebrackets", c.RemoveSquareBrackets);
            c.RemoveLetters = config.GetBool("clean_letters", c.RemoveLetters);
            c.AddZeroPrefix = config.GetBool("clean_addzeroprefix", c.AddZeroPrefix);

            settings.NameStyle = config.GetString("combo_namestyle");
            settings.TypePreference = config.GetString("combo_typeprefer");
            settings.OutlookImport = config.GetString("combo_outlookimport");
            settings.Vip = config.GetString("combo_VIP");
            settings.CountryPrefix = config.GetString("combo_prefix");
            settings.PictureExport = config.GetString("combo_picexport");
            settings.FritzPicturePath = config.GetString("fritzPicPath");
            settings.ContainsLegacyGooglePassword = !string.IsNullOrEmpty(config.GetString("g_pass"));

            return settings;
        }

        public void Save(string path)
        {
            var config = new ConfigFile();
            config.Set("cfg_hideemptycols", HideEmptyColumns);
            config.Set("cfg_adjustablecols", AdjustableColumns);
            config.Set("cfg_prefixNONFB", ApplyPrefixToAllFormats);
            config.Set("cfg_importOther", ImportOtherNumbers);
            config.Set("cfg_DUPren", RenameDuplicates);
            config.Set("outlook_empty_number", ImportWithoutNumber);
            config.Set("outlook_empty_name", ImportWithoutName);
            config.Set("nextcloud_vcf_fix", NextcloudVCardFix);
            config.Set("cfg_checkVersion", CheckForUpdates);

            config.Set("clean_brackets", Cleanup.RemoveBrackets);
            config.Set("clean_hashkey", Cleanup.RemoveHashKey);
            config.Set("clean_slash", Cleanup.RemoveSlash);
            config.Set("clean_hyphen", Cleanup.RemoveHyphen);
            config.Set("clean_xchar", Cleanup.RemoveXChar);
            config.Set("clean_space", Cleanup.RemoveSpace);
            config.Set("clean_squarebrackets", Cleanup.RemoveSquareBrackets);
            config.Set("clean_letters", Cleanup.RemoveLetters);
            config.Set("clean_addzeroprefix", Cleanup.AddZeroPrefix);

            config.Set("combo_namestyle", NameStyle);
            config.Set("combo_typeprefer", TypePreference);
            config.Set("combo_outlookimport", OutlookImport);
            config.Set("combo_VIP", Vip);
            config.Set("combo_prefix", CountryPrefix);
            config.Set("combo_picexport", PictureExport);
            config.Set("fritzPicPath", FritzPicturePath);
            // "g_login" and "g_pass" (the Google account of the removed Google Contacts support) are intentionally
            // not written again, so that the password that older versions stored in clear text gets deleted.

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            config.Save(path);
        }
    }
}
