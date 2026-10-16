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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditConfiguration));
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
            this.OPT_DUPren = new System.Windows.Forms.CheckBox();
            this.checkBox_checkVersion = new System.Windows.Forms.CheckBox();
            this.checkBox_cleanSpace = new System.Windows.Forms.CheckBox();
            this.checkBox_cleanSquareBrackets = new System.Windows.Forms.CheckBox();
            this.checkBox_cleanLetters = new System.Windows.Forms.CheckBox();
            this.checkBox_cleanaddzeroprefix = new System.Windows.Forms.CheckBox();
            this.label_export = new System.Windows.Forms.Label();
            this.checkBox_multiNumbers = new System.Windows.Forms.CheckBox();
            this.label_Outlook = new System.Windows.Forms.Label();
            this.checkBox_OutlookEmptyNumber = new System.Windows.Forms.CheckBox();
            this.checkBox_OutlookEmptyName = new System.Windows.Forms.CheckBox();
            this.label_Nextcloud = new System.Windows.Forms.Label();
            this.checkBox_nextcloud_vfc_fix = new System.Windows.Forms.CheckBox();
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
            this.checkBox_cleanXchar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_cleanXchar.AutoSize = true;
            this.checkBox_cleanXchar.Location = new System.Drawing.Point(16, 404);
            this.checkBox_cleanXchar.Name = "checkBox_cleanXchar";
            this.checkBox_cleanXchar.Size = new System.Drawing.Size(69, 17);
            this.checkBox_cleanXchar.TabIndex = 8;
            this.checkBox_cleanXchar.Text = "Keep \"x\"";
            this.checkBox_cleanXchar.UseVisualStyleBackColor = true;
            // 
            // checkBox_cleanHyphen
            // 
            this.checkBox_cleanHyphen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_cleanHyphen.AutoSize = true;
            this.checkBox_cleanHyphen.Location = new System.Drawing.Point(16, 381);
            this.checkBox_cleanHyphen.Name = "checkBox_cleanHyphen";
            this.checkBox_cleanHyphen.Size = new System.Drawing.Size(67, 17);
            this.checkBox_cleanHyphen.TabIndex = 7;
            this.checkBox_cleanHyphen.Text = "Keep \"-\"";
            this.checkBox_cleanHyphen.UseVisualStyleBackColor = true;
            // 
            // checkBox_cleanHashKey
            // 
            this.checkBox_cleanHashKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_cleanHashKey.AutoSize = true;
            this.checkBox_cleanHashKey.Location = new System.Drawing.Point(16, 358);
            this.checkBox_cleanHashKey.Name = "checkBox_cleanHashKey";
            this.checkBox_cleanHashKey.Size = new System.Drawing.Size(71, 17);
            this.checkBox_cleanHashKey.TabIndex = 10;
            this.checkBox_cleanHashKey.Text = "Keep \"#\"";
            this.checkBox_cleanHashKey.UseVisualStyleBackColor = true;
            // 
            // checkBox_cleanBrackets
            // 
            this.checkBox_cleanBrackets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_cleanBrackets.AutoSize = true;
            this.checkBox_cleanBrackets.Location = new System.Drawing.Point(16, 335);
            this.checkBox_cleanBrackets.Name = "checkBox_cleanBrackets";
            this.checkBox_cleanBrackets.Size = new System.Drawing.Size(104, 17);
            this.checkBox_cleanBrackets.TabIndex = 9;
            this.checkBox_cleanBrackets.Text = "Keep \"(\" and \")\"";
            this.checkBox_cleanBrackets.UseVisualStyleBackColor = true;
            // 
            // label_cleanheader
            // 
            this.label_cleanheader.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_cleanheader.AutoSize = true;
            this.label_cleanheader.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_cleanheader.Location = new System.Drawing.Point(12, 312);
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
            this.checkBox_cleanSlash.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_cleanSlash.AutoSize = true;
            this.checkBox_cleanSlash.Location = new System.Drawing.Point(16, 427);
            this.checkBox_cleanSlash.Name = "checkBox_cleanSlash";
            this.checkBox_cleanSlash.Size = new System.Drawing.Size(69, 17);
            this.checkBox_cleanSlash.TabIndex = 13;
            this.checkBox_cleanSlash.Text = "Keep \"/\"";
            this.checkBox_cleanSlash.UseVisualStyleBackColor = true;
            // 
            // OPT_importOther
            // 
            this.OPT_importOther.AutoSize = true;
            this.OPT_importOther.Location = new System.Drawing.Point(16, 109);
            this.OPT_importOther.Name = "OPT_importOther";
            this.OPT_importOther.Size = new System.Drawing.Size(301, 30);
            this.OPT_importOther.TabIndex = 14;
            this.OPT_importOther.Text = "Import additional phone numbers if a home or work number\r\nfield remains empty. Source: Outlook:Other.\r\n";
            this.OPT_importOther.UseVisualStyleBackColor = true;
            // 
            // OPT_DUPren
            // 
            this.OPT_DUPren.AutoSize = true;
            this.OPT_DUPren.Location = new System.Drawing.Point(16, 144);
            this.OPT_DUPren.Name = "OPT_DUPren";
            this.OPT_DUPren.Size = new System.Drawing.Size(296, 17);
            this.OPT_DUPren.TabIndex = 24;
            this.OPT_DUPren.Text = "Allow import of duplicate names by numbering them 00-99";
            this.OPT_DUPren.UseVisualStyleBackColor = true;
            // 
            // checkBox_checkVersion
            // 
            this.checkBox_checkVersion.AutoSize = true;
            this.checkBox_checkVersion.Location = new System.Drawing.Point(16, 167);
            this.checkBox_checkVersion.Name = "checkBox_checkVersion";
            this.checkBox_checkVersion.Size = new System.Drawing.Size(182, 17);
            this.checkBox_checkVersion.TabIndex = 25;
            this.checkBox_checkVersion.Text = "Check for new version on startup";
            this.checkBox_checkVersion.UseVisualStyleBackColor = true;
            // 
            // checkBox_cleanSpace
            // 
            this.checkBox_cleanSpace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_cleanSpace.AutoSize = true;
            this.checkBox_cleanSpace.Location = new System.Drawing.Point(16, 450);
            this.checkBox_cleanSpace.Name = "checkBox_cleanSpace";
            this.checkBox_cleanSpace.Size = new System.Drawing.Size(158, 17);
            this.checkBox_cleanSpace.TabIndex = 26;
            this.checkBox_cleanSpace.Text = "Keep \" \" (space characters)";
            this.checkBox_cleanSpace.UseVisualStyleBackColor = true;
            // 
            // checkBox_cleanSquareBrackets
            // 
            this.checkBox_cleanSquareBrackets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_cleanSquareBrackets.AutoSize = true;
            this.checkBox_cleanSquareBrackets.Location = new System.Drawing.Point(16, 473);
            this.checkBox_cleanSquareBrackets.Name = "checkBox_cleanSquareBrackets";
            this.checkBox_cleanSquareBrackets.Size = new System.Drawing.Size(104, 17);
            this.checkBox_cleanSquareBrackets.TabIndex = 27;
            this.checkBox_cleanSquareBrackets.Text = "Keep \"[\" and \"]\"";
            this.checkBox_cleanSquareBrackets.UseVisualStyleBackColor = true;
            // 
            // checkBox_cleanLetters
            // 
            this.checkBox_cleanLetters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_cleanLetters.AutoSize = true;
            this.checkBox_cleanLetters.Location = new System.Drawing.Point(16, 496);
            this.checkBox_cleanLetters.Name = "checkBox_cleanLetters";
            this.checkBox_cleanLetters.Size = new System.Drawing.Size(129, 17);
            this.checkBox_cleanLetters.TabIndex = 28;
            this.checkBox_cleanLetters.Text = "Keep \"a-z\" and \"A-Z\"";
            this.checkBox_cleanLetters.UseVisualStyleBackColor = true;
            // 
            // checkBox_cleanaddzeroprefix
            // 
            this.checkBox_cleanaddzeroprefix.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_cleanaddzeroprefix.AutoSize = true;
            this.checkBox_cleanaddzeroprefix.Location = new System.Drawing.Point(16, 519);
            this.checkBox_cleanaddzeroprefix.Name = "checkBox_cleanaddzeroprefix";
            this.checkBox_cleanaddzeroprefix.Size = new System.Drawing.Size(232, 17);
            this.checkBox_cleanaddzeroprefix.TabIndex = 30;
            this.checkBox_cleanaddzeroprefix.Text = "Alway add \"0\" prefix to access outside lines";
            this.checkBox_cleanaddzeroprefix.UseVisualStyleBackColor = true;
            // 
            // label_export
            // 
            this.label_export.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_export.AutoSize = true;
            this.label_export.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_export.Location = new System.Drawing.Point(12, 546);
            this.label_export.Name = "label_export";
            this.label_export.Size = new System.Drawing.Size(65, 20);
            this.label_export.TabIndex = 36;
            this.label_export.Text = "Export:";
            // 
            // checkBox_multiNumbers
            // 
            this.checkBox_multiNumbers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_multiNumbers.AutoSize = true;
            this.checkBox_multiNumbers.Location = new System.Drawing.Point(16, 569);
            this.checkBox_multiNumbers.Name = "checkBox_multiNumbers";
            this.checkBox_multiNumbers.Size = new System.Drawing.Size(320, 30);
            this.checkBox_multiNumbers.TabIndex = 37;
            this.checkBox_multiNumbers.Text = "Export several numbers of the same type (e.g. 2 mobile\r\nnumbers) where supported: vCard, Fritz!Box XML";
            this.checkBox_multiNumbers.UseVisualStyleBackColor = true;
            // 
            // label_Outlook
            // 
            this.label_Outlook.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label_Outlook.AutoSize = true;
            this.label_Outlook.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Outlook.Location = new System.Drawing.Point(12, 192);
            this.label_Outlook.Name = "label_Outlook";
            this.label_Outlook.Size = new System.Drawing.Size(242, 20);
            this.label_Outlook.TabIndex = 31;
            this.label_Outlook.Text = "Import (all sources)";
            // 
            // checkBox_OutlookEmptyNumber
            // 
            this.checkBox_OutlookEmptyNumber.AutoSize = true;
            this.checkBox_OutlookEmptyNumber.Location = new System.Drawing.Point(16, 215);
            this.checkBox_OutlookEmptyNumber.Name = "checkBox_OutlookEmptyNumber";
            this.checkBox_OutlookEmptyNumber.Size = new System.Drawing.Size(213, 17);
            this.checkBox_OutlookEmptyNumber.TabIndex = 32;
            this.checkBox_OutlookEmptyNumber.Text = "Import contacts without phone number";
            this.checkBox_OutlookEmptyNumber.UseVisualStyleBackColor = true;
            // 
            // checkBox_OutlookEmptyName
            // 
            this.checkBox_OutlookEmptyName.AutoSize = true;
            this.checkBox_OutlookEmptyName.Location = new System.Drawing.Point(16, 238);
            this.checkBox_OutlookEmptyName.Name = "checkBox_OutlookEmptyName";
            this.checkBox_OutlookEmptyName.Size = new System.Drawing.Size(204, 17);
            this.checkBox_OutlookEmptyName.TabIndex = 33;
            this.checkBox_OutlookEmptyName.Text = "Import contacts without name";
            this.checkBox_OutlookEmptyName.UseVisualStyleBackColor = true;
            // 
            // label_Nextcloud
            // 
            this.label_Nextcloud.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label_Nextcloud.AutoSize = true;
            this.label_Nextcloud.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Nextcloud.Location = new System.Drawing.Point(12, 263);
            this.label_Nextcloud.Name = "label_Nextcloud";
            this.label_Nextcloud.Size = new System.Drawing.Size(259, 20);
            this.label_Nextcloud.TabIndex = 34;
            this.label_Nextcloud.Text = "Nextcloud related configuration";
            // 
            // checkBox_nextcloud_vfc_fix
            // 
            this.checkBox_nextcloud_vfc_fix.AutoSize = true;
            this.checkBox_nextcloud_vfc_fix.Location = new System.Drawing.Point(16, 286);
            this.checkBox_nextcloud_vfc_fix.Name = "checkBox_nextcloud_vfc_fix";
            this.checkBox_nextcloud_vfc_fix.Size = new System.Drawing.Size(208, 17);
            this.checkBox_nextcloud_vfc_fix.TabIndex = 35;
            this.checkBox_nextcloud_vfc_fix.Text = "Apply fix for malformatted .vcf contacts";
            this.checkBox_nextcloud_vfc_fix.UseVisualStyleBackColor = true;
            // 
            // EditConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 610);
            this.Controls.Add(this.checkBox_nextcloud_vfc_fix);
            this.Controls.Add(this.label_Nextcloud);
            this.Controls.Add(this.checkBox_OutlookEmptyName);
            this.Controls.Add(this.checkBox_OutlookEmptyNumber);
            this.Controls.Add(this.label_Outlook);
            this.Controls.Add(this.checkBox_multiNumbers);
            this.Controls.Add(this.label_export);
            this.Controls.Add(this.checkBox_cleanaddzeroprefix);
            this.Controls.Add(this.checkBox_cleanLetters);
            this.Controls.Add(this.checkBox_cleanSquareBrackets);
            this.Controls.Add(this.checkBox_cleanSpace);
            this.Controls.Add(this.checkBox_checkVersion);
            this.Controls.Add(this.OPT_DUPren);
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
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
        private System.Windows.Forms.CheckBox OPT_DUPren;
        private System.Windows.Forms.CheckBox checkBox_checkVersion;
        private System.Windows.Forms.CheckBox checkBox_cleanSpace;
        private System.Windows.Forms.CheckBox checkBox_cleanSquareBrackets;
        private System.Windows.Forms.CheckBox checkBox_cleanLetters;
        private System.Windows.Forms.CheckBox checkBox_cleanaddzeroprefix;
        private System.Windows.Forms.Label label_export;
        private System.Windows.Forms.CheckBox checkBox_multiNumbers;
        private System.Windows.Forms.Label label_Outlook;
        private System.Windows.Forms.CheckBox checkBox_OutlookEmptyNumber;
        private System.Windows.Forms.CheckBox checkBox_OutlookEmptyName;
        private System.Windows.Forms.Label label_Nextcloud;
        private System.Windows.Forms.CheckBox checkBox_nextcloud_vfc_fix;
    }
}