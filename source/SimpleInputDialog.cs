using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Contact_Conversion_Wizard
{
    public partial class SimpleInputDialog : Form
    {
        public string resultstring;
        bool my_onlynumeric;

        public SimpleInputDialog(string question, string windowtitle, bool onlynumeric)
        {
            InitializeComponent();

            label1.Text = question;
            this.Text = windowtitle;

            my_onlynumeric = onlynumeric;
        }

        private void SimpleInputDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (textBox1.Text == "")
            {
                e.Cancel = true;
            }
            else
            {
                resultstring = textBox1.Text;
            }

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (my_onlynumeric == true)
            {
                if (!Char.IsNumber(e.KeyChar) && !Char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = false;
            }
        }
    }
}
