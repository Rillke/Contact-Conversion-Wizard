using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ContactConversionWizard.Core.Config;
using ContactConversionWizard.Core.Import;
using ContactConversionWizard.Core.Text;


namespace Contact_Conversion_Wizard
{
    public partial class CSV_Row_Assigner : Form
    {
        ComboBox[] combo_array;
        Label[,] label_array;
        Label[] label_header_array;
        readonly string[] combo_options = CsvContactMapper.FieldNames.ToArray();
        const int row_distance = 24;
        const int top_free = 60;
        string[] headers;
        readonly string my_filename;
        List<string[]> my_LineList = new List<string[]>();
        bool already_drawn = false;

        static string ConfigFilePath => Path.Combine(AppSettings.SettingsFolder, "CCW-CSV-Assigner.config");

        /// <summary>The data rows of the CSV file (without header).</summary>
        public List<string[]> Rows => my_LineList;

        /// <summary>The field each column is assigned to, set when the dialog is closed.</summary>
        public CsvField[] Assignment { get; private set; } = new CsvField[0];

        public CSV_Row_Assigner(string the_filename)
        {
            my_filename = the_filename;

            InitializeComponent();

            // files with many columns need more space than small screens offer (issue #33)
            AutoScroll = true;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            combo_Separator.SelectedIndex = 0;
            combo_Headers.SelectedIndex = 0;
            combo_Encoding.SelectedIndex = 0;

            combo_Encoding.SelectedIndexChanged += new System.EventHandler(this.someCombo_SelectedIndexChanged);
            combo_Separator.SelectedIndexChanged += new System.EventHandler(this.someCombo_SelectedIndexChanged);
            combo_Headers.SelectedIndexChanged += new System.EventHandler(this.someCombo_SelectedIndexChanged);

            CSV_SettingsChanged();
        }

        private void CSV_SettingsChanged()
        {
            bool has_headers = false;
            if (combo_Headers.SelectedIndex == 0) { has_headers = true; }

            bool has_unicode = false;
            if (combo_Encoding.SelectedIndex == 0) { has_unicode = true; }

            char char_sep = ',';
            if (combo_Separator.SelectedIndex == 1) { char_sep = ';'; }
            if (combo_Separator.SelectedIndex == 2) { char_sep = '\t'; }

            string reload_success = CSV_ReloadData(my_filename, has_headers, has_unicode, char_sep, ref my_LineList, ref headers);
            CSV_ReDrawMain(reload_success);
        }

        private string CSV_ReloadData(string my_filename, bool is_headers, bool is_unicode, char char_separator, ref List<string[]> LineList, ref string[] headers)
        {
            LineList.Clear();
            headers = null;

            try
            {
                Encoding encoding = is_unicode ? Encoding.UTF8 : TextEncodings.Latin1;
                List<string[]> rows = CsvParser.Parse(File.ReadAllText(my_filename, encoding), char_separator);
                if (rows.Count == 0 || (is_headers && rows.Count == 1))
                {
                    return "The file contains no data rows.";
                }

                int fieldCount = rows[0].Length;
                if (is_headers)
                {
                    headers = rows[0];
                    rows.RemoveAt(0);
                }

                // make all rows as long as the first one
                foreach (string[] row in rows)
                {
                    var normalized = new string[fieldCount];
                    for (int i = 0; i < fieldCount; i++) { normalized[i] = i < row.Length ? row[i] : ""; }
                    LineList.Add(normalized);
                }
            }
            catch (Exception e) when (e is IOException || e is FormatException || e is UnauthorizedAccessException)
            {
                return e.Message;
            }

            return "success";
        }

