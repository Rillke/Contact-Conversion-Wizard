using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using ContactConversionWizard.Core.Updates;
using System.Text;
using System.Windows.Forms;
using ContactConversionWizard.Core.Export;
using ContactConversionWizard.Core.Import;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace Contact_Conversion_Wizard
{
    public partial class Form1 : Form
    {
        private const string FritzNasPicturePath = @"\\fritz.nas\FRITZ.NAS\FRITZ\fonpix-custom\";

        private readonly AppSettings settings;
        private readonly ContactBook contacts = new ContactBook();

        public Form1()
        {
            InitializeComponent();
            Text = ProductName + " v" + Application.ProductVersion;
            if (Application.ProductVersion.Contains("-"))
            {
                // pre-release versions such as 3.6.0.0-alpha.1
                Text += " - EXPERIMENTAL TEST VERSION, keep a backup of your contacts";
            }

            Directory.CreateDirectory(AppSettings.SettingsFolder);

            combo_prefix.SelectedIndex = DefaultCountryIndex(System.Globalization.RegionInfo.CurrentRegion.TwoLetterISORegionName);
            combo_namestyle.SelectedIndex = 0;
            combo_typeprefer.SelectedIndex = 0;
            combo_outlookimport.SelectedIndex = 0;
            combo_VIP.SelectedIndex = 0;
            combo_picexport.SelectedIndex = 0;

            // override the defaults with the saved configuration (if any)
            settings = AppSettings.Load(AppSettings.SettingsFile);
            ApplySettingsToControls();
            RemoveLegacyGooglePassword();

            if (settings.CheckForUpdates)
            {
                backgroundWorker_updateCheck.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Versions up to 3.5.0.2 could store the Google password in clear text. The Google Contacts support has been
        /// removed (Google switched off the used API), so the password is deleted from the configuration file.
        /// </summary>
        private void RemoveLegacyGooglePassword()
        {
            if (!settings.ContainsLegacyGooglePassword) { return; }

            StoreControlsInSettings();
            try
            {
                settings.Save(AppSettings.SettingsFile);
                MessageBox.Show(
                    "A previous version of this program stored your Google account password in clear text in" + Environment.NewLine
                    + AppSettings.SettingsFile + Environment.NewLine + Environment.NewLine
                    + "The password has now been removed from this file. If you use this password anywhere else, please consider changing it.",
                    "Stored Google password removed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show("Your Google password is stored in clear text in " + AppSettings.SettingsFile + " but the file could not be updated: " + ex.Message
                    + Environment.NewLine + "Please delete the line starting with \"g_pass\" manually.");
            }
        }

        private static int DefaultCountryIndex(string region)
        {
            switch (region)
            {
                case "DE": return 0;
                case "AT": return 1;
                case "CH": return 2;
                case "GB": return 3;
                case "IT": return 4;
                case "NL": return 5;
                case "BE": return 6;
                default: return 7;
            }
        }

        #region Settings

        private void ApplySettingsToControls()
        {
            SelectItem(combo_namestyle, settings.NameStyle);
            SelectItem(combo_typeprefer, settings.TypePreference);
            SelectItem(combo_outlookimport, settings.OutlookImport);
            SelectItem(combo_VIP, settings.Vip);
            SelectItem(combo_prefix, settings.CountryPrefix);
            SelectItem(combo_picexport, settings.PictureExport);
            if (settings.FritzPicturePath != null) { textBox_PicPath.Text = settings.FritzPicturePath; }
        }

        private static void SelectItem(ComboBox comboBox, string text)
        {
            if (text != null && comboBox.Items.Contains(text)) { comboBox.SelectedItem = text; }
        }

        private void StoreControlsInSettings()
        {
            settings.NameStyle = combo_namestyle.SelectedItem.ToString();
            settings.TypePreference = combo_typeprefer.SelectedItem.ToString();
            settings.OutlookImport = combo_outlookimport.SelectedItem.ToString();
            settings.Vip = combo_VIP.SelectedItem.ToString();
            settings.CountryPrefix = combo_prefix.SelectedItem.ToString();
            settings.PictureExport = combo_picexport.SelectedItem.ToString();
            settings.FritzPicturePath = textBox_PicPath.Text;
        }

        private NameStyle SelectedNameStyle => (NameStyle)combo_namestyle.SelectedIndex;

        private TypePreference SelectedTypePreference => (TypePreference)combo_typeprefer.SelectedIndex;

        private bool ExportPictures => combo_picexport.SelectedIndex == 1;

        private ImportOptions CreateImportOptions()
        {
            return new ImportOptions
            {
                NameStyle = SelectedNameStyle,
                TypePreference = SelectedTypePreference,
                VipMode = (VipMode)combo_VIP.SelectedIndex,
                NextcloudVCardFix = settings.NextcloudVCardFix,
                ImportWithoutNumber = settings.ImportWithoutNumber,
                ImportWithoutName = settings.ImportWithoutName,
            };
        }

        /// <summary>Creates the export options; returns <c>null</c> if the user cancelled the country prefix input.</summary>
        private ExportOptions CreateExportOptions()
        {
            string countryPrefix = RetrieveCountryPrefix(combo_prefix.SelectedItem.ToString());
            if (string.IsNullOrEmpty(countryPrefix)) { return null; }

            return new ExportOptions
            {
                CountryPrefix = countryPrefix,
                Cleanup = settings.Cleanup,
                ApplyPrefixToAllFormats = settings.ApplyPrefixToAllFormats,
                TypePreference = SelectedTypePreference,
                NameStyle = SelectedNameStyle,
                ExportPictures = ExportPictures,
                PicturePath = textBox_PicPath.Text,
            };
        }

        /// <summary>The country prefix from the selection ("0049 (DE - Germany)"), asking the user for a custom one.</summary>
        private static string RetrieveCountryPrefix(string selection)
        {
            if (selection.StartsWith("00", StringComparison.Ordinal))
            {
                return selection.Substring(0, selection.IndexOf(' '));
            }
            if (selection.StartsWith("Keep", StringComparison.Ordinal))
            {
                return PhoneNumberCleaner.KeepCountryPrefix;
            }
            return AskUser("Please enter your local Country Prefix here (in the format 00x or 00xx)", "CustomCountryID", true, false);
        }

        private static string AskUser(string question, string title, bool onlyNumeric, bool password)
        {
            using (var dialog = new SimpleInputDialog(question, title, onlyNumeric, password))
            {
                dialog.ShowDialog();
                return dialog.resultstring;
            }
        }

        private void button_config_Click(object sender, EventArgs e)
        {
            using (var dialog = new EditConfiguration(settings))
            {
                dialog.ShowDialog();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StoreControlsInSettings();
            try
            {
                settings.Save(AppSettings.SettingsFile);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show("The configuration could not be saved: " + ex.Message);
            }
        }

        private void button_7390_Click(object sender, EventArgs e)
        {
            textBox_PicPath.Text = "file:///var/InternerSpeicher/FRITZ/fonpix-custom/";
        }

        private void button_7270_Click(object sender, EventArgs e)
        {
            textBox_PicPath.Text = "file:///var/media/ftp/<USB-Stick-Name>/FRITZ/fonpix/";
        }

        #endregion

        #region Contact list

        private void disable_buttons(bool busy)
        {
            // while the program is processing, the user must not be able to click any buttons
            Control[] controls =
            {
                btn_read_Outlook, btn_read_FritzXML, btn_read_vCard, btn_read_FritzAdress, btn_read_genericCSV,
                btn_save_Outlook, btn_save_FritzXML, btn_save_vCard, btn_save_vCard_Gigaset, btn_save_FritzAdress, btn_save_SnomCSV7,
                btn_save_SnomCSV8, btn_save_TalkSurfCSV, btn_save_AastraCSV, btn_save_GrandstreamGXV, btn_save_GrandstreamGXP,
                btn_save_Auerswald, btn_save_panasonicCSV, btn_save_genericCSV, button_clear, button_config,
            };
            foreach (Control control in controls) { control.Enabled = !busy; }
            Cursor.Current = busy ? Cursors.WaitCursor : Cursors.Default;
        }

        private static string[] GridValues(Contact contact)
        {
            return new[]
            {
                contact.CombinedName, contact.LastName, contact.FirstName, contact.Company, contact.Home, contact.Work, contact.Mobile,
                PreferredText(contact.Preferred), contact.HomeFax, contact.WorkFax, contact.Street, contact.Zip, contact.City, contact.Email,
                contact.Birthday, contact.IsVip ? "Yes" : "No", contact.SpeedDial, contact.Photo != null ? "Yes" : "No",
            };
        }

        private static string PreferredText(PhoneType type) => type == PhoneType.None ? "" : type.ToString().ToLowerInvariant();

        private void update_datagrid()
        {
            MyDataGridView.SuspendLayout();
            MyDataGridView.Rows.Clear();

            int columnCount = MyDataGridView.Columns.Count;
            var columnHasData = new bool[columnCount];
            foreach (DataGridViewColumn column in MyDataGridView.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                column.Visible = true;
            }

            foreach (Contact contact in contacts)
            {
                string[] values = GridValues(contact);
                for (int i = 0; i < columnCount && i < values.Length; i++)
                {
                    if (values[i] != "" && values[i] != "No") { columnHasData[i] = true; }
                }
                MyDataGridView.Rows.Add(values);
            }

            for (int i = 0; i < columnCount; i++)
            {
                if (settings.AdjustableColumns) { MyDataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; }
                if (settings.HideEmptyColumns && !columnHasData[i]) { MyDataGridView.Columns[i].Visible = false; }
            }

            MyDataGridView.Sort(MyDataGridView.Columns[0], ListSortDirection.Ascending);
            MyDataGridView.ResumeLayout();
            MyDataGridView.Refresh();

            button_clear.Text = "Clear List (" + contacts.Count + ")";
            MyDataGridView.Focus();
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            contacts.Clear();
            update_datagrid();
        }

        /// <summary>Adds imported contacts to the list and reports duplicates and other problems.</summary>
        private void AddImported(ImportResult result)
        {
            if (result.Warnings.Count > 0)
            {
                ShowLongMessage("The following problems occurred while reading the data:", result.Warnings);
            }

            contacts.RenameDuplicates = settings.RenameDuplicates;
            IList<string> duplicates = contacts.AddRange(result.Contacts);
            if (duplicates.Count > 0)
            {
                MessageBox.Show(string.Join(Environment.NewLine, duplicates), "The following duplicate entries could not be imported");
            }

            update_datagrid();
        }

        private static void ShowLongMessage(string intro, IEnumerable<string> lines)
        {
            const int maxLines = 30;
            List<string> list = lines.ToList();
            string text = intro + Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine, list.Take(maxLines));
            if (list.Count > maxLines) { text += Environment.NewLine + "... and " + (list.Count - maxLines) + " more."; }
            MessageBox.Show(text);
        }

        /// <summary>Runs an import step with disabled buttons and reports unexpected errors instead of crashing.</summary>
        private void RunImport(Func<ImportResult> import)
        {
            disable_buttons(true);
            try
            {
                ImportResult result = import();
                if (result != null) { AddImported(result); }
            }
            catch (Exception ex) when (IsReportable(ex))
            {
                MessageBox.Show("Reading the data failed: " + ex.Message);
            }
            finally
            {
                disable_buttons(false);
            }
        }

        private static bool IsReportable(Exception ex)
        {
            return ex is IOException || ex is UnauthorizedAccessException || ex is FormatException
                || ex is System.Runtime.InteropServices.COMException || ex is InvalidCastException || ex is WebException
                || ex is System.Xml.XmlException || ex is ArgumentException || ex is Microsoft.CSharp.RuntimeBinder.RuntimeBinderException;
        }

        private static string AskForFile(string title, string filter)
        {
            using (var dialog = new OpenFileDialog { Title = title, Filter = filter })
            {
                return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
            }
        }

        private static bool ShiftPressed => (ModifierKeys & Keys.Shift) == Keys.Shift;

        private static bool CtrlPressed => (ModifierKeys & Keys.Control) == Keys.Control;

        #endregion

        #region Import

        private void btn_read_Outlook_Click(object sender, EventArgs e)
        {
            bool pickFolder = ShiftPressed;
            bool filterCategories = CtrlPressed;

            OutlookConnector outlook;
            try
            {
                outlook = OutlookConnector.Connect();
            }
            catch (OutlookNotAvailableException ex)
            {
                MessageBox.Show("Unable to access Outlook!" + Environment.NewLine + Environment.NewLine + "This program needs Outlook to continue, are you sure it's installed and working?" + Environment.NewLine + Environment.NewLine + "Error returned was: " + ex.Message);
                return;
            }

            var options = new OutlookImportOptions
            {
                Import = CreateImportOptions(),
                PickFolder = pickFolder,
                UseFileAs = combo_outlookimport.SelectedIndex == 1,
                ImportOtherNumber = settings.ImportOtherNumbers,
            };
            if (filterCategories)
            {
                options.CategoryFilter = AskUser(
                    "Please enter the categories to import, separated by \";\"." + Environment.NewLine
                    + "Prefix categories to exclude with \"!\", e.g.: Family; Friends; !Business",
                    "Category Filter", false, false) ?? "";
            }

            RunImport(() => outlook.Import(options));
        }

        private void btn_read_FritzXML_Click(object sender, EventArgs e)
        {
            string file = AskForFile("Select the Fritz!Box XML file you wish to load", "XML Files|*.xml");
            if (file == null) { return; }

            RunImport(() =>
            {
                using (FileStream stream = File.OpenRead(file))
                {
                    return new FritzXmlImporter(CreateImportOptions()).Import(stream);
                }
            });
        }

        private void btn_read_VC_Click(object sender, EventArgs e)
        {
            // SHIFT: read the file as ISO-8859-1 instead of detecting UTF-8 / Windows-1252
            bool latin1 = ShiftPressed;
            string file = AskForFile("Select the vCard file you wish to load", "vCard Files|*.vcf");
            if (file == null) { return; }

            RunImport(() => new VCardImporter(CreateImportOptions()).Import(File.ReadAllBytes(file), latin1 ? TextEncodings.Latin1 : null));
        }

        private void btn_read_FritzAdr_Click(object sender, EventArgs e)
        {
            string file = AskForFile("Select the Fritz!Adress file you wish to load", "Text (Tabstop separated)|*.txt");
            if (file == null) { return; }

            RunImport(() =>
            {
                using (var reader = new StreamReader(file, TextEncodings.Latin1))
                {
                    return new FritzAdrImporter(CreateImportOptions()).Import(reader);
                }
            });
        }

        private void btn_read_genericCSV_Click(object sender, EventArgs e)
        {
            string file = AskForFile("Select the comma separated CSV file you wish to load", "CSV (Comma separated)|*.csv");
            if (file == null) { return; }

            List<string[]> rows;
            CsvField[] assignment;
            using (var assigner = new CSV_Row_Assigner(file))
            {
                assigner.ShowDialog();
                rows = assigner.Rows;
                assignment = assigner.Assignment;
            }
            if (assignment.Length == 0) { return; }

            RunImport(() => new CsvContactMapper(CreateImportOptions()).Map(rows, assignment));
        }

        #endregion

        #region Export

        /// <summary>Asks for the target file, runs the exporter and writes the result.</summary>
        private void ExportToFile(ContactExporter exporter, Action<ExportOptions> configure = null)
        {
            ExportOptions options = CreateExportOptions();
            if (options == null) { return; }
            configure?.Invoke(options);

            string file;
            using (var dialog = new SaveFileDialog
            {
                Title = "Select the " + exporter.FormatName + " file you wish to create",
                DefaultExt = exporter.FileExtension,
                Filter = exporter.FileFilter,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                FileName = exporter.DefaultFileName,
            })
            {
                if (dialog.ShowDialog() != DialogResult.OK) { return; }
                file = dialog.FileName;
            }

            disable_buttons(true);
            try
            {
                ExportResult result = exporter.Export(contacts, options);
                File.WriteAllBytes(file, result.GetBytes());
                string pictureFolder = WritePictures(file, result);

                if (result.Warnings.Count > 0)
                {
                    ShowLongMessage("The following problems occurred during the export:", result.Warnings);
                }

                var message = new StringBuilder(result.EntryCount + " entries written to " + file + " !");
                foreach (string notice in result.Notices) { message.AppendLine().Append(notice); }
                MessageBox.Show(message.ToString());

                if (pictureFolder != null) { OfferCopyToFritzNas(pictureFolder); }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show("Error writing to file \"" + file + "\": " + ex.Message);
            }
            finally
            {
                disable_buttons(false);
            }
        }

        /// <summary>Writes the pictures of a FRITZ!Box export into "&lt;file&gt; - fonpix-custom"; returns that folder or null.</summary>
        private static string WritePictures(string exportFile, ExportResult result)
        {
            if (result.AdditionalFiles.Count == 0) { return null; }

            string folder = Path.Combine(Path.GetDirectoryName(exportFile), Path.GetFileNameWithoutExtension(exportFile) + " - fonpix-custom");
            Directory.CreateDirectory(folder);
            foreach (ExportedFile picture in result.AdditionalFiles)
            {
                File.WriteAllBytes(Path.Combine(folder, Path.GetFileName(picture.FileName)), picture.Content);
            }
            return folder;
        }

        private void OfferCopyToFritzNas(string pictureFolder)
        {
            if (!textBox_PicPath.Text.Contains("fonpix-custom")) { return; }

            DialogResult answer = MessageBox.Show(
                "Do you want CCW to try copying the contact pictures to " + FritzNasPicturePath + " ?" + Environment.NewLine
                + "This can only work if your windows user already has access rights to this network share!" + Environment.NewLine
                + "Existing files in this directory will be removed!",
                "Try copying pictures to \\\\FRITZ.NAS\\ ?", MessageBoxButtons.YesNo);
            if (answer != DialogResult.Yes) { return; }

            try
            {
                Directory.CreateDirectory(FritzNasPicturePath);
                foreach (FileInfo existing in new DirectoryInfo(FritzNasPicturePath).GetFiles("*.jpg")) { existing.Delete(); }
                foreach (FileInfo picture in new DirectoryInfo(pictureFolder).GetFiles("*.jpg"))
                {
                    picture.CopyTo(Path.Combine(FritzNasPicturePath, picture.Name), true);
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show("Copying failed with exception: " + ex);
            }
        }

        private void btn_save_FritzXML_Click(object sender, EventArgs e)
        {
            bool includeFax = ShiftPressed;
            ExportToFile(new FritzXmlExporter(), o => o.IncludeFax = includeFax);
        }

        private void btn_save_vCard_Click(object sender, EventArgs e)
        {
            bool onlyWithPhone = ShiftPressed;
            ExportToFile(new VCardExporter(), o => o.OnlyWithPhoneNumber = onlyWithPhone);
        }

        private void btn_save_vCard_Gigaset_Click(object sender, EventArgs e) => ExportToFile(new GigasetVCardExporter());

        private void btn_save_FritzAdr_Click(object sender, EventArgs e)
        {
            bool onlyWithFax = ShiftPressed;
            ExportToFile(new FritzAdrExporter(), o => o.OnlyWithFaxNumber = onlyWithFax);
        }

        private void btn_save_SnomXMLv7_Click(object sender, EventArgs e) => ExportToFile(new SnomV7CsvExporter());

        private void btn_save_SnomXMLv8_Click(object sender, EventArgs e) => ExportToFile(new SnomV8CsvExporter());

        private void btn_save_TalkSurfCSV_Click(object sender, EventArgs e) => ExportToFile(new TalkSurfCsvExporter());

        private void btn_save_AastraCSV_Click(object sender, EventArgs e) => ExportToFile(new AastraCsvExporter());

        private void btn_save_GrandstreamGXP_Click(object sender, EventArgs e) => ExportToFile(new GrandstreamGxpExporter());

        private void btn_save_GrandstreamGXV_Click(object sender, EventArgs e) => ExportToFile(new GrandstreamGxvExporter());

        private void btn_save_Auerswald_Click(object sender, EventArgs e) => ExportToFile(new AuerswaldCsvExporter());

        private void btn_save_panasonicCSV_Click(object sender, EventArgs e) => ExportToFile(new PanasonicCsvExporter());

        private void btn_save_genericCSV_Click(object sender, EventArgs e)
        {
            bool onlyWithPhone = ShiftPressed;
            ExportToFile(new GenericCsvExporter(), o => o.OnlyWithPhoneNumber = onlyWithPhone);
        }

        private void btn_save_Outlook_Click(object sender, EventArgs e)
        {
            ExportOptions options = CreateExportOptions();
            if (options == null) { return; }

            disable_buttons(true);
            try
            {
                OutlookConnector outlook = OutlookConnector.Connect();
                var cleaner = new PhoneNumberCleaner(options.Cleanup, options.CountryPrefix);
                int count = outlook.Export(contacts, cleaner, options.ApplyPrefixToAllFormats, options.ExportPictures);
                if (count >= 0)
                {
                    MessageBox.Show(count + " contacts have been written to the selected Outlook folder!");
                }
            }
            catch (Exception ex) when (IsReportable(ex) || ex is OutlookNotAvailableException || ex is Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
            {
                MessageBox.Show("Writing to Outlook failed!" + Environment.NewLine + Environment.NewLine + "Error returned was: " + ex.Message);
            }
            finally
            {
                disable_buttons(false);
            }
        }

        #endregion

        #region Website and update check

        private void button_website_Click(object sender, EventArgs e)
        {
            Process.Start(ShiftPressed || button_website.ForeColor == System.Drawing.Color.Green
                ? ReleaseVersions.LatestReleasePageUrl
                : "https://github.com/Rillke/Contact-Conversion-Wizard");
        }

        private void button_forum_Click(object sender, EventArgs e)
        {
            Process.Start(ShiftPressed
                ? "https://github.com/Rillke/Contact-Conversion-Wizard"
                : "https://github.com/Rillke/Contact-Conversion-Wizard/issues");
        }

        private void backgroundWorker_updateCheck_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                e.Result = UpdateCheck.GetLatestReleaseVersion(ProductVersion);
            }
            catch (Exception ex) when (ex is WebException || ex is IOException || ex is FormatException)
            {
                e.Result = null; // no network or GitHub not reachable: just don't show anything
            }
        }

        private void backgroundWorker_updateCheck_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null && e.Result is Version latest && ReleaseVersions.IsNewer(latest, ProductVersion))
            {
                button_website.ForeColor = System.Drawing.Color.Green;
                button_website.Text = "Update to: v" + latest;
            }
        }

        #endregion
    }
}
