using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Contact_Conversion_Wizard
{
    public partial class CSV_Row_Assigner : Form
    {
        ComboBox[] combo_array;
        Label[,] label_array;
        int[] assist_helper;
        string[] combo_options = new string[] { "(ignore)", "Lastname", "Firstname", "Company", "Home", "Work", "Mobile", "HomeFax", "WorkFax", "Street", "ZIP Code", "City", "eMail", "Notes" };
        const int row_distance = 24;

        public CSV_Row_Assigner(string[] headers, List<string[]> LineList, ref int[] my_assist_helper)
        {
            int coloumns = headers.GetLength(0);

            combo_array = new ComboBox[coloumns];
            label_array = new Label[5, coloumns];

            InitializeComponent();

            for (int i = 0; i < coloumns; i++)
            {
                combo_array[i] = new ComboBox();
                combo_array[i].DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
                combo_array[i].FormattingEnabled = true;
                combo_array[i].Items.AddRange(combo_options);
                combo_array[i].Location = new System.Drawing.Point(9, 43 + (row_distance * i));
                combo_array[i].Name = "combo_sel" + i;
                combo_array[i].Size = new System.Drawing.Size(118, 21);
                this.Controls.Add(combo_array[i]);

                for (int j = 0; j < 5; j++)
                {
                    label_array[j, i] = new Label();
                    label_array[j, i].AutoSize = true;
                    label_array[j, i].Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    label_array[j, i].Location = new System.Drawing.Point(167 + (j * 170), 46 + (row_distance * i));
                    label_array[j, i].Name = "label_r" + j.ToString() + "c" + i.ToString("00");
                    label_array[j, i].Size = new System.Drawing.Size(73, 13);
                    label_array[j, i].Text = "label_r" + j.ToString() + "c" + i.ToString("00");
                    this.Controls.Add(label_array[j, i]);
                }
            }

            // label_bottom
            Label label_bottom = new Label();
            label_bottom.AutoSize = true;
            label_bottom.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label_bottom.Location = new System.Drawing.Point(12, 46 + row_distance * coloumns);
            label_bottom.Name = "label_bottom";
            label_bottom.Size = new System.Drawing.Size(852, 25);
            label_bottom.Text = "Please assign the CSV coloumns to the right data fields, then close the window!";
            this.Controls.Add(label_bottom);

            this.Size = new System.Drawing.Size(this.Size.Width, (110 + row_distance * coloumns));

            assist_helper = my_assist_helper;
            
            foreach (ComboBox my_cb in combo_array)
            { my_cb.SelectedIndex = 0; }

            for (int i = 0; i < coloumns; i++)
            {
                for (int j = 0; j < 4 ; j++)
                {
                    if (j == 0)
                    {
                        if (headers[i] != "")
                        {
                            label_array[j, i].Text = headers[i];
                        }
                        else
                        {
                            label_array[j, i].Text = "";
                        }
                    }

                    if (j < LineList.Count)
                    {
                        label_array[j+1, i].Text = LineList[j][i];
                    }
                    else
                    {
                        label_array[j+1, i].Text = "";
                    }
                }
            }

            // auto assign selected-text if header matches an option available in the combobox
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

        private void CSV_Row_Assigner_FormClosing(object sender, FormClosingEventArgs e)
        {
            // save data from comboboxes to assist_helper int[], which will then hopefully be available to the main form
            for (int i = 0; i < assist_helper.GetLength(0); i++)
            {
                // assist_helper[i]
                assist_helper[i] = combo_array[i].SelectedIndex-1;
            }
        }
    }
}
