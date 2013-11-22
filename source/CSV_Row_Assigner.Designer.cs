namespace Contact_Conversion_Wizard
{
    partial class CSV_Row_Assigner
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CSV_Row_Assigner));
            this.label1 = new System.Windows.Forms.Label();
            this.combo_Encoding = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.combo_Separator = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.combo_Headers = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label_error = new System.Windows.Forms.Label();
            this.button_regsave = new System.Windows.Forms.Button();
            this.button_regload = new System.Windows.Forms.Button();
            this.label_assign = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(136, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Encoding:";
            // 
            // combo_Encoding
            // 
            this.combo_Encoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_Encoding.FormattingEnabled = true;
            this.combo_Encoding.Items.AddRange(new object[] {
            "Unicode",
            "ISO-8859-1"});
            this.combo_Encoding.Location = new System.Drawing.Point(194, 10);
            this.combo_Encoding.Name = "combo_Encoding";
            this.combo_Encoding.Size = new System.Drawing.Size(121, 21);
            this.combo_Encoding.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(329, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Separator:";
            // 
            // combo_Separator
            // 
            this.combo_Separator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_Separator.FormattingEnabled = true;
            this.combo_Separator.Items.AddRange(new object[] {
            ",",
            ";",
            "TAB"});
            this.combo_Separator.Location = new System.Drawing.Point(389, 10);
            this.combo_Separator.Name = "combo_Separator";
            this.combo_Separator.Size = new System.Drawing.Size(63, 21);
            this.combo_Separator.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(470, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Headers:";
            // 
            // combo_Headers
            // 
            this.combo_Headers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_Headers.FormattingEnabled = true;
            this.combo_Headers.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.combo_Headers.Location = new System.Drawing.Point(522, 10);
            this.combo_Headers.Name = "combo_Headers";
            this.combo_Headers.Size = new System.Drawing.Size(96, 21);
            this.combo_Headers.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(22, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Import Settings =>";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(19, 40);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(878, 25);
            this.label5.TabIndex = 7;
            this.label5.Text = "Please assign the CSV coloumns to the correct data fields, then close the window!" +
    "";
            // 
            // label_error
            // 
            this.label_error.AutoSize = true;
            this.label_error.Location = new System.Drawing.Point(22, 107);
            this.label_error.Name = "label_error";
            this.label_error.Size = new System.Drawing.Size(226, 13);
            this.label_error.TabIndex = 8;
            this.label_error.Text = "Loading the CSV failed with the following error:";
            // 
            // button_regsave
            // 
            this.button_regsave.Location = new System.Drawing.Point(907, 10);
            this.button_regsave.Name = "button_regsave";
            this.button_regsave.Size = new System.Drawing.Size(95, 23);
            this.button_regsave.TabIndex = 9;
            this.button_regsave.Text = "Save To File";
            this.button_regsave.UseVisualStyleBackColor = true;
            this.button_regsave.Click += new System.EventHandler(this.button_regsave_Click);
            // 
            // button_regload
            // 
            this.button_regload.Location = new System.Drawing.Point(808, 10);
            this.button_regload.Name = "button_regload";
            this.button_regload.Size = new System.Drawing.Size(95, 23);
            this.button_regload.TabIndex = 10;
            this.button_regload.Text = "Load From File";
            this.button_regload.UseVisualStyleBackColor = true;
            this.button_regload.Click += new System.EventHandler(this.button_regload_Click);
            // 
            // label_assign
            // 
            this.label_assign.AutoSize = true;
            this.label_assign.Location = new System.Drawing.Point(733, 14);
            this.label_assign.Name = "label_assign";
            this.label_assign.Size = new System.Drawing.Size(69, 13);
            this.label_assign.TabIndex = 11;
            this.label_assign.Text = "Assignments:";
            // 
            // CSV_Row_Assigner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1014, 629);
            this.Controls.Add(this.label_assign);
            this.Controls.Add(this.button_regload);
            this.Controls.Add(this.button_regsave);
            this.Controls.Add(this.label_error);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.combo_Headers);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.combo_Separator);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.combo_Encoding);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "CSV_Row_Assigner";
            this.Text = "CSV Row Assigner";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CSV_Row_Assigner_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox combo_Encoding;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox combo_Separator;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox combo_Headers;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label_error;
        private System.Windows.Forms.Button button_regsave;
        private System.Windows.Forms.Button button_regload;
        private System.Windows.Forms.Label label_assign;


    }
}