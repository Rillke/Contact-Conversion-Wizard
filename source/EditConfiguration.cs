using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Contact_Conversion_Wizard
{
    public partial class EditConfiguration : Form
    {
        public EditConfiguration()
        {
            InitializeComponent();
            OPT_hidecol.Checked = Form1.cfg_hideemptycols;
            OPT_adjustcol.Checked = Form1.cfg_adjustablecols;
            OPT_prefixNONFB.Checked = Form1.cfg_prefixNONFB;
            OPT_importOther.Checked = Form1.cfg_importOther;
            OPT_DUPren.Checked = Form1.cfg_DUPren;


            checkBox_cleanBrackets.Checked = !Form1.clean_brackets;
            checkBox_cleanHashKey.Checked = !Form1.clean_hashkey;
            checkBox_cleanSlash.Checked = !Form1.clean_slash;
            checkBox_cleanHyphen.Checked = !Form1.clean_hyphen;
            checkBox_cleanXchar.Checked = !Form1.clean_xchar;
            checkBox_cleanSpace.Checked = !Form1.clean_space;
            checkBox_cleanSquareBrackets.Checked = !Form1.clean_squarebrackets;
            checkBox_cleanLetters.Checked = !Form1.clean_letters;
            checkBox_cleanaddzeroprefix.Checked = Form1.clean_addzeroprefix;


            textBox_gLogin.Text = Form1.g_login;
            textBox_gPass.Text = Form1.g_pass;

            checkBox_checkVersion.Checked = Form1.cfg_checkVersion;


        }

        private void EditConfiguration_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1.cfg_hideemptycols = OPT_hidecol.Checked;
            Form1.cfg_adjustablecols = OPT_adjustcol.Checked;
            Form1.cfg_prefixNONFB = OPT_prefixNONFB.Checked;
            Form1.cfg_importOther = OPT_importOther.Checked;
            Form1.cfg_DUPren = OPT_DUPren.Checked;

            Form1.clean_brackets = !checkBox_cleanBrackets.Checked;
            Form1.clean_hashkey = !checkBox_cleanHashKey.Checked;
            Form1.clean_slash = !checkBox_cleanSlash.Checked;
            Form1.clean_hyphen = !checkBox_cleanHyphen.Checked;
            Form1.clean_xchar = !checkBox_cleanXchar.Checked;
            Form1.clean_space = !checkBox_cleanSpace.Checked;
            Form1.clean_squarebrackets = !checkBox_cleanSquareBrackets.Checked;
            Form1.clean_letters = !checkBox_cleanLetters.Checked;
            Form1.clean_addzeroprefix = checkBox_cleanaddzeroprefix.Checked;


            Form1.g_login = textBox_gLogin.Text;
            Form1.g_pass = textBox_gPass.Text;

            Form1.cfg_checkVersion = checkBox_checkVersion.Checked;

        }

    }

}