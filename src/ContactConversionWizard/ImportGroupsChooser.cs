using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Contact_Conversion_Wizard
{
    public partial class ImportGroupsChooser : Form
    {
        private List<string[]> intList;
        public List<string[]> resultList;


        public ImportGroupsChooser(string question, string windowtitle, List<string[]> myList)
        {
            InitializeComponent();

            intList = myList;

            label1.Text = question;
            this.Text = windowtitle;

            foreach (string[] x in myList)
            {
                listBox1.Items.Add(x[0]);
            }
            

        }

        private void ImportGroupsChooser_FormClosing(object sender, FormClosingEventArgs e)
        {
            resultList = new List<string[]>();

            for (int i = 0; i < listBox1.Items.Count; i++)
            {   // Determine if the item is selected.
                if (listBox1.GetSelected(i) == true) { resultList.Add(this.intList[i]); }
            }


        }
    }
}
