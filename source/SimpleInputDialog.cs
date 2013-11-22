using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Contact_Conversion_Wizard
{
    public partial class SimpleInputDialog : Form
    {
        public string resultstring;
        bool my_onlynumeric;

        public SimpleInputDialog(string question, string windowtitle, bool onlynumeric, bool hide_password)
        {
            InitializeComponent();

            label1.Text = question;
            this.Text = windowtitle;
            textBox1.UseSystemPasswordChar = hide_password;

            my_onlynumeric = onlynumeric;

            textBox1.AcceptsReturn = true;
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
            if (e.KeyChar == (char)Keys.Return)
            {
                // if enter is pressed and we have valid input in the textbox, quit this form and return result to caller
                if (textBox1.Text != "")
                {
                    e.Handled = true; // prevents bling sound
                    this.Close();
                    return;
                }
            }

            if (my_onlynumeric == true)
            {
                if (!Char.IsNumber(e.KeyChar) && !Char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                    return;
                }
            }
            else
            {
                e.Handled = false;
                return;
            }



        }

        private void button_OK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
