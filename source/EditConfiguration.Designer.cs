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
            this.checkBox_cleanXchar.Location = new System.Drawing.Point(16, 230);
            this.checkBox_cleanXchar.Name = "checkBox_cleanXchar";
            this.checkBox_cleanXchar.Size = new System.Drawing.Size(84, 17);
            this.checkBox_cleanXchar.TabIndex = 8;
            this.checkBox_cleanXchar.Text = "Remove \"x\"";
            this.checkBox_cleanXchar.UseVisualStyleBackColor = true;
            // 
            // checkBox_cleanHyphen
            // 
            this.checkBox_cleanHyphen.AutoSize = true;
            this.checkBox_cleanHyphen.Location = new System.Drawing.Point(16, 207);
            this.checkBox_cleanHyphen.Name = "checkBox_cleanHyphen";
            this.checkBox_cleanHyphen.Size = new System.Drawing.Size(82, 17);
            this.checkBox_cleanHyphen.TabIndex = 7;
            this.checkBox_cleanHyphen.Text = "Remove \"-\"";
            this.checkBox_cleanHyphen.UseVisualStyleBackColor = true;
            // 
            // checkBox_cleanHashKey
            // 
            this.checkBox_cleanHashKey.AutoSize = true;
            this.checkBox_cleanHashKey.Location = new System.Drawing.Point(16, 184);
            this.checkBox_cleanHashKey.Name = "checkBox_cleanHashKey";
            this.checkBox_cleanHashKey.Size = new System.Drawing.Size(83, 17);
            this.checkBox_cleanHashKey.TabIndex = 10;
            this.checkBox_cleanHashKey.Text = "Remote \"#\"";
            this.checkBox_cleanHashKey.UseVisualStyleBackColor = true;
            // 
            // checkBox_cleanBrackets
            // 
            this.checkBox_cleanBrackets.AutoSize = true;
            this.checkBox_cleanBrackets.Location = new System.Drawing.Point(16, 161);
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
            this.label_cleanheader.Location = new System.Drawing.Point(13, 136);
            this.label_cleanheader.Name = "label_cleanheader";
            this.label_cleanheader.Size = new System.Drawing.Size(289, 20);
            this.label_cleanheader.TabIndex = 11;
            this.label_cleanheader.Text = "Cleanup phone numbers on export:";
            // 
            // EditConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 262);
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
    }
}