namespace Fritz_XML_Wizard
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btn_read_Outlook = new System.Windows.Forms.Button();
            this.btn_read_vCard = new System.Windows.Forms.Button();
            this.btn_save_FritzXML = new System.Windows.Forms.Button();
            this.btn_save_FritzAdress = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.button_clear = new System.Windows.Forms.Button();
            this.panel_right = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.combo_prefix = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.combo_VIP = new System.Windows.Forms.ComboBox();
            this.btn_save_SnomCSV7 = new System.Windows.Forms.Button();
            this.btn_read_FritzAdress = new System.Windows.Forms.Button();
            this.btn_read_FritzXML = new System.Windows.Forms.Button();
            this.btn_save_vCard = new System.Windows.Forms.Button();
            this.btn_save_Outlook = new System.Windows.Forms.Button();
            this.MyDataGridView = new System.Windows.Forms.DataGridView();
            this.label6 = new System.Windows.Forms.Label();
            this.label_save = new System.Windows.Forms.Label();
            this.btn_read_SnomCSV8 = new System.Windows.Forms.Button();
            this.btn_save_SnomCSV8 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.combo_typeprefer = new System.Windows.Forms.ComboBox();
            this.combo_namestyle = new System.Windows.Forms.ComboBox();
            this.combo_outlookimport = new System.Windows.Forms.ComboBox();
            this.panel_left = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.Fullname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MyColLastname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MyColFirstname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MyColCompany = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MyColHome = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MyColWork = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MyColMobile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MyColHomeFax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MyColWorkFax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Street = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ZIP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.City = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eMail = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VIP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel_right.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MyDataGridView)).BeginInit();
            this.panel_left.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_read_Outlook
            // 
            this.btn_read_Outlook.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_read_Outlook.Location = new System.Drawing.Point(4, 71);
            this.btn_read_Outlook.Name = "btn_read_Outlook";
            this.btn_read_Outlook.Size = new System.Drawing.Size(167, 35);
            this.btn_read_Outlook.TabIndex = 0;
            this.btn_read_Outlook.Text = "Outlook";
            this.toolTip1.SetToolTip(this.btn_read_Outlook, "Import Contacts from Outlook. Hold SHIFT to select custom folder!");
            this.btn_read_Outlook.UseVisualStyleBackColor = true;
            this.btn_read_Outlook.Click += new System.EventHandler(this.btn_read_Outlook_Click);
            // 
            // btn_read_vCard
            // 
            this.btn_read_vCard.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_read_vCard.Location = new System.Drawing.Point(4, 153);
            this.btn_read_vCard.Name = "btn_read_vCard";
            this.btn_read_vCard.Size = new System.Drawing.Size(167, 35);
            this.btn_read_vCard.TabIndex = 1;
            this.btn_read_vCard.Text = "vCard";
            this.toolTip1.SetToolTip(this.btn_read_vCard, resources.GetString("btn_read_vCard.ToolTip"));
            this.btn_read_vCard.UseVisualStyleBackColor = true;
            this.btn_read_vCard.Click += new System.EventHandler(this.btn_read_VC_Click);
            // 
            // btn_save_FritzXML
            // 
            this.btn_save_FritzXML.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save_FritzXML.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save_FritzXML.Location = new System.Drawing.Point(836, 112);
            this.btn_save_FritzXML.Name = "btn_save_FritzXML";
            this.btn_save_FritzXML.Size = new System.Drawing.Size(167, 35);
            this.btn_save_FritzXML.TabIndex = 2;
            this.btn_save_FritzXML.Text = "Fritz!Box XML";
            this.toolTip1.SetToolTip(this.btn_save_FritzXML, "Exports contacts to the XML file the Fritz!Box needs when restoring the phonebook" +
                    ".");
            this.btn_save_FritzXML.UseVisualStyleBackColor = true;
            this.btn_save_FritzXML.Click += new System.EventHandler(this.btn_save_FritzXML_Click);
            // 
            // btn_save_FritzAdress
            // 
            this.btn_save_FritzAdress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save_FritzAdress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save_FritzAdress.Location = new System.Drawing.Point(836, 194);
            this.btn_save_FritzAdress.Name = "btn_save_FritzAdress";
            this.btn_save_FritzAdress.Size = new System.Drawing.Size(167, 35);
            this.btn_save_FritzAdress.TabIndex = 3;
            this.btn_save_FritzAdress.Text = "Fritz!adr";
            this.toolTip1.SetToolTip(this.btn_save_FritzAdress, "Export contacts for use in AVM\'s Fritz!Fax & Co programs.\r\nKeep SHIFT pressed to " +
                    "export only contacts that have a fax number.");
            this.btn_save_FritzAdress.UseVisualStyleBackColor = true;
            this.btn_save_FritzAdress.Click += new System.EventHandler(this.btn_save_FritzAdr_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // button_clear
            // 
            this.button_clear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.button_clear.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_clear.Location = new System.Drawing.Point(378, 558);
            this.button_clear.Name = "button_clear";
            this.button_clear.Size = new System.Drawing.Size(250, 69);
            this.button_clear.TabIndex = 6;
            this.button_clear.Text = "Clear List (0)";
            this.button_clear.UseVisualStyleBackColor = true;
            this.button_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // panel_right
            // 
            this.panel_right.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_right.Controls.Add(this.label2);
            this.panel_right.Controls.Add(this.combo_prefix);
            this.panel_right.Location = new System.Drawing.Point(633, 533);
            this.panel_right.Name = "panel_right";
            this.panel_right.Size = new System.Drawing.Size(367, 104);
            this.panel_right.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Output Prefix Removal";
            // 
            // combo_prefix
            // 
            this.combo_prefix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_prefix.FormattingEnabled = true;
            this.combo_prefix.Items.AddRange(new object[] {
            "0049 (DE - Germany)",
            "0043 (AT - Austria)",
            "0041 (CH - Switzerland)",
            "0044 (UK - United Kingdom)",
            "0039 (IT - Italy)",
            "0031 (NL - Netherlands)",
            "0032 (BE - Belgium)",
            "001 (US - United States)",
            "Custom Prefix Code"});
            this.combo_prefix.Location = new System.Drawing.Point(138, 6);
            this.combo_prefix.Name = "combo_prefix";
            this.combo_prefix.Size = new System.Drawing.Size(221, 21);
            this.combo_prefix.TabIndex = 0;
            this.toolTip1.SetToolTip(this.combo_prefix, resources.GetString("combo_prefix.ToolTip"));
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Treat contacts as VIP";
            // 
            // combo_VIP
            // 
            this.combo_VIP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_VIP.FormattingEnabled = true;
            this.combo_VIP.Items.AddRange(new object[] {
            "when hell freezes over (never)",
            "when they have a nickname",
            "when text VIP is in the Comment/Notes"});
            this.combo_VIP.Location = new System.Drawing.Point(132, 78);
            this.combo_VIP.Name = "combo_VIP";
            this.combo_VIP.Size = new System.Drawing.Size(221, 21);
            this.combo_VIP.TabIndex = 3;
            this.toolTip1.SetToolTip(this.combo_VIP, "Allows you to select how the missing VIP functionality in the address \r\nbooks can" +
                    " be mapped to the VIP setting in AVM/Snom Phones\r\n");
            // 
            // btn_save_SnomCSV7
            // 
            this.btn_save_SnomCSV7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save_SnomCSV7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save_SnomCSV7.Location = new System.Drawing.Point(836, 235);
            this.btn_save_SnomCSV7.Name = "btn_save_SnomCSV7";
            this.btn_save_SnomCSV7.Size = new System.Drawing.Size(167, 35);
            this.btn_save_SnomCSV7.TabIndex = 8;
            this.btn_save_SnomCSV7.Text = "Snom CSV v7";
            this.toolTip1.SetToolTip(this.btn_save_SnomCSV7, "Simple CSV Export for Snom v7 Firmware, containing just 2 columns (Name / Number)" +
                    "");
            this.btn_save_SnomCSV7.UseVisualStyleBackColor = true;
            this.btn_save_SnomCSV7.Click += new System.EventHandler(this.btn_save_SnomXMLv7_Click);
            // 
            // btn_read_FritzAdress
            // 
            this.btn_read_FritzAdress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_read_FritzAdress.Location = new System.Drawing.Point(4, 194);
            this.btn_read_FritzAdress.Name = "btn_read_FritzAdress";
            this.btn_read_FritzAdress.Size = new System.Drawing.Size(167, 35);
            this.btn_read_FritzAdress.TabIndex = 9;
            this.btn_read_FritzAdress.Text = "Fritz!adr";
            this.toolTip1.SetToolTip(this.btn_read_FritzAdress, resources.GetString("btn_read_FritzAdress.ToolTip"));
            this.btn_read_FritzAdress.UseVisualStyleBackColor = true;
            this.btn_read_FritzAdress.Click += new System.EventHandler(this.btn_read_FritzAdr_Click);
            // 
            // btn_read_FritzXML
            // 
            this.btn_read_FritzXML.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_read_FritzXML.Location = new System.Drawing.Point(4, 112);
            this.btn_read_FritzXML.Name = "btn_read_FritzXML";
            this.btn_read_FritzXML.Size = new System.Drawing.Size(167, 35);
            this.btn_read_FritzXML.TabIndex = 11;
            this.btn_read_FritzXML.Text = "Fritz!Box XML";
            this.toolTip1.SetToolTip(this.btn_read_FritzXML, "Import contacts from the XML file the Fritz!Box creates when backing up the phone" +
                    "book.");
            this.btn_read_FritzXML.UseVisualStyleBackColor = true;
            this.btn_read_FritzXML.Click += new System.EventHandler(this.btn_read_FritzXML_Click);
            // 
            // btn_save_vCard
            // 
            this.btn_save_vCard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save_vCard.Enabled = false;
            this.btn_save_vCard.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save_vCard.Location = new System.Drawing.Point(836, 153);
            this.btn_save_vCard.Name = "btn_save_vCard";
            this.btn_save_vCard.Size = new System.Drawing.Size(167, 35);
            this.btn_save_vCard.TabIndex = 14;
            this.btn_save_vCard.Text = "vCard";
            this.toolTip1.SetToolTip(this.btn_save_vCard, "Not implemented yet!");
            this.btn_save_vCard.UseVisualStyleBackColor = true;
            this.btn_save_vCard.Click += new System.EventHandler(this.btn_save_vCard_Click);
            // 
            // btn_save_Outlook
            // 
            this.btn_save_Outlook.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save_Outlook.Enabled = false;
            this.btn_save_Outlook.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save_Outlook.Location = new System.Drawing.Point(836, 71);
            this.btn_save_Outlook.Name = "btn_save_Outlook";
            this.btn_save_Outlook.Size = new System.Drawing.Size(167, 35);
            this.btn_save_Outlook.TabIndex = 15;
            this.btn_save_Outlook.Text = "Outlook";
            this.toolTip1.SetToolTip(this.btn_save_Outlook, "Not implemented yet!");
            this.btn_save_Outlook.UseVisualStyleBackColor = true;
            this.btn_save_Outlook.Click += new System.EventHandler(this.btn_save_Outlook_Click);
            // 
            // MyDataGridView
            // 
            this.MyDataGridView.AllowUserToAddRows = false;
            this.MyDataGridView.AllowUserToDeleteRows = false;
            this.MyDataGridView.AllowUserToResizeRows = false;
            this.MyDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MyDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.MyDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Fullname,
            this.MyColLastname,
            this.MyColFirstname,
            this.MyColCompany,
            this.MyColHome,
            this.MyColWork,
            this.MyColMobile,
            this.MyColHomeFax,
            this.MyColWorkFax,
            this.Street,
            this.ZIP,
            this.City,
            this.eMail,
            this.VIP});
            this.MyDataGridView.Location = new System.Drawing.Point(177, 32);
            this.MyDataGridView.MultiSelect = false;
            this.MyDataGridView.Name = "MyDataGridView";
            this.MyDataGridView.RowHeadersVisible = false;
            this.MyDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.MyDataGridView.Size = new System.Drawing.Size(653, 495);
            this.MyDataGridView.TabIndex = 17;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(21, 32);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(136, 20);
            this.label6.TabIndex = 18;
            this.label6.Text = "Load data from:";
            // 
            // label_save
            // 
            this.label_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label_save.AutoSize = true;
            this.label_save.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_save.Location = new System.Drawing.Point(856, 32);
            this.label_save.Name = "label_save";
            this.label_save.Size = new System.Drawing.Size(116, 20);
            this.label_save.TabIndex = 19;
            this.label_save.Text = "Save data to:";
            // 
            // btn_read_SnomCSV8
            // 
            this.btn_read_SnomCSV8.Enabled = false;
            this.btn_read_SnomCSV8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_read_SnomCSV8.Location = new System.Drawing.Point(4, 276);
            this.btn_read_SnomCSV8.Name = "btn_read_SnomCSV8";
            this.btn_read_SnomCSV8.Size = new System.Drawing.Size(167, 35);
            this.btn_read_SnomCSV8.TabIndex = 20;
            this.btn_read_SnomCSV8.Text = "Snom CSV v8";
            this.toolTip1.SetToolTip(this.btn_read_SnomCSV8, "Not implemented yet!");
            this.btn_read_SnomCSV8.UseVisualStyleBackColor = true;
            this.btn_read_SnomCSV8.Click += new System.EventHandler(this.btn_read_SnomXMLv8_Click);
            // 
            // btn_save_SnomCSV8
            // 
            this.btn_save_SnomCSV8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save_SnomCSV8.Enabled = false;
            this.btn_save_SnomCSV8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save_SnomCSV8.Location = new System.Drawing.Point(836, 276);
            this.btn_save_SnomCSV8.Name = "btn_save_SnomCSV8";
            this.btn_save_SnomCSV8.Size = new System.Drawing.Size(167, 35);
            this.btn_save_SnomCSV8.TabIndex = 21;
            this.btn_save_SnomCSV8.Text = "Snom CSV v8";
            this.toolTip1.SetToolTip(this.btn_save_SnomCSV8, "Not implemented yet!");
            this.btn_save_SnomCSV8.UseVisualStyleBackColor = true;
            this.btn_save_SnomCSV8.Click += new System.EventHandler(this.btn_save_SnomXMLv8_Click);
            // 
            // combo_typeprefer
            // 
            this.combo_typeprefer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_typeprefer.FormattingEnabled = true;
            this.combo_typeprefer.Items.AddRange(new object[] {
            "Home Phone / Home Fax / Home Adr.",
            "Work Phone / Work Fax / Work Adr."});
            this.combo_typeprefer.Location = new System.Drawing.Point(132, 28);
            this.combo_typeprefer.Name = "combo_typeprefer";
            this.combo_typeprefer.Size = new System.Drawing.Size(221, 21);
            this.combo_typeprefer.TabIndex = 4;
            this.toolTip1.SetToolTip(this.combo_typeprefer, resources.GetString("combo_typeprefer.ToolTip"));
            // 
            // combo_namestyle
            // 
            this.combo_namestyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_namestyle.FormattingEnabled = true;
            this.combo_namestyle.Items.AddRange(new object[] {
            "Lastname Firstname [Company]",
            "Lastname, Firstname [Company]",
            "Lastname, Firstname, Company",
            "Company [Lastname Firstname]",
            "Company [Lastname, Firstname]",
            "Company, Lastname, Firstname",
            "Firstname Lastname [Company]",
            "Firstname, Lastname [Company]",
            "Firstname, Lastname, Company",
            "Company [Firstname Lastname]",
            "Company [Firstname, Lastname]",
            "Company, Firstname, Lastname",
            "Lastname Firstname",
            "Lastname, Firstname",
            "Firstname Lastname",
            "Firstname, Lastname"});
            this.combo_namestyle.Location = new System.Drawing.Point(132, 2);
            this.combo_namestyle.Name = "combo_namestyle";
            this.combo_namestyle.Size = new System.Drawing.Size(221, 21);
            this.combo_namestyle.TabIndex = 1;
            this.toolTip1.SetToolTip(this.combo_namestyle, resources.GetString("combo_namestyle.ToolTip"));
            // 
            // combo_outlookimport
            // 
            this.combo_outlookimport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_outlookimport.FormattingEnabled = true;
            this.combo_outlookimport.Items.AddRange(new object[] {
            "use generated Combined Name field",
            "use Outlooks SaveAs field instead"});
            this.combo_outlookimport.Location = new System.Drawing.Point(132, 53);
            this.combo_outlookimport.Name = "combo_outlookimport";
            this.combo_outlookimport.Size = new System.Drawing.Size(221, 21);
            this.combo_outlookimport.TabIndex = 6;
            this.toolTip1.SetToolTip(this.combo_outlookimport, resources.GetString("combo_outlookimport.ToolTip"));
            // 
            // panel_left
            // 
            this.panel_left.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panel_left.Controls.Add(this.label5);
            this.panel_left.Controls.Add(this.label4);
            this.panel_left.Controls.Add(this.combo_VIP);
            this.panel_left.Controls.Add(this.combo_outlookimport);
            this.panel_left.Controls.Add(this.label3);
            this.panel_left.Controls.Add(this.combo_typeprefer);
            this.panel_left.Controls.Add(this.label1);
            this.panel_left.Controls.Add(this.combo_namestyle);
            this.panel_left.Location = new System.Drawing.Point(5, 533);
            this.panel_left.Name = "panel_left";
            this.panel_left.Size = new System.Drawing.Size(367, 104);
            this.panel_left.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "On Outlook import";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "If in doubt use/store as";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Combined Name Style";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(5, 513);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(160, 15);
            this.label7.TabIndex = 22;
            this.label7.Text = "Options affecting Import";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(837, 513);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(160, 15);
            this.label8.TabIndex = 23;
            this.label8.Text = "Options affecting Export";
            // 
            // Fullname
            // 
            this.Fullname.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Fullname.FillWeight = 30F;
            this.Fullname.HeaderText = "Combined Name";
            this.Fullname.Name = "Fullname";
            this.Fullname.ReadOnly = true;
            this.Fullname.Width = 101;
            // 
            // MyColLastname
            // 
            this.MyColLastname.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MyColLastname.FillWeight = 20F;
            this.MyColLastname.HeaderText = "Lastname";
            this.MyColLastname.Name = "MyColLastname";
            this.MyColLastname.ReadOnly = true;
            this.MyColLastname.Width = 78;
            // 
            // MyColFirstname
            // 
            this.MyColFirstname.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MyColFirstname.FillWeight = 20F;
            this.MyColFirstname.HeaderText = "Firstname";
            this.MyColFirstname.Name = "MyColFirstname";
            this.MyColFirstname.ReadOnly = true;
            this.MyColFirstname.Width = 77;
            // 
            // MyColCompany
            // 
            this.MyColCompany.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MyColCompany.FillWeight = 20F;
            this.MyColCompany.HeaderText = "Company";
            this.MyColCompany.Name = "MyColCompany";
            this.MyColCompany.ReadOnly = true;
            this.MyColCompany.Width = 76;
            // 
            // MyColHome
            // 
            this.MyColHome.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MyColHome.FillWeight = 15F;
            this.MyColHome.HeaderText = "Home";
            this.MyColHome.Name = "MyColHome";
            this.MyColHome.ReadOnly = true;
            this.MyColHome.Width = 60;
            // 
            // MyColWork
            // 
            this.MyColWork.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MyColWork.FillWeight = 15F;
            this.MyColWork.HeaderText = "Work";
            this.MyColWork.Name = "MyColWork";
            this.MyColWork.ReadOnly = true;
            this.MyColWork.Width = 58;
            // 
            // MyColMobile
            // 
            this.MyColMobile.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MyColMobile.FillWeight = 15F;
            this.MyColMobile.HeaderText = "Mobile";
            this.MyColMobile.Name = "MyColMobile";
            this.MyColMobile.ReadOnly = true;
            this.MyColMobile.Width = 63;
            // 
            // MyColHomeFax
            // 
            this.MyColHomeFax.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MyColHomeFax.FillWeight = 15F;
            this.MyColHomeFax.HeaderText = "HomeFax";
            this.MyColHomeFax.Name = "MyColHomeFax";
            this.MyColHomeFax.ReadOnly = true;
            this.MyColHomeFax.Width = 77;
            // 
            // MyColWorkFax
            // 
            this.MyColWorkFax.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MyColWorkFax.FillWeight = 15F;
            this.MyColWorkFax.HeaderText = "WorkFax";
            this.MyColWorkFax.Name = "MyColWorkFax";
            this.MyColWorkFax.ReadOnly = true;
            this.MyColWorkFax.Width = 75;
            // 
            // Street
            // 
            this.Street.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Street.FillWeight = 15F;
            this.Street.HeaderText = "Street";
            this.Street.Name = "Street";
            this.Street.ReadOnly = true;
            this.Street.Width = 60;
            // 
            // ZIP
            // 
            this.ZIP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ZIP.FillWeight = 7F;
            this.ZIP.HeaderText = "ZIP Code";
            this.ZIP.Name = "ZIP";
            this.ZIP.ReadOnly = true;
            this.ZIP.Width = 71;
            // 
            // City
            // 
            this.City.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.City.FillWeight = 12F;
            this.City.HeaderText = "City";
            this.City.Name = "City";
            this.City.ReadOnly = true;
            this.City.Width = 49;
            // 
            // eMail
            // 
            this.eMail.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.eMail.FillWeight = 20F;
            this.eMail.HeaderText = "eMail";
            this.eMail.Name = "eMail";
            this.eMail.ReadOnly = true;
            this.eMail.Width = 57;
            // 
            // VIP
            // 
            this.VIP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.VIP.FillWeight = 6F;
            this.VIP.HeaderText = "VIP";
            this.VIP.Name = "VIP";
            this.VIP.ReadOnly = true;
            this.VIP.Width = 49;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 639);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.panel_left);
            this.Controls.Add(this.btn_save_SnomCSV8);
            this.Controls.Add(this.btn_read_SnomCSV8);
            this.Controls.Add(this.label_save);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.MyDataGridView);
            this.Controls.Add(this.btn_save_Outlook);
            this.Controls.Add(this.btn_save_vCard);
            this.Controls.Add(this.btn_read_FritzXML);
            this.Controls.Add(this.btn_read_FritzAdress);
            this.Controls.Add(this.btn_save_SnomCSV7);
            this.Controls.Add(this.panel_right);
            this.Controls.Add(this.button_clear);
            this.Controls.Add(this.btn_save_FritzAdress);
            this.Controls.Add(this.btn_save_FritzXML);
            this.Controls.Add(this.btn_read_vCard);
            this.Controls.Add(this.btn_read_Outlook);
            this.MinimumSize = new System.Drawing.Size(1020, 596);
            this.Name = "Form1";
            this.Text = "Contact Conversion Wizard";
            this.panel_right.ResumeLayout(false);
            this.panel_right.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MyDataGridView)).EndInit();
            this.panel_left.ResumeLayout(false);
            this.panel_left.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_read_Outlook;
        private System.Windows.Forms.Button btn_read_vCard;
        private System.Windows.Forms.Button btn_save_FritzXML;
        private System.Windows.Forms.Button btn_save_FritzAdress;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button button_clear;
        private System.Windows.Forms.Panel panel_right;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox combo_prefix;
        private System.Windows.Forms.Button btn_save_SnomCSV7;
        private System.Windows.Forms.Button btn_read_FritzAdress;
        private System.Windows.Forms.Button btn_read_FritzXML;
        private System.Windows.Forms.Button btn_save_vCard;
        private System.Windows.Forms.Button btn_save_Outlook;
        private System.Windows.Forms.DataGridView MyDataGridView;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label_save;
        private System.Windows.Forms.Button btn_read_SnomCSV8;
        private System.Windows.Forms.Button btn_save_SnomCSV8;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel panel_left;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox combo_namestyle;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox combo_typeprefer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox combo_outlookimport;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox combo_VIP;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Fullname;
        private System.Windows.Forms.DataGridViewTextBoxColumn MyColLastname;
        private System.Windows.Forms.DataGridViewTextBoxColumn MyColFirstname;
        private System.Windows.Forms.DataGridViewTextBoxColumn MyColCompany;
        private System.Windows.Forms.DataGridViewTextBoxColumn MyColHome;
        private System.Windows.Forms.DataGridViewTextBoxColumn MyColWork;
        private System.Windows.Forms.DataGridViewTextBoxColumn MyColMobile;
        private System.Windows.Forms.DataGridViewTextBoxColumn MyColHomeFax;
        private System.Windows.Forms.DataGridViewTextBoxColumn MyColWorkFax;
        private System.Windows.Forms.DataGridViewTextBoxColumn Street;
        private System.Windows.Forms.DataGridViewTextBoxColumn ZIP;
        private System.Windows.Forms.DataGridViewTextBoxColumn City;
        private System.Windows.Forms.DataGridViewTextBoxColumn eMail;
        private System.Windows.Forms.DataGridViewTextBoxColumn VIP;
    }
}

