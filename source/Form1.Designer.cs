namespace Contact_Conversion_Wizard
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
            this.button_7270 = new System.Windows.Forms.Button();
            this.button_7390 = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.textBox_PicPath = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.combo_picexport = new System.Windows.Forms.ComboBox();
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
            this.Fullname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MyColLastname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MyColFirstname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MyColCompany = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MyColHome = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MyColWork = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MyColMobile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MyColPreferred = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MyColHomeFax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MyColWorkFax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Street = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ZIP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.City = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eMail = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VIP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Speeddial = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Photo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label6 = new System.Windows.Forms.Label();
            this.label_save = new System.Windows.Forms.Label();
            this.btn_save_SnomCSV8 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btn_save_TalkSurfCSV = new System.Windows.Forms.Button();
            this.btn_read_genericCSV = new System.Windows.Forms.Button();
            this.btn_save_AastraCSV = new System.Windows.Forms.Button();
            this.btn_save_GrandstreamGXV = new System.Windows.Forms.Button();
            this.btn_read_googleContacts = new System.Windows.Forms.Button();
            this.btn_save_GrandstreamGXP = new System.Windows.Forms.Button();
            this.btn_save_Auerswald = new System.Windows.Forms.Button();
            this.btn_save_googleContacts = new System.Windows.Forms.Button();
            this.btn_save_panasonicCSV = new System.Windows.Forms.Button();
            this.btn_save_vCard_Gigaset = new System.Windows.Forms.Button();
            this.combo_typeprefer = new System.Windows.Forms.ComboBox();
            this.combo_namestyle = new System.Windows.Forms.ComboBox();
            this.combo_outlookimport = new System.Windows.Forms.ComboBox();
            this.panel_left = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.button_config = new System.Windows.Forms.Button();
            this.button_website = new System.Windows.Forms.Button();
            this.button_forum = new System.Windows.Forms.Button();
            this.backgroundWorker_updateCheck = new System.ComponentModel.BackgroundWorker();
            this.panel_right.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MyDataGridView)).BeginInit();
            this.panel_left.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_read_Outlook
            // 
            this.btn_read_Outlook.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_read_Outlook.Location = new System.Drawing.Point(4, 32);
            this.btn_read_Outlook.Name = "btn_read_Outlook";
            this.btn_read_Outlook.Size = new System.Drawing.Size(167, 35);
            this.btn_read_Outlook.TabIndex = 0;
            this.btn_read_Outlook.Text = "Outlook";
            this.toolTip1.SetToolTip(this.btn_read_Outlook, "Import Contacts from Outlook.\r\n\r\nHold SHIFT to select custom folder!\r\nHold CTRL t" +
        "o import only Contacts matching a category!\r\n");
            this.btn_read_Outlook.UseVisualStyleBackColor = true;
            this.btn_read_Outlook.Click += new System.EventHandler(this.btn_read_Outlook_Click);
            // 
            // btn_read_vCard
            // 
            this.btn_read_vCard.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_read_vCard.Location = new System.Drawing.Point(4, 108);
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
            this.btn_save_FritzXML.Location = new System.Drawing.Point(836, 70);
            this.btn_save_FritzXML.Name = "btn_save_FritzXML";
            this.btn_save_FritzXML.Size = new System.Drawing.Size(167, 35);
            this.btn_save_FritzXML.TabIndex = 2;
            this.btn_save_FritzXML.Text = "Fritz!Box XML";
            this.toolTip1.SetToolTip(this.btn_save_FritzXML, "Exports contacts to the XML file the Fritz!Box needs when restoring the phonebook" +
        ".\r\n\r\nIf shift is pressed when clicking this button, the fax numbers will also be" +
        " exported to the XML file.");
            this.btn_save_FritzXML.UseVisualStyleBackColor = true;
            this.btn_save_FritzXML.Click += new System.EventHandler(this.btn_save_FritzXML_Click);
            // 
            // btn_save_FritzAdress
            // 
            this.btn_save_FritzAdress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save_FritzAdress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save_FritzAdress.Location = new System.Drawing.Point(836, 146);
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
            this.button_clear.Location = new System.Drawing.Point(378, 476);
            this.button_clear.Name = "button_clear";
            this.button_clear.Size = new System.Drawing.Size(249, 49);
            this.button_clear.TabIndex = 6;
            this.button_clear.Text = "Clear List (0)";
            this.button_clear.UseVisualStyleBackColor = true;
            this.button_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // panel_right
            // 
            this.panel_right.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panel_right.Controls.Add(this.button_7270);
            this.panel_right.Controls.Add(this.button_7390);
            this.panel_right.Controls.Add(this.label10);
            this.panel_right.Controls.Add(this.textBox_PicPath);
            this.panel_right.Controls.Add(this.label9);
            this.panel_right.Controls.Add(this.combo_picexport);
            this.panel_right.Controls.Add(this.label2);
            this.panel_right.Controls.Add(this.combo_prefix);
            this.panel_right.Location = new System.Drawing.Point(633, 476);
            this.panel_right.Name = "panel_right";
            this.panel_right.Size = new System.Drawing.Size(367, 104);
            this.panel_right.TabIndex = 7;
            // 
            // button_7270
            // 
            this.button_7270.Location = new System.Drawing.Point(198, 54);
            this.button_7270.Name = "button_7270";
            this.button_7270.Size = new System.Drawing.Size(66, 19);
            this.button_7270.TabIndex = 8;
            this.button_7270.Text = "USB Stick";
            this.button_7270.UseVisualStyleBackColor = true;
            this.button_7270.Click += new System.EventHandler(this.button_7270_Click);
            // 
            // button_7390
            // 
            this.button_7390.Location = new System.Drawing.Point(267, 54);
            this.button_7390.Name = "button_7390";
            this.button_7390.Size = new System.Drawing.Size(93, 19);
            this.button_7390.TabIndex = 7;
            this.button_7390.Text = "Internal Memory";
            this.button_7390.UseVisualStyleBackColor = true;
            this.button_7390.Click += new System.EventHandler(this.button_7390_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 56);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(175, 13);
            this.label10.TabIndex = 6;
            this.label10.Text = "Fritz!Box path to embedded images:";
            // 
            // textBox_PicPath
            // 
            this.textBox_PicPath.Location = new System.Drawing.Point(15, 73);
            this.textBox_PicPath.Name = "textBox_PicPath";
            this.textBox_PicPath.Size = new System.Drawing.Size(344, 20);
            this.textBox_PicPath.TabIndex = 5;
            this.textBox_PicPath.Text = "file:///var/InternerSpeicher/FRITZ/fonpix-custom/";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 31);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(118, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Export Contact Pictures";
            // 
            // combo_picexport
            // 
            this.combo_picexport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combo_picexport.FormattingEnabled = true;
            this.combo_picexport.Items.AddRange(new object[] {
            "No",
            "Yes"});
            this.combo_picexport.Location = new System.Drawing.Point(138, 27);
            this.combo_picexport.Name = "combo_picexport";
            this.combo_picexport.Size = new System.Drawing.Size(221, 21);
            this.combo_picexport.TabIndex = 3;
            this.toolTip1.SetToolTip(this.combo_picexport, "Please select whether you want to export contact pictures or ignore them\r\n");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 6);
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
            "Custom prefix code",
            "Keep \'+XX\' prefixes intact"});
            this.combo_prefix.Location = new System.Drawing.Point(138, 2);
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
            "never",
            "if contact has a nickname",
            "if text VIP is in the Comment/Notes"});
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
            this.btn_save_SnomCSV7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.btn_save_SnomCSV7.Location = new System.Drawing.Point(836, 184);
            this.btn_save_SnomCSV7.Name = "btn_save_SnomCSV7";
            this.btn_save_SnomCSV7.Size = new System.Drawing.Size(83, 35);
            this.btn_save_SnomCSV7.TabIndex = 8;
            this.btn_save_SnomCSV7.Text = "Snom\r\nCSV v7";
            this.toolTip1.SetToolTip(this.btn_save_SnomCSV7, "Simple CSV Export for Snom v7 Firmware, containing just 2 columns (Name / Number)" +
        "\r\n\r\nFile Format is UTF8 without BOM.\r\n");
            this.btn_save_SnomCSV7.UseVisualStyleBackColor = true;
            this.btn_save_SnomCSV7.Click += new System.EventHandler(this.btn_save_SnomXMLv7_Click);
            // 
            // btn_read_FritzAdress
            // 
            this.btn_read_FritzAdress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_read_FritzAdress.Location = new System.Drawing.Point(4, 146);
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
            this.btn_read_FritzXML.Location = new System.Drawing.Point(4, 70);
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
            this.btn_save_vCard.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save_vCard.Location = new System.Drawing.Point(836, 108);
            this.btn_save_vCard.Name = "btn_save_vCard";
            this.btn_save_vCard.Size = new System.Drawing.Size(83, 35);
            this.btn_save_vCard.TabIndex = 14;
            this.btn_save_vCard.Text = "vCard";
            this.toolTip1.SetToolTip(this.btn_save_vCard, resources.GetString("btn_save_vCard.ToolTip"));
            this.btn_save_vCard.UseVisualStyleBackColor = true;
            this.btn_save_vCard.Click += new System.EventHandler(this.btn_save_vCard_Click);
            // 
            // btn_save_Outlook
            // 
            this.btn_save_Outlook.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save_Outlook.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save_Outlook.Location = new System.Drawing.Point(836, 32);
            this.btn_save_Outlook.Name = "btn_save_Outlook";
            this.btn_save_Outlook.Size = new System.Drawing.Size(167, 35);
            this.btn_save_Outlook.TabIndex = 15;
            this.btn_save_Outlook.Text = "Outlook";
            this.toolTip1.SetToolTip(this.btn_save_Outlook, "Save contacts to an Outlook folder of your choice!");
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
            this.MyColPreferred,
            this.MyColHomeFax,
            this.MyColWorkFax,
            this.Street,
            this.ZIP,
            this.City,
            this.eMail,
            this.VIP,
            this.Speeddial,
            this.Photo});
            this.MyDataGridView.Location = new System.Drawing.Point(177, 32);
            this.MyDataGridView.MultiSelect = false;
            this.MyDataGridView.Name = "MyDataGridView";
            this.MyDataGridView.RowHeadersVisible = false;
            this.MyDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.MyDataGridView.Size = new System.Drawing.Size(653, 438);
            this.MyDataGridView.TabIndex = 17;
            // 
            // Fullname
            // 
            this.Fullname.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Fullname.HeaderText = "Combined Name";
            this.Fullname.Name = "Fullname";
            this.Fullname.ReadOnly = true;
            this.Fullname.Width = 101;
            // 
            // MyColLastname
            // 
            this.MyColLastname.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MyColLastname.HeaderText = "Lastname";
            this.MyColLastname.Name = "MyColLastname";
            this.MyColLastname.ReadOnly = true;
            this.MyColLastname.Width = 78;
            // 
            // MyColFirstname
            // 
            this.MyColFirstname.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MyColFirstname.HeaderText = "Firstname";
            this.MyColFirstname.Name = "MyColFirstname";
            this.MyColFirstname.ReadOnly = true;
            this.MyColFirstname.Width = 77;
            // 
            // MyColCompany
            // 
            this.MyColCompany.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MyColCompany.HeaderText = "Company";
            this.MyColCompany.Name = "MyColCompany";
            this.MyColCompany.ReadOnly = true;
            this.MyColCompany.Width = 76;
            // 
            // MyColHome
            // 
            this.MyColHome.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MyColHome.HeaderText = "Home";
            this.MyColHome.Name = "MyColHome";
            this.MyColHome.ReadOnly = true;
            this.MyColHome.Width = 60;
            // 
            // MyColWork
            // 
            this.MyColWork.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MyColWork.HeaderText = "Work";
            this.MyColWork.Name = "MyColWork";
            this.MyColWork.ReadOnly = true;
            this.MyColWork.Width = 58;
            // 
            // MyColMobile
            // 
            this.MyColMobile.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MyColMobile.HeaderText = "Mobile";
            this.MyColMobile.Name = "MyColMobile";
            this.MyColMobile.ReadOnly = true;
            this.MyColMobile.Width = 63;
            // 
            // MyColPreferred
            // 
            this.MyColPreferred.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MyColPreferred.HeaderText = "Preferred";
            this.MyColPreferred.Name = "MyColPreferred";
            this.MyColPreferred.ReadOnly = true;
            this.MyColPreferred.Width = 75;
            // 
            // MyColHomeFax
            // 
            this.MyColHomeFax.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MyColHomeFax.HeaderText = "HomeFax";
            this.MyColHomeFax.Name = "MyColHomeFax";
            this.MyColHomeFax.ReadOnly = true;
            this.MyColHomeFax.Width = 77;
            // 
            // MyColWorkFax
            // 
            this.MyColWorkFax.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MyColWorkFax.HeaderText = "WorkFax";
            this.MyColWorkFax.Name = "MyColWorkFax";
            this.MyColWorkFax.ReadOnly = true;
            this.MyColWorkFax.Width = 75;
            // 
            // Street
            // 
            this.Street.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Street.HeaderText = "Street";
            this.Street.Name = "Street";
            this.Street.ReadOnly = true;
            this.Street.Width = 60;
            // 
            // ZIP
            // 
            this.ZIP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ZIP.HeaderText = "ZIP Code";
            this.ZIP.Name = "ZIP";
            this.ZIP.ReadOnly = true;
            this.ZIP.Width = 71;
            // 
            // City
            // 
            this.City.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.City.HeaderText = "City";
            this.City.Name = "City";
            this.City.ReadOnly = true;
            this.City.Width = 49;
            // 
            // eMail
            // 
            this.eMail.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.eMail.HeaderText = "eMail";
            this.eMail.Name = "eMail";
            this.eMail.ReadOnly = true;
            this.eMail.Width = 57;
            // 
            // VIP
            // 
            this.VIP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.VIP.HeaderText = "VIP";
            this.VIP.Name = "VIP";
            this.VIP.ReadOnly = true;
            this.VIP.Width = 49;
            // 
            // Speeddial
            // 
            this.Speeddial.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Speeddial.HeaderText = "Speeddial";
            this.Speeddial.Name = "Speeddial";
            this.Speeddial.ReadOnly = true;
            this.Speeddial.Width = 79;
            // 
            // Photo
            // 
            this.Photo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Photo.HeaderText = "Photo";
            this.Photo.Name = "Photo";
            this.Photo.ReadOnly = true;
            this.Photo.Width = 60;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(21, 10);
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
            this.label_save.Location = new System.Drawing.Point(856, 10);
            this.label_save.Name = "label_save";
            this.label_save.Size = new System.Drawing.Size(116, 20);
            this.label_save.TabIndex = 19;
            this.label_save.Text = "Save data to:";
            // 
            // btn_save_SnomCSV8
            // 
            this.btn_save_SnomCSV8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save_SnomCSV8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.btn_save_SnomCSV8.Location = new System.Drawing.Point(920, 184);
            this.btn_save_SnomCSV8.Name = "btn_save_SnomCSV8";
            this.btn_save_SnomCSV8.Size = new System.Drawing.Size(83, 35);
            this.btn_save_SnomCSV8.TabIndex = 21;
            this.btn_save_SnomCSV8.Text = "Snom\r\nCSV v8";
            this.toolTip1.SetToolTip(this.btn_save_SnomCSV8, resources.GetString("btn_save_SnomCSV8.ToolTip"));
            this.btn_save_SnomCSV8.UseVisualStyleBackColor = true;
            this.btn_save_SnomCSV8.Click += new System.EventHandler(this.btn_save_SnomXMLv8_Click);
            // 
            // btn_save_TalkSurfCSV
            // 
            this.btn_save_TalkSurfCSV.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save_TalkSurfCSV.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save_TalkSurfCSV.Location = new System.Drawing.Point(836, 222);
            this.btn_save_TalkSurfCSV.Name = "btn_save_TalkSurfCSV";
            this.btn_save_TalkSurfCSV.Size = new System.Drawing.Size(167, 35);
            this.btn_save_TalkSurfCSV.TabIndex = 24;
            this.btn_save_TalkSurfCSV.Text = "Talk+Surf CSV";
            this.toolTip1.SetToolTip(this.btn_save_TalkSurfCSV, "CSV Export (Non-Unicode) for Gigasets old Talk & Surf 6.0 Software.\r\nExports a sh" +
        "ortened name and the phone numbers.\r\n");
            this.btn_save_TalkSurfCSV.UseVisualStyleBackColor = true;
            this.btn_save_TalkSurfCSV.Click += new System.EventHandler(this.btn_save_TalkSurfCSV_Click);
            // 
            // btn_read_genericCSV
            // 
            this.btn_read_genericCSV.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_read_genericCSV.Location = new System.Drawing.Point(4, 184);
            this.btn_read_genericCSV.Name = "btn_read_genericCSV";
            this.btn_read_genericCSV.Size = new System.Drawing.Size(167, 35);
            this.btn_read_genericCSV.TabIndex = 25;
            this.btn_read_genericCSV.Text = "Generic CSV";
            this.toolTip1.SetToolTip(this.btn_read_genericCSV, "Imports contacts from a generic CSV file with comma separated values.\r\n");
            this.btn_read_genericCSV.UseVisualStyleBackColor = true;
            this.btn_read_genericCSV.Click += new System.EventHandler(this.btn_read_genericCSV_Click);
            // 
            // btn_save_AastraCSV
            // 
            this.btn_save_AastraCSV.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save_AastraCSV.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save_AastraCSV.Location = new System.Drawing.Point(836, 260);
            this.btn_save_AastraCSV.Name = "btn_save_AastraCSV";
            this.btn_save_AastraCSV.Size = new System.Drawing.Size(167, 35);
            this.btn_save_AastraCSV.TabIndex = 26;
            this.btn_save_AastraCSV.Text = "Aastra CSV";
            this.toolTip1.SetToolTip(this.btn_save_AastraCSV, "CSV Export for Aastra Phones\r\n\r\nFile Format is UTF8 without BOM and with Unix Lin" +
        "efeeds.\r\n");
            this.btn_save_AastraCSV.UseVisualStyleBackColor = true;
            this.btn_save_AastraCSV.Click += new System.EventHandler(this.btn_save_AastraCSV_Click);
            // 
            // btn_save_GrandstreamGXV
            // 
            this.btn_save_GrandstreamGXV.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save_GrandstreamGXV.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save_GrandstreamGXV.Location = new System.Drawing.Point(920, 298);
            this.btn_save_GrandstreamGXV.Name = "btn_save_GrandstreamGXV";
            this.btn_save_GrandstreamGXV.Size = new System.Drawing.Size(83, 35);
            this.btn_save_GrandstreamGXV.TabIndex = 28;
            this.btn_save_GrandstreamGXV.Text = "Grandstream\r\nGXV Series";
            this.toolTip1.SetToolTip(this.btn_save_GrandstreamGXV, "XML Export for Grandstream Phones\r\n\r\nFile Format is UTF8 without BOM and with Uni" +
        "x Linefeeds.\r\n");
            this.btn_save_GrandstreamGXV.UseVisualStyleBackColor = true;
            this.btn_save_GrandstreamGXV.Click += new System.EventHandler(this.btn_save_GrandstreamGXV_Click);
            // 
            // btn_read_googleContacts
            // 
            this.btn_read_googleContacts.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_read_googleContacts.Location = new System.Drawing.Point(4, 222);
            this.btn_read_googleContacts.Name = "btn_read_googleContacts";
            this.btn_read_googleContacts.Size = new System.Drawing.Size(167, 35);
            this.btn_read_googleContacts.TabIndex = 29;
            this.btn_read_googleContacts.Text = "Google Contacts";
            this.toolTip1.SetToolTip(this.btn_read_googleContacts, "Directly imports contacts from a Google Mail Account\r\n\r\nHold SHIFT to select the " +
        "groups you want to import from!\r\n");
            this.btn_read_googleContacts.UseVisualStyleBackColor = true;
            this.btn_read_googleContacts.Click += new System.EventHandler(this.btn_read_googleContacts_Click);
            // 
            // btn_save_GrandstreamGXP
            // 
            this.btn_save_GrandstreamGXP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save_GrandstreamGXP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save_GrandstreamGXP.Location = new System.Drawing.Point(836, 298);
            this.btn_save_GrandstreamGXP.Name = "btn_save_GrandstreamGXP";
            this.btn_save_GrandstreamGXP.Size = new System.Drawing.Size(83, 35);
            this.btn_save_GrandstreamGXP.TabIndex = 32;
            this.btn_save_GrandstreamGXP.Text = "Grandstream\r\nGXP Series";
            this.toolTip1.SetToolTip(this.btn_save_GrandstreamGXP, "XML Export for Grandstream Phones\r\n\r\nFile Format is ISO-8859-1 and with Unix Line" +
        "feeds.\r\n");
            this.btn_save_GrandstreamGXP.UseVisualStyleBackColor = true;
            this.btn_save_GrandstreamGXP.Click += new System.EventHandler(this.btn_save_GrandstreamGXP_Click);
            // 
            // btn_save_Auerswald
            // 
            this.btn_save_Auerswald.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save_Auerswald.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save_Auerswald.Location = new System.Drawing.Point(836, 336);
            this.btn_save_Auerswald.Name = "btn_save_Auerswald";
            this.btn_save_Auerswald.Size = new System.Drawing.Size(167, 35);
            this.btn_save_Auerswald.TabIndex = 33;
            this.btn_save_Auerswald.Text = "Auerswald CSV";
            this.toolTip1.SetToolTip(this.btn_save_Auerswald, "CSV Export for Auerswald\r\n\r\nFile Format is Non-Unicode with Unix Linefeeds.\r\n");
            this.btn_save_Auerswald.UseVisualStyleBackColor = true;
            this.btn_save_Auerswald.Click += new System.EventHandler(this.btn_save_Auerswald_Click);
            // 
            // btn_save_googleContacts
            // 
            this.btn_save_googleContacts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save_googleContacts.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save_googleContacts.Location = new System.Drawing.Point(836, 374);
            this.btn_save_googleContacts.Name = "btn_save_googleContacts";
            this.btn_save_googleContacts.Size = new System.Drawing.Size(167, 35);
            this.btn_save_googleContacts.TabIndex = 34;
            this.btn_save_googleContacts.Text = "Google Contacts";
            this.toolTip1.SetToolTip(this.btn_save_googleContacts, "Export for Google Contacts");
            this.btn_save_googleContacts.UseVisualStyleBackColor = true;
            this.btn_save_googleContacts.Click += new System.EventHandler(this.btn_save_googleContacts_Click);
            // 
            // btn_save_panasonicCSV
            // 
            this.btn_save_panasonicCSV.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save_panasonicCSV.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save_panasonicCSV.Location = new System.Drawing.Point(836, 412);
            this.btn_save_panasonicCSV.Name = "btn_save_panasonicCSV";
            this.btn_save_panasonicCSV.Size = new System.Drawing.Size(167, 35);
            this.btn_save_panasonicCSV.TabIndex = 35;
            this.btn_save_panasonicCSV.Text = "Panasonic";
            this.toolTip1.SetToolTip(this.btn_save_panasonicCSV, resources.GetString("btn_save_panasonicCSV.ToolTip"));
            this.btn_save_panasonicCSV.UseVisualStyleBackColor = true;
            this.btn_save_panasonicCSV.Click += new System.EventHandler(this.btn_save_panasonicCSV_Click);
            // 
            // btn_save_vCard_Gigaset
            // 
            this.btn_save_vCard_Gigaset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_save_vCard_Gigaset.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save_vCard_Gigaset.Location = new System.Drawing.Point(920, 108);
            this.btn_save_vCard_Gigaset.Name = "btn_save_vCard_Gigaset";
            this.btn_save_vCard_Gigaset.Size = new System.Drawing.Size(83, 35);
            this.btn_save_vCard_Gigaset.TabIndex = 36;
            this.btn_save_vCard_Gigaset.Text = "Simple vCard\r\nfor Gigaset";
            this.toolTip1.SetToolTip(this.btn_save_vCard_Gigaset, resources.GetString("btn_save_vCard_Gigaset.ToolTip"));
            this.btn_save_vCard_Gigaset.UseVisualStyleBackColor = true;
            this.btn_save_vCard_Gigaset.Click += new System.EventHandler(this.btn_save_vCard_Gigaset_Click);
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
            this.panel_left.Location = new System.Drawing.Point(5, 476);
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
            this.label7.Location = new System.Drawing.Point(5, 456);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(146, 15);
            this.label7.TabIndex = 22;
            this.label7.Text = "Options Import/Export";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(837, 456);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(160, 15);
            this.label8.TabIndex = 23;
            this.label8.Text = "Options affecting Export";
            // 
            // button_config
            // 
            this.button_config.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_config.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_config.Location = new System.Drawing.Point(378, 529);
            this.button_config.Name = "button_config";
            this.button_config.Size = new System.Drawing.Size(135, 49);
            this.button_config.TabIndex = 27;
            this.button_config.Text = "Configuration";
            this.button_config.UseVisualStyleBackColor = true;
            this.button_config.Click += new System.EventHandler(this.button_config_Click);
            // 
            // button_website
            // 
            this.button_website.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_website.Location = new System.Drawing.Point(519, 530);
            this.button_website.Name = "button_website";
            this.button_website.Size = new System.Drawing.Size(108, 23);
            this.button_website.TabIndex = 30;
            this.button_website.Text = "CCW Homepage";
            this.button_website.UseVisualStyleBackColor = true;
            this.button_website.Click += new System.EventHandler(this.button_website_Click);
            // 
            // button_forum
            // 
            this.button_forum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_forum.Location = new System.Drawing.Point(519, 555);
            this.button_forum.Name = "button_forum";
            this.button_forum.Size = new System.Drawing.Size(108, 23);
            this.button_forum.TabIndex = 31;
            this.button_forum.Text = "Report issue";
            this.button_forum.UseVisualStyleBackColor = true;
            this.button_forum.Click += new System.EventHandler(this.button_forum_Click);
            // 
            // backgroundWorker_updateCheck
            // 
            this.backgroundWorker_updateCheck.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_updateCheck_DoWork);
            this.backgroundWorker_updateCheck.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_updateCheck_RunWorkerCompleted);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 582);
            this.Controls.Add(this.btn_save_vCard_Gigaset);
            this.Controls.Add(this.btn_save_panasonicCSV);
            this.Controls.Add(this.btn_save_googleContacts);
            this.Controls.Add(this.btn_save_Auerswald);
            this.Controls.Add(this.btn_save_GrandstreamGXP);
            this.Controls.Add(this.button_forum);
            this.Controls.Add(this.button_website);
            this.Controls.Add(this.btn_read_googleContacts);
            this.Controls.Add(this.btn_save_GrandstreamGXV);
            this.Controls.Add(this.button_config);
            this.Controls.Add(this.btn_save_AastraCSV);
            this.Controls.Add(this.btn_read_genericCSV);
            this.Controls.Add(this.btn_save_TalkSurfCSV);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.panel_left);
            this.Controls.Add(this.btn_save_SnomCSV8);
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
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1020, 620);
            this.Name = "Form1";
            this.Text = "Contact Conversion Wizard";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
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
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox combo_picexport;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBox_PicPath;
        private System.Windows.Forms.Button btn_save_TalkSurfCSV;
        private System.Windows.Forms.Button btn_read_genericCSV;
        private System.Windows.Forms.Button btn_save_AastraCSV;
        private System.Windows.Forms.Button button_config;
        private System.Windows.Forms.Button button_7270;
        private System.Windows.Forms.Button button_7390;
        private System.Windows.Forms.Button btn_save_GrandstreamGXV;
        private System.Windows.Forms.Button btn_read_googleContacts;
        private System.Windows.Forms.DataGridViewTextBoxColumn Fullname;
        private System.Windows.Forms.DataGridViewTextBoxColumn MyColLastname;
        private System.Windows.Forms.DataGridViewTextBoxColumn MyColFirstname;
        private System.Windows.Forms.DataGridViewTextBoxColumn MyColCompany;
        private System.Windows.Forms.DataGridViewTextBoxColumn MyColHome;
        private System.Windows.Forms.DataGridViewTextBoxColumn MyColWork;
        private System.Windows.Forms.DataGridViewTextBoxColumn MyColMobile;
        private System.Windows.Forms.DataGridViewTextBoxColumn MyColPreferred;
        private System.Windows.Forms.DataGridViewTextBoxColumn MyColHomeFax;
        private System.Windows.Forms.DataGridViewTextBoxColumn MyColWorkFax;
        private System.Windows.Forms.DataGridViewTextBoxColumn Street;
        private System.Windows.Forms.DataGridViewTextBoxColumn ZIP;
        private System.Windows.Forms.DataGridViewTextBoxColumn City;
        private System.Windows.Forms.DataGridViewTextBoxColumn eMail;
        private System.Windows.Forms.DataGridViewTextBoxColumn VIP;
        private System.Windows.Forms.DataGridViewTextBoxColumn Speeddial;
        private System.Windows.Forms.DataGridViewTextBoxColumn Photo;
        private System.Windows.Forms.Button button_website;
        private System.Windows.Forms.Button button_forum;
        private System.ComponentModel.BackgroundWorker backgroundWorker_updateCheck;
        private System.Windows.Forms.Button btn_save_GrandstreamGXP;
        private System.Windows.Forms.Button btn_save_Auerswald;
        private System.Windows.Forms.Button btn_save_googleContacts;
        private System.Windows.Forms.Button btn_save_panasonicCSV;
        private System.Windows.Forms.Button btn_save_vCard_Gigaset;
    }
}