        private void CSV_ReDrawMain(string reader_success)
        {
            // first remove everything already in the arrays!
            if (already_drawn == true)
            {
                foreach (Label x in label_header_array)
                { x.Dispose(); }

                foreach (Label y in label_array)
                { y.Dispose(); }
                
                foreach (ComboBox z in combo_array)
                { z.Dispose(); }
                
            }

            if (reader_success != "success")
            {
                // draw the content of reader_success somewhere in the middle of the screen
                label_error.Text = "Loading the CSV failed with the following error:" + Environment.NewLine + reader_success;
                label_error.Visible = true;
                return;
            }
            else
            {
                label_error.Visible = false;
            }


            // from some place else
            // read first line and place it in header

            // 13 fields can be used for:
            // (3) lastname / firstname / company 
            // (3) home / work / mobile
            // (2) homefax / workfax
            // (5) street / zip / city / email / comments

            // then reset the arrays
            int coloumns = my_LineList[0].GetLength(0);
            this.Text = "CSV Row Assigner" +  " (" + my_LineList.Count + " x " + coloumns + ")";

            // the dynamic controls are positioned relative to the unscrolled form
            this.AutoScrollPosition = new System.Drawing.Point(0, 0);

            combo_array = new ComboBox[coloumns];
            label_array = new Label[5, coloumns];
            label_header_array = new Label[5];

            // and then draw stuff again
            for (int i = 0; i < coloumns; i++)
            {
                combo_array[i] = new ComboBox();
                combo_array[i].DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                combo_array[i].FormattingEnabled = true;
                combo_array[i].Items.AddRange(combo_options);
                combo_array[i].Location = new System.Drawing.Point(9, 43 + top_free + (row_distance * i));
                combo_array[i].Name = "combo_sel" + i;
                combo_array[i].Size = new System.Drawing.Size(118, 21);
                this.Controls.Add(combo_array[i]);

                for (int j = 0; j < 5; j++)
                {
                    if (i == 0)
                    {
                        // draw the header
                        label_header_array[j] = new Label();
                        // 
                        // label2
                        // 
                        label_header_array[j].AutoSize = true;
                        label_header_array[j].Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        label_header_array[j].Location = new System.Drawing.Point(166 + (170 * j), 16 + top_free);
                        label_header_array[j].Name = "label_h" + j.ToString();
                        label_header_array[j].Size = new System.Drawing.Size(68, 20);
                        label_header_array[j].TabIndex = 79;
                        if (headers != null && j == 0)
                        { label_header_array[j].Text = "Header"; }
                        else
                        {
                            if (headers != null)
                            { label_header_array[j].Text = "Row #" + (j-1); }
                            else
                            { label_header_array[j].Text = "Row #" + j; }
                        }
                        this.Controls.Add(label_header_array[j]);


                    }

                    label_array[j, i] = new Label();
                    label_array[j, i].AutoSize = true;
                    label_array[j, i].Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    label_array[j, i].Location = new System.Drawing.Point(167 + (j * 170), 46 + top_free + (row_distance * i));
                    label_array[j, i].Name = "label_r" + j.ToString() + "c" + i.ToString("00");
                    label_array[j, i].Size = new System.Drawing.Size(73, 13);
                    label_array[j, i].Text = "label_r" + j.ToString() + "c" + i.ToString("00");
                    this.Controls.Add(label_array[j, i]);
                }

                
            }

            already_drawn = true;
            
            // as high as needed, but not higher than the screen; the rest can be scrolled
            int wantedHeight = 110 + row_distance * coloumns + top_free;
            int screenHeight = Screen.FromControl(this).WorkingArea.Height;
            this.Size = new System.Drawing.Size(this.Size.Width, Math.Min(wantedHeight, screenHeight));
            if (wantedHeight > screenHeight) { this.Location = new System.Drawing.Point(this.Location.X, Screen.FromControl(this).WorkingArea.Top); }

            foreach (ComboBox my_cb in combo_array)
            { my_cb.SelectedIndex = 0; }

            int still_to_fill = 5;
            int adder = 0;

            if (headers != null)
            {
                adder = 1;
                still_to_fill = 4;
            }


            for (int i = 0; i < coloumns; i++)
            {
                if (headers != null)
                {
                    label_array[0, i].Text = "H:" + headers[i];
                }


                for (int j = 0; j < still_to_fill; j++)
                {

                    if (j < my_LineList.Count)
                    {
                        label_array[j + adder, i].Text = my_LineList[j][i];
                    }
                    else
                    {
                        label_array[j + adder, i].Text = "";
                    }
                }
            }

            // auto assign selected-text if header matches an option available in the combobox
            if (headers != null)
            {
                for (int i = 0; i < coloumns; i++)
                {
                    for (int j = 0; j < combo_options.GetLength(0); j++)
                    {
                        if (headers[i] == combo_options[j])
                        {
                            combo_array[i].SelectedIndex = j;
                        }
                    }
                }
            }

        }

        private void CSV_Row_Assigner_FormClosing(object sender, FormClosingEventArgs e)
        {
            // hand the selected assignment to the main form
            Assignment = combo_array == null || label_error.Visible
                ? new CsvField[0]
                : combo_array.Select(c => (CsvField)(c.SelectedIndex - 1)).ToArray();
        }

        private void someCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // re run everything
            CSV_SettingsChanged();
        }

        private void combo_set(ComboBox cb_to_set, string value_to_set)
        {
            if (cb_to_set.Items.Count > 0)
            {
                for (int i = 0; i < cb_to_set.Items.Count; i++)
                {
                    if (cb_to_set.Items[i].ToString() == value_to_set)
                    {
                        cb_to_set.SelectedIndex = i;
                        return;
                    }
                }
            }
        }

        private void button_regsave_Click(object sender, EventArgs e)
        {
            if (combo_array == null) { return; }

            var config = new ConfigFile();
            config.Set("cfg_csv_encoding", combo_Encoding.SelectedItem.ToString());
            config.Set("cfg_csv_separator", combo_Separator.SelectedItem.ToString());
            config.Set("cfg_csv_headers", combo_Headers.SelectedItem.ToString());
            for (int i = 0; i < combo_array.Length; i++)
            {
                config.Set("cfg_csv_combo_" + i, combo_array[i].SelectedItem.ToString());
            }

            Directory.CreateDirectory(AppSettings.SettingsFolder);
            config.Save(ConfigFilePath);
        }

        private void button_regload_Click(object sender, EventArgs e)
        {
            ConfigFile config = ConfigFile.Load(ConfigFilePath);

            // these trigger a reload of the file and redraw the column selection
            combo_set(combo_Separator, config.GetString("cfg_csv_separator"));
            combo_set(combo_Headers, config.GetString("cfg_csv_headers"));
            combo_set(combo_Encoding, config.GetString("cfg_csv_encoding"));

            if (combo_array == null) { return; }
            for (int i = 0; i < combo_array.Length; i++)
            {
                combo_set(combo_array[i], config.GetString("cfg_csv_combo_" + i));
            }
        }
    }
}
