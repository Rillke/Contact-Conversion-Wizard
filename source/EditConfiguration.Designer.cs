namespace Contact_Conversion_Wizard
{
    partial class EditConfiguration
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbl_configtitle = new System.Windows.Forms.Label();
            this.OPT_adjustcol = new System.Windows.Forms.CheckBox();
            this.OPT_hidecol = new System.Windows.Forms.CheckBox();
            this.checkBox_cleanXchar = new System.Windows.Forms.CheckBox();
            this.checkBox_cleanHyphen = new System.Windows.Forms.CheckBox();
            this.checkBox_cleanHashKey = new System.Windows.Forms.CheckBox();
            this.checkBox_cleanBrackets = new System.Windows.Forms.CheckBox();
            this.label_cleanheader = new System.Windows.Forms.Label();
            this.OPT_prefixNONFB = new System.Windows.Forms.CheckBox();
            this.checkBox_cleanSlash = new System.Windows.Forms.CheckBox();
            this.OPT_importOther = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_gMail_login = new System.Windows.Forms.Label();
            this.lbl_gMail_pass = new System.Windows.Forms.Label();
            this.textBox_gLogin = new System.Windows.Forms.TextBox();
            this.textBox_gPass = new System.Windows.Forms.TextBox();
            this.OPT_fritzXML_order = new System.Windows.Forms.CheckBox();
            this.OPT_DUPren = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lbl_configtitle
            // 
            this.lbl_configtitle.AutoSize = true;
            this.lbl_configtitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_configtitle.Location = new System.Drawing.Point(12, 9);
            this.lbl_configtitle.Name = "lbl_configtitle";
            this.lbl_configtitle.Size = new System.Drawing.Size(252, 20);
            this.lbl_configtitle.TabIndex = 6;
            this.lbl_configtitle.Text = "General configuration settings";
            // 
            // OPT_adjustcol
            // 
            this.OPT_adjustcol.AutoSize = true;
            this.OPT_adjustcol.Location = new System.Drawing.Point(16, 68);
            this.OPT_adjustcol.Name = "OPT_adjustcol";
            this.OPT_adjustcol.Size = new System.Drawing.Size(196, 17);
            this.OPT_adjustcol.TabIndex = 5;
            this.OPT_adjustcol.Text = "Allow adjusting the width of columns";
            this.OPT_adjustcol.UseVisualStyleBackColor = true;
            // 
            // OPT_hidecol
            // 
            this.OPT_hidecol.AutoSize = true;
            this.OPT_hidecol.Location = new System.Drawing.Point(16, 45);
            this.OPT_hidecol.Name = "OPT_hidecol";
            this.OPT_hidecol.Size = new System.Drawing.Size(188, 17);
            this.OPT_hidecol.TabIndex = 4;
            this.OPT_hidecol.Text = "Hide columns that contain no data";
            this.OPT_hidecol.UseVisualStyleBackColor = true;
            // 
            // checkBox_cleanXchar
            // 
            this.checkBox_cleanXchar.AutoSize = true;
            this.checkBox_cleanXchar.Location = new System.Drawing.Point(16, 307);
            this.checkBox_cleanXchar.Name = "checkBox_cleanXchar";
            this.checkBox_cleanXchar.Size = new System.Drawing.Size(84, 17);
            this.checkBox_cleanXchar.TabIndex = 8;
            this.checkBox_cleanXchar.Text = "Remove \"x\"";
            this.checkBox_cleanXchar.UseVisualStyleBackColor = true;
            // 
            // checkBox_cleanHyphen
            // 
            this.checkBox_cleanHyphen.AutoSize = true;
            this.checkBox_cleanHyphen.Location = new System.Drawing.Point(16, 284);
            this.checkBox_cleanHyphen.Name = "checkBox_cleanHyphen";
            this.checkBox_cleanHyphen.Size = new System.Drawing.Size(82, 17);
            this.checkBox_cleanHyphen.TabIndex = 7;
            this.checkBox_cleanHyphen.Text = "Remove \"-\"";
            this.checkBox_cleanHyphen.UseVisualStyleBackColor = true;
            // 
            // checkBox_cleanHashKey
            // 
            this.checkBox_cleanHashKey.AutoSize = true;
            this.checkBox_cleanHashKey.Location = new System.Drawing.Point(16, 261);
            this.checkBox_cleanHashKey.Name = "checkBox_cleanHashKey";
            this.checkBox_cleanHashKey.Size = new System.Drawing.Size(86, 17);
            this.checkBox_cleanHashKey.TabIndex = 10;
            this.checkBox_cleanHashKey.Text = "Remove \"#\"";
            this.checkBox_cleanHashKey.UseVisualStyleBackColor = true;
            // 
            // checkBox_cleanBrackets
            // 
            this.checkBox_cleanBrackets.AutoSize = true;
            this.checkBox_cleanBrackets.Location = new System.Drawing.Point(16, 238);
            this.checkBox_cleanBrackets.Name = "checkBox_cleanBrackets";
            this.checkBox_cleanBrackets.Size = new System.Drawing.Size(119, 17);
            this.checkBox_cleanBrackets.TabIndex = 9;
            this.checkBox_cleanBrackets.Text = "Remove \"(\" and \")\"";
            this.checkBox_cleanBrackets.UseVisualStyleBackColor = true;
            // 
            // label_cleanheader
            // 
            this.label_cleanheader.AutoSize = true;
            this.label_cleanheader.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_cleanheader.Location = new System.Drawing.Point(13, 213);
            this.label_cleanheader.Name = "label_cleanheader";
            this.label_cleanheader.Size = new System.Drawing.Size(289, 20);
            this.label_cleanheader.TabIndex = 11;
            this.label_cleanheader.Text = "Cleanup phone numbers on export:";
            // 
            // OPT_prefixNONFB
            // 
            this.OPT_prefixNONFB.AutoSize = true;
            this.OPT_prefixNONFB.Location = new System.Drawing.Point(16, 91);
            this.OPT_prefixNONFB.Name = "OPT_prefixNONFB";
            this.OPT_prefixNONFB.Size = new System.Drawing.Size(175, 17);
            this.OPT_prefixNONFB.TabIndex = 12;
            this.OPT_prefixNONFB.Text = "Export Prefixes for non-Fritz!Box";
            this.OPT_prefixNONFB.UseVisualStyleBackColor = true;
            // 
            // checkBox_cleanSlash
            // 
            this.checkBox_cleanSlash.AutoSize = true;
            this.checkBox_cleanSlash.Location = new System.Drawing.Point(16, 330);
            this.checkBox_cleanSlash.Name = "checkBox_cleanSlash";
            this.checkBox_cleanSlash.Size = new System.Drawing.Size(84, 17);
            this.checkBox_cleanSlash.TabIndex = 13;
            this.checkBox_cleanSlash.Text = "Remove \"/\"";
            this.checkBox_cleanSlash.UseVisualStyleBackColor = true;
            // 
            // OPT_importOther
            // 
            this.OPT_importOther.AutoSize = true;
            this.OPT_importOther.Location = new System.Drawing.Point(16, 137);
            this.OPT_importOther.Name = "OPT_importOther";
            this.OPT_importOther.Size = new System.Drawing.Size(319, 17);
            this.OPT_importOther.TabIndex = 14;
            this.OPT_importOther.Text = "Use \"Other\" phone number from Outlook if Home/Work empty";
            this.OPT_importOther.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 360);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(268, 20);
            this.label1.TabIndex = 17;
            this.label1.Text = "Google account for direct import";
            // 
            // lbl_gMail_login
            // 
            this.lbl_gMail_login.AutoSize = true;
            this.lbl_gMail_login.Location = new System.Drawing.Point(17, 391);
            this.lbl_gMail_login.Name = "lbl_gMail_login";
            this.lbl_gMail_login.Size = new System.Drawing.Size(64, 13);
            this.lbl_gMail_login.TabIndex = 18;
            this.lbl_gMail_login.Text = "gMail Login:";
            // 
            // lbl_gMail_pass
            // 
            this.lbl_gMail_pass.AutoSize = true;
            this.lbl_gMail_pass.Location = new System.Drawing.Point(17, 417);
            this.lbl_gMail_pass.Name = "lbl_gMail_pass";
            this.lbl_gMail_pass.Size = new System.Drawing.Size(84, 13);
            this.lbl_gMail_pass.TabIndex = 19;
            this.lbl_gMail_pass.Text = "gMail Password:";
            // 
            // textBox_gLogin
            // 
            this.textBox_gLogin.Location = new System.Drawing.Point(100, 388);
            this.textBox_gLogin.Name = "textBox_gLogin";
            this.textBox_gLogin.Size = new System.Drawing.Size(180, 20);
            this.textBox_gLogin.TabIndex = 20;
            // 
            // textBox_gPass
            // 
            this.textBox_gPass.Location = new System.Drawing.Point(100, 415);
            this.textBox_gPass.Name = "textBox_gPass";
            this.textBox_gPass.Size = new System.Drawing.Size(180, 20);
            this.textBox_gPass.TabIndex = 21;
            this.textBox_gPass.UseSystemPasswordChar = true;
            // 
            // OPT_fritzXML_order
            // 
            this.OPT_fritzXML_order.AutoSize = true;
            this.OPT_fritzXML_order.Location = new System.Drawing.Point(16, 114);
            this.OPT_fritzXML_order.Name = "OPT_fritzXML_order";
            this.OPT_fritzXML_order.Size = new System.Drawing.Size(242, 17);
            this.OPT_fritzXML_order.TabIndex = 22;
            this.OPT_fritzXML_order.Text = "Fritz!XML Export: Work Nr. before Home Nr. ?";
            this.OPT_fritzXML_order.UseVisualStyleBackColor = true;
            // 
            // OPT_DUPren
            // 
            this.OPT_DUPren.AutoSize = true;
            this.OPT_DUPren.Location = new System.Drawing.Point(16, 161);
            this.OPT_DUPren.Name = "OPT_DUPren";
            this.OPT_DUPren.Size = new System.Drawing.Size(296, 17);
            this.OPT_DUPren.TabIndex = 24;
            this.OPT_DUPren.Text = "Allow import of duplicate names by numbering them 00-99";
            this.OPT_DUPren.UseVisualStyleBackColor = true;
            // 
            // EditConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 474);
            this.Controls.Add(this.OPT_DUPren);
            this.Controls.Add(this.OPT_fritzXML_order);
            this.Controls.Add(this.textBox_gPass);
            this.Controls.Add(this.textBox_gLogin);
            this.Controls.Add(this.lbl_gMail_pass);
            this.Controls.Add(this.lbl_gMail_login);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OPT_importOther);
            this.Controls.Add(this.checkBox_cleanSlash);
            this.Controls.Add(this.OPT_prefixNONFB);
            this.Controls.Add(this.label_cleanheader);
            this.Controls.Add(this.checkBox_cleanHashKey);
            this.Controls.Add(this.checkBox_cleanBrackets);
            this.Controls.Add(this.checkBox_cleanXchar);
            this.Controls.Add(this.checkBox_cleanHyphen);
            this.Controls.Add(this.lbl_configtitle);
            this.Controls.Add(this.OPT_adjustcol);
            this.Controls.Add(this.OPT_hidecol);
            this.Name = "EditConfiguration";
            this.Text = "EditConfiguration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditConfiguration_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_configtitle;
        private System.Windows.Forms.CheckBox OPT_adjustcol;
        private System.Windows.Forms.CheckBox OPT_hidecol;
        private System.Windows.Forms.CheckBox checkBox_cleanXchar;
        private System.Windows.Forms.CheckBox checkBox_cleanHyphen;
        private System.Windows.Forms.CheckBox checkBox_cleanHashKey;
        private System.Windows.Forms.CheckBox checkBox_cleanBrackets;
        private System.Windows.Forms.Label label_cleanheader;
        private System.Windows.Forms.CheckBox OPT_prefixNONFB;
        private System.Windows.Forms.CheckBox checkBox_cleanSlash;
        private System.Windows.Forms.CheckBox OPT_importOther;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_gMail_login;
        private System.Windows.Forms.Label lbl_gMail_pass;
        private System.Windows.Forms.TextBox textBox_gLogin;
        private System.Windows.Forms.TextBox textBox_gPass;
        private System.Windows.Forms.CheckBox OPT_fritzXML_order;
        private System.Windows.Forms.CheckBox OPT_DUPren;
    }
}