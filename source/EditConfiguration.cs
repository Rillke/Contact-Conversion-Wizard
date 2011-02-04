using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

            checkBox_cleanBrackets.Checked = Form1.clean_brackets;
            checkBox_cleanHashKey.Checked = Form1.clean_hashkey;
            checkBox_cleanHyphen.Checked = Form1.clean_hyphen;
            checkBox_cleanXchar.Checked = Form1.clean_xchar;
        }

        private void EditConfiguration_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1.cfg_hideemptycols = OPT_hidecol.Checked;
            Form1.cfg_adjustablecols = OPT_adjustcol.Checked;
            Form1.cfg_prefixNONFB = OPT_prefixNONFB.Checked;

            Form1.clean_brackets = checkBox_cleanBrackets.Checked;
            Form1.clean_hashkey = checkBox_cleanHashKey.Checked;
            Form1.clean_hyphen = checkBox_cleanHyphen.Checked;
            Form1.clean_xchar = checkBox_cleanXchar.Checked;
        }

    }

}