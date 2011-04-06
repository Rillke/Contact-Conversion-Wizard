using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Web;

using Google.GData.Contacts;
using Google.GData.Client;
using Google.GData.Extensions;
using Google.Contacts;


namespace Contact_Conversion_Wizard
{
    public partial class Form1 : Form
    {
        public static bool cfg_hideemptycols = false;
        public static bool cfg_adjustablecols = false;
        public static bool cfg_prefixNONFB = false;
        public static bool cfg_fritzWorkFirst = false;
        public static bool cfg_importOther = true;
        public static bool cfg_OLpics = true;
        public static bool clean_brackets = true;
        public static bool clean_slash = true;
        public static bool clean_hashkey = true;
        public static bool clean_hyphen = true;
        public static bool clean_xchar = true;
        public static string g_login = "";
        public static string g_pass = "";

        System.Collections.Hashtable myGroupDataHash;
        string MySaveFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + System.IO.Path.DirectorySeparatorChar + "ContactConversionWizard";

        public Form1()
        {
            InitializeComponent();
            this.Text = this.ProductName + " v" + Application.ProductVersion;

            if (!System.IO.Directory.Exists(MySaveFolder))
            { System.IO.Directory.CreateDirectory(MySaveFolder); }

            myGroupDataHash = new System.Collections.Hashtable();



            // initialize country selection combobox to the country the windows OS is set to
            switch (System.Globalization.RegionInfo.CurrentRegion.TwoLetterISORegionName)
            {
                case "DE":
                    combo_prefix.SelectedIndex = 0;
                    break;
                case "AT":
                    combo_prefix.SelectedIndex = 1;
                    break;
                case "CH":
                    combo_prefix.SelectedIndex = 2;
                    break;
                case "UK":
                    combo_prefix.SelectedIndex = 3;
                    break;
                case "IT":
                    combo_prefix.SelectedIndex = 4;
                    break;
                case "NL":
                    combo_prefix.SelectedIndex = 5;
                    break;
                case "BE":
                    combo_prefix.SelectedIndex = 6;
                    break;
                default:
                    combo_prefix.SelectedIndex = 7;
                    break;
            }

            // choose the default naming style for the combined fullname
            // this may later be implemented as saving and reading from the Registry
            combo_namestyle.SelectedIndex = 0;
            combo_typeprefer.SelectedIndex = 0;
            combo_outlookimport.SelectedIndex = 0;
            combo_VIP.SelectedIndex = 0;
            combo_picexport.SelectedIndex = 0;

            // override default load configuration (from above) with settings from file (if any)
            myConfig_Load();


        }

        private string CleanUpNumber(string number, string country_prefix, string dial_prefix)
        {

            string return_str = number;

            // clean up white spaces and brackets and some other stuff
            return_str = return_str.Replace(" ", "");

            if (clean_brackets == true)
            {
                return_str = return_str.Replace("(", "");
                return_str = return_str.Replace(")", "");
            }
            if (clean_slash == true)
            {
                return_str = return_str.Replace("/", "");
            }

            // return_str = return_str.Replace("*", ""); ( we are no longer replacing this, since this is actually used by the Fritz!Box for internal numbers)

            if (clean_hashkey == true) { return_str = return_str.Replace("#", ""); }

            // if number is not an eMail address (which cannot contain () or space checked above), we do some further cleanup
            if (return_str.Contains("@") == false)
            {
                if (clean_hyphen == true) { return_str = return_str.Replace("-", ""); }
                if (clean_xchar == true) { return_str = return_str.Replace("x", ""); }

                // clean up country code
                if (country_prefix != "keep")
                {
                    if (return_str.StartsWith("+")) { return_str = "00" + return_str.Substring(1); }
                    if (return_str.StartsWith(country_prefix)) { return_str = "0" + return_str.Substring(country_prefix.Length); }
                }
            }

            if (return_str != "") { return_str = dial_prefix + return_str; }

            return return_str;
        }

        private string RetrieveCountryID(string combo_string)
        {
            // retrieve country_id from combobox, if set to custom ask the user for it
            string my_country_id = combo_string;
            if (my_country_id.StartsWith("00"))
            {
                my_country_id = my_country_id.Substring(0, my_country_id.IndexOf(" "));
            }
            else
            {
                if (my_country_id.StartsWith("Keep"))
                {
                    my_country_id = "keep";
                }
                else
                {
                    // Ask the user for the correct country code
                    SimpleInputDialog MySimpleInputDialog = new SimpleInputDialog("Please enter your local Country Prefix here (in the format 00x or 00xx)", "CustomCountryID", true);
                    MySimpleInputDialog.ShowDialog();
                    my_country_id = MySimpleInputDialog.resultstring;
                    MySimpleInputDialog.Dispose();
                }
            }

            return my_country_id;
        }

        private string CheckVIPflag(string myNickname, string myStringToCheck, bool ignoreComboBox)
        {
            string result_temp = "No"; // be default, VIP is set to no
            int select_mode = combo_VIP.SelectedIndex;
            if (ignoreComboBox == true) { select_mode = 2; }

            switch (select_mode)
            {
                case 0: // never set as VIP, nothing to do, since already the default
                    break;
                case 1: // check if nickname exists, if yes, set to VIP
                    if (myNickname != "") { result_temp = "Yes"; }
                    break;
                case 2: // check notes field, if contains VIP then set to VIP
                    if (myStringToCheck.Contains("VIP") == true) { result_temp = "Yes"; }
                    break;
                default:
                    MessageBox.Show("Default case in CheckVIPflag, this should not have happened, report this bug!");
                    break;
            }

            return result_temp;
        }

        private string CheckPREFIXflag(string myStringToCheck)
        {
            if (myStringToCheck.Contains("PREFIX(") == true) // if something found in comments
            {
                string prefixstring = myStringToCheck.Substring(myStringToCheck.IndexOf("PREFIX(") + 7); // cut away everything before the keyword
                if (prefixstring.Contains(")") == true)
                {
                    prefixstring = prefixstring.Substring(0, prefixstring.IndexOf(")"));                       // cut away everything after the keyword contents
                    return prefixstring;
                }
            }
            // if any of the if's has not been executed we land here and return an empty string
            return "";
        }

        private string CheckSPEEDDIALflag(string myStringToCheck)
        {
            if (myStringToCheck.Contains("SPEEDDIAL(") == true) // if something found in comments
            {
                string speedstring = myStringToCheck.Substring(myStringToCheck.IndexOf("SPEEDDIAL(") + 10); // cut away everything before the keyword
                if (speedstring.Contains(")") == true)
                {
                    speedstring = speedstring.Substring(0, speedstring.IndexOf(")"));                       // cut away everything after the keyword contents

                    string[] partialstrings = speedstring.Split(',');                                       // split the contents

                    if (partialstrings.Length == 1)                           // if 1 or 2 contents, continue
                    {
                        int Num;
                        bool isNum = int.TryParse(speedstring, out Num);
                        if (isNum)
                        {
                            if (Num < 100)
                            { return Num.ToString("00"); }
                            else
                            { return ""; }

                        }
                        else // must be a vanity code without a speeddial number
                        {
                            if (speedstring.Length <= 8)
                            { return "XX," + speedstring; }
                            else // too long to be anything
                            { return ""; }
                        }
                    }

                    if (partialstrings.Length == 2)
                    {
                        int Num;
                        bool isNum = int.TryParse(partialstrings[0], out Num);
                        if (isNum == true && partialstrings[1].Length <= 8)
                        { // all should be ok
                            return Num.ToString("00") + "," + partialstrings[1];
                        }
                        if (partialstrings[0] == "XX" && partialstrings[1].Length <= 8)
                        {  // wildcard entry, all ok here
                            return "XX" + "," + partialstrings[1];
                        }
                    }
                }
            }

            return "";
        }

        private string GenerateFullName(string Firstname, string Lastname, string theCompany, int style)
        {
            string my_fullname = "";

            if (Firstname == "" && Lastname != "" && theCompany == "") { my_fullname = Lastname;                                           /* use only lastname */ }
            if (Firstname != "" && Lastname == "" && theCompany == "") { my_fullname = Firstname;                                          /* use only firstname */ }
            if (Firstname == "" && Lastname == "" && theCompany != "") { my_fullname = theCompany;                                            /* use only company */ }

            switch (style)
            {
                case 0: // Lastname Firstname [Company]
                    if (Firstname != "" && Lastname != "" && theCompany == "") { my_fullname = Lastname + " " + Firstname;                         /* use both names */ }
                    if (Firstname == "" && Lastname != "" && theCompany != "") { my_fullname = Lastname + " [" + theCompany + "]";                    /* use only lastname + company */ }
                    if (Firstname != "" && Lastname == "" && theCompany != "") { my_fullname = Firstname + " [" + theCompany + "]";                   /* use only firstname + company */ }
                    if (Firstname != "" && Lastname != "" && theCompany != "") { my_fullname = Lastname + " " + Firstname + " [" + theCompany + "]";  /* use both names  + company */ }
                    break;
                case 1: // Lastname, Firstname [Company]
                    if (Firstname != "" && Lastname != "" && theCompany == "") { my_fullname = Lastname + ", " + Firstname;                        /* use both names */ }
                    if (Firstname == "" && Lastname != "" && theCompany != "") { my_fullname = Lastname + " [" + theCompany + "]";                    /* use only lastname + company */ }
                    if (Firstname != "" && Lastname == "" && theCompany != "") { my_fullname = Firstname + " [" + theCompany + "]";                   /* use only firstname + company */ }
                    if (Firstname != "" && Lastname != "" && theCompany != "") { my_fullname = Lastname + ", " + Firstname + " [" + theCompany + "]"; /* use both names  + company */ }
                    break;
                case 2: // Lastname, Firstname, Company
                    if (Firstname != "" && Lastname != "" && theCompany == "") { my_fullname = Lastname + ", " + Firstname;                        /* use both names */ }
                    if (Firstname == "" && Lastname != "" && theCompany != "") { my_fullname = Lastname + ", " + theCompany;                          /* use only lastname + company */ }
                    if (Firstname != "" && Lastname == "" && theCompany != "") { my_fullname = Firstname + ", " + theCompany;                         /* use only firstname + company */ }
                    if (Firstname != "" && Lastname != "" && theCompany != "") { my_fullname = Lastname + ", " + Firstname + ", " + theCompany;       /* use both names  + company */ }
                    break;
                case 3: // Company [Lastname Firstname]
                    if (Firstname != "" && Lastname != "" && theCompany == "") { my_fullname = Lastname + " " + Firstname;                         /* use both names */ }
                    if (Firstname == "" && Lastname != "" && theCompany != "") { my_fullname = theCompany + " [" + Lastname + "]";                   /* use only lastname + company */ }
                    if (Firstname != "" && Lastname == "" && theCompany != "") { my_fullname = theCompany + " [" + Firstname + "]";                   /* use only firstname + company */ }
                    if (Firstname != "" && Lastname != "" && theCompany != "") { my_fullname = theCompany + " [" + Lastname + " " + Firstname + "]";  /* use both names  + company */ }
                    break;
                case 4: // Company [Lastname, Firstname]
                    if (Firstname != "" && Lastname != "" && theCompany == "") { my_fullname = Lastname + ", " + Firstname;                        /* use both names */ }
                    if (Firstname == "" && Lastname != "" && theCompany != "") { my_fullname = theCompany + " [" + Lastname + "]";                    /* use only lastname + company */ }
                    if (Firstname != "" && Lastname == "" && theCompany != "") { my_fullname = theCompany + " [" + Firstname + "]";                   /* use only firstname + company */ }
                    if (Firstname != "" && Lastname != "" && theCompany != "") { my_fullname = theCompany + " [" + Lastname + ", " + Firstname + "]"; /* use both names  + company */ }
                    break;
                case 5: // Company, Lastname, Firstname
                    if (Firstname != "" && Lastname != "" && theCompany == "") { my_fullname = Lastname + ", " + Firstname;                        /* use both names */ }
                    if (Firstname == "" && Lastname != "" && theCompany != "") { my_fullname = theCompany + ", " + Lastname;                         /* use only lastname + company */ }
                    if (Firstname != "" && Lastname == "" && theCompany != "") { my_fullname = theCompany + ", " + Firstname;                         /* use only firstname + company */ }
                    if (Firstname != "" && Lastname != "" && theCompany != "") { my_fullname = theCompany + ", " + Lastname + ", " + Firstname;       /* use both names  + company */ }
                    break;
                case 6: // Firstname Lastname [Company]
                    if (Firstname != "" && Lastname != "" && theCompany == "") { my_fullname = Firstname + " " + Lastname;                         /* use both names */ }
                    if (Firstname == "" && Lastname != "" && theCompany != "") { my_fullname = Lastname + " [" + theCompany + "]";                    /* use only lastname + company */ }
                    if (Firstname != "" && Lastname == "" && theCompany != "") { my_fullname = Firstname + " [" + theCompany + "]";                   /* use only firstname + company */ }
                    if (Firstname != "" && Lastname != "" && theCompany != "") { my_fullname = Firstname + " " + Lastname + " [" + theCompany + "]";  /* use both names  + company */ }
                    break;
                case 7: // Firstname, Lastname [Company]
                    if (Firstname != "" && Lastname != "" && theCompany == "") { my_fullname = Firstname + ", " + Lastname;                        /* use both names */ }
                    if (Firstname == "" && Lastname != "" && theCompany != "") { my_fullname = Lastname + " [" + theCompany + "]";                    /* use only lastname + company */ }
                    if (Firstname != "" && Lastname == "" && theCompany != "") { my_fullname = Firstname + " [" + theCompany + "]";                   /* use only firstname + company */ }
                    if (Firstname != "" && Lastname != "" && theCompany != "") { my_fullname = Firstname + ", " + Lastname + " [" + theCompany + "]"; /* use both names  + company */ }
                    break;
                case 8: // Firstname, Lastname, Company
                    if (Firstname != "" && Lastname != "" && theCompany == "") { my_fullname = Firstname + ", " + Lastname;                        /* use both names */ }
                    if (Firstname == "" && Lastname != "" && theCompany != "") { my_fullname = Lastname + ", " + theCompany;                          /* use only lastname + company */ }
                    if (Firstname != "" && Lastname == "" && theCompany != "") { my_fullname = Firstname + ", " + theCompany;                         /* use only firstname + company */ }
                    if (Firstname != "" && Lastname != "" && theCompany != "") { my_fullname = Firstname + ", " + Lastname + ", " + theCompany;       /* use both names  + company */ }
                    break;
                case 9: // Company [Firstname Lastname]
                    if (Firstname != "" && Lastname != "" && theCompany == "") { my_fullname = Firstname + " " + Lastname;                         /* use both names */ }
                    if (Firstname == "" && Lastname != "" && theCompany != "") { my_fullname = theCompany + " [" + Lastname + "]";                    /* use only lastname + company */ }
                    if (Firstname != "" && Lastname == "" && theCompany != "") { my_fullname = theCompany + " [" + Firstname + "]";                   /* use only firstname + company */ }
                    if (Firstname != "" && Lastname != "" && theCompany != "") { my_fullname = theCompany + " [" + Firstname + " " + Lastname + "]";  /* use both names  + company */ }
                    break;
                case 10: // Company [Firstname, Lastname]
                    if (Firstname != "" && Lastname != "" && theCompany == "") { my_fullname = Firstname + ", " + Lastname;                        /* use both names */ }
                    if (Firstname == "" && Lastname != "" && theCompany != "") { my_fullname = theCompany + " [" + Lastname + "]";                    /* use only lastname + company */ }
                    if (Firstname != "" && Lastname == "" && theCompany != "") { my_fullname = theCompany + " [" + Firstname + "]";                   /* use only firstname + company */ }
                    if (Firstname != "" && Lastname != "" && theCompany != "") { my_fullname = theCompany + " [" + Firstname + ", " + Lastname + "]"; /* use both names  + company */ }
                    break;
                case 11: // Company, Firstname, Lastname
                    if (Firstname != "" && Lastname != "" && theCompany == "") { my_fullname = Firstname + ", " + Lastname;                        /* use both names */ }
                    if (Firstname == "" && Lastname != "" && theCompany != "") { my_fullname = theCompany + ", " + Lastname;                          /* use only lastname + company */ }
                    if (Firstname != "" && Lastname == "" && theCompany != "") { my_fullname = theCompany + ", " + Firstname;                         /* use only firstname + company */ }
                    if (Firstname != "" && Lastname != "" && theCompany != "") { my_fullname = theCompany + ", " + Firstname + ", " + Lastname;       /* use both names  + company */ }
                    break;
                case 12: // Lastname Firstname
                    if (Firstname != "" && Lastname != "" && theCompany == "") { my_fullname = Lastname + " " + Firstname;                         /* use both names */ }
                    if (Firstname == "" && Lastname != "" && theCompany != "") { my_fullname = Lastname;                                           /* use only lastname, ignore company */ }
                    if (Firstname != "" && Lastname == "" && theCompany != "") { my_fullname = Firstname;                                          /* use only firstname, ignore company */ }
                    if (Firstname != "" && Lastname != "" && theCompany != "") { my_fullname = Lastname + " " + Firstname;                         /* use both names, ignore company */ }
                    break;
                case 13: // Lastname, Firstname
                    if (Firstname != "" && Lastname != "" && theCompany == "") { my_fullname = Lastname + ", " + Firstname;                        /* use both names */ }
                    if (Firstname == "" && Lastname != "" && theCompany != "") { my_fullname = Lastname;                                           /* use only lastname + company */ }
                    if (Firstname != "" && Lastname == "" && theCompany != "") { my_fullname = Firstname;                                          /* use only firstname, ignore company */ }
                    if (Firstname != "" && Lastname != "" && theCompany != "") { my_fullname = Lastname + ", " + Firstname;                        /* use both names  + company */ }
                    break;
                case 14: // Firstname Lastname
                    if (Firstname != "" && Lastname != "" && theCompany == "") { my_fullname = Firstname + " " + Lastname;                         /* use both names */ }
                    if (Firstname == "" && Lastname != "" && theCompany != "") { my_fullname = Lastname;                                           /* use only lastname, ignore company */ }
                    if (Firstname != "" && Lastname == "" && theCompany != "") { my_fullname = Firstname;                                          /* use only firstname, ignore company */ }
                    if (Firstname != "" && Lastname != "" && theCompany != "") { my_fullname = Firstname + " " + Lastname;                         /* use both names, ignore company */ }
                    break;
                case 15: // Firstname, Lastname
                    if (Firstname != "" && Lastname != "" && theCompany == "") { my_fullname = Firstname + ", " + Lastname;                        /* use both names */ }
                    if (Firstname == "" && Lastname != "" && theCompany != "") { my_fullname = Lastname;                                           /* use only lastname + company */ }
                    if (Firstname != "" && Lastname == "" && theCompany != "") { my_fullname = Firstname;                                          /* use only firstname, ignore company */ }
                    if (Firstname != "" && Lastname != "" && theCompany != "") { my_fullname = Firstname + ", " + Lastname;                        /* use both names  + company */ }
                    break;
                default:
                    MessageBox.Show("Unsupported Output Name Style, this error should not have occured. Please report this!");
                    break;
            }

            return my_fullname;
        }

