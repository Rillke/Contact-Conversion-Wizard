using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Fritz_XML_Wizard
{
    public partial class CustomCountryID : Form
    {
        public string country_id_transfer;

        public CustomCountryID()
        {
            InitializeComponent();
        }

        private void CustomCountryID_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (textBox1.Text == "")
            {
                e.Cancel = true;
            }
            else
            {
                country_id_transfer = textBox1.Text;
            }

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar) && !Char.IsControl(e.KeyChar))
            {

                e.Handled = true;

            }
        }
    }
}
