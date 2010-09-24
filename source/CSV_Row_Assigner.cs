using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LumenWorks.Framework.IO.Csv;


namespace Contact_Conversion_Wizard
{
    public partial class CSV_Row_Assigner : Form
    {
        ComboBox[] combo_array;
        Label[,] label_array;
        Label[] label_header_array;
        string[] combo_options = new string[] { "(ignore)", "Lastname", "Firstname", "Company", "Home", "Work", "Mobile", "HomeFax", "WorkFax", "Street", "ZIP Code", "City", "eMail", "Notes" };
        const int row_distance = 24;
        const int top_free = 60;
        string[] headers;
        string my_filename;
        List<string[]> my_LineList;
        int[] my_assist_helper;
        bool already_drawn = false;
        
        public CSV_Row_Assigner(string the_filename, ref List<string[]> the_LineList, ref int[] the_assist_helper)
        {
            my_LineList = the_LineList;
            my_filename = the_filename;
            my_assist_helper = the_assist_helper;

            InitializeComponent();
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

            // uses CSVreader Library from http://www.codeproject.com/KB/database/CsvReader.aspx under MIT License
            int fieldCount;

            System.IO.StreamReader myReader;
            if (is_unicode == true)
            { myReader = new System.IO.StreamReader(my_filename, Encoding.UTF8); }
            else
            { myReader = new System.IO.StreamReader(my_filename, Encoding.GetEncoding("ISO-8859-1")); }

            // open the file, which is a CSV file with or without headers

            try
            {
                using (CsvReader csv = new CsvReader(myReader, is_headers, char_separator))
                {
                    fieldCount = csv.FieldCount;
                    if (is_headers == true) { headers = csv.GetFieldHeaders(); }
                    else { headers = null; } 

                    while (csv.ReadNextRecord())
                    {
                        string[] temparray = new string[fieldCount];
                        csv.CopyCurrentRecordTo(temparray);
                        LineList.Add(temparray);
                    }
                }
            }
            catch (Exception e)
            {
                myReader.Close();
                return (e.ToString());
            }
            
            myReader.Close();
            return ("success");
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
            label_dim.Text = "Dimensions: " + my_LineList.Count + " x " + coloumns;

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
            
            this.Size = new System.Drawing.Size(this.Size.Width, (110 + row_distance * coloumns) + top_free);

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
            // save data from comboboxes to assist_helper int[], which will then hopefully be available to the main form
            for (int i = 0; i < my_LineList[0].GetLength(0); i++)
            { my_assist_helper[i] = combo_array[i].SelectedIndex - 1; }
        }

        private void someCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // re run everything
            CSV_SettingsChanged();
        }
    }
}