        private string LimitNameLength(string my_name, int my_limit)
        {
            if (my_name.Length > my_limit) // only do something if the string is not short enough
            {
                // if the last character is a ], then trim the string to one less and add ] again
                if (my_name.Substring(my_name.Length - 1, 1) == "]")
                {
                    my_name = my_name.Substring(0, (my_limit - 1)) + "]";
                }
                else
                // just trim it to the limiting number
                {
                    my_name = my_name.Substring(0, my_limit);
                }
            }

            return my_name;
        }

        private void disable_buttons(bool on_off)
        {
            // while the program is processing, the user must not be able to click any buttons or edit any checkboxes
            if (on_off == true)
            {
                // processing starts, so all buttons will be disabled and the hourglass cursor will be selected
                Cursor.Current = Cursors.WaitCursor;
                btn_read_Outlook.Enabled = false;
                btn_read_FritzXML.Enabled = false;
                btn_read_vCard.Enabled = false;
                btn_read_FritzAdress.Enabled = false;
                btn_read_genericCSV.Enabled = false;
                btn_read_googleContacts.Enabled = false;

                btn_save_Outlook.Enabled = false;
                btn_save_FritzXML.Enabled = false;
                btn_save_vCard.Enabled = false;
                btn_save_FritzAdress.Enabled = false;
                btn_save_SnomCSV7.Enabled = false;
                btn_save_SnomCSV8.Enabled = false;
                btn_save_TalkSurfCSV.Enabled = false;
                btn_save_AastraCSV.Enabled = false;
                btn_save_GrandstreamXml.Enabled = false;

                button_clear.Enabled = false;
                button_config.Enabled = false;
            }
            else
            {
                // processing has finished, so all buttons will be re-enabled and the normal cursor will be restored
                btn_read_Outlook.Enabled = true;
                btn_read_FritzXML.Enabled = true;
                btn_read_vCard.Enabled = true;
                btn_read_FritzAdress.Enabled = true;
                btn_read_genericCSV.Enabled = true;
                btn_read_googleContacts.Enabled = true;

                btn_save_Outlook.Enabled = true;
                btn_save_FritzXML.Enabled = true;
                btn_save_vCard.Enabled = true;
                btn_save_FritzAdress.Enabled = true;
                btn_save_SnomCSV7.Enabled = true;
                btn_save_SnomCSV8.Enabled = true;
                btn_save_TalkSurfCSV.Enabled = true;
                btn_save_AastraCSV.Enabled = true;
                btn_save_GrandstreamXml.Enabled = true;

                button_clear.Enabled = true;
                button_config.Enabled = true;

                Cursor.Current = Cursors.Default;
            }

        }

        private void add_to_database(System.Collections.Hashtable AllToAdd)
        {
            string mergefailures = "";
            foreach (System.Collections.DictionaryEntry addHash in AllToAdd)
            {
                try
                {
                    myGroupDataHash.Add(addHash.Key, addHash.Value);
                }
                catch (ArgumentException) // unable to add to groupdatahash, must mean that something with fullname is already in there!
                { mergefailures += "Entry already in database: " + addHash.Key + Environment.NewLine; }
            }
            if (mergefailures != "") MessageBox.Show(mergefailures, "Unable to merge, because an entry of this name already exists");
        }

        private void update_datagrid()
        {

            MyDataGridView.SuspendLayout();

            MyDataGridView.Rows.Clear();

            // if (cfg_adjustablecols == false)
            {
                MyDataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                MyDataGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                MyDataGridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                MyDataGridView.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                MyDataGridView.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                MyDataGridView.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                MyDataGridView.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                MyDataGridView.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                MyDataGridView.Columns[8].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                MyDataGridView.Columns[9].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                MyDataGridView.Columns[10].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                MyDataGridView.Columns[11].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                MyDataGridView.Columns[12].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                MyDataGridView.Columns[13].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                MyDataGridView.Columns[14].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                MyDataGridView.Columns[15].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            }

            // preparations in case cols should be hidden later
            bool hide_combinedname = true;
            bool hide_lastname = true;
            bool hide_firstname = true;
            bool hide_company = true;
            bool hide_home = true;
            bool hide_work = true;
            bool hide_mobile = true;
            bool hide_homefax = true;
            bool hide_workfax = true;
            bool hide_street = true;
            bool hide_zip = true;
            bool hide_city = true;
            bool hide_email = true;
            bool hide_isVIP = true;
            bool hide_speeddial = true;
            bool hide_PhotoPresent = true;


            foreach (System.Collections.DictionaryEntry contactHash in myGroupDataHash)
            {
                // extract GroupDataList from hashtable contents
                GroupDataContact contactData = (GroupDataContact)contactHash.Value;
                string PhotoPresent = "No";
                if (contactData.jpeg != null) PhotoPresent = "Yes";

                // make sure everything is visible to start with
                MyDataGridView.Columns[0].Visible = true;
                MyDataGridView.Columns[1].Visible = true;
                MyDataGridView.Columns[2].Visible = true;
                MyDataGridView.Columns[3].Visible = true;
                MyDataGridView.Columns[4].Visible = true;
                MyDataGridView.Columns[5].Visible = true;
                MyDataGridView.Columns[6].Visible = true;
                MyDataGridView.Columns[7].Visible = true;
                MyDataGridView.Columns[8].Visible = true;
                MyDataGridView.Columns[9].Visible = true;
                MyDataGridView.Columns[10].Visible = true;
                MyDataGridView.Columns[11].Visible = true;
                MyDataGridView.Columns[12].Visible = true;
                MyDataGridView.Columns[13].Visible = true;
                MyDataGridView.Columns[14].Visible = true;
                MyDataGridView.Columns[15].Visible = true;

                if (cfg_hideemptycols == true)
                {   // for each row, of col still hidden set it to false if relevant data in set for the col
                    if (hide_combinedname == true && contactData.combinedname != "") hide_combinedname = false;
                    if (hide_lastname == true && contactData.lastname != "") hide_lastname = false;
                    if (hide_firstname == true && contactData.firstname != "") hide_firstname = false;
                    if (hide_company == true && contactData.company != "") hide_company = false;
                    if (hide_home == true && contactData.home != "") hide_home = false;
                    if (hide_work == true && contactData.work != "") hide_work = false;
                    if (hide_mobile == true && contactData.mobile != "") hide_mobile = false;
                    if (hide_homefax == true && contactData.homefax != "") hide_homefax = false;
                    if (hide_workfax == true && contactData.workfax != "") hide_workfax = false;
                    if (hide_street == true && contactData.street != "") hide_street = false;
                    if (hide_zip == true && contactData.zip != "") hide_zip = false;
                    if (hide_city == true && contactData.city != "") hide_city = false;
                    if (hide_email == true && contactData.email != "") hide_email = false;
                    if (hide_isVIP == true && contactData.isVIP != "No") hide_isVIP = false;
                    if (hide_speeddial == true && contactData.speeddial != "") hide_speeddial = false;
                    if (hide_PhotoPresent == true && PhotoPresent != "No") hide_PhotoPresent = false;
                }

                MyDataGridView.Rows.Add(new string[] { contactData.combinedname, contactData.lastname, contactData.firstname, contactData.company, contactData.home, contactData.work, contactData.mobile, contactData.homefax, contactData.workfax, contactData.street, contactData.zip, contactData.city, contactData.email, contactData.isVIP, contactData.speeddial, PhotoPresent });
            }

            if (cfg_adjustablecols == true)
            {
                MyDataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                MyDataGridView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                MyDataGridView.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                MyDataGridView.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                MyDataGridView.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                MyDataGridView.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                MyDataGridView.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                MyDataGridView.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                MyDataGridView.Columns[8].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                MyDataGridView.Columns[9].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                MyDataGridView.Columns[10].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                MyDataGridView.Columns[11].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                MyDataGridView.Columns[12].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                MyDataGridView.Columns[13].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                MyDataGridView.Columns[14].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                MyDataGridView.Columns[15].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            if (cfg_hideemptycols == true)
            {   // actually hid the cols depending on results from above
                if (hide_combinedname == true) MyDataGridView.Columns[0].Visible = false;
                if (hide_lastname == true) MyDataGridView.Columns[1].Visible = false;
                if (hide_firstname == true) MyDataGridView.Columns[2].Visible = false;
                if (hide_company == true) MyDataGridView.Columns[3].Visible = false;
                if (hide_home == true) MyDataGridView.Columns[4].Visible = false;
                if (hide_work == true) MyDataGridView.Columns[5].Visible = false;
                if (hide_mobile == true) MyDataGridView.Columns[6].Visible = false;
                if (hide_homefax == true) MyDataGridView.Columns[7].Visible = false;
                if (hide_workfax == true) MyDataGridView.Columns[8].Visible = false;
                if (hide_street == true) MyDataGridView.Columns[9].Visible = false;
                if (hide_zip == true) MyDataGridView.Columns[10].Visible = false;
                if (hide_city == true) MyDataGridView.Columns[11].Visible = false;
                if (hide_email == true) MyDataGridView.Columns[12].Visible = false;
                if (hide_isVIP == true) MyDataGridView.Columns[13].Visible = false;
                if (hide_speeddial == true) MyDataGridView.Columns[14].Visible = false;
                if (hide_PhotoPresent == true) MyDataGridView.Columns[15].Visible = false;
            }


            MyDataGridView.Sort(MyDataGridView.Columns[0], 0);
            MyDataGridView.ResumeLayout();
            MyDataGridView.Refresh();

            button_clear.Text = "Clear List (" + myGroupDataHash.Count.ToString() + ")";
            MyDataGridView.Focus();
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            myGroupDataHash = new System.Collections.Hashtable();
            update_datagrid();
        }

        public bool writeByteArrayToFile(byte[] buff, string fileName)
        {
            string savePath = System.IO.Path.Combine(MySaveFolder, fileName);

            bool response = false;

            try
            {
                System.IO.FileStream fs = new System.IO.FileStream(savePath, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite);
                System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs);
                bw.Write(buff);
                bw.Close();
                response = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error writing to file \"" + savePath + "\":" + ex.Message);
            }

            return response;
        }

        public Microsoft.Office.Interop.Outlook.Attachment GetContactPhoto(Microsoft.Office.Interop.Outlook.ContactItem contact)
        {
            // Find the attchment where PR_ATTACHMENT_CONTACTPHOTO is true
            foreach (Microsoft.Office.Interop.Outlook.Attachment attachment in contact.Attachments)
            {
                try
                {
                    bool isContactPhoto = (bool)attachment.PropertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x7FFF000B");
                    if (isContactPhoto) { return attachment; } // You can then use the Attachment.SaveAsFile method to save the file as a JPEG image.
                }
                catch
                { // do nothing, if somehow attachment processing leads to a crash }
                }
            }
            return null;
        }

        // Section Code for Import Functionality

        private void btn_read_Outlook_Click(object sender, EventArgs e)
        {
            // processing starts, so now we will disable the buttons first to make sure the user knows this by not having buttons to click on
            disable_buttons(true);

            // check whether the user had the shift key pressed while calling this function and store this in a variable for further use
            bool shiftpressed_for_custom_folder = false;
            bool ctrlpressed_for_category_filter = false;

            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) { shiftpressed_for_custom_folder = true; }
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control) { ctrlpressed_for_category_filter = true; }

            ReadDataReturn myReadDataReturn = read_data_Outlook(shiftpressed_for_custom_folder, ctrlpressed_for_category_filter);

            if (myReadDataReturn.duplicates != "") MessageBox.Show(myReadDataReturn.duplicates, "The following duplicate entries could not be imported");
            add_to_database(myReadDataReturn.importedHash);
            update_datagrid();

            disable_buttons(false);
        }    // just click handler

        private void btn_read_FritzXML_Click(object sender, EventArgs e)
        {
            OpenFileDialog Load_Dialog = new OpenFileDialog();
            Load_Dialog.Title = "Select the Fritz!Box XML file you wish to load";
            Load_Dialog.Filter = "XML Files|*.xml";

            if (Load_Dialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            // processing starts, so now we will disable the buttons first to make sure the user knows this by not having buttons to click on
            disable_buttons(true);

            ReadDataReturn myReadDataReturn = read_data_FritzXML(Load_Dialog.FileName);
            if (myReadDataReturn.duplicates != "") MessageBox.Show(myReadDataReturn.duplicates, "The following duplicate entries could not be imported");
            add_to_database(myReadDataReturn.importedHash);
            update_datagrid();

            disable_buttons(false);
        }   // just click handler

        private void btn_read_VC_Click(object sender, EventArgs e)
        {
            // check whether the user had the shift key pressed while calling this function and store this in a variable for further use
            bool shiftpressed_for_custom_folder = false;
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) { shiftpressed_for_custom_folder = true; }

            OpenFileDialog Load_Dialog = new OpenFileDialog();
            Load_Dialog.Title = "Select the vCard file you wish to load";
            Load_Dialog.Filter = "vCard Files|*.vcf";

            if (Load_Dialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            // processing starts, so now we will disable the buttons first to make sure the user knows this by not having buttons to click on
            disable_buttons(true);

            ReadDataReturn myReadDataReturn = read_data_vCard(Load_Dialog.FileName, shiftpressed_for_custom_folder);
            if (myReadDataReturn.duplicates != "") MessageBox.Show(myReadDataReturn.duplicates, "The following duplicate entries could not be imported");
            add_to_database(myReadDataReturn.importedHash);
            update_datagrid();

            disable_buttons(false);
        }         // just click handler

        private void btn_read_FritzAdr_Click(object sender, EventArgs e)
        {
            OpenFileDialog Load_Dialog = new OpenFileDialog();
            Load_Dialog.Title = "Select the Fritz!Adress file you wish to load";
            Load_Dialog.Filter = "Text (Tabstop separated)|*.txt";

            if (Load_Dialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            // processing starts, so now we will disable the buttons first to make sure the user knows this by not having buttons to click on
            disable_buttons(true);

            ReadDataReturn myReadDataReturn = read_data_FritzAdr(Load_Dialog.FileName);
            if (myReadDataReturn.duplicates != "") MessageBox.Show(myReadDataReturn.duplicates, "The following duplicate entries could not be imported");
            add_to_database(myReadDataReturn.importedHash);
            update_datagrid();

            disable_buttons(false);
        }   // just click handler

        private void btn_read_genericCSV_Click(object sender, EventArgs e)
        {
            // check whether the user had the shift key pressed while calling this function and store this in a variable for further use
            OpenFileDialog Load_Dialog = new OpenFileDialog();
            Load_Dialog.Title = "Select the comma separated CSV file you wish to load";
            Load_Dialog.Filter = "CSV (Comma separated)|*.csv";

            if (Load_Dialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            // processing starts, so now we will disable the buttons first to make sure the user knows this by not having buttons to click on
            disable_buttons(true);

            ReadDataReturn myReadDataReturn = read_data_genericCSV(Load_Dialog.FileName);
            if (myReadDataReturn.duplicates != "") MessageBox.Show(myReadDataReturn.duplicates, "The following duplicate entries could not be imported");
            add_to_database(myReadDataReturn.importedHash);
            update_datagrid();

            disable_buttons(false);
        }   // just click handler                       

        private void btn_read_googleContacts_Click(object sender, EventArgs e)
        {
            // processing starts, so now we will disable the buttons first to make sure the user knows this by not having buttons to click on
            disable_buttons(true);

            if (g_login == string.Empty || g_pass == string.Empty)
            {
                MessageBox.Show("Please configure gMail Login and Password in the configuration menu first!");
                disable_buttons(false);
                return;
            }

            ReadDataReturn myReadDataReturn = read_data_googleContacts();

            if (myReadDataReturn.duplicates != "") MessageBox.Show(myReadDataReturn.duplicates, "The following duplicate entries could not be imported");
            add_to_database(myReadDataReturn.importedHash);
            update_datagrid();

            disable_buttons(false);
        }    // just click handler



        private ReadDataReturn read_data_Outlook(bool customfolder, bool categoryfilter)
        {
            // read all information from Outlook and save all contacts that have at least one phone or fax number
            System.Collections.Hashtable loadDataHash = new System.Collections.Hashtable();

            // a string containing the information on what duplicates were encoutered during import
            string duplicates = "";

            // some basic initialization of the outlook support
            Microsoft.Office.Interop.Outlook.Application outlookObj = null;
            Microsoft.Office.Interop.Outlook.MAPIFolder outlookFolder = null;

            // connect to outlook, if possible. Else, complain and abort.
            try
            {
                outlookObj = new Microsoft.Office.Interop.Outlook.Application();
            }
            catch (Exception outlook_exception)
            {
                MessageBox.Show("Unable to access Outlook!" + Environment.NewLine + Environment.NewLine + "This program needs Outlook to continue, are you sure it's installed and working?" + Environment.NewLine + Environment.NewLine + "Error returned was: " + outlook_exception.ToString() + Environment.NewLine);
                return (new ReadDataReturn(duplicates, loadDataHash));
            }

            // sucessfully connected to outlook, do some further setup work
            Microsoft.Office.Interop.Outlook.NameSpace outlookNS = outlookObj.GetNamespace("MAPI");

            // if the user had shift pressed, allow him to select a custom folder. If not, select the default folder.
            if (customfolder == false)
            {
                // select default contacts folder
                outlookFolder = (Microsoft.Office.Interop.Outlook.MAPIFolder)outlookObj.Session.GetDefaultFolder(Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderContacts);
            }
            else
            {
                // force the user to chose a custom folder. If user clicks cancel, just open the folderchooser again until he does select a folder.
                while (outlookFolder == null) { outlookFolder = outlookNS.PickFolder(); }
            }

            string my_category_filter = "";
            if (categoryfilter == true)
            {
                SimpleInputDialog MySimpleInputDialog = new SimpleInputDialog("Please enter the string that must be present in the category field:", "Category Filter", false);
                MySimpleInputDialog.ShowDialog();
                my_category_filter = MySimpleInputDialog.resultstring;
                MySimpleInputDialog.Dispose();
            }

            Microsoft.Office.Interop.Outlook.Items oContactItems = outlookFolder.Items;

            // loop through all contacts and extract the data from them, then generate the [Fullname] and store all stuff in the hashtable
            for (int i = 1; i <= outlookFolder.Items.Count; i++)
            {
                // initialize variable for later use
                Microsoft.Office.Interop.Outlook.ContactItem myContactItem = null;

                // limit items processed to those items which actually are contacts
                try
                {
                    // we try to cast it to a contactitem, if its not a contactitem this will fail and an exception will be raised, leading to a messagebox and skipping of the contact
                    myContactItem = (Microsoft.Office.Interop.Outlook.ContactItem)outlookFolder.Items[i];
                }
                catch
                {
                    // MessageBox.Show("Error while parsing Item #" + i.ToString() + " in Folder. Probably not a Contactitem!" + Environment.NewLine + Environment.NewLine + "The exception that was raised is: "  + e.ToString());
                    // no longer shown, since we probably just want to ignore such things. If we need this again, add (Exception e) above
                    continue;
                }

                // all contacts that were successfully casted now should proably be of type IPM.Contact*, but we check this just to be sure and abort if not
                if (myContactItem.MessageClass.StartsWith("IPM.Contact") == false)
                {
                    MessageBox.Show("Not a valid contact class " + myContactItem.MessageClass);
                    continue;
                }

                // if a category filter has been selected, we now check this first in order to save time (we can skip the contact data transfer if not in the right category)
                string categories = (myContactItem.Categories == null) ? string.Empty : myContactItem.Categories;
                if (my_category_filter != "" && categories.Contains(my_category_filter) == false)
                {
                    continue;   // abort this foreach loop and switch to the next contact
                }

                GroupDataContact myContact = new GroupDataContact();
                myContact.lastname = (myContactItem.LastName == null) ? string.Empty : myContactItem.LastName;
                myContact.firstname = (myContactItem.FirstName == null) ? string.Empty : myContactItem.FirstName;
                myContact.company = (myContactItem.CompanyName == null) ? string.Empty : myContactItem.CompanyName;
                myContact.home = (myContactItem.HomeTelephoneNumber == null) ? string.Empty : myContactItem.HomeTelephoneNumber;
                myContact.work = (myContactItem.BusinessTelephoneNumber == null) ? string.Empty : myContactItem.BusinessTelephoneNumber;
                myContact.homefax = (myContactItem.HomeFaxNumber == null) ? string.Empty : myContactItem.HomeFaxNumber;
                myContact.workfax = (myContactItem.BusinessFaxNumber == null) ? string.Empty : myContactItem.BusinessFaxNumber;
                myContact.mobile = (myContactItem.MobileTelephoneNumber == null) ? string.Empty : myContactItem.MobileTelephoneNumber;
                myContact.preferred = string.Empty; // outlook has no preferred phone
                myContact.street = (myContactItem.MailingAddressStreet == null) ? string.Empty : myContactItem.MailingAddressStreet;
                myContact.street = myContact.street.Replace(Environment.NewLine, " - ");
                myContact.zip = (myContactItem.MailingAddressPostalCode == null) ? string.Empty : myContactItem.MailingAddressPostalCode;
                myContact.city = (myContactItem.MailingAddressCity == null) ? string.Empty : myContactItem.MailingAddressCity;
                myContact.email = (myContactItem.Email1Address == null) ? string.Empty : myContactItem.Email1Address;


                if (cfg_importOther == true && !(string.IsNullOrEmpty(myContactItem.OtherTelephoneNumber)))
                { // if we are to import the other number and it is NOT null or empty, then

                    if (combo_typeprefer.SelectedIndex == 0)
                    { // prefer storing as home
                        if (myContact.home == string.Empty) { myContact.home = myContactItem.OtherTelephoneNumber; }
                        else if (myContact.work == string.Empty) { myContact.work = myContactItem.OtherTelephoneNumber; }
                    }     
                    if (combo_typeprefer.SelectedIndex == 1)
                    { // prefer storing as work
                        if (myContact.work == string.Empty) { myContact.work = myContactItem.OtherTelephoneNumber; }
                        else if (myContact.home == string.Empty) { myContact.home = myContactItem.OtherTelephoneNumber; }
                    }
                }

                // store picture in myContact.jpeg, if present:
                if (cfg_OLpics == true && myContactItem.HasPicture == true)
                {
                    Microsoft.Office.Interop.Outlook.Attachment myAttachmentPhoto = GetContactPhoto(myContactItem);
                    if (myAttachmentPhoto != null)
                    {
                        string tempname = System.IO.Path.GetTempFileName();
                        myAttachmentPhoto.SaveAsFile(tempname);
                        byte[] encodedDataAsBytes = System.IO.File.ReadAllBytes(tempname);
                        System.IO.File.Delete(tempname);
                        myContact.jpeg = encodedDataAsBytes;
                    }
                }

                // generate full name from parts or from FileAs field, depending on combobox selection
                switch (combo_outlookimport.SelectedIndex)
                {
                    case 0:
                        myContact.combinedname = GenerateFullName(myContact.firstname, myContact.lastname, myContact.company, combo_namestyle.SelectedIndex);
                        break;
                    case 1:
                        myContact.combinedname = (myContactItem.FileAs == null) ? string.Empty : myContactItem.FileAs;
                        break;
                    default:
                        break;
                }
                // check if contact is supposed to be VIP
                string NotesBody = (myContactItem.Body == null) ? string.Empty : myContactItem.Body;
                string nickname = (myContactItem.NickName == null) ? string.Empty : myContactItem.NickName;

                myContact.isVIP = CheckVIPflag(nickname, NotesBody, false);
                myContact.speeddial = CheckSPEEDDIALflag(NotesBody);
                myContact.FRITZprefix = CheckPREFIXflag(NotesBody);

                if (myContact.combinedname != "" && (myContact.home != string.Empty || myContact.work != string.Empty || myContact.mobile != string.Empty || myContact.homefax != string.Empty || myContact.workfax != string.Empty) && (NotesBody.Contains("CCW-IGNORE") == false))
                {
                    try
                    {
                        loadDataHash.Add(myContact.combinedname, myContact);
                    }
                    catch (ArgumentException) // unable to add to groupdatahash, must mean that something with fullname is already in there!
                    { duplicates += "Duplicate entry in source (Outlook): " + myContact.combinedname + Environment.NewLine; }

                }
            }

            return (new ReadDataReturn(duplicates, loadDataHash));

        }                   // should work fine

        private ReadDataReturn read_data_FritzXML(string filename)
        {
            System.Collections.Hashtable loadDataHash = new System.Collections.Hashtable();
            string duplicates = "";

            GroupDataContact myContact = new GroupDataContact();
            System.IO.StreamReader file1 = new System.IO.StreamReader(filename, Encoding.GetEncoding("ISO-8859-1"));

            try
            {

                System.Xml.XmlReaderSettings xml_settings = new System.Xml.XmlReaderSettings();
                xml_settings.ConformanceLevel = System.Xml.ConformanceLevel.Fragment;
                xml_settings.IgnoreWhitespace = true;
                xml_settings.IgnoreComments = true;
                System.Xml.XmlReader r = System.Xml.XmlReader.Create(file1, xml_settings);

                r.MoveToContent();

                while (r.ReadToFollowing("contact"))                // loop starts here, if we are able to arrive at a new contact
                {
                    myContact = new GroupDataContact();                 // then we first clean out myContact Storage

                    // and then proceed to retrieve the name
                    r.ReadToFollowing("category");                           // we arrive at category enclosure
                    if (r.ReadElementContentAsString() == "1")
                    {
                        myContact.isVIP = "Yes ";
                    }
                    r.ReadToFollowing("realName");                          // we have already arrived at person enclosure due to category, so we proceed to the realname tag
                    myContact.lastname = r.ReadElementContentAsString();    // we read the person enclosure's contents
                    myContact.combinedname = myContact.lastname; // and also save them to the combined name field, because thats the only one we have
                    r.ReadToFollowing("telephony");                         // we go to the phone number section
                    r.ReadToFollowing("number");                            // retrieve the first number

                    // and then retrieve all Elements in the number part
                    while (r.NodeType == System.Xml.XmlNodeType.Element)
                    {
                        string quickdial_item = r.GetAttribute("quickdial");
                        string vanity_item = r.GetAttribute("vanity");
                        if (quickdial_item != null)
                        {
                            if (vanity_item != null)
                            { // both quickdial and vanity are present
                                myContact.speeddial = quickdial_item + "," + vanity_item;
                            }
                            else
                            { // only quickdial is present
                                myContact.speeddial += quickdial_item;
                            }
                        }

                        if (r.GetAttribute("type") == "home")
                        {
                            if (r.GetAttribute("prio") == "1") { myContact.preferred = "home"; }
                            myContact.home = r.ReadElementContentAsString();
                            continue;
                        }
                        if (r.GetAttribute("type") == "work")
                        {
                            if (r.GetAttribute("prio") == "1") { myContact.preferred = "work"; }
                            myContact.work = r.ReadElementContentAsString();
                            continue;
                        }
                        if (r.GetAttribute("type") == "mobile")
                        {
                            if (r.GetAttribute("prio") == "1") { myContact.preferred = "mobile"; }
                            myContact.mobile = r.ReadElementContentAsString();
                            continue;
                        }
                    }

                    // Now all information should be stored in the array, so save it to the hashtable!
                    try
                    {
                        loadDataHash.Add(myContact.lastname, myContact);
                    }
                    catch (ArgumentException)
                    { duplicates += "Duplicate entry in source (Fritz!Box XML file): " + myContact.combinedname + Environment.NewLine; }

                }

            }
            catch (System.Xml.XmlException e)
            {
                MessageBox.Show("error occured: " + e.Message);
            }

            file1.Close();
            return (new ReadDataReturn(duplicates, loadDataHash));
        }                   // should work fine

        private ReadDataReturn read_data_vCard(string filename, bool non_unicode)
        {
            // read all information from the vCard file and save all contacts that have at least one phone or fax number
            System.Collections.Hashtable loadDataHash = new System.Collections.Hashtable();

            string duplicates = "";

            // try
            {
                // do whats necessary to import a VCF File!
                // for further use: spec can be found here: http://www.ietf.org/rfc/rfc2426.txt

                GroupDataContact myContact = new GroupDataContact();
                string vcard_fullname = "";
                string vcard_notes = "";
                string vcard_nickname = "";
                char[] char_split_array = { ';', ',' };

                int address_value_stored = 0;
                int email_value_stored = 0;

                System.IO.StreamReader file1;
                if (non_unicode == false)
                {
                    file1 = new System.IO.StreamReader(filename, Encoding.UTF8);
                }
                else
                {
                    file1 = new System.IO.StreamReader(filename, Encoding.GetEncoding("ISO-8859-1"));
                }

                // first read everything into builder "string"
                string curline;
                StringBuilder builder = new StringBuilder();
                while ((curline = file1.ReadLine()) != null)
                {
                    builder.Append(curline + "\r\n");
                }
                file1.Close();

                // then strip away "\r\n " stuff to unfold everything, and then regEx Split by "\r\n" into array of lines
                string[] vParseLines = System.Text.RegularExpressions.Regex.Split(builder.ToString().Replace("\r\n ", ""), "\r\n");

                foreach (string ParseLine in vParseLines)
                {
                    // replaced escaped ":" characters, they should not be a problem when parsing
                    string vParseLine = ParseLine.Replace("\\:", ":").Replace("\\\\", "\\").Replace("\\;", ",").Replace("\\,", ",");

                    // if line starts with item1, remove this to allow normal processing (apple used this, item2 and up are therefore silently ignored)
                    if (vParseLine.StartsWith("item1.", StringComparison.OrdinalIgnoreCase) == true)
                    { vParseLine = vParseLine.Substring("item1.".Length); }

                    if (vParseLine.StartsWith("BEGIN:VCARD", StringComparison.OrdinalIgnoreCase) == true)
                    { // reset global settings for contact
                        myContact = new GroupDataContact();
                        vcard_fullname = "";
                        vcard_notes = "";
                        vcard_nickname = "";
                        address_value_stored = 0;
                        email_value_stored = 0;
                        continue;
                    }
                    if (vParseLine.StartsWith("VERSION", StringComparison.OrdinalIgnoreCase) == true) { continue; }

                    if (vParseLine.StartsWith("NOTE:", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        // holt den rest der Zeile und speichert es in der vcard_nickname zwischen um am ende verarbeitet zu werden
                        vcard_notes = vParseLine.Substring(5).Trim();
                        continue;
                    }

                    if (vParseLine.StartsWith("N:", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        // this is firstname and lastname, extract those and ignore other stuff in this line
                        myContact.lastname = vParseLine.Substring(2, vParseLine.IndexOf(";") - 2).Trim();
                        myContact.firstname = vParseLine.Substring(vParseLine.IndexOf(";") + 1).Trim();
                        if (myContact.firstname.Contains(";") == true) // this if is neccessary because windows live mail exports without trailing ; chars for unused fields
                        { myContact.firstname = myContact.firstname.Substring(0, myContact.firstname.IndexOf(";")).Trim(); }
                        continue;
                    }

                    if (vParseLine.StartsWith("FN:", StringComparison.OrdinalIgnoreCase) == true)
                    {   // this is last name and first name, save to special storage string
                        vcard_fullname = vParseLine.Substring(3).Trim(); // holt den ganzen namen
                        continue;
                    }

                    if (vParseLine.StartsWith("NICKNAME:", StringComparison.OrdinalIgnoreCase) == true)
                    {   // holt den ganzen nickname aus der zeile und speichert in für verarbeitung am ende
                        vcard_nickname = vParseLine.Substring(9).Trim();
                        continue;
                    }

                    if (vParseLine.StartsWith("ORG:", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        myContact.company = vParseLine.Substring(4).TrimEnd(';'); // if ends with ; (like on macos) remove trailing ;
                        myContact.company = myContact.company.Replace(';', ' ').Trim(); // if consists of multiple business subunits, combine in one field by removing separators
                        continue;
                    }

                    #region Process-TEL-Lines-in-vCard
                    if (vParseLine.StartsWith("TEL;", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        // we are about to process a phone or fax number

                        // trim stuff away that we've already recognized
                        vParseLine = vParseLine.Substring("TEL;".Length);
                        string telnumber = vParseLine.Substring(vParseLine.IndexOf(":") + 1);

                        string types = vParseLine.Substring(0, vParseLine.IndexOf(":"));
                        types = types.ToLower().Replace("type=", "");
                        string[] typearray = types.Split(char_split_array);

                        bool bit_preferred = false;
                        bool bit_fax = false;
                        string bit_type = "";

                        foreach (string type in typearray)
                        {
                            switch (type)
                            {
                                case "home":
                                    bit_type = "home";
                                    break;
                                case "work":
                                    bit_type = "work";
                                    break;
                                case "cell":
                                    bit_type = "mobile";
                                    break;
                                case "fax":
                                    bit_fax = true;
                                    break;
                                case "pref":
                                    bit_preferred = true;
                                    break;
                                default:
                                    // unknown type, just ignore
                                    break;
                            }
                        }

                        if (bit_type == "home" && bit_fax == false) // handle home phone numbers
                        {
                            if (bit_preferred == true)
                            { myContact.home = telnumber; myContact.preferred = bit_type; }
                            else
                            { if (myContact.home == "") { myContact.home = telnumber; } }
                            continue;
                        }


                        if (bit_type == "work" && bit_fax == false) // handle work phone numbers
                        {
                            if (bit_preferred == true)
                            { myContact.work = telnumber; myContact.preferred = bit_type; }
                            else
                            { if (myContact.work == "") { myContact.work = telnumber; } }
                            continue;
                        }

                        if (bit_type == "mobile") // handle mobile phone numbers
                        {
                            if (bit_preferred == true)
                            { myContact.mobile = telnumber; myContact.preferred = bit_type; }
                            else
                            { if (myContact.mobile == "") { myContact.mobile = telnumber; } }
                            continue;
                        }


                        if (bit_type == "home" && bit_fax == true) // handle work phone numbers
                        {
                            if (bit_preferred == true || myContact.homefax == "")
                            { myContact.homefax = telnumber; }
                            continue;
                        }

                        if (bit_type == "work" && bit_fax == true) // handle work phone numbers
                        {
                            if (bit_preferred == true || myContact.workfax == "")
                            { myContact.workfax = telnumber; }
                            continue;
                        }

                    }
                    #endregion

                    #region Process-ADR-Lines-in-vCard
                    if (vParseLine.StartsWith("ADR;", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        // we are about to process a home or work address

                        // trim stuff away that we've already recognized
                        vParseLine = vParseLine.Substring("ADR;".Length);
                        string address = vParseLine.Substring(vParseLine.IndexOf(":") + 1);

                        string types = vParseLine.Substring(0, vParseLine.IndexOf(":"));
                        types = types.ToLower().Replace("type=", "");
                        string[] typearray = types.Split(char_split_array);

                        bool bit_preferred = false;
                        string bit_type = "";

                        foreach (string type in typearray)
                        {
                            switch (type)
                            {
                                case "home":
                                    bit_type = "home";
                                    break;
                                case "work":
                                    bit_type = "work";
                                    break;
                                case "pref":
                                    bit_preferred = true;
                                    break;
                                default:
                                    // unknown type, just ignore
                                    break;
                            }
                        }


                        if (bit_type == "home" || bit_type == "work")
                        {
                            // calculate address value:
                            int address_value = 0;
                            if (combo_typeprefer.SelectedIndex == 0) { if (bit_type == "home") { address_value += 1; } }     // 1 pt. for home address, 0 pt. for work
                            if (combo_typeprefer.SelectedIndex == 1) { if (bit_type == "work") { address_value += 1; } }     // 0 pt. for home address, 1 pt. for work
                            if (bit_preferred == true) { address_value += 2; }      // bonus of 2 for prefferred one

                            vCardAddressParser myParseVC = new vCardAddressParser(address);
                            if ((address_value > address_value_stored) || ((myContact.street == "" && myContact.zip == "" && myContact.city == "") == true))
                            {
                                myContact.street = myParseVC.parsed_street;
                                myContact.zip = myParseVC.parsed_zip;
                                myContact.city = myParseVC.parsed_city;

                                // save what kind of address we have store, so that it can be overwritten in case a better one comes along
                                address_value_stored = address_value;
                            }
                            continue;
                        }
                    }
                    #endregion

                    #region Process-EMAIL-Lines-in-vCard
                    if (vParseLine.StartsWith("EMAIL;", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        // we are about to process a home or work email address

                        // trim stuff away that we've already recognized
                        vParseLine = vParseLine.Substring("EMAIL;".Length);
                        string emailaddress = vParseLine.Substring(vParseLine.IndexOf(":") + 1);

                        string types = vParseLine.Substring(0, vParseLine.IndexOf(":"));
                        types = types.ToLower().Replace("type=", "");
                        string[] typearray = types.Split(';');

                        bool bit_preferred = false;
                        string bit_type = "";

                        foreach (string type in typearray)
                        {
                            switch (type)
                            {
                                case "home":
                                    bit_type = "home";
                                    break;
                                case "work":
                                    bit_type = "work";
                                    break;
                                case "pref":
                                    bit_preferred = true;
                                    break;
                                default:
                                    // unknown type, just ignore
                                    break;
                            }
                        }

                        // calculate email address value:
                        int email_value = 0;
                        if (combo_typeprefer.SelectedIndex == 0) { if (bit_type == "home") { email_value += 1; } }      // 1 pt. for home address, 0 pt. for work
                        if (combo_typeprefer.SelectedIndex == 1) { if (bit_type == "work") { email_value += 1; } }      // 0 pt. for home address, 1 pt. for work
                        if (bit_preferred == true) { email_value += 2; }      // bonus of 2 for prefferred one

                        if ((email_value > email_value_stored) || (myContact.email == ""))  // if we have an email of higher value, or no email so far
                        {
                            myContact.email = emailaddress;

                            // save what kind of address we have store, so that it can be overwritten in case a better one comes along
                            email_value_stored = email_value;
                        }
                        continue;
                    }
                    #endregion

                    if (vParseLine.StartsWith("CATEGORIES", StringComparison.OrdinalIgnoreCase) == true) { continue; }
                    if (vParseLine.StartsWith("X-ABUID", StringComparison.OrdinalIgnoreCase) == true) { continue; }

                    if (vParseLine.StartsWith("PHOTO", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        if (vParseLine.Contains("BASE64:") == true)
                        {
                            string encodedData = vParseLine.Substring(vParseLine.IndexOf(":") + 1).Replace(" ", "");
                            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
                            myContact.jpeg = encodedDataAsBytes;
                        }

                        continue;
                    }

                    if (vParseLine.StartsWith("END:VCARD", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        // übergibt notes und nickname der methode die VIP und Speeddial extrahiert
                        myContact.isVIP = CheckVIPflag(vcard_nickname, vcard_notes, false);
                        myContact.speeddial = CheckSPEEDDIALflag(vcard_notes);
                        myContact.FRITZprefix = CheckPREFIXflag(vcard_notes);

                        // if vcard_fullname is identical to companyname (but not empty), use only company name and do not use N: line (which contains duplicate information)
                        if ((myContact.company == vcard_fullname) && (vcard_fullname != "")) { myContact.firstname = ""; myContact.lastname = ""; }

                        // if all name fields except (Bugfix for exports from jFritz v0.7.3.10, not needed for 0.7.3.33 which properly exports a "N: " field as required)
                        if (myContact.firstname == "" && myContact.lastname == "" && myContact.company == "" && vcard_fullname != "") { myContact.lastname = vcard_fullname; }


                        // generate fullname from parts for hash-ident
                        myContact.combinedname = GenerateFullName(myContact.firstname, myContact.lastname, myContact.company, combo_namestyle.SelectedIndex);


                        // now, if a full name is present, and any of the phone or fax numbers is set, and no CCW-IGNORE is in the comments we proceed trying to add the user
                        if (myContact.combinedname != "" && (myContact.home != string.Empty || myContact.work != string.Empty || myContact.mobile != string.Empty || myContact.homefax != string.Empty || myContact.workfax != string.Empty) && (vcard_notes.Contains("CCW-IGNORE") == false))
                        {
                            try
                            {
                                loadDataHash.Add(myContact.combinedname, myContact);
                            }
                            catch (ArgumentException)
                            { duplicates += "Duplicate entry in source (vCard file): " + myContact.combinedname + Environment.NewLine; }
                        }

                    }
                }

            }
            // catch (Exception vcard_exception)
            // {
            //     MessageBox.Show("Unable to parse given vCard file!" + Environment.NewLine + "Contact Conversion Wizard has been tested with vCard files generated by MacOS X 10.6 \"Address book\" and Google Mail vCard exports" + Environment.NewLine + "If you think this file should have been imported properly, please report a bug!" + Environment.NewLine + Environment.NewLine + "Error returned was: " + vcard_exception.ToString() + Environment.NewLine);
            // }

            return (new ReadDataReturn(duplicates, loadDataHash));
        }     // should work fine

        private ReadDataReturn read_data_FritzAdr(string filename) {
            System.Collections.Hashtable loadDataHash = new System.Collections.Hashtable();
            string duplicates = "";

            GroupDataContact myContact = new GroupDataContact();
            System.IO.StreamReader file1 = new System.IO.StreamReader(filename, Encoding.GetEncoding("ISO-8859-1"));
            string line;
            int linecounter = -1;

            try
            {
                while ((line = file1.ReadLine()) != null)
                // loop starts here, if we are able to arrive at a new contact
                {
                    linecounter++;
                    myContact = new GroupDataContact();                 // then we first clean out myContact Storage
                    string[] cDataArray = line.Split('\t');
                    if (cDataArray.Length != 21)
                    {
                        MessageBox.Show("Unable to parse line " + linecounter + ", found only " + cDataArray.Length.ToString() + " TAB separated segments instead of 21:" + Environment.NewLine + line);
                        continue;
                    }
                    if (cDataArray[0] == "BEZCHNG") // check if it's the header line, which we of course ignore!
                    { continue; }

                    #region Comments: Data stored in the array
                    // 0: "BEZCHNG                          // STUFF we don't use:
                    // 1: FIRMA                             // 20: HOMEPAGE
                    // 2: NAME                              // 17: NOTIZEN
                    // 3: VORNAME                           // 16: TERMMODE
                    // 19: EMAIL                            // 15: TRANSPROT
                    // 5: STRASSE                           // 14: PASSWORT
                    // 6: PLZ                               // 13: BENUTZER
                    // 7: ORT                               // 12: TERMINAL
                    // 9: TELEFON                           // 11: TRANSFER
                    // 10: TELEFAX                          // 8: KOMMENT (ok we use that now)
                    // 18: MOBILFON                         // 4: ABTEILUNG 
                    #endregion

                    myContact.combinedname = cDataArray[0];
                    myContact.company = cDataArray[1];
                    myContact.lastname = cDataArray[2];
                    myContact.firstname = cDataArray[3];
                    myContact.email = cDataArray[19];
                    myContact.street = cDataArray[5];
                    myContact.zip = cDataArray[6];
                    myContact.city = cDataArray[7];

                    // check if contact is supposed to be VIP or has Speeddial Settings
                    myContact.isVIP = CheckVIPflag("", cDataArray[8], false);
                    myContact.speeddial = CheckSPEEDDIALflag(cDataArray[8]);
                    myContact.FRITZprefix = CheckPREFIXflag(cDataArray[8]);

                    // depending on setting, import phone and fax number into home or work fields)
                    int wheretostore = combo_typeprefer.SelectedIndex;
                    // unless combinedname ends on (gesch.) or (privat), the override using that!
                    if (myContact.combinedname.EndsWith(" (privat)") == true)
                    { wheretostore = 0; } // store as home 
                    if (myContact.combinedname.EndsWith(" (gesch.)") == true)
                    { wheretostore = 1; } // store as work 

                    switch (wheretostore)
                    {
                        case 0:
                            myContact.home = cDataArray[9];
                            myContact.homefax = cDataArray[10];
                            break;

                        case 1:
                            myContact.work = cDataArray[9];
                            myContact.workfax = cDataArray[10];
                            break;

                        default:
                            MessageBox.Show("Default case in \"switch (combo_typeprefer.SelectedIndex)\", this should not have happened. Please report this bug!");
                            break;
                    }

                    myContact.mobile = cDataArray[18];

                    // Now all information should be stored in the array, so save it to the hashtable!
                    if (myContact.combinedname != "" && (myContact.home != string.Empty || myContact.work != string.Empty || myContact.mobile != string.Empty || myContact.homefax != string.Empty || myContact.workfax != string.Empty) && (cDataArray[8].Contains("CCW-IGNORE") == false))
                    {
                        try
                        { loadDataHash.Add(myContact.combinedname, myContact); }
                        catch (ArgumentException)
                        { duplicates += "Duplicate entry in source (Fritz!Adr Text with Tabstops file): " + myContact.combinedname + Environment.NewLine; }
                    }
                } // end while loop going through the lines of the file
            }
            catch (Exception e)
            { MessageBox.Show("Error occured while parsing file: " + e.Message); }

            file1.Close();
            return (new ReadDataReturn(duplicates, loadDataHash));
        }                   // should work fine

        private ReadDataReturn read_data_genericCSV(string filename)
        {
            // repare array that will contain the returned data
            System.Collections.Hashtable loadDataHash = new System.Collections.Hashtable();
            string duplicates = "";

            // individual contacts to be stored in the loadDataHash
            GroupDataContact myContact = new GroupDataContact();

            // prepare return arrays for the form
            List<string[]> LineList = new List<string[]>();
            int[] assign_helper = new int[99];

            // actually run the form that fills the return arrays
            CSV_Row_Assigner my_assigner = new CSV_Row_Assigner(filename, ref LineList, ref assign_helper);
            my_assigner.ShowDialog();
            my_assigner.Dispose();

            // actually do some stuff with the returned arrays (if any)
            for (int i = 0; i < LineList.Count; i++)
            {
                myContact = new GroupDataContact();                 // then we first clean out myContact Storage
                myContact.isVIP = "No"; // by default, it's not a VIP unless afterwards changed

                for (int j = 0; j < LineList[0].GetLength(0); j++)
                {
                    if (assign_helper[j] != -1)
                    {
                        switch (assign_helper[j])
                        {
                            case 0: // Lastname
                                myContact.lastname = LineList[i][j];
                                break;
                            case 1: // Firstname
                                myContact.firstname = LineList[i][j];
                                break;
                            case 2: // Company
                                myContact.company = LineList[i][j];
                                break;
                            case 3: // Home Phone Nr.
                                myContact.home = LineList[i][j];
                                break;
                            case 4: // Work Phone Nr.
                                myContact.work = LineList[i][j];
                                break;
                            case 5: // Mobile Phone Nr.
                                myContact.mobile = LineList[i][j];
                                break;
                            case 6: // Home Fax Nr.
                                myContact.homefax = LineList[i][j];
                                break;
                            case 7: // Work Fax Nr.
                                myContact.workfax = LineList[i][j];
                                break;
                            case 8: // Street
                                myContact.street = LineList[i][j];
                                break;
                            case 9: // ZIP Code
                                myContact.zip = LineList[i][j];
                                break;
                            case 10: // City
                                myContact.city = LineList[i][j];
                                break;
                            case 11: // eMail
                                myContact.email = LineList[i][j];
                                break;
                            case 12: // Notes
                                // check if contact is supposed to be VIP or has Speeddial Settings
                                myContact.isVIP = CheckVIPflag("", LineList[i][j], false);
                                myContact.speeddial = CheckSPEEDDIALflag(LineList[i][j]);
                                myContact.FRITZprefix = CheckPREFIXflag(LineList[i][j]);
                                break;
                            default:
                                MessageBox.Show("Default case in switch (assign_helper[j]) - this should not have happened, please report this bug!");
                                break;
                        }
                    }
                }
                // Now all information should be stored in the array, so save it to the hashtable!
                myContact.combinedname = GenerateFullName(myContact.firstname, myContact.lastname, myContact.company, combo_namestyle.SelectedIndex);

                // if we have a combined name and at least one number
                if (myContact.combinedname != "" && (myContact.home != string.Empty || myContact.work != string.Empty || myContact.mobile != string.Empty || myContact.homefax != string.Empty || myContact.workfax != string.Empty))
                {
                    try // we try to save it to the contact database
                    { loadDataHash.Add(myContact.combinedname, myContact); }
                    catch (ArgumentException)
                    { duplicates += "Duplicate entry in source (Fritz!Adr Text with Tabstops file): " + myContact.combinedname + Environment.NewLine; }
                }
            }

            return (new ReadDataReturn(duplicates, loadDataHash));
        }

        private ReadDataReturn read_data_googleContacts()
        {
            System.Collections.Hashtable loadDataHash = new System.Collections.Hashtable();
            string duplicates = "";

            RequestSettings rs = new RequestSettings(this.ProductName + " v" + this.ProductVersion, g_login, g_pass);
            rs.AutoPaging = true; // AutoPaging results in automatic paging in order to retrieve all contacts
            ContactsRequest cr = new ContactsRequest(rs);

            Feed<Contact> f = cr.GetContacts();

            string der_token = cr.Service.QueryClientLoginToken();

            foreach (Contact entry in f.Entries)
            {
                if (entry.Name != null)
                {

                    GroupDataContact myContact = new GroupDataContact();

                    myContact.lastname = (string.IsNullOrEmpty(entry.Name.FamilyName)) ? string.Empty : entry.Name.FamilyName;
                    myContact.firstname = (string.IsNullOrEmpty(entry.Name.GivenName)) ? string.Empty : entry.Name.GivenName;
                    if (entry.Organizations.Count > 0)
                    {
                        myContact.company = (string.IsNullOrEmpty(entry.Organizations[0].Name)) ? string.Empty : entry.Organizations[0].Name;
                    }

                    foreach (PhoneNumber pnr in entry.Phonenumbers)
                    {
                        if (string.IsNullOrEmpty(pnr.Value) == true) { continue; } // if phone number string should be empty, go to next item

                        if (pnr.Rel == ContactsRelationships.IsHome) { if (myContact.home == string.Empty)    myContact.home = pnr.Value; }
                        if (pnr.Rel == ContactsRelationships.IsWork) { if (myContact.work == string.Empty)    myContact.work = pnr.Value; }
                        if (pnr.Rel == ContactsRelationships.IsHomeFax) { if (myContact.homefax == string.Empty) myContact.homefax = pnr.Value; }
                        if (pnr.Rel == ContactsRelationships.IsWorkFax) { if (myContact.workfax == string.Empty) myContact.workfax = pnr.Value; }
                        if (pnr.Rel == ContactsRelationships.IsMobile) { if (myContact.mobile == string.Empty)  myContact.mobile = pnr.Value; }
                    }

                    myContact.preferred = string.Empty; // Google Contacts has no preferred phone (at least not in the GUI, so we don't support it here)



                    if (entry.PostalAddresses.Count > 0)
                    {

                        if (!string.IsNullOrEmpty(entry.PostalAddresses[0].Street))
                        { myContact.street = entry.PostalAddresses[0].Street; }

                        if (!string.IsNullOrEmpty(entry.PostalAddresses[0].Postcode))
                        { myContact.zip = entry.PostalAddresses[0].Postcode; }

                        if (!string.IsNullOrEmpty(entry.PostalAddresses[0].City))
                        { myContact.city = entry.PostalAddresses[0].City; }
                    }

                    if (entry.PrimaryEmail != null)
                    { myContact.email = entry.PrimaryEmail.Address; }

                    if (entry.PhotoEtag != null)
                    {
                        // MessageBox.Show(entry.Name.FullName + " has a photo, now retrieving...");

                        // System.IO.Stream photoStream = cr.GetPhoto(entry);
                        // does not work, because google complains data has already been retrieved, unsure how to proceed

                        // replacement code
                        byte[] myAttachmentPhoto = GooglePhotoGet(entry.ContactEntry, der_token);

                        myContact.jpeg = myAttachmentPhoto;


                    }



                    // all initial processing has finished, do some cleanup work
                    myContact.combinedname = GenerateFullName(myContact.firstname, myContact.lastname, myContact.company, combo_namestyle.SelectedIndex);

                    // check if contact is supposed to be VIP
                    string NotesBody = (string.IsNullOrEmpty(entry.Content)) ? string.Empty : entry.Content;
                    string nickname = (string.IsNullOrEmpty(entry.ContactEntry.Nickname)) ? string.Empty : entry.ContactEntry.Nickname;


                    myContact.isVIP = CheckVIPflag(nickname, NotesBody, false);
                    myContact.speeddial = CheckSPEEDDIALflag(NotesBody);
                    myContact.FRITZprefix = CheckPREFIXflag(NotesBody);

                    if (myContact.combinedname != "" && (myContact.home != string.Empty || myContact.work != string.Empty || myContact.mobile != string.Empty || myContact.homefax != string.Empty || myContact.workfax != string.Empty) && (NotesBody.Contains("CCW-IGNORE") == false))
                    {
                        try
                        {
                            loadDataHash.Add(myContact.combinedname, myContact);
                        }
                        catch (ArgumentException) // unable to add to groupdatahash, must mean that something with fullname is already in there!
                        { duplicates += "Duplicate entry in source (gContacts): " + myContact.combinedname + Environment.NewLine; }

                    }


                }
                else
                    MessageBox.Show("An entry in the google contact database was retrieved that had no name, ignoring.");

            }
            return (new ReadDataReturn(duplicates, loadDataHash));
        }

        // Section Code for Export Functionality

        private void btn_save_Outlook_Click(object sender, EventArgs e)
        {
            disable_buttons(true);

            save_data_Outlook(myGroupDataHash);
            disable_buttons(false);

        }    // just click handler

        private void btn_save_FritzXML_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveXML_Dialog = new SaveFileDialog();
            SaveXML_Dialog.Title = "Select the XML file you wish to create";
            SaveXML_Dialog.DefaultExt = "xml";
            SaveXML_Dialog.Filter = "XML files (*.xml)|*.xml";
            SaveXML_Dialog.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
            SaveXML_Dialog.FileName = "FritzExport";

            if (SaveXML_Dialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            disable_buttons(true);

            save_data_FritzXML(SaveXML_Dialog.FileName, myGroupDataHash);

            // and reenable user interface
            disable_buttons(false);
        }   // just click handler

        private void btn_save_vCard_Click(object sender, EventArgs e)
        {
            // check whether the user had the shift key pressed while calling this function and store this in a variable for further use
            bool shiftpressed_for_only_phone = false;
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) { shiftpressed_for_only_phone = true; }

            SaveFileDialog SaveVCF_Dialog = new SaveFileDialog();
            SaveVCF_Dialog.Title = "Select the vCard file you wish to create";
            SaveVCF_Dialog.DefaultExt = "vcf";
            SaveVCF_Dialog.Filter = "VCF files (*.vcf)|*.vcf";
            SaveVCF_Dialog.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
            SaveVCF_Dialog.FileName = "vCard Export";

            if (SaveVCF_Dialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            disable_buttons(true);

            save_data_vCard(SaveVCF_Dialog.FileName, myGroupDataHash, shiftpressed_for_only_phone);

            // and reenable user interface
            disable_buttons(false);

        }      // just click handler

        private void btn_save_FritzAdr_Click(object sender, EventArgs e)
        {
            // check whether the user had the shift key pressed while calling this function and store this in a variable for further use
            bool shiftpressed_for_fax_only = false;
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) { shiftpressed_for_fax_only = true; }

            SaveFileDialog SaveTXT_Dialog = new SaveFileDialog();
            SaveTXT_Dialog.Title = "Select the TXT file you wish to create";
            SaveTXT_Dialog.DefaultExt = "txt";
            SaveTXT_Dialog.Filter = "TXT files (*.txt)|*.txt";
            SaveTXT_Dialog.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
            SaveTXT_Dialog.FileName = "FritzFax Export";

            if (SaveTXT_Dialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            disable_buttons(true);

            save_data_FritzAdress(SaveTXT_Dialog.FileName, myGroupDataHash, shiftpressed_for_fax_only);

            // and reenable user interface
            disable_buttons(false);
        }   // just click handler

        private void btn_save_SnomXMLv7_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveCSV_Dialog = new SaveFileDialog();
            SaveCSV_Dialog.Title = "Select the Snom v7 CSV file you wish to create";
            SaveCSV_Dialog.DefaultExt = "csv";
            SaveCSV_Dialog.Filter = "CSV files (*.csv)|*.csv";
            SaveCSV_Dialog.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
            SaveCSV_Dialog.FileName = "tbook_v7";

            if (SaveCSV_Dialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            disable_buttons(true);

            save_data_SnomCSV7(SaveCSV_Dialog.FileName, myGroupDataHash);

            // and reenable user interface
            disable_buttons(false);
        }  // just click handler

        private void btn_save_SnomXMLv8_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveCSV_Dialog = new SaveFileDialog();
            SaveCSV_Dialog.Title = "Select the Snom v8 CSV file you wish to create";
            SaveCSV_Dialog.DefaultExt = "csv";
            SaveCSV_Dialog.Filter = "CSV files (*.csv)|*.csv";
            SaveCSV_Dialog.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
            SaveCSV_Dialog.FileName = "tbook_v8";

            if (SaveCSV_Dialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            disable_buttons(true);

            save_data_SnomCSV8(SaveCSV_Dialog.FileName, myGroupDataHash);

            // and reenable user interface
            disable_buttons(false);
        }  // just click handler

        private void btn_save_TalkSurfCSV_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveCSV_Dialog = new SaveFileDialog();
            SaveCSV_Dialog.Title = "Select the Talk&Surf CSV file you wish to create";
            SaveCSV_Dialog.DefaultExt = "csv";
            SaveCSV_Dialog.Filter = "CSV files (*.csv)|*.csv";
            SaveCSV_Dialog.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
            SaveCSV_Dialog.FileName = "export_talksurf";

            if (SaveCSV_Dialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            disable_buttons(true);

            save_data_TalkSurfCSV(SaveCSV_Dialog.FileName, myGroupDataHash);

            // and reenable user interface
            disable_buttons(false);
        }   // just click handler

        private void btn_save_AastraCSV_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveCSV_Dialog = new SaveFileDialog();
            SaveCSV_Dialog.Title = "Select the Aastra CSV file you wish to create";
            SaveCSV_Dialog.DefaultExt = "csv";
            SaveCSV_Dialog.Filter = "CSV files (*.csv)|*.csv";
            SaveCSV_Dialog.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
            SaveCSV_Dialog.FileName = "aastra_csv";

            if (SaveCSV_Dialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            disable_buttons(true);

            save_data_AastraCSV(SaveCSV_Dialog.FileName, myGroupDataHash);

            // and reenable user interface
            disable_buttons(false);
        }

        private void btn_save_GrandstreamXml_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveXML_Dialog = new SaveFileDialog();
            SaveXML_Dialog.Title = "Select the XML file you wish to create";
            SaveXML_Dialog.DefaultExt = "xml";
            SaveXML_Dialog.Filter = "XML files (*.xml)|*.xml";
            SaveXML_Dialog.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
            SaveXML_Dialog.FileName = "GrandstreamExport";

            if (SaveXML_Dialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            disable_buttons(true);

            save_data_GrandstreamXml(SaveXML_Dialog.FileName, myGroupDataHash);

            // and reenable user interface
            disable_buttons(false);
        }


        private void save_data_Outlook(System.Collections.Hashtable workDataHash)
        {
            // get the country ID from the combobox or from user input
            string country_id = RetrieveCountryID(combo_prefix.SelectedItem.ToString());

            // some basic initialization of the outlook support
            Microsoft.Office.Interop.Outlook.Application outlookObj = null;
            Microsoft.Office.Interop.Outlook.MAPIFolder outlookFolder = null;

            // connect to outlook, if possible. Else, complain and abort.
            try { outlookObj = new Microsoft.Office.Interop.Outlook.Application(); }
            catch (Exception outlook_exception) { MessageBox.Show("Unable to access Outlook!" + Environment.NewLine + Environment.NewLine + "This program needs Outlook to continue, are you sure it's installed and working?" + Environment.NewLine + Environment.NewLine + "Error returned was: " + outlook_exception.ToString() + Environment.NewLine); }

            // sucessfully connected to outlook, do some further setup work
            Microsoft.Office.Interop.Outlook.NameSpace outlookNS = outlookObj.GetNamespace("MAPI");

            // Allow the user to chose a custom destination folder
            outlookFolder = outlookNS.PickFolder();
            if (outlookFolder == null)
            { return; } // if no folder has been selected, user must have pressed cancel and therefore we abort

            // iterate through the contacts in workDatahash and save them to the selected Outlook Folder
            int count = 0;
            foreach (System.Collections.DictionaryEntry contactHash in workDataHash)
            {
                // extract GroupDataList from hashtable contents
                GroupDataContact contactData = (GroupDataContact)contactHash.Value;

                string prefix_string = (cfg_prefixNONFB == true) ? contactData.FRITZprefix : string.Empty;

                // clean up phone number
                string CleanUpNumberHome = CleanUpNumber(contactData.home, country_id, prefix_string);
                string CleanUpNumberWork = CleanUpNumber(contactData.work, country_id, prefix_string);
                string CleanUpNumberHomefax = CleanUpNumber(contactData.homefax, country_id, prefix_string);
                string CleanUpNumberWorkfax = CleanUpNumber(contactData.workfax, country_id, prefix_string);
                string CleanUpNumberMobile = CleanUpNumber(contactData.mobile, country_id, prefix_string);

                // create new contact
                Microsoft.Office.Interop.Outlook.ContactItem newContact = (Microsoft.Office.Interop.Outlook.ContactItem)outlookFolder.Items.Add(Microsoft.Office.Interop.Outlook.OlItemType.olContactItem);

                newContact.FirstName = contactData.firstname;
                newContact.LastName = contactData.lastname;
                newContact.Email1Address = contactData.email;
                newContact.MailingAddressCity = contactData.city;
                newContact.MailingAddressStreet = contactData.street;
                newContact.MailingAddressPostalCode = contactData.zip;
                newContact.HomeTelephoneNumber = CleanUpNumberHome;
                newContact.HomeFaxNumber = CleanUpNumberHomefax;
                newContact.BusinessTelephoneNumber = CleanUpNumberWork;
                newContact.BusinessFaxNumber = CleanUpNumberWorkfax;
                newContact.FileAs = contactData.combinedname;
                newContact.CompanyName = contactData.company;
                newContact.MobileTelephoneNumber = CleanUpNumberMobile;

                // VIP Functionality (fully implemented now)
                if (contactData.isVIP == "Yes") { newContact.Body += "VIP" + Environment.NewLine; }

                // Speeddial Functionality
                if (contactData.speeddial != "") { newContact.Body += "SPEEDDIAL(" + contactData.speeddial + ")"; }

                // Contact Picture Functionality, more info on this here: http://www.c-sharpcorner.com/UploadFile/Nimusoft/OutlookwithNET06262007081811AM/OutlookwithNET.aspx
                if (cfg_OLpics == true && combo_picexport.SelectedIndex == 1 && contactData.jpeg != null)
                {
                    string tempname = System.IO.Path.GetTempFileName();
                    System.IO.File.WriteAllBytes(tempname, contactData.jpeg);
                    newContact.AddPicture(tempname);
                    System.IO.File.Delete(tempname);
                }

                newContact.Save();
                count++;

            } // end of foreach loop for the contacts

            // tell the user what has been done
            MessageBox.Show(count + " contacts have been written to the selected Outlook folder!" + Environment.NewLine);
        }

        private void save_data_FritzXML(string filename, System.Collections.Hashtable workDataHash)
        {
            // get the country ID from the combobox or from user input
            string country_id = RetrieveCountryID(combo_prefix.SelectedItem.ToString());

            // create output path for fonpix, if necessary
            string pic_export_path = "";
            if (combo_picexport.SelectedIndex == 1)
            {
                pic_export_path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filename), System.IO.Path.GetFileNameWithoutExtension(filename) + " - fonpix-custom");
                if (!System.IO.Directory.Exists(pic_export_path)) { System.IO.Directory.CreateDirectory(pic_export_path); }
            }

            // process with exporting
            StringBuilder resultSB = new StringBuilder();

            // write the header
            resultSB.Append("<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>\n<phonebooks>\n<phonebook>");

            // initialize quickdial_remaining veriable
            int qd_remaining = 97;

            // initialize hashtable to store generated data results
            System.Collections.Hashtable MySaveDataHash = new System.Collections.Hashtable();

            foreach (System.Collections.DictionaryEntry contactHash in workDataHash)
            {
                StringBuilder contactSB = new StringBuilder();

                // extract GroupDataList from hashtable contents
                GroupDataContact contactData = (GroupDataContact)contactHash.Value;

                // check if all relevant phone numbers for this export here are empty
                if (contactData.home == string.Empty && contactData.work == string.Empty && contactData.mobile == string.Empty)
                { continue; /* if yes, abort this foreach loop and contine to the next one */ }

                string SaveAsName = contactData.combinedname;

                // limit to 32 chars
                SaveAsName = LimitNameLength(SaveAsName, 32);

                // replace "&","<",">"
                SaveAsName = SaveAsName.Replace("&", "&amp;");
                SaveAsName = SaveAsName.Replace("<", "&lt;");
                SaveAsName = SaveAsName.Replace(">", "&gt;");

                // clean up phone number
                string CleanUpNumberHome = CleanUpNumber(contactData.home, country_id, contactData.FRITZprefix);
                string CleanUpNumberWork = CleanUpNumber(contactData.work, country_id, contactData.FRITZprefix);
                string CleanUpNumberMobile = CleanUpNumber(contactData.mobile, country_id, contactData.FRITZprefix);

                // write contact header

                // generate VIP ID - if VIP then assign category 1, else 0
                string VIPid = "0";
                if (contactData.isVIP == "Yes") { VIPid = "1"; }

                // if picture export is set to yes and picture is stored, then export it to a jpeg file and add imageURL to export XML
                string imageURLstring = "";
                if (combo_picexport.SelectedIndex == 1 && contactData.jpeg != null)
                {
                    string picfile = System.IO.Path.ChangeExtension(System.IO.Path.GetRandomFileName(), "jpg");
                    writeByteArrayToFile(contactData.jpeg, System.IO.Path.Combine(pic_export_path, picfile));
                    imageURLstring = "<imageURL>" + textBox_PicPath.Text + picfile + "</imageURL>\n";
                }

                contactSB.Append("<contact>\n<category>" + VIPid + "</category>\n<person>\n<realName>" + SaveAsName + "</realName>\n" + imageURLstring + "</person>\n<telephony>\n");

                // prepare preferred number setting:
                string pref_QD_string = "prio=\"1\"";

                // add quickdial and vanity information to it
                if (contactData.speeddial != "")
                { // then we have a quickdial number then add it to the prio entry

                    string qd_number = "";
                    string qd_vanity = "";

                    if (contactData.speeddial.Contains(","))
                    { // two parts (01,VANITY or XX,VANITY)
                        qd_number = contactData.speeddial.Substring(0, contactData.speeddial.IndexOf(','));
                        qd_vanity = contactData.speeddial.Substring(contactData.speeddial.IndexOf(',') + 1);

                        // now handle automatic distribution of quickdial numbers for vanities without number
                        if ((qd_number == "XX") && (qd_remaining > 9)) { qd_number = qd_remaining.ToString(); qd_remaining--; }
                        if (qd_number == "XX")
                        { // no remaining numbers left, so we cannot assign any quickdial number or vanity code (the field quickdial will be set to "" in the XML anyway but that probably won't matter)
                            qd_number = "";
                            qd_vanity = "";
                        }
                    }
                    else
                    { // only 1 part, can only be number
                        qd_number = contactData.speeddial;
                    }

                    // add quickdial number which always has to exist here (unless the autodistribtion of numbers above ran out of available numbers)
                    if (qd_number != "")
                    {
                        pref_QD_string += " quickdial=\"" + qd_number + "\"";

                        // and if we also have a vanity entry, then add this one too
                        if (qd_vanity != "")
                        {
                            pref_QD_string += " vanity=\"" + qd_vanity + "\"";
                        }
                    }

                }

                // add home phone number
                string fritzXMLhome = "";
                if (CleanUpNumberHome != "")
                {
                    if (contactData.preferred == "home")
                    { fritzXMLhome = "<number type=\"home\" " + pref_QD_string + ">" + CleanUpNumberHome + "</number>\n"; }
                    else
                    {  fritzXMLhome = "<number type=\"home\">" + CleanUpNumberHome + "</number>\n"; }

                }
                else { fritzXMLhome = "<number type=\"home\" />\n"; }

                // add work phone number
                string fritzXMLwork = "";
                if (CleanUpNumberWork != "")
                {
                    if (contactData.preferred == "work")
                    { fritzXMLwork = "<number type=\"work\" " + pref_QD_string + ">" + CleanUpNumberWork + "</number>\n"; }
                    else
                    { fritzXMLwork = "<number type=\"work\">" + CleanUpNumberWork + "</number>\n"; }
                }
                else { fritzXMLwork = "<number type=\"work\" />\n"; }

                // add mobile phone number
                string fritzXMLmobile = "";
                if (CleanUpNumberMobile != "")
                {
                    if (contactData.preferred == "mobile")
                    { fritzXMLmobile = "<number type=\"mobile\" " + pref_QD_string + ">" + CleanUpNumberMobile + "</number>\n"; }
                    else
                    { fritzXMLmobile = "<number type=\"mobile\">" + CleanUpNumberMobile + "</number>\n"; }
                }
                else { fritzXMLmobile = "<number type=\"mobile\" />\n"; }


                // actually add stuff we have prepared above
                if (cfg_fritzWorkFirst == false)
                {
                    contactSB.Append(fritzXMLhome);
                    contactSB.Append(fritzXMLwork);
                }
                else
                {
                    contactSB.Append(fritzXMLwork);
                    contactSB.Append(fritzXMLhome);
                }
                
                contactSB.Append(fritzXMLmobile);



                // add telephony end and services start
                contactSB.Append("</telephony>\n<services>\n");

                // depending on whether an eMail exists, add email or not
                if (contactData.email != "")
                {
                    contactSB.Append("<email classifier=\"private\">" + contactData.email + "</email>");
                    contactSB.Append("<email />\n");
                }
                else
                {
                    contactSB.Append("<email />\n");
                }

                // add serviced end and rest of contact footer
                contactSB.Append("</services>\n<setup>\n<ringTone />\n<ringVolume />\n</setup>\n</contact>");

                try
                {
                    MySaveDataHash.Add(SaveAsName, contactSB.ToString());
                }
                catch (ArgumentException) // unable to add to MySaveDataHash, must mean that something with saveasname is already in there!
                {
                    MessageBox.Show("Unable to export the entry \"" + SaveAsName + "\", another entry with this name already exists! Ignoring duplicate.");
                }

            } // end of foreach loop for the contacts

            foreach (System.Collections.DictionaryEntry saveDataHash in MySaveDataHash)
            { resultSB.Append((string)saveDataHash.Value); }

            // write file footer with AVM HD Music test stuff, currently not added. Not sure if thats a good idea to add this always.
            // resultstring += "<contact><category /><person><realName>~AVM-HD-Musik</realName></person><telephony><number\nprio=\"1\" type=\"home\" quickdial=\"98\">200@hd-telefonie.avm.de</number></telephony><services /><setup /></contact><contact><category /><person><realName>~AVM-HD-Sprache</realName></person><telephony><number\nprio=\"1\" type=\"home\" quickdial=\"99\">100@hd-telefonie.avm.de</number></telephony><services /><setup /></contact>";
            resultSB.Append("</phonebook>\n</phonebooks>");

            // actually write the file to disk
            System.IO.File.WriteAllText(filename, resultSB.ToString(), Encoding.GetEncoding("ISO-8859-1"));

            // tell the user this has been done
            string errorwarning = "";
            if (MySaveDataHash.Count > 300) { errorwarning = Environment.NewLine + "Warning: Over 300 contacts have been exported! This might or might not be officially supported by AVM's Fritz!Box and may cause problems. Proceed with care!"; }
            MessageBox.Show(MySaveDataHash.Count + " contacts written to " + filename + " !" + Environment.NewLine + errorwarning);

        }

        private void save_data_vCard(string filename, System.Collections.Hashtable workDataHash, bool export_only_phone)
        {
            // get the country ID from the combobox or from user input
            string country_id = RetrieveCountryID(combo_prefix.SelectedItem.ToString());

            System.Collections.Hashtable MySaveDataHash = new System.Collections.Hashtable();
            foreach (System.Collections.DictionaryEntry contactHash in workDataHash)
            {
                // process with exporting
                StringBuilder sb_resultstring = new StringBuilder();

                // extract GroupDataList from hashtable contents
                GroupDataContact contactData = (GroupDataContact)contactHash.Value;

                string prefix_string = (cfg_prefixNONFB == true) ? contactData.FRITZprefix : string.Empty;

                // clean up phone numbers
                string CleanUpNumberHome = CleanUpNumber(contactData.home, country_id, prefix_string);
                string CleanUpNumberWork = CleanUpNumber(contactData.work, country_id, prefix_string);
                string CleanUpNumberHomefax = CleanUpNumber(contactData.homefax, country_id, prefix_string);
                string CleanUpNumberWorkfax = CleanUpNumber(contactData.workfax, country_id, prefix_string);
                string CleanUpNumberMobile = CleanUpNumber(contactData.mobile, country_id, prefix_string);

                // clean up the rest of the data
                string CleanUpStreet = contactData.street.Replace("\\", "\\\\").Replace(";", "\\;").Replace(",", "\\,");
                string CleanUpCity = contactData.city.Replace("\\", "\\\\").Replace(";", "\\;").Replace(",", "\\,");
                string CleanUpZIP = contactData.zip.Replace("\\", "\\\\").Replace(";", "\\;").Replace(",", "\\,");
                string CleanUpeMail = contactData.email.Replace("\\", "\\\\").Replace(";", "\\;").Replace(",", "\\,");
                string CleanUpFirstName = contactData.firstname.Replace("\\", "\\\\").Replace(";", "\\;").Replace(",", "\\,");
                string CleanUpLastName = contactData.lastname.Replace("\\", "\\\\").Replace(";", "\\;").Replace(",", "\\,");
                string CleanUpCompany = contactData.company.Replace("\\", "\\\\").Replace(";", "\\;").Replace(",", "\\,");
                string CleanUpCombined = contactData.combinedname.Replace("\\", "\\\\").Replace(";", "\\;").Replace(",", "\\,");

                // if we only wish to export phone numbers => check if all relevant phone fields for this export are empty
                if (export_only_phone == true && (CleanUpNumberHome == string.Empty && CleanUpNumberWork == string.Empty && CleanUpNumberMobile == string.Empty))
                {
                    MessageBox.Show("Contact |" + CleanUpCombined + "| ignored, due to missing numbers");
                    // if yes, abort this foreach loop and contine to the next one
                    continue;
                }

                sb_resultstring.AppendLine("BEGIN:VCARD");
                sb_resultstring.AppendLine("VERSION:3.0");
                sb_resultstring.AppendLine("N:" + CleanUpLastName + ";" + CleanUpFirstName + ";;;");
                sb_resultstring.AppendLine("FN:" + CleanUpCombined);
                if (CleanUpCompany != string.Empty) { sb_resultstring.AppendLine("ORG:" + CleanUpCompany + ";"); }

                // save home phone number
                if (CleanUpNumberHome != string.Empty)
                {
                    if (contactData.preferred == "home")
                    { sb_resultstring.AppendLine("TEL;type=HOME;type=pref:" + CleanUpNumberHome); }
                    else
                    { sb_resultstring.AppendLine("TEL;type=HOME:" + CleanUpNumberHome); }
                }

                // save work phone number
                if (CleanUpNumberWork != string.Empty)
                {
                    if (contactData.preferred == "work")
                    { sb_resultstring.AppendLine("TEL;type=WORK;type=pref:" + CleanUpNumberWork); }
                    else
                    { sb_resultstring.AppendLine("TEL;type=WORK:" + CleanUpNumberWork); }
                }

                // save mobile phone number
                if (CleanUpNumberMobile != string.Empty)
                {
                    if (contactData.preferred == "mobile")
                    { sb_resultstring.AppendLine("TEL;type=CELL;type=pref:" + CleanUpNumberMobile); }
                    else
                    { sb_resultstring.AppendLine("TEL;type=CELL:" + CleanUpNumberMobile); }
                }

                // save home fax phone number
                if (CleanUpNumberHomefax != string.Empty)
                {
                    sb_resultstring.AppendLine("TEL;type=HOME;type=FAX:" + CleanUpNumberHomefax);
                }

                // save work fax phone number
                if (CleanUpNumberWorkfax != string.Empty)
                {
                    sb_resultstring.AppendLine("TEL;type=WORK;type=FAX:" + CleanUpNumberWorkfax);
                }

                // save street, zip and city information, if one of them is available
                if (!(CleanUpStreet == string.Empty && CleanUpZIP == string.Empty && CleanUpCity == string.Empty))
                {
                    sb_resultstring.AppendLine("ADR;type=HOME;type=pref:;;" + CleanUpStreet + ";" + CleanUpCity + ";;" + CleanUpZIP + ";");
                }

                // save email
                if (CleanUpeMail != string.Empty)
                {
                    sb_resultstring.AppendLine("EMAIL;type=INTERNET;type=pref:" + CleanUpeMail);
                }

                // we will now save VIP and speeddial information to the first of those two entries
                string extra_comments = string.Empty;
                if (contactData.isVIP == "Yes") { extra_comments = "VIP "; }
                if (contactData.speeddial != "") { extra_comments += "SPEEDDIAL(" + contactData.speeddial + ")"; }
                if (extra_comments != string.Empty)
                {
                    sb_resultstring.AppendLine("NOTE:" + extra_comments);
                }

                sb_resultstring.AppendLine("END:VCARD");

                try
                {
                    MySaveDataHash.Add(contactData.combinedname, sb_resultstring.ToString());
                }
                catch (ArgumentException) // unable to add to MySaveFaxDataHash, must mean that something with saveasname is already in there!
                {
                    MessageBox.Show("Unable to export the entry \"" + contactData.combinedname + "\", another entry with this name already exists! Ignoring duplicate.");
                }

            } // end of foreach loop for the contacts

            StringBuilder resultstring = new StringBuilder();
            // retrieve stuff from hastable and put in resultstring:
            foreach (System.Collections.DictionaryEntry saveDataHash in MySaveDataHash)
            {
                resultstring.Append((string)saveDataHash.Value);
            }

            // actually write the file to disk
            System.IO.File.WriteAllText(filename, resultstring.ToString(), Encoding.Unicode);
        }

        private void save_data_FritzAdress(string filename, System.Collections.Hashtable workDataHash, bool only_export_fax)
        {

            // get the country ID from the combobox or from user input
            string country_id = RetrieveCountryID(combo_prefix.SelectedItem.ToString());

            // process with exporting
            string resultstring;

            // write the header
            resultstring = "BEZCHNG\tFIRMA\tNAME\tVORNAME\tABTEILUNG\tSTRASSE\tPLZ\tORT\tKOMMENT\tTELEFON\tTELEFAX\tTRANSFER\tTERMINAL\tBENUTZER\tPASSWORT\tTRANSPROT\tTERMMODE\tNOTIZEN\tMOBILFON\tEMAIL\tHOMEPAGE\r\n";

            System.Collections.Hashtable MySaveDataHash = new System.Collections.Hashtable();

            foreach (System.Collections.DictionaryEntry contactHash in workDataHash)
            {
                // extract GroupDataList from hashtable contents
                GroupDataContact contactData = (GroupDataContact)contactHash.Value;

                // check if both relevant fax fields for this export are empty - AND we only wish to export those who have faxnumbers
                if (contactData.homefax == string.Empty && contactData.workfax == string.Empty && (only_export_fax == true))
                {
                    // if yes, abort this foreach loop and contine to the next one
                    continue;
                }

                string prefix_string = (cfg_prefixNONFB == true) ? contactData.FRITZprefix : string.Empty;

                // clean up phone numbers
                string CleanUpNumberHome = CleanUpNumber(contactData.home, country_id, prefix_string);
                string CleanUpNumberWork = CleanUpNumber(contactData.work, country_id, prefix_string);
                string CleanUpNumberHomefax = CleanUpNumber(contactData.homefax, country_id, prefix_string);
                string CleanUpNumberWorkfax = CleanUpNumber(contactData.workfax, country_id, prefix_string);
                string CleanUpNumberMobile = CleanUpNumber(contactData.mobile, country_id, prefix_string);

                string[,] save_iterate_array;


                if ((CleanUpNumberHomefax != "" && CleanUpNumberWorkfax != "") || (CleanUpNumberHome != "" && CleanUpNumberWork != ""))
                { // we need to save two separate entries since there are there are either seperate phone numbers or fax numbers for work and home (or both)
                    save_iterate_array = new string[2, 4];
                    save_iterate_array[0, 0] = LimitNameLength(contactData.combinedname, 22) + " (privat)";
                    save_iterate_array[0, 1] = CleanUpNumberHomefax;
                    save_iterate_array[0, 2] = CleanUpNumberHome;
                    save_iterate_array[1, 0] = LimitNameLength(contactData.combinedname, 22) + " (gesch.)";
                    save_iterate_array[1, 1] = CleanUpNumberWorkfax;
                    save_iterate_array[1, 2] = CleanUpNumberWork;
                }
                else
                { // we need to save only 1 entry
                    save_iterate_array = new string[1, 4];
                    save_iterate_array[0, 0] = LimitNameLength(contactData.combinedname, 31);
                    if (CleanUpNumberHomefax != "") { save_iterate_array[0, 1] = CleanUpNumberHomefax; }
                    if (CleanUpNumberWorkfax != "") { save_iterate_array[0, 1] = CleanUpNumberWorkfax; }
                    if (CleanUpNumberHome != "") { save_iterate_array[0, 2] = CleanUpNumberHome; }
                    if (CleanUpNumberWork != "") { save_iterate_array[0, 2] = CleanUpNumberWork; }
                }

                // we will now save VIP and speeddial information to the first of those two entries
                if (contactData.isVIP == "Yes")
                {
                    save_iterate_array[0, 3] = "VIP ";
                }

                if (contactData.speeddial != "")
                {
                    save_iterate_array[0, 3] += "SPEEDDIAL(" + contactData.speeddial + ")";
                }

                // write contact line, maybe twice
                for (int i = 0; i < save_iterate_array.GetLength(0); i++)
                {

                    try
                    {
                        MySaveDataHash.Add(save_iterate_array[i, 0], save_iterate_array[i, 0] + "\t" + contactData.company + "\t" + contactData.lastname + "\t" + contactData.firstname + "\t" + string.Empty + "\t" + contactData.street + "\t" + contactData.zip + "\t" + contactData.city + "\t" + "\t" + save_iterate_array[i, 2] + "\t" + save_iterate_array[i, 1] + "\t" + "\t" + "\t" + "\t" + "\t" + "A" + "\t" + "\t" + save_iterate_array[i, 3] + "\t" + CleanUpNumberMobile + "\t" + contactData.email + "\t" + "\r\n");
                    }
                    catch (ArgumentException) // unable to add to MySaveFaxDataHash, must mean that something with saveasname is already in there!
                    {
                        MessageBox.Show("Unable to export the entry \"" + save_iterate_array[i, 0] + "\", another entry with this name already exists! Ignoring duplicate.");
                    }


                }
            } // end of foreach loop for the contacts

            // retrieve stuff from hastable and put in resultstring:
            foreach (System.Collections.DictionaryEntry saveDataHash in MySaveDataHash)
            {
                resultstring += (string)saveDataHash.Value;
            }

            // actually write the file to disk
            System.IO.File.WriteAllText(filename, resultstring, Encoding.GetEncoding("ISO-8859-1"));
        }

        private void save_data_SnomCSV7(string filename, System.Collections.Hashtable workDataHash)
        {
            // get the country ID from the combobox or from user input
            string country_id = RetrieveCountryID(combo_prefix.SelectedItem.ToString());

            // process with exporting
            StringBuilder resultSB = new StringBuilder();

            // write the header (none needed for snom)
            resultSB.Append("\"Name\",\"Number\"\r\n");

            System.Collections.Hashtable MySaveDataHash = new System.Collections.Hashtable();

            foreach (System.Collections.DictionaryEntry contactHash in workDataHash)
            {
                // extract GroupDataList from hashtable contents
                GroupDataContact contactData = (GroupDataContact)contactHash.Value;

                //  check if all relevant phone numbers for this export here are empty
                if (contactData.home == string.Empty && contactData.work == string.Empty && contactData.mobile == string.Empty)
                { continue; /* if yes, abort this foreach loop and contine to the next one */ }

                string prefix_string = (cfg_prefixNONFB == true) ? contactData.FRITZprefix : string.Empty;

                // clean up phone number
                string CleanUpNumberHome = CleanUpNumber(contactData.home, country_id, prefix_string);
                string CleanUpNumberWork = CleanUpNumber(contactData.work, country_id, prefix_string);
                string CleanUpNumberMobile = CleanUpNumber(contactData.mobile, country_id, prefix_string);

                // add privat/gesch. to name if necessary (if two entries for one name necessary)
                string name_home = contactData.combinedname;
                string name_work = contactData.combinedname;
                string name_mobile = contactData.combinedname;
                int nr_in_use = 0;
                if (CleanUpNumberHome != "") { nr_in_use++; }
                if (CleanUpNumberWork != "") { nr_in_use++; }
                if (CleanUpNumberMobile != "") { nr_in_use++; }

                if (nr_in_use > 1)
                {
                    // limit to 26 chars
                    name_home = LimitNameLength(name_home, 26);
                    name_work = LimitNameLength(name_work, 26);
                    name_mobile = LimitNameLength(name_mobile, 26);

                    // then add 5 additional chars
                    name_home += " home";
                    name_work += " work";
                    name_mobile += " mobile";
                }
                else
                {
                    // limit to 31 chars
                    name_home = LimitNameLength(name_home, 31);
                    name_work = LimitNameLength(name_work, 31);
                    name_mobile = LimitNameLength(name_mobile, 31);
                }

                // write contact line, maybe twice
                if (CleanUpNumberHome != "")
                {
                    try
                    {
                        MySaveDataHash.Add(name_home, "\"" + name_home + "\",\"" + CleanUpNumberHome + "\"\r\n");
                    }
                    catch (ArgumentException) // unable to add to MySaveFaxDataHash, must mean that something with (modified) contactData.combinedname is already in there!
                    {
                        MessageBox.Show("Unable to export the shortened entry \"" + name_home + "\",\r\nanother entry with this name already exists!\r\n\r\nIgnoring duplicate for contact source: " + contactData.combinedname);
                    }
                }

                if (CleanUpNumberWork != "")
                {
                    try
                    {
                        MySaveDataHash.Add(name_work, "\"" + name_work + "\",\"" + CleanUpNumberWork + "\"\r\n");
                    }
                    catch (ArgumentException) // unable to add to MySaveFaxDataHash, must mean that something with (modified) contactData.combinedname is already in there!
                    {
                        MessageBox.Show("Unable to export the shortened entry \"" + name_work + "\",\r\nanother entry with this name already exists!\r\n\r\nIgnoring duplicate for contact source: " + contactData.combinedname);
                    }
                }

                if (CleanUpNumberMobile != "")
                {
                    try
                    {
                        MySaveDataHash.Add(name_mobile, "\"" + name_mobile + "\",\"" + CleanUpNumberMobile + "\"\r\n");
                    }
                    catch (ArgumentException) // unable to add to MySaveFaxDataHash, must mean that something with (modified) contactData.combinedname is already in there!
                    {
                        MessageBox.Show("Unable to export the shortened entry \"" + name_mobile + "\",\r\nanother entry with this name already exists!\r\n\r\nIgnoring duplicate for contact source: " + contactData.combinedname);
                    }
                }

            } // end of foreach loop for the contacts

            // retrieve stuff from hastable and put in resultstring:
            foreach (System.Collections.DictionaryEntry saveDataHash in MySaveDataHash)
            {
                resultSB.Append((string)saveDataHash.Value);
            }

            // actually write the file to disk
            System.Text.Encoding utf8WithoutBom = new System.Text.UTF8Encoding(false);
            System.IO.File.WriteAllText(filename, resultSB.ToString(), utf8WithoutBom);
        }

        private void save_data_SnomCSV8(string filename, System.Collections.Hashtable workDataHash)
        {
            // get the country ID from the combobox or from user input
            string country_id = RetrieveCountryID(combo_prefix.SelectedItem.ToString());

            // process with exporting
            StringBuilder resultSB = new StringBuilder();

            System.Collections.Hashtable MySaveDataHash = new System.Collections.Hashtable();

            foreach (System.Collections.DictionaryEntry contactHash in workDataHash)
            {
                // extract GroupDataList from hashtable contents
                GroupDataContact contactData = (GroupDataContact)contactHash.Value;

                //  check if all relevant phone numbers for this export here are empty
                if (contactData.home == string.Empty && contactData.work == string.Empty && contactData.mobile == string.Empty)
                { continue; /* if yes, abort this foreach loop and contine to the next one */ }

                string prefix_string = (cfg_prefixNONFB == true) ? contactData.FRITZprefix : string.Empty;

                // clean up phone number
                string CleanUpNumberHome = CleanUpNumber(contactData.home, country_id, prefix_string);
                string CleanUpNumberWork = CleanUpNumber(contactData.work, country_id, prefix_string);
                string CleanUpNumberMobile = CleanUpNumber(contactData.mobile, country_id, prefix_string);

                // limit full name to 31 chars
                string name_contact = LimitNameLength(contactData.combinedname, 31);

                // we now do all sorts of complicated stuff to generate a seemingly simple array of numbers and types
                int phone_numbers = 0;
                if (CleanUpNumberHome != "") { phone_numbers++; }
                if (CleanUpNumberWork != "") { phone_numbers++; }
                if (CleanUpNumberMobile != "") { phone_numbers++; }

                string[,] phones = new string[phone_numbers, 3];

                if (contactData.preferred == "")
                {
                    if (CleanUpNumberHome != "") { contactData.preferred = "home"; }
                    else if (CleanUpNumberWork != "") { contactData.preferred = "work"; }
                    else if (CleanUpNumberMobile != "") { contactData.preferred = "mobile"; }
                }

                // start with the (now set) preffered number
                switch (contactData.preferred)
                {
                    case "home":
                        phones[0, 0] = "home";
                        phones[0, 1] = CleanUpNumberHome;
                        if (contactData.isVIP == "Yes") { phones[0, 2] = "vip"; }
                        CleanUpNumberHome = "";
                        break;
                    case "work":
                        phones[0, 0] = "work";
                        phones[0, 1] = CleanUpNumberWork;
                        if (contactData.isVIP == "Yes") { phones[0, 2] = "vip"; }
                        CleanUpNumberWork = "";
                        break;
                    case "mobile":
                        phones[0, 0] = "mobile";
                        phones[0, 1] = CleanUpNumberMobile;
                        if (contactData.isVIP == "Yes") { phones[0, 2] = "vip"; }
                        CleanUpNumberMobile = "";
                        break;
                    default:
                        MessageBox.Show("Default case while parsing contactData.preferred, this should not have happened. Report this bug please!");
                        break;
                }

                if (phone_numbers >= 2)
                {
                    if (CleanUpNumberHome != "")
                    {
                        phones[1, 0] = "home";
                        phones[1, 1] = CleanUpNumberHome;
                        if (contactData.isVIP == "Yes") { phones[1, 2] = "vip"; }
                        CleanUpNumberHome = "";
                    }
                    else if (CleanUpNumberWork != "")
                    {
                        phones[1, 0] = "work";
                        phones[1, 1] = CleanUpNumberWork;
                        if (contactData.isVIP == "Yes") { phones[1, 2] = "vip"; }
                        CleanUpNumberWork = "";
                    }
                    else if (CleanUpNumberMobile != "")
                    {
                        phones[1, 0] = "mobile";
                        phones[1, 1] = CleanUpNumberMobile;
                        if (contactData.isVIP == "Yes") { phones[1, 2] = "vip"; }
                        CleanUpNumberMobile = "";
                    }
                }

                if (phone_numbers == 3)
                {
                    if (CleanUpNumberHome != "")
                    {
                        phones[2, 0] = "home";
                        phones[2, 1] = CleanUpNumberHome;
                        if (contactData.isVIP == "Yes") { phones[2, 2] = "vip"; }
                        CleanUpNumberHome = "";
                    }
                    else if (CleanUpNumberWork != "")
                    {
                        phones[2, 0] = "work";
                        phones[2, 1] = CleanUpNumberWork;
                        if (contactData.isVIP == "Yes") { phones[2, 2] = "vip"; }
                        CleanUpNumberWork = "";
                    }
                    else if (CleanUpNumberMobile != "")
                    {
                        phones[2, 0] = "mobile";
                        phones[2, 1] = CleanUpNumberMobile;
                        if (contactData.isVIP == "Yes") { phones[2, 2] = "vip"; }
                        CleanUpNumberMobile = "";
                    }
                }

                string sep = "\",\"";
                string out_string = "";

                if (phone_numbers > 1)
                {

                    out_string += "\"" + name_contact
                                        + sep + phones[0, 1]
                                        + sep + "MASTER"
                                        + sep + string.Empty
                                        + sep + contactData.firstname
                                        + sep + contactData.lastname
                                        + sep + string.Empty
                                        + sep + string.Empty
                                        + sep + contactData.email
                                        + sep + string.Empty
                                        + sep + string.Empty
                                        + sep + string.Empty
                                        + sep + "false"
                                        + sep + string.Empty
                                        + sep + phones[0, 0]
                                        + sep + string.Empty
                                        + "\"" + "\r\n";

                    // now that the phones[phone_numbers, 2] array is complete, iterate through it and write the subcontact code for the csv:
                    for (int i = 0; i < phone_numbers; i++)
                    {
                        out_string += "\"" + string.Empty
                                                    + sep + phones[i, 1] // actual number
                                                    + sep + phones[i, 2] // add vip if VIP
                                                    + sep + string.Empty
                                                    + sep + "Member_Alias"
                                                    + sep + phones[0, 1] // main number
                                                    + sep + string.Empty
                                                    + sep + string.Empty
                                                    + sep + string.Empty
                                                    + sep + string.Empty
                                                    + sep + string.Empty
                                                    + sep + string.Empty
                                                    + sep + "false"
                                                    + sep + string.Empty
                                                    + sep + phones[i, 0]
                                                    + sep + string.Empty
                                                    + "\"" + "\r\n";
                    }
                }
                else
                {
                    // only one contact, so don't write subcontacts but only simplified version

                    out_string += "\"" + name_contact
                    + sep + phones[0, 1]
                    + sep + phones[0, 2] // add VIP if VIP
                    + sep + string.Empty
                    + sep + contactData.firstname
                    + sep + contactData.lastname
                    + sep + string.Empty
                    + sep + string.Empty
                    + sep + contactData.email
                    + sep + string.Empty
                    + sep + string.Empty
                    + sep + string.Empty
                    + sep + "false"
                    + sep + string.Empty
                    + sep + phones[0, 0]
                    + sep + string.Empty
                    + "\"" + "\r\n";
                }

                // write contact line, maybe twice
                try
                {
                    MySaveDataHash.Add(name_contact, out_string);

                }
                catch (ArgumentException) // unable to add to MySaveFaxDataHash, must mean that something with (modified) contactData.combinedname is already in there!
                {
                    MessageBox.Show("Unable to export the entry \"" + name_contact + "\", another entry with this name already exists! Ignoring duplicate.");
                }

            } // end of foreach loop for the contacts

            // retrieve stuff from hastable and put in resultstring:
            foreach (System.Collections.DictionaryEntry saveDataHash in MySaveDataHash)
            {
                resultSB.Append((string)saveDataHash.Value);
            }

            // actually write the file to disk
            System.Text.Encoding utf8WithoutBom = new System.Text.UTF8Encoding(false);
            System.IO.File.WriteAllText(filename, resultSB.ToString(), utf8WithoutBom);

        }

        private void save_data_TalkSurfCSV(string filename, System.Collections.Hashtable workDataHash)
        {
            // get the country ID from the combobox or from user input
            string country_id = RetrieveCountryID(combo_prefix.SelectedItem.ToString());

            // process with exporting
            string resultstring;

            // write the header (none needed for snom)
            resultstring = "Name,Number\r\n";

            System.Collections.Hashtable MySaveDataHash = new System.Collections.Hashtable();

            foreach (System.Collections.DictionaryEntry contactHash in workDataHash)
            {
                // extract GroupDataList from hashtable contents
                GroupDataContact contactData = (GroupDataContact)contactHash.Value;

                //  check if all relevant phone numbers for this export here are empty
                if (contactData.home == string.Empty && contactData.work == string.Empty && contactData.mobile == string.Empty)
                { continue; /* if yes, abort this foreach loop and contine to the next one */ }

                string prefix_string = (cfg_prefixNONFB == true) ? contactData.FRITZprefix : string.Empty;

                // clean up phone number
                string CleanUpNumberHome = CleanUpNumber(contactData.home, country_id, prefix_string);
                string CleanUpNumberWork = CleanUpNumber(contactData.work, country_id, prefix_string);
                string CleanUpNumberMobile = CleanUpNumber(contactData.mobile, country_id, prefix_string);

                // add privat/gesch. to name if necessary (if seperate entries for one name necessary) - and remove "," because talk&surf can't handle it properly
                string name_home = contactData.combinedname.Replace(",", "");
                string name_work = contactData.combinedname.Replace(",", "");
                string name_mobile = contactData.combinedname.Replace(",", "");
                int nr_in_use = 0;
                if (CleanUpNumberHome != "") { nr_in_use++; }
                if (CleanUpNumberWork != "") { nr_in_use++; }
                if (CleanUpNumberMobile != "") { nr_in_use++; }

                if (nr_in_use > 1)
                {
                    // limit to 26 chars
                    name_home = LimitNameLength(name_home, 14);
                    name_work = LimitNameLength(name_work, 14);
                    name_mobile = LimitNameLength(name_mobile, 14);

                    // then add 5 additional chars
                    name_home += " H";
                    name_work += " W";
                    name_mobile += " M";
                }
                else
                {
                    // limit to 31 chars
                    name_home = LimitNameLength(name_home, 16);
                    name_work = LimitNameLength(name_work, 16);
                    name_mobile = LimitNameLength(name_mobile, 16);
                }

                // write contact line, maybe twice
                if (CleanUpNumberHome != "")
                {
                    try
                    {
                        MySaveDataHash.Add(name_home, name_home + "," + CleanUpNumberHome + "\r\n");
                    }
                    catch (ArgumentException) // unable to add to MySaveFaxDataHash, must mean that something with (modified) contactData.combinedname is already in there!
                    {
                        MessageBox.Show("Unable to export the shortened entry \"" + name_home + "\",\r\nanother entry with this name already exists!\r\n\r\nIgnoring duplicate for contact source: " + contactData.combinedname);
                    }
                }

                if (CleanUpNumberWork != "")
                {
                    try
                    {
                        MySaveDataHash.Add(name_work, name_work + "," + CleanUpNumberWork + "\r\n");
                    }
                    catch (ArgumentException) // unable to add to MySaveFaxDataHash, must mean that something with (modified) contactData.combinedname is already in there!
                    {
                        MessageBox.Show("Unable to export the shortened entry \"" + name_work + "\",\r\nanother entry with this name already exists!\r\n\r\nIgnoring duplicate for contact source: " + contactData.combinedname);
                    }
                }

                if (CleanUpNumberMobile != "")
                {
                    try
                    {
                        MySaveDataHash.Add(name_mobile, name_mobile + "," + CleanUpNumberMobile + "\r\n");
                    }
                    catch (ArgumentException) // unable to add to MySaveFaxDataHash, must mean that something with (modified) contactData.combinedname is already in there!
                    {
                        MessageBox.Show("Unable to export the shortened entry \"" + name_mobile + "\",\r\nanother entry with this name already exists!\r\n\r\nIgnoring duplicate for contact source: " + contactData.combinedname);
                    }
                }

            } // end of foreach loop for the contacts

            // retrieve stuff from hastable and put in resultstring:
            foreach (System.Collections.DictionaryEntry saveDataHash in MySaveDataHash)
            {
                resultstring += (string)saveDataHash.Value;
            }

            // actually write the file to disk
            System.IO.File.WriteAllText(filename, resultstring, Encoding.GetEncoding("ISO-8859-1"));
        }

        private void save_data_AastraCSV(string filename, System.Collections.Hashtable workDataHash)
        {
            // get the country ID from the combobox or from user input
            string country_id = RetrieveCountryID(combo_prefix.SelectedItem.ToString());

            // process with exporting
            StringBuilder resultSB = new StringBuilder();

            System.Collections.Hashtable MySaveDataHash = new System.Collections.Hashtable();

            foreach (System.Collections.DictionaryEntry contactHash in workDataHash)
            {
                // extract GroupDataList from hashtable contents
                GroupDataContact contactData = (GroupDataContact)contactHash.Value;

                //  check if all relevant phone numbers for this export here are empty
                if (contactData.home == string.Empty && contactData.work == string.Empty && contactData.mobile == string.Empty)
                { continue; /* if yes, abort this foreach loop and contine to the next one */ }

                string prefix_string = (cfg_prefixNONFB == true) ? contactData.FRITZprefix : string.Empty;

                // clean up phone number
                string CleanUpNumberHome = CleanUpNumber(contactData.home, country_id, prefix_string);
                string CleanUpNumberWork = CleanUpNumber(contactData.work, country_id, prefix_string);
                string CleanUpNumberMobile = CleanUpNumber(contactData.mobile, country_id, prefix_string);

                // limit to 31 chars
                string name_home = LimitNameLength(contactData.combinedname, 31).Replace(",", " ");
                string name_work = LimitNameLength(contactData.combinedname, 31).Replace(",", " ");
                string name_mobile = LimitNameLength(contactData.combinedname, 31).Replace(",", " ");

                // write contact lines
                if (CleanUpNumberHome != "")
                {
                    try
                    {
                        MySaveDataHash.Add(name_home + "#Home", name_home + "," + CleanUpNumberHome + ",1,Home,public\n");
                    }
                    catch (ArgumentException) // unable to add
                    {
                        MessageBox.Show("Unable to export the shortened entry \"" + name_home + "#Home" + "\",\r\nanother entry with this name already exists!\r\n\r\nIgnoring duplicate for contact source: " + contactData.combinedname);
                    }
                }

                if (CleanUpNumberWork != "")
                {
                    try
                    {
                        MySaveDataHash.Add(name_work + "#Work", name_work + "," + CleanUpNumberWork + ",1,Work,public\n");
                    }
                    catch (ArgumentException) // unable to add 
                    {
                        MessageBox.Show("Unable to export the shortened entry \"" + name_work + "#Work" + "\",\r\nanother entry with this name already exists!\r\n\r\nIgnoring duplicate for contact source: " + contactData.combinedname);
                    }
                }

                if (CleanUpNumberMobile != "")
                {
                    try
                    {
                        MySaveDataHash.Add(name_mobile + "#Mobile", name_mobile + "," + CleanUpNumberMobile + ",1,Mobile,public\n");
                    }
                    catch (ArgumentException) // unable to add
                    {
                        MessageBox.Show("Unable to export the shortened entry \"" + name_mobile + "#Mobile" + "\",\r\nanother entry with this name already exists!\r\n\r\nIgnoring duplicate for contact source: " + contactData.combinedname);
                    }
                }
            } // end of foreach loop for the contacts

            // retrieve stuff from hastable and put in resultstring:
            foreach (System.Collections.DictionaryEntry saveDataHash in MySaveDataHash)
            {
                resultSB.Append((string)saveDataHash.Value);
            }

            // actually write the file to disk
            System.Text.Encoding utf8WithoutBom = new System.Text.UTF8Encoding(false);
            System.IO.File.WriteAllText(filename, resultSB.ToString(), utf8WithoutBom);

        }

        private void save_data_GrandstreamXml(string filename, System.Collections.Hashtable workDataHash)
        {
            // get the country ID from the combobox or from user input
            string country_id = RetrieveCountryID(combo_prefix.SelectedItem.ToString());

            // create output path for fonpix, if necessary
            string pic_export_path = "";
            if (combo_picexport.SelectedIndex == 1)
            {
                pic_export_path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filename), System.IO.Path.GetFileNameWithoutExtension(filename) + " - fonpix-custom");
                if (!System.IO.Directory.Exists(pic_export_path)) { System.IO.Directory.CreateDirectory(pic_export_path); }
            }

            // process with exporting
            StringBuilder resultSB = new StringBuilder();

            // write the header
            resultSB.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<AddressBook>\n  <version>1</version>");

            // initialize hashtable to store generated data results
            System.Collections.Hashtable MySaveDataHash = new System.Collections.Hashtable();

            foreach (System.Collections.DictionaryEntry contactHash in workDataHash)
            {
                StringBuilder contactSB = new StringBuilder();

                // extract GroupDataList from hashtable contents
                GroupDataContact contactData = (GroupDataContact)contactHash.Value;

                // check if all relevant phone numbers for this export here are empty
                if (contactData.home == string.Empty && contactData.work == string.Empty && contactData.mobile == string.Empty)
                { continue; /* if yes, abort this foreach loop and contine to the next one */ }

                string FirstName = contactData.firstname;
                string LastName = contactData.lastname;

                string SaveAsName = contactData.combinedname;

                // limit to 32 chars
                SaveAsName = LimitNameLength(SaveAsName, 32);

                // replace "&","<",">"
                SaveAsName = SaveAsName.Replace("&", "&amp;");
                SaveAsName = SaveAsName.Replace("<", "&lt;");
                SaveAsName = SaveAsName.Replace(">", "&gt;");

                // clean up phone number
                string CleanUpNumberHome = CleanUpNumber(contactData.home, country_id, contactData.FRITZprefix);
                string CleanUpNumberWork = CleanUpNumber(contactData.work, country_id, contactData.FRITZprefix);
                string CleanUpNumberMobile = CleanUpNumber(contactData.mobile, country_id, contactData.FRITZprefix);

                contactSB.Append("\n  <Contact>\n    <FirstName>" + FirstName + "</FirstName>\n    <LastName>" + LastName + "</LastName>");

                // add home phone number
                if (CleanUpNumberHome != "")
                {
                    contactSB.Append("\n    <Phone>\n      <phonenumber>" + CleanUpNumberHome + "</phonenumber>\n      <accountindex>0</accountindex>\n    </Phone>");
                }

                // add work phone number
                if (CleanUpNumberWork != "")
                {
                    contactSB.Append("\n    <Phone>\n      <phonenumber>" + CleanUpNumberWork + "</phonenumber>\n      <accountindex>0</accountindex>\n    </Phone>");
                }

                // add mobile phone number
                if (CleanUpNumberMobile != "")
                {
                    contactSB.Append("\n    <Phone>\n      <phonenumber>" + CleanUpNumberMobile + "</phonenumber>\n      <accountindex>0</accountindex>\n    </Phone>");
                }

                contactSB.Append("\n    <Group>0</Group>\n    <PhotoUrl></PhotoUrl>\n    <RingtoneUrl>./</RingtoneUrl>\n    <RingtoneIndex>0</RingtoneIndex>\n  </Contact>");

                try
                {
                    MySaveDataHash.Add(SaveAsName, contactSB.ToString());
                }
                catch (ArgumentException) // unable to add to MySaveDataHash, must mean that something with saveasname is already in there!
                {
                    MessageBox.Show("Unable to export the entry \"" + SaveAsName + "\", another entry with this name already exists! Ignoring duplicate.");
                }

            } // end of foreach loop for the contacts

            foreach (System.Collections.DictionaryEntry saveDataHash in MySaveDataHash)
            { resultSB.Append((string)saveDataHash.Value); }

            // write file footer.
            resultSB.Append("</AddressBook>");

            // actually write the file to disk
            System.IO.File.WriteAllText(filename, resultSB.ToString(), Encoding.GetEncoding("UTF-8"));

            // tell the user this has been done
            string errorwarning = "";
            if (MySaveDataHash.Count > 500) { errorwarning = Environment.NewLine + "Warning: Over 500 contacts have been exported! This is not supported by the Grandstream GXV 3140. Proceed with care!"; }
            MessageBox.Show(MySaveDataHash.Count + " contacts written to " + filename + " !" + Environment.NewLine + errorwarning);

        }


        private void button_config_Click(object sender, EventArgs e)
        {
            EditConfiguration myEditConf = new EditConfiguration();
            myEditConf.ShowDialog();
        }

        private void myConfig_Load()
        {
            string FullCFGfilePath = MySaveFolder + System.IO.Path.DirectorySeparatorChar + "CCW.config";

            if (System.IO.File.Exists(FullCFGfilePath))
            {
                System.IO.StreamReader file_cleartext_read;
                file_cleartext_read = new System.IO.StreamReader(FullCFGfilePath, Encoding.UTF8);
                string curline;
                StringBuilder builder = new StringBuilder();
                while ((curline = file_cleartext_read.ReadLine()) != null)
                {
                    // hachre CCW Bugfix: 3.0.0.2 (ComboBox loading Problem in Mono) - previously: builder.Append(curline + "\r\n");
                    builder.Append(curline + Environment.NewLine);
                }
                file_cleartext_read.Close();

                // then regEx Split by NewLine into array of lines
                string[] allParseLines = System.Text.RegularExpressions.Regex.Split(builder.ToString(), Environment.NewLine);

                foreach (string ParseLine in allParseLines)
                {
                    if (ParseLine == string.Empty) { continue; } // skip empty lines
                    if (ParseLine.IndexOf("\t") == -1) { continue; } // skip lines without TAB
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "cfg_hideemptycols") { cfg_hideemptycols = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "cfg_adjustablecols") { cfg_adjustablecols = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "cfg_prefixNONFB") { cfg_prefixNONFB = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "cfg_fritzWorkFirst") { cfg_fritzWorkFirst = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "cfg_importOther") { cfg_importOther = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "cfg_OLpics") { cfg_OLpics = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "clean_brackets") { clean_brackets = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "clean_hashkey") { clean_hashkey = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "clean_slash") { clean_slash = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "clean_hyphen") { clean_hyphen = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "clean_xchar") { clean_xchar = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }

                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "combo_namestyle") { combo_namestyle.SelectedItem = ParseLine.Substring(ParseLine.IndexOf("\t") + 1); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "combo_typeprefer") { combo_typeprefer.SelectedItem = ParseLine.Substring(ParseLine.IndexOf("\t") + 1); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "combo_outlookimport") { combo_outlookimport.SelectedItem = ParseLine.Substring(ParseLine.IndexOf("\t") + 1); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "combo_VIP") { combo_VIP.SelectedItem = ParseLine.Substring(ParseLine.IndexOf("\t") + 1); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "combo_prefix") { combo_prefix.SelectedItem = ParseLine.Substring(ParseLine.IndexOf("\t") + 1); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "combo_picexport") { combo_picexport.SelectedItem = ParseLine.Substring(ParseLine.IndexOf("\t") + 1); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "fritzPicPath") { textBox_PicPath.Text = ParseLine.Substring(ParseLine.IndexOf("\t") + 1); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "g_login") { g_login = ParseLine.Substring(ParseLine.IndexOf("\t") + 1); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "g_pass") { g_pass = ParseLine.Substring(ParseLine.IndexOf("\t") + 1); continue; }
                }

            }
        }

        private void myConfig_Save()
        {
            string FullCFGfilePath = MySaveFolder + System.IO.Path.DirectorySeparatorChar + "CCW.config";
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("cfg_hideemptycols" + "\t" + cfg_hideemptycols.ToString());
            sb.AppendLine("cfg_adjustablecols" + "\t" + cfg_adjustablecols.ToString());
            sb.AppendLine("cfg_prefixNONFB" + "\t" + cfg_prefixNONFB.ToString());
            sb.AppendLine("cfg_fritzWorkFirst" + "\t" + cfg_fritzWorkFirst.ToString());
            sb.AppendLine("cfg_importOther" + "\t" + cfg_importOther.ToString());
            sb.AppendLine("cfg_OLpics" + "\t" + cfg_OLpics.ToString());
            sb.AppendLine("clean_brackets" + "\t" + clean_brackets.ToString());
            sb.AppendLine("clean_hashkey" + "\t" + clean_hashkey.ToString());
            sb.AppendLine("clean_slash" + "\t" + clean_slash.ToString());
            sb.AppendLine("clean_hyphen" + "\t" + clean_hyphen.ToString());
            sb.AppendLine("clean_xchar" + "\t" + clean_xchar.ToString());

            sb.AppendLine("combo_namestyle" + "\t" + combo_namestyle.SelectedItem.ToString());
            sb.AppendLine("combo_typeprefer" + "\t" + combo_typeprefer.SelectedItem.ToString());
            sb.AppendLine("combo_outlookimport" + "\t" + combo_outlookimport.SelectedItem.ToString());
            sb.AppendLine("combo_VIP" + "\t" + combo_VIP.SelectedItem.ToString());
            sb.AppendLine("combo_prefix" + "\t" + combo_prefix.SelectedItem.ToString());
            sb.AppendLine("combo_picexport" + "\t" + combo_picexport.SelectedItem.ToString());

            sb.AppendLine("fritzPicPath" + "\t" + textBox_PicPath.Text);
            sb.AppendLine("g_login" + "\t" + g_login);
            sb.AppendLine("g_pass" + "\t" + g_pass);

            System.IO.File.WriteAllText(FullCFGfilePath, sb.ToString(), Encoding.UTF8);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            myConfig_Save();
        }

        private void button_7390_Click(object sender, EventArgs e)
        {
            textBox_PicPath.Text = "file:///var/InternerSpeicher/FRITZ/fonpix-custom/";
        }

        private void button_7270_Click(object sender, EventArgs e)
        {
            textBox_PicPath.Text = "file:///var/media/ftp/<USB-Stick-Name>/FRITZ/fonpix/";
        }

        private static byte[] GooglePhotoGet(ContactEntry contactoGoogle, string AuthToken)
        { 
            System.Net.HttpWebResponse myResponse;
            System.Net.HttpWebRequest MyRq;

            // do the login and get the cookies
            MyRq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(contactoGoogle.PhotoUri);
            MyRq.Headers.Add("Authorization", "GoogleLogin auth=" + AuthToken);

            myResponse = (System.Net.HttpWebResponse)MyRq.GetResponse();

            System.IO.Stream responseStream = myResponse.GetResponseStream();
            byte[] data = ReadFully(responseStream, myResponse.ContentLength);

            return data;
        }
    
        public static byte[] ReadFully (System.IO.Stream stream, long initialLength)
    {
    // If we've been passed an unhelpful initial length, just
    // use 32K.
    if (initialLength < 1)
    {
        initialLength = 32768;
    }
    
    byte[] buffer = new byte[initialLength];
    int read=0;
    
    int chunk;
    while ( (chunk = stream.Read(buffer, read, buffer.Length-read)) > 0)
    {
        read += chunk;
        
        // If we've reached the end of our buffer, check to see if there's
        // any more information
        if (read == buffer.Length)
        {
            int nextByte = stream.ReadByte();
            
            // End of stream? If so, we're done
            if (nextByte==-1)
            {
                return buffer;
            }
            
            // Nope. Resize the buffer, put in the byte we've just
            // read, and continue
            byte[] newBuffer = new byte[buffer.Length*2];
            Array.Copy(buffer, newBuffer, buffer.Length);
            newBuffer[read]=(byte)nextByte;
            buffer = newBuffer;
            read++;
        }
    }
    // Buffer is now too big. Shrink it.
    byte[] ret = new byte[read];
    Array.Copy(buffer, ret, read);
    return ret;
    }

    }

    public class GroupDataContact
    {
        string int_preferred;

        public string firstname;
        public string lastname;
        public string company;
        public string home;
        public string work;
        public string homefax;
        public string workfax;
        public string mobile;
        public string combinedname;
        public string isVIP;
        public string FRITZprefix;
        public string speeddial;
        public byte[] jpeg;
        public string preferred
        {
            get
            {
                string returnvalue = int_preferred;

                if (returnvalue == "")
                {
                    if (home != "")
                    { returnvalue = "home"; }
                    else
                        if (work != "")
                        { returnvalue = "work"; }
                        else
                            if (mobile != "")
                            { returnvalue = "mobile"; }
                }

                return returnvalue;
            }
            set
            {
                int_preferred = value;
            }
        }
        public string street;
        public string zip;
        public string city;
        public string email;

        public GroupDataContact()
        {
            // initialize all public strings to empty values when creating an empty contact
            firstname = "";
            lastname = "";
            company = "";
            home = "";
            work = "";
            homefax = "";
            workfax = "";
            mobile = "";
            int_preferred = "";
            street = "";
            zip = "";
            city = "";
            email = "";
            combinedname = "";
            isVIP = "";
            speeddial = "";
            jpeg = null;
        }

    }         // class to store all the information collected about a contact before adding it to a hashtable of those

    public class vCardAddressParser
    {
        string[] data_received;


        public string postofficebox;    // 0: 
        public string extendedaddress;  // 1: google: street+ \n\ 
        public string streetaddress;    // 2:
        public string locality;         // 3: (e.g., city);
        public string region;           // 4: (e.g., state or province);
        public string postalcode;       // 5: 
        public string country;          // 6: 

        public string parsed_street;    // aus 2 holen, falls leer aus 1 ?
        public string parsed_zip;       // aus 5 holen, falls leer aus 1 ?
        public string parsed_city;      // aus 3 holen, falls leer aus 1 ?

        public vCardAddressParser(string my_data_received)
        {
            data_received = my_data_received.Split(';');

            if (data_received.Length > 0) { postofficebox = data_received[0]; }
            if (data_received.Length > 1) { extendedaddress = data_received[1]; }
            if (data_received.Length > 2) { streetaddress = data_received[2]; }
            if (data_received.Length > 3) { locality = data_received[3]; }
            if (data_received.Length > 4) { region = data_received[4]; }
            if (data_received.Length > 5) { postalcode = data_received[5]; }
            if (data_received.Length > 6) { country = data_received[6]; }

            if (streetaddress == "" && postalcode == "" && locality == "" && extendedaddress != "")  // if no information present in normal fields and we have extended information
            {   // now parse the suff we have and fill the parsed fields with stuff that makes more sense:

                // first do some cleanups with googles newline insertions
                while (extendedaddress.StartsWith("\\n") == true) // remove line breaks after text
                {
                    extendedaddress = extendedaddress.Substring("\\n".Length);
                }
                while (extendedaddress.EndsWith("\\n") == true) // remove line breaks before text starts
                {
                    extendedaddress = extendedaddress.Substring(0, extendedaddress.Length - "\\n".Length);
                }
                while (extendedaddress.Contains("\\n\\n") == true) // remove duplicate line breaks
                {
                    extendedaddress = extendedaddress.Replace("\\n\\n", "\\n");
                }

                // if the extended information does not contain multiple pieces of information
                if (extendedaddress.Contains("\\n") == false)
                {
                    // just store it as street and leave the rest empty
                    parsed_street = extendedaddress;
                    parsed_zip = "";
                    parsed_city = "";

                }
                else
                {
                    // if there are multiple lines of text in the extended info, separate them by \n
                    parsed_street = extendedaddress.Substring(0, extendedaddress.IndexOf("\\n"));
                    extendedaddress = extendedaddress.Substring(extendedaddress.IndexOf("\\n") + "\\n".Length);

                    if (extendedaddress.Contains(" ") == true)
                    { // split again into zip and city
                        parsed_zip = extendedaddress.Substring(0, extendedaddress.IndexOf(" "));
                        extendedaddress = extendedaddress.Substring(extendedaddress.IndexOf(" ") + " ".Length);
                        parsed_city = extendedaddress.Replace("\\n", " - ");
                    }
                    else
                    { // just put everything into city
                        parsed_zip = "";
                        parsed_city = extendedaddress.Replace("\\n", " - ");
                    }

                }




            }
            else
            {
                // use the normal stuff for parsed information
                parsed_street = streetaddress;
                parsed_zip = postalcode;
                parsed_city = locality;
            }

        }

    }       // small helper method to parse Addresse stored in vCard lines

    public class ReadDataReturn
    {
        public string duplicates;
        public System.Collections.Hashtable importedHash = new System.Collections.Hashtable();

        public ReadDataReturn(string my_duplicates, System.Collections.Hashtable my_importedHash)
        {
            duplicates = my_duplicates;
            importedHash = my_importedHash;
        }
    }           // small helper class for the return values of the data/file reader methods

}
