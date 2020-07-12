using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Web;

using Google.GData.Contacts;
using Google.GData.Client;
using Google.GData.Extensions;
using Google.Contacts;

using System.IO;
using System.Collections;
using System.Net;

using System.Linq;

namespace Contact_Conversion_Wizard
{
    public partial class Form1 : Form
    {
        public static bool cfg_hideemptycols = false;
        public static bool cfg_adjustablecols = false;
        public static bool cfg_prefixNONFB = false;
        public static bool cfg_importOther = true;
        public static bool clean_brackets = true;
        public static bool clean_slash = true;
        public static bool clean_hashkey = true;
        public static bool clean_hyphen = true;
        public static bool clean_xchar = true;
        public static bool clean_space = true;
        public static bool clean_squarebrackets = true;
        public static bool clean_letters = true;
        public static bool clean_addzeroprefix = false;


        public static bool cfg_DUPren = false;
        public static bool cfg_checkVersion = true;


        public static string g_login = "";
        public static string g_pass = "";

        public static System.Text.Encoding utf8WithoutBom = new System.Text.UTF8Encoding(false);


        System.Collections.Hashtable myGroupDataHash;
        public static string MySaveFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + System.IO.Path.DirectorySeparatorChar + "ContactConversionWizard";

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



            if (cfg_checkVersion == true)
            {
                backgroundWorker_updateCheck.RunWorkerAsync();
            }

        }

        private string CleanUpNumber(string number, string country_prefix, string dial_prefix)
        {

            string return_str;

            if (number.Contains("@") == false)
            { // treat as phone number, not as email

                char[] buffer = new char[number.Length];
                int idx = 0;

                foreach (char c in number)
                {
                    // keep numbers, the international + sign and * for fritz!box prefixes
                    if ((c >= '0' && c <= '9') || (c == '+') || (c == '*'))
                    {
                        buffer[idx] = c; idx++;
                    }
                    else if (clean_brackets == false && ((c == '(') || (c == ')')))
                    { // keep brackets if option to clean them set to false
                        buffer[idx] = c; idx++;
                    }
                    else if (clean_hashkey == false && (c == '#'))
                    { // keep slash if option to clean it set to false
                        buffer[idx] = c; idx++;
                    }
                    else if (clean_hyphen == false && (c == '-'))
                    { // keep slash if option to clean it set to false
                        buffer[idx] = c; idx++;
                    }
                    else if (clean_xchar == false && (c == 'x'))
                    { // keep slash if option to clean it set to false
                        buffer[idx] = c; idx++;
                    }
                    else if (clean_slash == false && (c == '/'))
                    { // keep slash if option to clean it set to false
                        buffer[idx] = c; idx++;
                    }
                    else if (clean_space == false && (c == ' '))
                    { // keep space if option to clean it set to false
                        buffer[idx] = c; idx++;
                    }
                    else if (clean_squarebrackets == false && ((c == '[') || (c == ']')))
                    { // keep square brackets if option to clean them set to false
                        buffer[idx] = c; idx++;
                    }
                    else if (clean_letters == false && ((c >= 'a' && c <= 'z') || (c >= 'A') && (c <= 'Z')))
                    {
                        buffer[idx] = c; idx++;
                    }


                }
                return_str = new string(buffer, 0, idx);

                // clean up country code
                if (country_prefix != "keep")
                {
                    if (return_str.StartsWith("+")) { return_str = "00" + return_str.Substring(1); }
                    if (return_str.StartsWith(country_prefix)) { return_str = "0" + return_str.Substring(country_prefix.Length); }
                }

                // add fritz!prefix is detected in the source
                if (return_str != "") { return_str = dial_prefix + return_str; }

                // alway add "0" prefix for outside lines if the corresponding configuration option is active
                if (return_str != "" && clean_addzeroprefix == true ) { return_str = "0" + return_str; }



            }
            else
            { // treat as email
                return_str = number;

                if ((number.Contains("<") == true) || (number.Contains(">") == true))
                {
                    MessageBox.Show("Warning: eMail '" + number + "' contains '<' and/or '>' characters. This is probably not a good idea!");
                }

            }

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
                    SimpleInputDialog MySimpleInputDialog = new SimpleInputDialog("Please enter your local Country Prefix here (in the format 00x or 00xx)", "CustomCountryID", true, false);
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

        private string CheckPREFERflag(string myStringToCheck)
        {
            if (myStringToCheck.Contains("PREFER(") == true) // if something found in comments
            {
                string preferstring = myStringToCheck.Substring(myStringToCheck.IndexOf("PREFER(") + 7); // cut away everything before the keyword
                if (preferstring.Contains(")") == true)
                {
                    preferstring = preferstring.Substring(0, preferstring.IndexOf(")"));                       // cut away everything after the keyword contents
                    if (preferstring.ToLower() == "home") return "home";
                    if (preferstring.ToLower() == "work") return "work";
                    if (preferstring.ToLower() == "mobile") return "mobile";

                    // if no match, return empty string, so autodetection will work something out
                    return "";
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

                    if (partialstrings.Length == 2 || partialstrings.Length == 3)
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

        private void add_nr_to_hash(ref System.Collections.Hashtable myHash, string id_to_add, string str_to_add, string error_name)
        {
                try
                {
                    myHash.Add(id_to_add, str_to_add);
                }
                catch (ArgumentException) // unable to add to MySaveFaxDataHash, must mean that something with (modified) contactData.combinedname is already in there!
                {
                    MessageBox.Show("Unable to export entry \"" + error_name + "=>" + id_to_add + "\",\r\nanother entry with this name already exists - ignoring duplicate!\r\n");
                }

        }

        private string Do_XML_replace(string to_replace)
        {
            string ret_val = to_replace;

            ret_val = ret_val.Replace("&", "&amp;");
            ret_val = ret_val.Replace("\"", "&quot;");
            ret_val = ret_val.Replace("'", "&apos;");
            ret_val = ret_val.Replace("<", "&lt;");
            ret_val = ret_val.Replace(">", "&gt;");

            return ret_val;
        }


        private string Do_VCARD_replace(string to_replace)
        {
            string ret_val = to_replace;

            ret_val = ret_val.Replace("\\", "\\\\");
            ret_val = ret_val.Replace(";", "\\;");
            ret_val = ret_val.Replace(",", "\\,");
            ret_val = ret_val.Replace(":", "\\:");
            ret_val = ret_val.Replace("\r\n", "\\n"); // this has to be done first since it's more specific
            ret_val = ret_val.Replace("\n", "\\n"); // this has to be done later
            return ret_val;
        }


        private string Do_VCARD_replace_Gigaset(string to_replace)
        {
            string ret_val = to_replace;
            ret_val = ret_val.Replace("\\", "\\\\");
            ret_val = ret_val.Replace(";", "\\;");

            // these two must explicitly not be replaced on the dx600a
            // ret_val = ret_val.Replace(",", "\\,");
            // ret_val = ret_val.Replace(":", "\\:");

            // no newlines in gigaset
            ret_val = ret_val.Replace("\r\n", ""); // this has to be done first since it's more specific
            ret_val = ret_val.Replace("\n", ""); // this has to be done later
            return ret_val;
        }


        private bool CheckIfSeparateEntries(string nr1, string nr2, string nr3)
        {
            int nr_in_use = 0;
            if (nr1 != "") { nr_in_use++; }
            if (nr2 != "") { nr_in_use++; }
            if (nr3 != "") { nr_in_use++; }

            if (nr_in_use > 1)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        private string LimitNameLength(string my_name, int my_limit, bool separate_entries, string separate_string)
        {
            if (separate_entries == true)
            {
                my_limit = my_limit - separate_string.Length;
            }

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

            if (separate_entries == true)
            {
                my_name = my_name + separate_string;
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
                btn_save_vCard_Gigaset.Enabled = false;
                btn_save_FritzAdress.Enabled = false;
                btn_save_SnomCSV7.Enabled = false;
                btn_save_SnomCSV8.Enabled = false;
                btn_save_TalkSurfCSV.Enabled = false;
                btn_save_AastraCSV.Enabled = false;
                btn_save_GrandstreamGXV.Enabled = false;
                btn_save_GrandstreamGXP.Enabled = false;
                btn_save_Auerswald.Enabled = false;
                btn_save_googleContacts.Enabled = false;
                btn_save_panasonicCSV.Enabled = false;

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
                btn_save_vCard_Gigaset.Enabled = true;
                btn_save_FritzAdress.Enabled = true;
                btn_save_SnomCSV7.Enabled = true;
                btn_save_SnomCSV8.Enabled = true;
                btn_save_TalkSurfCSV.Enabled = true;
                btn_save_AastraCSV.Enabled = true;
                btn_save_GrandstreamGXV.Enabled = true;
                btn_save_GrandstreamGXP.Enabled = true;
                btn_save_Auerswald.Enabled = true;
                btn_save_googleContacts.Enabled = true;
                btn_save_panasonicCSV.Enabled = true;


                button_clear.Enabled = true;
                button_config.Enabled = true;

                Cursor.Current = Cursors.Default;
            }

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
                MyDataGridView.Columns[16].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            }

            // preparations in case cols should be hidden later
            bool hide_combinedname = true;
            bool hide_lastname = true;
            bool hide_firstname = true;
            bool hide_company = true;
            bool hide_home = true;
            bool hide_work = true;
            bool hide_mobile = true;
            bool hide_preferred = true;
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
                MyDataGridView.Columns[16].Visible = true;

                if (cfg_hideemptycols == true)
                {   // for each row, of col still hidden set it to false if relevant data in set for the col
                    if (hide_combinedname == true && contactData.combinedname != "") hide_combinedname = false;
                    if (hide_lastname == true && contactData.lastname != "") hide_lastname = false;
                    if (hide_firstname == true && contactData.firstname != "") hide_firstname = false;
                    if (hide_company == true && contactData.company != "") hide_company = false;
                    if (hide_home == true && contactData.home != "") hide_home = false;
                    if (hide_work == true && contactData.work != "") hide_work = false;
                    if (hide_mobile == true && contactData.mobile != "") hide_mobile = false;
                    if (hide_preferred == true && contactData.preferred != "") hide_preferred = false;
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

                MyDataGridView.Rows.Add(new string[] { contactData.combinedname, contactData.lastname, contactData.firstname, contactData.company, contactData.home, contactData.work, contactData.mobile, contactData.preferred, contactData.homefax, contactData.workfax, contactData.street, contactData.zip, contactData.city, contactData.email, contactData.isVIP, contactData.speeddial, PhotoPresent });
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
                MyDataGridView.Columns[16].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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
                if (hide_preferred == true) MyDataGridView.Columns[7].Visible = false;
                if (hide_homefax == true) MyDataGridView.Columns[8].Visible = false;
                if (hide_workfax == true) MyDataGridView.Columns[9].Visible = false;
                if (hide_street == true) MyDataGridView.Columns[10].Visible = false;
                if (hide_zip == true) MyDataGridView.Columns[11].Visible = false;
                if (hide_city == true) MyDataGridView.Columns[12].Visible = false;
                if (hide_email == true) MyDataGridView.Columns[13].Visible = false;
                if (hide_isVIP == true) MyDataGridView.Columns[14].Visible = false;
                if (hide_speeddial == true) MyDataGridView.Columns[15].Visible = false;
                if (hide_PhotoPresent == true) MyDataGridView.Columns[16].Visible = false;
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
                catch // (Exception e)
                { // do nothing, if somehow attachment processing leads to a crash (mostly in outlook 2003 with contact pictures, but maybe sometimes else)
                    // MessageBox.Show("crash: " + e.ToString());
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
            update_datagrid();

            disable_buttons(false);
        }   // just click handler

        private void btn_read_googleContacts_Click(object sender, EventArgs e)
        {
            // processing starts, so now we will disable the buttons first to make sure the user knows this by not having buttons to click on
            disable_buttons(true);

            // check whether the user had the shift key pressed while calling this function and store this in a variable for further use
            bool shiftpressed_for_custom_folder = false;

            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) { shiftpressed_for_custom_folder = true; }

            if (g_login == string.Empty)
            {
                MessageBox.Show("Please configure Google login in the configuration menu first!");
                disable_buttons(false);
                return;
            }

            string custom_google_pass = "";
            if (g_pass != string.Empty)
            {
                custom_google_pass = g_pass;
            }
            else
            {
                SimpleInputDialog MySimpleInputDialog = new SimpleInputDialog("Please enter the Google password for login " + g_login + ":", "Enter Google Password", false, true);
                MySimpleInputDialog.ShowDialog();
                custom_google_pass = MySimpleInputDialog.resultstring;
                MySimpleInputDialog.Dispose();
            }


            ReadDataReturn myReadDataReturn = read_data_googleContacts(shiftpressed_for_custom_folder, custom_google_pass);

            if (myReadDataReturn.duplicates != "") MessageBox.Show(myReadDataReturn.duplicates, "The following duplicate entries could not be imported");
            update_datagrid();

            disable_buttons(false);
        }    // just click handler

        private bool CheckOutlookVersion(string OL_version)
        {
            string OL_v;

            if (OL_version.Contains(".") == true && OL_version.Substring(0, 1) != ".")
            {
                OL_v = OL_version.Substring(0, OL_version.IndexOf("."));
            }
            else
            {
                MessageBox.Show("#1: Unable to parse Outlook Version number, please report this bug." + Environment.NewLine + "Outlook version was self-reported to be: '" + OL_version + "'");
                OL_v = "0";
            }

            int OL_v_int = 0;

            try
            {
                OL_v_int = Convert.ToInt32(OL_v);
            }
            catch
            {
                OL_v_int = 0;
                MessageBox.Show("#2: Unable to parse Outlook Version number, please report this bug." + Environment.NewLine + "Outlook version was self-reported to be: '" + OL_version + "'");
            }

            if (OL_v_int >= 12)
            {
                return(true);
            }
            else
            {
                return(false);
            }



        }

        private ReadDataReturn read_data_Outlook(bool customfolder, bool categoryfilter)
        {
            // read all information from Outlook and save all contacts that have at least one phone or fax number

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
                return (new ReadDataReturn(duplicates));
            }

            bool get_OLpics = CheckOutlookVersion(outlookObj.Version);

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
                SimpleInputDialog MySimpleInputDialog = new SimpleInputDialog("Please enter the string that must be present in the category field:", "Category Filter", false, false);
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
                if (get_OLpics == true && myContactItem.HasPicture == true)
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
                myContact.preferred = CheckPREFERflag(NotesBody);
                myContact.FRITZprefix = CheckPREFIXflag(NotesBody);

                if (myContact.combinedname != "" && (myContact.home != string.Empty || myContact.work != string.Empty || myContact.mobile != string.Empty || myContact.homefax != string.Empty || myContact.workfax != string.Empty) && (NotesBody.Contains("CCW-IGNORE") == false))
                {
                    addDataHash(ref duplicates, myContact);
                }
            }

            return (new ReadDataReturn(duplicates));

        }                   // should work fine

        private ReadDataReturn read_data_FritzXML(string filename)
        {
            System.Collections.Hashtable loadDataHash = new System.Collections.Hashtable();
            string duplicates = "";

            GroupDataContact myContact = new GroupDataContact();

            // auto detect encoding used in xml file!
            string line1 = "";
            using (StreamReader reader = new StreamReader(filename)) { line1 = reader.ReadLine(); }

            // set default in case we don't find something in the first line
            System.Text.Encoding fritzXMLreadencoding = utf8WithoutBom;

            if (line1.Contains("encoding=\"utf-8\"") == true)           { fritzXMLreadencoding = utf8WithoutBom; }
            else if (line1.Contains("encoding=\"iso-8859-1\"") == true) { fritzXMLreadencoding = Encoding.GetEncoding("ISO-8859-1"); }
            else                                                        { MessageBox.Show("Unable to determine file encoding from header, defaulting to: " + fritzXMLreadencoding.EncodingName.ToString()); }

            System.IO.StreamReader file1 = new System.IO.StreamReader(filename, fritzXMLreadencoding);

            try
            {

                System.Xml.XmlReaderSettings xml_settings = new System.Xml.XmlReaderSettings();
                xml_settings.ConformanceLevel = System.Xml.ConformanceLevel.Fragment;
                xml_settings.IgnoreWhitespace = true;
                xml_settings.IgnoreComments = true;
                System.Xml.XmlReader r = System.Xml.XmlReader.Create(file1, xml_settings);

                r.MoveToContent();

                string parsing_errors = "";
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
                        if (r.GetAttribute("type") == "fax_work")
                        {
                            if (combo_typeprefer.SelectedIndex == 0)
                            { // prefer storing as home
                                myContact.homefax = r.ReadElementContentAsString();
                            }
                            if (combo_typeprefer.SelectedIndex == 1)
                            { // prefer storing as work
                                myContact.workfax = r.ReadElementContentAsString();
                            }
                            continue;
                        }

                        // if we are still here, there is an attribute we don't know about
                        parsing_errors += "Unknown XML Attribute: " + r.GetAttribute("type") + "=" + r.ReadElementContentAsString();
                        continue;
                    }

                    if (parsing_errors != "")
                    {
                        MessageBox.Show("The following parsing errors have occured:" + Environment.NewLine + Environment.NewLine + parsing_errors);
                    }


                    // Now all information should be stored in the array, so save it to the hashtable!
                    addDataHash(ref duplicates, myContact);
                }

            }
            catch (System.Xml.XmlException e)
            {
                MessageBox.Show("error occured: " + e.Message);
            }

            file1.Close();
            return (new ReadDataReturn(duplicates));
        }                   // should work fine

        private ReadDataReturn read_data_vCard(string filename, bool non_unicode)
        {
            // read all information from the vCard file and save all contacts that have at least one phone or fax number
            System.Collections.Hashtable loadDataHash = new System.Collections.Hashtable();

            string duplicates = "";

            // do whats necessary to import a VCF File!
            // for further use: spec can be found here: http://www.ietf.org/rfc/rfc2426.txt

            GroupDataContact myContact = new GroupDataContact();
            string vcard_fullname = "";
            string vcard_notes = "";
            string vcard_nickname = "";

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

            bool crashed = false;
            StringBuilder crashlog = new StringBuilder();

            foreach (string ParseLine in vParseLines)
            {
                string currentline_for_crash = "";
                try
                {
                    // replace escaped ":" characters, they should not be a problem when parsing
                    string vParseLine = ParseLine.Replace("\\:", ":").Replace("\\\\", "\\").Replace("\\;", ",").Replace("\\,", ",");
                    currentline_for_crash = vParseLine;

                    // if line starts with item?, remove this to allow normal processing (apple used this, maybe somedays we need more advanced processing, but for now we just strip it)
                    if (vParseLine.StartsWith("item", StringComparison.OrdinalIgnoreCase) == true)
                    { vParseLine = vParseLine.Substring(vParseLine.IndexOf(".") + 1); }

                    if (vParseLine == "") { continue; } // skip empty lines

                    // separate vParseLine into Item, Options and Value
                    string vParseItem = vParseLine.Substring(0, vParseLine.IndexOf(":"));
                    string vParseOptions = "";

                    if (vParseItem.IndexOfAny(new char[] { ':', ';' }) != -1) // if item string has options attached to it, separate those from each other
                    {
                        vParseOptions = vParseItem;
                        vParseItem = vParseItem.Substring(0, vParseItem.IndexOfAny(new char[] { ':', ';' }));
                        vParseOptions = vParseOptions.Substring(vParseItem.Length + 1);
                    }

                    string vParseValue = vParseLine.Substring(vParseLine.IndexOf(":") + 1);

                    // maybe someday ==> If value is quoted printable we need to decode it first, I was unable to find proper working code so far :-(
                    // if (vParseOptions.ToUpper().Contains("CHARSET=UTF-8") && vParseOptions.ToUpper().Contains("ENCODING=QUOTED-PRINTABLE"))
                    // {
                        // MessageBox.Show("Now decoding:\r\n" + vParseValue + "\r\n" + UTF8Decode(vParseValue));
                        // this doesn't really work yet
                    // }

                    // MessageBox.Show(vParseLine + "\r\n==> ITEM: " + vParseItem + "\r\n==> OPT:" + vParseOptions + "\r\n==> VALUE: " + vParseValue);

                    if (vParseItem.ToUpper() == ("BEGIN") && (vParseValue.ToUpper() == "VCARD"))
                    { // reset global settings for contact
                        myContact = new GroupDataContact();
                        vcard_fullname = "";
                        vcard_notes = "";
                        vcard_nickname = "";
                        address_value_stored = 0;
                        email_value_stored = 0;
                        continue;
                    }
                    if (vParseItem.ToUpper() == "VERSION") { continue; }

                    if (vParseItem.ToUpper() == "NOTE")
                    {
                        vcard_notes = vParseValue;
                        continue;
                    }

                    if (vParseItem.ToUpper() == "N")
                    {
                        // this is firstname and lastname, extract those and ignore other stuff in this line
                        myContact.lastname = vParseValue.Substring(0, vParseValue.IndexOf(";")).Trim();
                        myContact.firstname = vParseValue.Substring(vParseValue.IndexOf(";") + 1).Trim();
                        if (myContact.firstname.Contains(";") == true) // this if is neccessary because windows live mail exports without trailing ; chars for unused fields
                        { myContact.firstname = myContact.firstname.Substring(0, myContact.firstname.IndexOf(";")).Trim(); }
                        continue;
                    }

                    if (vParseItem.ToUpper() == "FN")
                    {   // this is last name and first name, save to special storage string
                        vcard_fullname = vParseValue.Trim(); // holt den ganzen namen
                        continue;
                    }

                    if (vParseItem.ToUpper() == "NICKNAME")
                    {   // holt den ganzen nickname aus der zeile und speichert in für verarbeitung am ende
                        vcard_nickname = vParseValue.Trim();
                        continue;
                    }

                    if (vParseItem.ToUpper() == "ORG")
                    {
                        myContact.company = vParseValue.TrimEnd(';'); // if ends with ; (like on macos) remove trailing ;
                        myContact.company = myContact.company.Replace(';', ' ').Trim(); // if consists of multiple business subunits, combine in one field by removing separators
                        continue;
                    }

                    #region Process-TEL-Lines-in-vCard
                    if (vParseItem == "TEL")
                    {
                        // we are about to process a phone or fax number
                        string telnumber = vParseValue;

                        string types = vParseOptions.ToLower().Replace("type=", "");
                        string[] typearray = types.Split(new char[] { ';', ',' });

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
                    if (vParseItem == "ADR")
                    {
                        // we are about to process a home or work address
                        string address = vParseValue;

                        string types = vParseOptions.ToLower().Replace("type=", "");
                        string[] typearray = types.Split(new char[] { ';', ',' });

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
                    if (vParseItem == "EMAIL")
                    {
                        // we are about to process a home or work email address
                        string emailaddress = vParseValue;

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

                    if (vParseItem == "CATEGORIES") { continue; }

                    if (vParseItem == "X-ABUID") { continue; }

                    if (vParseItem == "PHOTO")
                    {
                        if (vParseOptions.ToUpper().Contains("BASE64") == true)
                        {
                            string encodedData = vParseValue.Replace(" ", "");
                            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
                            myContact.jpeg = encodedDataAsBytes;
                        }
                        continue;
                    }

                    if (vParseItem.ToUpper() == ("END") && (vParseValue.ToUpper() == "VCARD"))
                    {

                        // übergibt notes und nickname der methode die VIP und Speeddial extrahiert
                        myContact.isVIP = CheckVIPflag(vcard_nickname, vcard_notes, false);
                        myContact.speeddial = CheckSPEEDDIALflag(vcard_notes);
                        myContact.preferred = CheckPREFERflag(vcard_notes);
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
                            addDataHash(ref duplicates, myContact);
                        }

                    }
                }
            catch (Exception vcard_exception)
            {
                crashed = true;
                string crash_cause = vcard_exception.ToString().Replace(Environment.NewLine, " ");
                if (crash_cause.IndexOf("(") != -1) // make error cause more concise
                {
                    crash_cause = crash_cause.Substring(0, crash_cause.IndexOf("("));
                }
                crashlog.AppendLine(currentline_for_crash + Environment.NewLine + "==> " + crash_cause + Environment.NewLine);
            }

            } // end of long foreach loop

            if (crashed == true)
            {
                // Hier wäre es natürlich schöner ein Custom Form für die Fehlermeldung zu haben, in dem man auch Markieren&Kopieren in die Zwischenablage kann und das viel Text besser anzeigt (Textbox?)
                // habe aber keine Lust sowas zu implementieren momentan :-)
                MessageBox.Show("One or more lines in the vCard file could not be parsed and were ignored. If you think those lines were standard-compliant please report a bug in the IP-Phone-Forum with the vCard entry that caused the problem" + Environment.NewLine
                                + Environment.NewLine
                                + crashlog.ToString());
            }

            return (new ReadDataReturn(duplicates));
        }     // should work fine

        private ReadDataReturn read_data_FritzAdr(string filename) {
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
                    myContact.preferred = CheckPREFERflag(cDataArray[8]);
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
                        addDataHash(ref duplicates, myContact);
                    }
                } // end while loop going through the lines of the file
            }
            catch (Exception e)
            { MessageBox.Show("Error occured while parsing file: " + e.Message); }

            file1.Close();
            return (new ReadDataReturn(duplicates));
        }                   // should work fine

        private ReadDataReturn read_data_genericCSV(string filename)
        {
            // repare array that will contain the returned data
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
                                myContact.preferred = CheckPREFERflag(LineList[i][j]);
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
                    addDataHash(ref duplicates, myContact);
                }
            }

            return (new ReadDataReturn(duplicates));
        }

        private ReadDataReturn read_data_googleContacts(bool customfolder, string google_pass)
        {
            string duplicates = "";

            RequestSettings rs = new RequestSettings(this.ProductName + " v" + this.ProductVersion, g_login, google_pass);
            rs.AutoPaging = true; // AutoPaging results in automatic paging in order to retrieve all contacts
            ContactsRequest cr = new ContactsRequest(rs);

            Feed<Contact> f = cr.GetContacts();


            // this is where the actual login/pass check happens
            string der_token = "";
            try
            { // we need the token for later (photo download)
                der_token = cr.Service.QueryClientLoginToken();
            }
            catch (Exception ex)
            {
                string exception_message = ex.ToString();
                if (exception_message.Contains(" at ") == true)
                {
                    exception_message = exception_message.Substring(0, exception_message.IndexOf(" at "));
                }

                MessageBox.Show("Google login failed with the following exception:" + Environment.NewLine + Environment.NewLine + exception_message);
                return (new ReadDataReturn(duplicates));
            }


            List<string> ListSelectedGroups = new List<string>();

            if (customfolder == true)
            {
                Feed<Group> g = cr.GetGroups();
                List<string[]> ListAllGroups = new List<string[]>();
                foreach (Group ge in g.Entries) { ListAllGroups.Add(new string[] { ge.Title, ge.Id}); }

                ImportGroupsChooser MyGoogleImportGroupsChooser = new ImportGroupsChooser("Please select the groups you want to import contacts from:", "Google Contact Group Chooser", ListAllGroups);
                MyGoogleImportGroupsChooser.ShowDialog();

                foreach (string[] z in MyGoogleImportGroupsChooser.resultList)
                {   // add group identifyer to selected groups list
                    ListSelectedGroups.Add(z[1]);
                }

                MyGoogleImportGroupsChooser.Dispose();
            }

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



                    if (customfolder == true)
                    {
                        bool is_member_of_selected_group = false;

                        foreach (GroupMembership memberGroup in entry.GroupMembership)
                        {
                            foreach (string currentGroup in ListSelectedGroups)
                            {
                                if (memberGroup.HRef == currentGroup)
                                {
                                    is_member_of_selected_group = true;
                                    break; // no need to check further groups, we already know we want this contact
                                }
                            }
                        }

                        if (is_member_of_selected_group == false)
                        {
                            // MessageBox.Show("Skipping: " + myContact.lastname + "/" + myContact.firstname + "/" + entry.GroupMembership.Count.ToString());
                            continue;
                        }

                    }

                    string google_main_number = "";

                    foreach (PhoneNumber pnr in entry.Phonenumbers)
                    {
                        if (string.IsNullOrEmpty(pnr.Value) == true) { continue; } // if phone number string should be empty, go to next item

                        if (pnr.Rel == ContactsRelationships.IsHome) { if (myContact.home == string.Empty)    myContact.home = pnr.Value; }
                        if (pnr.Rel == ContactsRelationships.IsWork) { if (myContact.work == string.Empty)    myContact.work = pnr.Value; }
                        if (pnr.Rel == ContactsRelationships.IsMain) { google_main_number = pnr.Value; }
                        if (pnr.Rel == ContactsRelationships.IsHomeFax) { if (myContact.homefax == string.Empty) myContact.homefax = pnr.Value; }
                        if (pnr.Rel == ContactsRelationships.IsWorkFax) { if (myContact.workfax == string.Empty) myContact.workfax = pnr.Value; }
                        if (pnr.Rel == ContactsRelationships.IsMobile) { if (myContact.mobile == string.Empty)  myContact.mobile = pnr.Value; }
                    }


                    if (cfg_importOther == true && google_main_number != "")
                    { // if we are to import the other number and it is NOT empty, then

                        if (combo_typeprefer.SelectedIndex == 0)
                        { // prefer storing as home
                            if (myContact.home == string.Empty) { myContact.home = google_main_number; }
                            else if (myContact.work == string.Empty) { myContact.work = google_main_number; }
                        }
                        if (combo_typeprefer.SelectedIndex == 1)
                        { // prefer storing as work
                            if (myContact.work == string.Empty) { myContact.work = google_main_number; }
                            else if (myContact.home == string.Empty) { myContact.home = google_main_number; }
                        }
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
                        try
                        {
                            byte[] myAttachmentPhoto = GooglePhotoGet(entry.ContactEntry, der_token);
                            myContact.jpeg = myAttachmentPhoto;
                        }
                        catch
                        {
                            // do nothing
                        }

                    }

                    // all initial processing has finished, do some cleanup work
                    myContact.combinedname = GenerateFullName(myContact.firstname, myContact.lastname, myContact.company, combo_namestyle.SelectedIndex);

                    // check if contact is supposed to be VIP
                    string NotesBody = (string.IsNullOrEmpty(entry.Content)) ? string.Empty : entry.Content;
                    string nickname = (string.IsNullOrEmpty(entry.ContactEntry.Nickname)) ? string.Empty : entry.ContactEntry.Nickname;


                    myContact.isVIP = CheckVIPflag(nickname, NotesBody, false);
                    myContact.speeddial = CheckSPEEDDIALflag(NotesBody);
                    myContact.preferred = CheckPREFERflag(NotesBody);
                    myContact.FRITZprefix = CheckPREFIXflag(NotesBody);

                    if (myContact.combinedname != "" && (myContact.home != string.Empty || myContact.work != string.Empty || myContact.mobile != string.Empty || myContact.homefax != string.Empty || myContact.workfax != string.Empty) && (NotesBody.Contains("CCW-IGNORE") == false))
                    {
                        addDataHash(ref duplicates, myContact);
                    }


                }
                else
                    MessageBox.Show("An entry in the google contact database was retrieved that had no name, ignoring.");

            }
            return (new ReadDataReturn(duplicates));
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
            // check whether the user had the shift key pressed while calling this function and store this in a variable for further use
            bool shiftpressed_for_export_fax = false;
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) { shiftpressed_for_export_fax = true; }

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

            save_data_FritzXML(SaveXML_Dialog.FileName, myGroupDataHash, shiftpressed_for_export_fax);

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


        private void btn_save_vCard_Gigaset_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveVCF_Dialog = new SaveFileDialog();
            SaveVCF_Dialog.Title = "Select the Gigaset vCard file you wish to create";
            SaveVCF_Dialog.DefaultExt = "vcf";
            SaveVCF_Dialog.Filter = "VCF files (*.vcf)|*.vcf";
            SaveVCF_Dialog.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
            SaveVCF_Dialog.FileName = "Simplified vCard Export for Gigaset";

            if (SaveVCF_Dialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            disable_buttons(true);

            save_data_vCard_Gigaset(SaveVCF_Dialog.FileName, myGroupDataHash);

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
        }     // just click handler

        private void btn_save_GrandstreamGXP_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveXML_Dialog = new SaveFileDialog();
            SaveXML_Dialog.Title = "Select the GXP XML file you wish to create";
            SaveXML_Dialog.DefaultExt = "xml";
            SaveXML_Dialog.Filter = "XML files (*.xml)|*.xml";
            SaveXML_Dialog.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
            SaveXML_Dialog.FileName = "gs_phonebook";

            if (SaveXML_Dialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            disable_buttons(true);

            save_data_GrandstreamGXP(SaveXML_Dialog.FileName, myGroupDataHash);

            // and reenable user interface
            disable_buttons(false);

        }   // just click handler

        private void btn_save_GrandstreamGXV_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveXML_Dialog = new SaveFileDialog();
            SaveXML_Dialog.Title = "Select the GXV XML file you wish to create";
            SaveXML_Dialog.DefaultExt = "xml";
            SaveXML_Dialog.Filter = "XML files (*.xml)|*.xml";
            SaveXML_Dialog.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
            SaveXML_Dialog.FileName = "gs_phonebook";

            if (SaveXML_Dialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            disable_buttons(true);

            save_data_GrandstreamGXV(SaveXML_Dialog.FileName, myGroupDataHash);

            // and reenable user interface
            disable_buttons(false);
        }   // just click handler

        private void btn_save_Auerswald_Click(object sender, EventArgs e)   // just click handler
        {
            SaveFileDialog SaveCSV_Dialog = new SaveFileDialog();
            SaveCSV_Dialog.Title = "Select the Auerswald CSV file you wish to create";
            SaveCSV_Dialog.DefaultExt = "csv";
            SaveCSV_Dialog.Filter = "CSV files (*.csv)|*.csv";
            SaveCSV_Dialog.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
            SaveCSV_Dialog.FileName = "auerswald_csv";

            if (SaveCSV_Dialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            disable_buttons(true);

            save_data_AuerswaldCSV(SaveCSV_Dialog.FileName, myGroupDataHash);

            // and reenable user interface
            disable_buttons(false);
        }

        private void btn_save_googleContacts_Click(object sender, EventArgs e)    // just click handler
        {
            disable_buttons(true);

            if (g_login == string.Empty)
            {
                MessageBox.Show("Please configure Google login in the configuration menu first!");
                disable_buttons(false);
                return;
            }

            string custom_google_pass = "";
            if (g_pass != string.Empty)
            {
                custom_google_pass = g_pass;
            }
            else
            {
                SimpleInputDialog MySimpleInputDialog = new SimpleInputDialog("Please enter the Google password for login " + g_login + ":", "Enter Google Password", false, true);
                MySimpleInputDialog.ShowDialog();
                custom_google_pass = MySimpleInputDialog.resultstring;
                MySimpleInputDialog.Dispose();
            }

            save_data_googleContacts(myGroupDataHash, custom_google_pass);
            disable_buttons(false);
        }

        private void btn_save_panasonicCSV_Click(object sender, EventArgs e)
        {

            SaveFileDialog SaveXML_Dialog = new SaveFileDialog();
            SaveXML_Dialog.Title = "Select the CSV file you wish to create";
            SaveXML_Dialog.DefaultExt = "phb";
            SaveXML_Dialog.Filter = "PHB files (*.phb)|*.phb";
            SaveXML_Dialog.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
            SaveXML_Dialog.FileName = "PanasonicExport";

            if (SaveXML_Dialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            disable_buttons(true);
            save_data_PanasonicCSV(SaveXML_Dialog.FileName, myGroupDataHash);

            // and reenable user interface
            disable_buttons(false);
        }

        private void save_data_PanasonicCSV(string filename, System.Collections.Hashtable workDataHash)
        {
            // get the country ID from the combobox or from user input
            string country_id = RetrieveCountryID(combo_prefix.SelectedItem.ToString());

            // process with exporting
            StringBuilder resultSB = new StringBuilder();

            // write the header for panasonic
            resultSB.Append("; Panasonic Communications Co., Ltd. Phonebook DECT Version 2.00\r\n");

            System.Collections.Hashtable MySaveDataHash = new System.Collections.Hashtable();

            foreach (System.Collections.DictionaryEntry contactHash in workDataHash)
            {
                // extract GroupDataList from hashtable contents
                GroupDataContact contactData = (GroupDataContact)contactHash.Value;

                // clean up phone number
                string prefix_string = (cfg_prefixNONFB == true) ? contactData.FRITZprefix : string.Empty;
                string CleanUpNumberHome = CleanUpNumber(contactData.home, country_id, prefix_string);
                string CleanUpNumberWork = CleanUpNumber(contactData.work, country_id, prefix_string);
                string CleanUpNumberMobile = CleanUpNumber(contactData.mobile, country_id, prefix_string);

                //  check if all relevant clean phone numbers for this export here are empty
                if (CleanUpNumberHome == string.Empty && CleanUpNumberWork == string.Empty && CleanUpNumberMobile == string.Empty)
                { continue; /* if yes, abort this foreach loop and contine to the next one */ }


                // this is the list of numbers for the contact
                List<String> numbers = new List<string>();

                if (CleanUpNumberHome != "") { numbers.Add(CleanUpNumberHome); }
                if (CleanUpNumberWork != "") { numbers.Add(CleanUpNumberWork); }
                if (CleanUpNumberMobile != "") { numbers.Add(CleanUpNumberMobile); }

                // if there are no phone numbers remaining after cleanup, abort this loop
                if (numbers.Count == 0) { MessageBox.Show("Skipping " + contactData.combinedname); continue; }

                string name_home = LimitNameLength(contactData.combinedname, 16, false, "");
                string sep = ",";

                //maybe one day add possibility for menaingful mapping of groups. use constant 1 for the moment
                string contactgroup = "1";

                try
                {
                    MySaveDataHash.Add(name_home, "\"" + name_home + "\"" + sep + contactgroup + sep + numbers.Aggregate((current, next) => current + sep + next) + "\n");
                }
                catch (ArgumentException) // unable to add to MySaveFaxDataHash, must mean that something with (modified) contactData.combinedname is already in there!
               {
                    MessageBox.Show("Unable to export the shortened entry \"" + name_home + "\",\r\nanother entry with this name already exists!\r\n\r\nIgnoring duplicate for contact source: " + contactData.combinedname);
                }

            } // end of foreach loop for the contacts

            // retrieve stuff from hastable and put in resultstring:
            foreach (System.Collections.DictionaryEntry saveDataHash in MySaveDataHash)
           {
                resultSB.Append((string)saveDataHash.Value);
            }

            // actually write the file to disk
            System.IO.File.WriteAllText(filename, resultSB.ToString(), Encoding.GetEncoding("ISO-8859-1"));
        }

        private void save_data_googleContacts(System.Collections.Hashtable workDataHash, string google_pass)
        {
            // get the country ID from the combobox or from user input
            string country_id = RetrieveCountryID(combo_prefix.SelectedItem.ToString());

            RequestSettings rs = new RequestSettings(this.ProductName + " v" + this.ProductVersion, g_login, google_pass);
            rs.AutoPaging = true; // AutoPaging results in automatic paging in order to retrieve all contacts
            ContactsRequest cr = new ContactsRequest(rs);

            // this is where the actual login/pass check happens
            try
            {   // this line does absolutely nothing here (we don't save the result), but it checks if l/p is ok so we don't run into trouble later!
                cr.Service.QueryClientLoginToken();
            }
            catch (Exception ex)
            {
                string exception_message = ex.ToString();
                if (exception_message.Contains(" at ") == true)
                {
                    exception_message = exception_message.Substring(0, exception_message.IndexOf(" at "));
                }

                MessageBox.Show("Google login failed with the following exception:" + Environment.NewLine + Environment.NewLine + exception_message);
                return;
            }




            // List that holds the batch request entries.
            List<Contact> requestFeed = new List<Contact>();

            // iterate through the contacts in workDatahash and save them to the selected Outlook Folder
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
                Contact newEntry = new Contact();

                newEntry.Content = "";
                // VIP Functionality (fully implemented now)
                if (contactData.isVIP == "Yes") { newEntry.Content += "VIP" + Environment.NewLine; }

                // Speeddial Functionality
                if (contactData.speeddial != "") { newEntry.Content += "SPEEDDIAL(" + contactData.speeddial + ")"; }


                newEntry.Name = new Name() { FullName = contactData.combinedname, GivenName = contactData.firstname, FamilyName = contactData.lastname, }; // Set the contact's name.
                if (contactData.company != "") newEntry.Organizations.Add(new Organization() { Primary = true, Rel = Google.GData.Extensions.ContactsRelationships.IsWork, Name = contactData.company });
                if (contactData.email != "") newEntry.Emails.Add(new EMail() { Primary = true, Rel = ContactsRelationships.IsHome, Address = contactData.email }); // Set the contact's e-mail addresses.

                // primary phone number
                bool prefer_homephone = false;
                bool prefer_workphone = false;
                bool prefer_mobilephone = false;

                if (contactData.preferred == "home") { prefer_homephone = true; }
                if (contactData.preferred == "work") { prefer_workphone = true; }
                if (contactData.preferred == "mobile") { prefer_mobilephone = true; }

                if (CleanUpNumberWork != "") newEntry.Phonenumbers.Add(new PhoneNumber() { Primary = prefer_homephone, Rel = ContactsRelationships.IsWork, Value = CleanUpNumberWork, });// Set the contact's phone numbers.
                if (CleanUpNumberHome != "") newEntry.Phonenumbers.Add(new PhoneNumber() { Primary = prefer_workphone, Rel = ContactsRelationships.IsHome, Value = CleanUpNumberHome, });
                if (CleanUpNumberMobile != "") newEntry.Phonenumbers.Add(new PhoneNumber() { Primary = prefer_mobilephone, Rel = ContactsRelationships.IsMobile, Value = CleanUpNumberMobile, });

                // fax numbers
                if (CleanUpNumberHomefax != "") newEntry.Phonenumbers.Add(new PhoneNumber() { Rel = ContactsRelationships.IsHomeFax, Value = CleanUpNumberHomefax, });
                if (CleanUpNumberWorkfax != "") newEntry.Phonenumbers.Add(new PhoneNumber() { Rel = ContactsRelationships.IsWorkFax, Value = CleanUpNumberWorkfax, });

                if (combo_typeprefer.SelectedIndex == 0)
                { // preferred is home for import/export
                    newEntry.PostalAddresses.Add(new StructuredPostalAddress() { Rel = ContactsRelationships.IsHome, Primary = true, Street = contactData.street, City = contactData.city, Postcode = contactData.zip, }); // Set the contact's postal address
                }
                else
                { // preferred is work for import/export
                    newEntry.PostalAddresses.Add(new StructuredPostalAddress() { Rel = ContactsRelationships.IsWork, Primary = true, Street = contactData.street, City = contactData.city, Postcode = contactData.zip, }); // Set the contact's postal address
                }

                // Insert the new contact.
                //  Uri feedUri = new Uri(ContactsQuery.CreateContactsUri("default"));
                //  Contact createdEntry = cr.Insert(feedUri, newEntry);

                requestFeed.Add(newEntry);

            } // end of foreach loop for the contacts


            // Submit the batch request to the server.
            List<Contact> uploadFeed = new List<Contact>();

            for (int i = 0; i < requestFeed.Count; i++)
            {
                uploadFeed.Add(requestFeed[i]);

                if (uploadFeed.Count == 100 || (i+1) == requestFeed.Count)
                {
                    // upload and reset upLoadFeed every 100 items, or if at the end of requestFeed

                    Feed<Contact> responseFeed = cr.Batch(uploadFeed, new Uri("https://www.google.com/m8/feeds/contacts/default/full/batch"), GDataBatchOperationType.Default);
                    StringBuilder responseData = new StringBuilder();

                    // Check the status of each operation.
                    foreach (Contact entry in responseFeed.Entries)
                    {
                        if (entry.BatchData.Status.Code != 201)
                        { responseData.Append(entry.BatchData.Status.Code + " (" + entry.BatchData.Status.Reason + ")" + " / "); }
                    }

                    uploadFeed.Clear();

                    // tell the user what has been done
                    MessageBox.Show((i+1).ToString() + "/" + requestFeed.Count + " contacts have been written to the Google Folder 'Other Contacts'!" + Environment.NewLine + responseData.ToString());
                }
            }

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

            bool get_OLpics = CheckOutlookVersion(outlookObj.Version);

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
                if (get_OLpics == true && combo_picexport.SelectedIndex == 1 && contactData.jpeg != null)
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

        private void save_data_FritzXML(string filename, System.Collections.Hashtable workDataHash, bool export_fax)
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

            bool pics_actually_exported = false;

            foreach (System.Collections.DictionaryEntry contactHash in workDataHash)
            {
                StringBuilder contactSB = new StringBuilder();

                // extract GroupDataList from hashtable contents
                GroupDataContact contactData = (GroupDataContact)contactHash.Value;

                // clean up phone number
                string CleanUpNumberHome = CleanUpNumber(contactData.home, country_id, contactData.FRITZprefix);
                string CleanUpNumberWork = CleanUpNumber(contactData.work, country_id, contactData.FRITZprefix);
                string CleanUpNumberMobile = CleanUpNumber(contactData.mobile, country_id, contactData.FRITZprefix);
                string CleanUpNumberHomeFax = CleanUpNumber(contactData.homefax, country_id, contactData.FRITZprefix);
                string CleanUpNumberWorkFax = CleanUpNumber(contactData.workfax, country_id, contactData.FRITZprefix);

                // check if all relevant phone numbers for this export here are empty
                if (CleanUpNumberHome == string.Empty && CleanUpNumberWork == string.Empty && CleanUpNumberMobile == string.Empty)
                { continue; /* if yes, abort this foreach loop and contine to the next one */ }

                // limit to 32 chars and replace "&","<",">", """
                string SaveAsName = Do_XML_replace(LimitNameLength(contactData.combinedname, 32, false, ""));

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

                    pics_actually_exported = true;
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

                string fritzXMLfax = "";

                // add fax phone number
                if (export_fax == true && CleanUpNumberHomeFax != "")
                {
                    { fritzXMLfax += "<number type=\"fax_work\">" + CleanUpNumberHomeFax + "</number>\n"; }
                }
                if (export_fax == true && CleanUpNumberWorkFax != "")
                {
                    { fritzXMLfax += "<number type=\"fax_work\">" + CleanUpNumberWorkFax + "</number>\n"; }
                }

                // actually add stuff we have prepared above
                if (combo_typeprefer.SelectedIndex == 0)
                { // preferred is home for import/export
                    contactSB.Append(fritzXMLhome);
                    contactSB.Append(fritzXMLwork);
                }
                else
                {
                    contactSB.Append(fritzXMLwork);
                    contactSB.Append(fritzXMLhome);
                }

                contactSB.Append(fritzXMLmobile);
                contactSB.Append(fritzXMLfax);



                // add telephony end and services start
                contactSB.Append("</telephony>\n<services>\n");

                // depending on whether an eMail exists, add email or not
                if (contactData.email != "")
                {
                    contactSB.Append("<email classifier=\"private\">" + Do_XML_replace(contactData.email) + "</email>");
                    contactSB.Append("<email />\n");
                }
                else
                {
                    contactSB.Append("<email />\n");
                }

                // add serviced end and rest of contact footer
                contactSB.Append("</services>\n<setup>\n<ringTone />\n<ringVolume />\n</setup>\n</contact>");

                // add contact line
                add_nr_to_hash(ref MySaveDataHash, SaveAsName, contactSB.ToString(), SaveAsName);

            } // end of foreach loop for the contacts

            foreach (System.Collections.DictionaryEntry saveDataHash in MySaveDataHash)
            { resultSB.Append((string)saveDataHash.Value); }

            // write file footer with AVM HD Music test stuff, currently not added. Not sure if it's a good idea to add this always.
            // resultstring += "<contact><category /><person><realName>~AVM-HD-Musik</realName></person><telephony><number\nprio=\"1\" type=\"home\" quickdial=\"98\">200@hd-telefonie.avm.de</number></telephony><services /><setup /></contact><contact><category /><person><realName>~AVM-HD-Sprache</realName></person><telephony><number\nprio=\"1\" type=\"home\" quickdial=\"99\">100@hd-telefonie.avm.de</number></telephony><services /><setup /></contact>";
            resultSB.Append("</phonebook>\n</phonebooks>");

            // actually write the file to disk
            System.IO.File.WriteAllText(filename, resultSB.ToString(), Encoding.GetEncoding("ISO-8859-1"));

            // tell the user this has been done
            string errorwarning = "";
            if (MySaveDataHash.Count > 300) { errorwarning = Environment.NewLine + "Warning: Over 300 contacts have been exported! This might or might not be officially supported by AVM's Fritz!Box and may cause problems."; }
            MessageBox.Show(MySaveDataHash.Count + " contacts written to " + filename + " !" + Environment.NewLine + errorwarning);

            if (textBox_PicPath.Text.Contains("fonpix-custom") == true && pics_actually_exported == true)
            {
                string targetPath = @"\\fritz.nas\FRITZ.NAS\FRITZ\fonpix-custom\";

                DialogResult dialogResult = MessageBox.Show("Do you want CCW to try copying the contact pictures to " + targetPath + " ?" + Environment.NewLine + "This can only work if your windows user already has access rights to this network share!" + Environment.NewLine + "Existing files in this directory will be removed!", "Try copying pictures to \\\\FRITZ.NAS\\ ?", MessageBoxButtons.YesNo);

                try
                {

                    if (dialogResult == DialogResult.Yes)
                    {

                        if (!System.IO.Directory.Exists(targetPath))
                        {
                            System.IO.Directory.CreateDirectory(targetPath);
                        }

                        foreach (FileInfo f in new DirectoryInfo(targetPath).GetFiles("*.jpg"))
                        {
                            f.Delete();
                        }

                        foreach (FileInfo f in new DirectoryInfo(pic_export_path).GetFiles("*.jpg"))
                        {
                            f.CopyTo(System.IO.Path.Combine(targetPath, f.Name), true);
                        }

                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        //do something else
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Copying failed with exception: " + e.ToString());
                }

            }

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
                string CleanUpStreet = Do_VCARD_replace(contactData.street);
                string CleanUpCity = Do_VCARD_replace(contactData.city);
                string CleanUpZIP = Do_VCARD_replace(contactData.zip);
                string CleanUpeMail = Do_VCARD_replace(contactData.email);
                string CleanUpFirstName = Do_VCARD_replace(contactData.firstname);
                string CleanUpLastName = Do_VCARD_replace(contactData.lastname);
                string CleanUpCompany = Do_VCARD_replace(contactData.company);
                string CleanUpCombined = Do_VCARD_replace(contactData.combinedname);

                // if we only wish to export phone numbers => check if all relevant phone fields for this export are empty
                if (export_only_phone == true && (CleanUpNumberHome == string.Empty && CleanUpNumberWork == string.Empty && CleanUpNumberMobile == string.Empty))
                {
                    // MessageBox.Show("Contact |" + CleanUpCombined + "| ignored, due to missing numbers");
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

                string ADRtype = "WORK";
                if (combo_typeprefer.SelectedIndex == 0)
                { // preferred is home for import/export
                    ADRtype = "HOME";
                }

                // save street, zip and city information, if one of them is available
                if (!(CleanUpStreet == string.Empty && CleanUpZIP == string.Empty && CleanUpCity == string.Empty))
                {
                    sb_resultstring.AppendLine("ADR;type=" + ADRtype + ";type=pref:;;" + CleanUpStreet + ";" + CleanUpCity + ";;" + CleanUpZIP + ";");
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

                // add contact line
                add_nr_to_hash(ref MySaveDataHash, contactData.combinedname, sb_resultstring.ToString(), contactData.combinedname);

            } // end of foreach loop for the contacts

            StringBuilder resultstring = new StringBuilder();
            // retrieve stuff from hastable and put in resultstring:
            foreach (System.Collections.DictionaryEntry saveDataHash in MySaveDataHash)
            {
                resultstring.Append((string)saveDataHash.Value);
            }

            // actually write the file to disk
            System.IO.File.WriteAllText(filename, resultstring.ToString(), Encoding.UTF8);
        }

        private void save_data_vCard_Gigaset(string filename, System.Collections.Hashtable workDataHash)
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
                string CleanUpNumberMobile = CleanUpNumber(contactData.mobile, country_id, prefix_string);

                // clean up the rest of the data
                string CleanUpeMail = Do_VCARD_replace(contactData.email);

                // limit to 16 chars (maximum for Gigaset DX600A)

                string CleanUpFirstName = Do_VCARD_replace_Gigaset(LimitNameLength(contactData.firstname, 16, false, ""));
                string CleanUpLastName = Do_VCARD_replace_Gigaset(LimitNameLength(contactData.lastname, 16, false, ""));
                string CleanUpCombined = Do_VCARD_replace_Gigaset(LimitNameLength(contactData.combinedname, 16, false, ""));

                // we only wish to export phone numbers for Gigaset (plus email) => check if all relevant phone fields for this export are empty and quit if so
                if (CleanUpNumberHome == string.Empty && CleanUpNumberWork == string.Empty && CleanUpNumberMobile == string.Empty)
                {
                    // MessageBox.Show("Contact |" + CleanUpCombined + "| ignored, due to missing numbers");
                    // if yes, abort this foreach loop and contine to the next one
                    continue;
                }

                sb_resultstring.AppendLine("BEGIN:VCARD");
                sb_resultstring.AppendLine("VERSION:2.1");

                // if we have chosen to not have a company or if the company string is empty, export lastname and firstname separately
                if (combo_namestyle.SelectedItem.ToString().Contains("Company") == true && contactData.company != "")
                {
                    sb_resultstring.AppendLine("N:" + CleanUpCombined + ";");
                }
                else
                {
                    sb_resultstring.AppendLine("N:" + CleanUpLastName + ";" + CleanUpFirstName);
                }


                // save home phone number
                if (CleanUpNumberHome != string.Empty)
                {
                    { sb_resultstring.AppendLine("TEL;HOME:" + CleanUpNumberHome); }
                }

                // save work phone number
                if (CleanUpNumberWork != string.Empty)
                {
                    { sb_resultstring.AppendLine("TEL;WORK:" + CleanUpNumberWork); }
                }

                // save mobile phone number
                if (CleanUpNumberMobile != string.Empty)
                {
                    { sb_resultstring.AppendLine("TEL;CELL:" + CleanUpNumberMobile); }
                }

                // save email
                if (CleanUpeMail != string.Empty)
                {
                    sb_resultstring.AppendLine("EMAIL:" + CleanUpeMail);
                }

                sb_resultstring.AppendLine("END:VCARD");

                // add contact line
                add_nr_to_hash(ref MySaveDataHash, contactData.combinedname, sb_resultstring.ToString(), contactData.combinedname);

            } // end of foreach loop for the contacts

            StringBuilder resultstring = new StringBuilder();
            // retrieve stuff from hastable and put in resultstring:
            foreach (System.Collections.DictionaryEntry saveDataHash in MySaveDataHash)
            {
                resultstring.Append((string)saveDataHash.Value);
            }

            // actually write the file to disk
            System.IO.File.WriteAllText(filename, resultstring.ToString(), utf8WithoutBom);
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

                // clean up phone numbers
                string prefix_string = (cfg_prefixNONFB == true) ? contactData.FRITZprefix : string.Empty;
                string CleanUpNumberHome = CleanUpNumber(contactData.home, country_id, prefix_string);
                string CleanUpNumberWork = CleanUpNumber(contactData.work, country_id, prefix_string);
                string CleanUpNumberHomefax = CleanUpNumber(contactData.homefax, country_id, prefix_string);
                string CleanUpNumberWorkfax = CleanUpNumber(contactData.workfax, country_id, prefix_string);
                string CleanUpNumberMobile = CleanUpNumber(contactData.mobile, country_id, prefix_string);

                // check if both relevant fax fields for this export are empty - AND we only wish to export those who have faxnumbers
                if (CleanUpNumberHomefax == string.Empty && CleanUpNumberWorkfax == string.Empty && (only_export_fax == true))
                {
                    // if yes, abort this foreach loop and contine to the next one
                    continue;
                }


                string[,] save_iterate_array;


                if ((CleanUpNumberHomefax != "" && CleanUpNumberWorkfax != "") || (CleanUpNumberHome != "" && CleanUpNumberWork != ""))
                { // we need to save two separate entries since there are there are either seperate phone numbers or fax numbers for work and home (or both)
                    save_iterate_array = new string[2, 4];
                    save_iterate_array[0, 0] = LimitNameLength(contactData.combinedname, 31, true, " H");
                    save_iterate_array[0, 1] = CleanUpNumberHomefax;
                    save_iterate_array[0, 2] = CleanUpNumberHome;
                    save_iterate_array[1, 0] = LimitNameLength(contactData.combinedname, 31, true, " W");
                    save_iterate_array[1, 1] = CleanUpNumberWorkfax;
                    save_iterate_array[1, 2] = CleanUpNumberWork;
                }
                else
                { // we need to save only 1 entry
                    save_iterate_array = new string[1, 4];
                    save_iterate_array[0, 0] = LimitNameLength(contactData.combinedname, 31, false, "");
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

                // write contact line(s)
                for (int i = 0; i < save_iterate_array.GetLength(0); i++)
                {
                    string str_to_save = save_iterate_array[i, 0] + "\t" + contactData.company + "\t" + contactData.lastname + "\t" + contactData.firstname + "\t" + string.Empty + "\t" + contactData.street + "\t" + contactData.zip + "\t" + contactData.city + "\t" + "\t" + save_iterate_array[i, 2] + "\t" + save_iterate_array[i, 1] + "\t" + "\t" + "\t" + "\t" + "\t" + "A" + "\t" + "\t" + save_iterate_array[i, 3] + "\t" + CleanUpNumberMobile + "\t" + contactData.email + "\t" + "\r\n";
                    add_nr_to_hash(ref MySaveDataHash, save_iterate_array[i, 0], str_to_save, contactData.combinedname);
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

                // clean up phone number
                string prefix_string = (cfg_prefixNONFB == true) ? contactData.FRITZprefix : string.Empty;
                string CleanUpNumberHome = CleanUpNumber(contactData.home, country_id, prefix_string);
                string CleanUpNumberWork = CleanUpNumber(contactData.work, country_id, prefix_string);
                string CleanUpNumberMobile = CleanUpNumber(contactData.mobile, country_id, prefix_string);


                //  check if all relevant phone numbers for this export here are empty
                if (CleanUpNumberHome == string.Empty && CleanUpNumberWork == string.Empty && CleanUpNumberMobile == string.Empty)
                { continue; /* if yes, abort this foreach loop and contine to the next one */ }

                // add privat/gesch. to name if necessary (if two entries for one name necessary)
                bool separate_entries = CheckIfSeparateEntries(CleanUpNumberHome, CleanUpNumberWork, CleanUpNumberMobile);

                string name_home = LimitNameLength(contactData.combinedname, 31, separate_entries, " H");
                string name_work = LimitNameLength(contactData.combinedname, 31, separate_entries, " W");
                string name_mobile = LimitNameLength(contactData.combinedname, 31, separate_entries, " M");

                // write contact line if not empty
                if (CleanUpNumberHome != "") add_nr_to_hash(ref MySaveDataHash, name_home, "\"" + name_home + "\",\"" + CleanUpNumberHome + "\"\r\n", contactData.combinedname);
                if (CleanUpNumberWork != "") add_nr_to_hash(ref MySaveDataHash, name_work, "\"" + name_work + "\",\"" + CleanUpNumberWork + "\"\r\n", contactData.combinedname);
                if (CleanUpNumberMobile != "") add_nr_to_hash(ref MySaveDataHash, name_mobile, "\"" + name_mobile + "\",\"" + CleanUpNumberMobile + "\"\r\n", contactData.combinedname);


            } // end of foreach loop for the contacts

            // retrieve stuff from hastable and put in resultstring:
            foreach (System.Collections.DictionaryEntry saveDataHash in MySaveDataHash)
            {
                resultSB.Append((string)saveDataHash.Value);
            }

            // actually write the file to disk
            System.IO.File.WriteAllText(filename, resultSB.ToString(), utf8WithoutBom);
        }

        private void save_data_SnomCSV8(string filename, System.Collections.Hashtable workDataHash)
        {
            // get the country ID from the combobox or from user input
            string country_id = RetrieveCountryID(combo_prefix.SelectedItem.ToString());

            // process with exporting
            StringBuilder resultSB = new StringBuilder();

            System.Collections.Hashtable MySaveDataHash = new System.Collections.Hashtable();

            int master_id = 100000;

            foreach (System.Collections.DictionaryEntry contactHash in workDataHash)
            {
                // extract GroupDataList from hashtable contents
                GroupDataContact contactData = (GroupDataContact)contactHash.Value;

                // clean up phone number
                string prefix_string = (cfg_prefixNONFB == true) ? contactData.FRITZprefix : string.Empty;
                string CleanUpNumberHome = CleanUpNumber(contactData.home, country_id, prefix_string);
                string CleanUpNumberWork = CleanUpNumber(contactData.work, country_id, prefix_string);
                string CleanUpNumberMobile = CleanUpNumber(contactData.mobile, country_id, prefix_string);

                //  check if all relevant phone numbers for this export here are empty
                if (CleanUpNumberHome == string.Empty && CleanUpNumberWork == string.Empty && CleanUpNumberMobile == string.Empty)
                { continue; /* if yes, abort this foreach loop and contine to the next one */ }

                // limit full name to 31 chars
                string name_contact = LimitNameLength(contactData.combinedname, 31, false, "");

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

                    master_id = master_id + 1;

                    out_string += "\"" + name_contact
                                        + sep + master_id.ToString("D8")
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
                                                    + sep + master_id.ToString("D8") // master id for association with main contact
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

                // write contact line
                add_nr_to_hash(ref MySaveDataHash, name_contact, out_string, contactData.combinedname);

            } // end of foreach loop for the contacts

            // retrieve stuff from hastable and put in resultstring:
            foreach (System.Collections.DictionaryEntry saveDataHash in MySaveDataHash)
            {
                resultSB.Append((string)saveDataHash.Value);
            }

            // actually write the file to disk
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

                // clean up phone number
                string prefix_string = (cfg_prefixNONFB == true) ? contactData.FRITZprefix : string.Empty;
                string CleanUpNumberHome = CleanUpNumber(contactData.home, country_id, prefix_string);
                string CleanUpNumberWork = CleanUpNumber(contactData.work, country_id, prefix_string);
                string CleanUpNumberMobile = CleanUpNumber(contactData.mobile, country_id, prefix_string);

                //  check if all relevant phone numbers for this export here are empty
                if (CleanUpNumberHome == string.Empty && CleanUpNumberWork == string.Empty && CleanUpNumberMobile == string.Empty)
                { continue; /* if yes, abort this foreach loop and contine to the next one */ }

                // add privat/gesch. to name if necessary (if seperate entries for one name necessary) - and remove "," because talk&surf can't handle it properly
                string name_home = contactData.combinedname.Replace(",", "");
                string name_work = contactData.combinedname.Replace(",", "");
                string name_mobile = contactData.combinedname.Replace(",", "");

                bool separate_entries = CheckIfSeparateEntries(CleanUpNumberHome, CleanUpNumberWork, CleanUpNumberMobile);

                // limit to 16 chars and append if necessary
                name_home = LimitNameLength(name_home, 16, separate_entries, " H");
                name_work = LimitNameLength(name_work, 16, separate_entries, " W");
                name_mobile = LimitNameLength(name_mobile, 16, separate_entries, " M");

                // write contact line if not empty
                if (CleanUpNumberHome != "")    add_nr_to_hash(ref MySaveDataHash, name_home, name_home + "," + CleanUpNumberHome + "\r\n", contactData.combinedname);
                if (CleanUpNumberWork != "")    add_nr_to_hash(ref MySaveDataHash, name_work, name_work + "," + CleanUpNumberWork + "\r\n", contactData.combinedname);
                if (CleanUpNumberMobile != "")  add_nr_to_hash(ref MySaveDataHash, name_mobile, name_mobile + "," + CleanUpNumberMobile + "\r\n", contactData.combinedname);

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

                // clean up phone number
                string prefix_string = (cfg_prefixNONFB == true) ? contactData.FRITZprefix : string.Empty;
                string CleanUpNumberHome = CleanUpNumber(contactData.home, country_id, prefix_string);
                string CleanUpNumberWork = CleanUpNumber(contactData.work, country_id, prefix_string);
                string CleanUpNumberMobile = CleanUpNumber(contactData.mobile, country_id, prefix_string);

                //  check if all relevant phone numbers for this export here are empty
                if (CleanUpNumberHome == string.Empty && CleanUpNumberWork == string.Empty && CleanUpNumberMobile == string.Empty)
                { continue; /* if yes, abort this foreach loop and contine to the next one */ }

                bool separate_entries = CheckIfSeparateEntries(CleanUpNumberHome, CleanUpNumberWork, CleanUpNumberMobile);

                // limit to 31 chars
                string name_home = LimitNameLength(contactData.combinedname, 31, separate_entries, " H").Replace(",", " ");
                string name_work = LimitNameLength(contactData.combinedname, 31, separate_entries, " W").Replace(",", " ");
                string name_mobile = LimitNameLength(contactData.combinedname, 31, separate_entries, " M").Replace(",", " ");

                // write contact line if not empty
                if (CleanUpNumberHome != "") add_nr_to_hash(ref MySaveDataHash, name_home + "#Home", name_home + "," + CleanUpNumberHome + ",1,Home,public\n", contactData.combinedname);
                if (CleanUpNumberWork != "") add_nr_to_hash(ref MySaveDataHash, name_work + "#Work", name_work + "," + CleanUpNumberWork + ",1,Work,public\n", contactData.combinedname);
                if (CleanUpNumberMobile != "") add_nr_to_hash(ref MySaveDataHash, name_mobile + "#Mobile", name_mobile + "," + CleanUpNumberMobile + ",1,Mobile,public\n", contactData.combinedname);

            } // end of foreach loop for the contacts

            // retrieve stuff from hastable and put in resultstring:
            foreach (System.Collections.DictionaryEntry saveDataHash in MySaveDataHash)
            {
                resultSB.Append((string)saveDataHash.Value);
            }

            // actually write the file to disk
            System.IO.File.WriteAllText(filename, resultSB.ToString(), utf8WithoutBom);

        }

        private void save_data_AuerswaldCSV(string filename, System.Collections.Hashtable workDataHash)
        {
            // get the country ID from the combobox or from user input
            string country_id = RetrieveCountryID(combo_prefix.SelectedItem.ToString());

            // process with exporting
            StringBuilder resultSB = new StringBuilder();

            resultSB.Append("Kurzwahl;Rufnummer;Name\n");

            // initialize quickdial_remaining veriable
            int qd_remaining = 97;

            System.Collections.Hashtable MySaveDataHash = new System.Collections.Hashtable();

            foreach (System.Collections.DictionaryEntry contactHash in workDataHash)
            {
                // extract GroupDataList from hashtable contents
                GroupDataContact contactData = (GroupDataContact)contactHash.Value;

                // clean up phone number
                string prefix_string = (cfg_prefixNONFB == true) ? contactData.FRITZprefix : string.Empty;
                string CleanUpNumberHome = CleanUpNumber(contactData.home, country_id, prefix_string);
                string CleanUpNumberWork = CleanUpNumber(contactData.work, country_id, prefix_string);
                string CleanUpNumberMobile = CleanUpNumber(contactData.mobile, country_id, prefix_string);

                //  check if all relevant phone numbers for this export here are empty
                if (CleanUpNumberHome == string.Empty && CleanUpNumberWork == string.Empty && CleanUpNumberMobile == string.Empty)
                { continue; /* if yes, abort this foreach loop and contine to the next one */ }

                string qd_home = "";
                string qd_work = "";
                string qd_mobile = "";

                // add quickdial and vanity information to it
                if (contactData.speeddial != "")
                { // then we have a quickdial number then add it to the prio entry

                    string qd_number = "";

                    if (contactData.speeddial.Contains(","))
                    { // two parts (01,VANITY or XX,VANITY)
                        qd_number = contactData.speeddial.Substring(0, contactData.speeddial.IndexOf(','));

                        // now handle automatic distribution of quickdial numbers for vanities without number
                        if ((qd_number == "XX") && (qd_remaining > 9)) { qd_number = qd_remaining.ToString(); qd_remaining--; }
                        if (qd_number == "XX")
                        { // no remaining numbers left, so we cannot assign any quickdial number or vanity code (the field quickdial will be set to "" in the XML anyway but that probably won't matter)
                            qd_number = "";
                        }
                    }
                    else
                    { // only 1 part, can only be number
                        qd_number = contactData.speeddial;
                    }

                    // add quickdial number which always has to exist here (unless the autodistribtion of numbers above ran out of available numbers)
                    if (qd_number != "")
                    {
                        if (contactData.preferred == "home") { qd_home = qd_number; }
                        if (contactData.preferred == "work") { qd_work = qd_number; }
                        if (contactData.preferred == "mobile") { qd_mobile = qd_number; }
                    }

                }

                bool separate_entries = CheckIfSeparateEntries(CleanUpNumberHome, CleanUpNumberWork, CleanUpNumberMobile);

                // limit to 31 chars
                string name_home = LimitNameLength(contactData.combinedname, 31, separate_entries, " H").Replace(";", " ");
                string name_work = LimitNameLength(contactData.combinedname, 31, separate_entries, " W").Replace(";", " ");
                string name_mobile = LimitNameLength(contactData.combinedname, 31, separate_entries, " M").Replace(";", " ");

                // write contact line if not empty
                if (CleanUpNumberHome != "")    add_nr_to_hash(ref MySaveDataHash, name_home,   qd_home + ";" + CleanUpNumberHome   + ";" + name_home   + "\n", contactData.combinedname);
                if (CleanUpNumberWork != "")    add_nr_to_hash(ref MySaveDataHash, name_work,   qd_work + ";" + CleanUpNumberWork   + ";" + name_work   + "\n", contactData.combinedname);
                if (CleanUpNumberMobile != "")  add_nr_to_hash(ref MySaveDataHash, name_mobile, qd_mobile + ";" + CleanUpNumberMobile + ";" + name_mobile + "\n", contactData.combinedname);

            } // end of foreach loop for the contacts

            // retrieve stuff from hastable and put in resultstring:
            foreach (System.Collections.DictionaryEntry saveDataHash in MySaveDataHash)
            {
                resultSB.Append((string)saveDataHash.Value);
            }

            // actually write the file to disk
            System.IO.File.WriteAllText(filename, resultSB.ToString(), Encoding.GetEncoding("ISO-8859-1"));

        }

        private void save_data_GrandstreamGXV(string filename, System.Collections.Hashtable workDataHash)
        {
            // get the country ID from the combobox or from user input
            string country_id = RetrieveCountryID(combo_prefix.SelectedItem.ToString());

            // process with exporting
            StringBuilder resultSB = new StringBuilder();

            // write the header
            resultSB.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n"
                          + "<AddressBook>\n"
                          + "    <version>1</version>\n");

            // initialize hashtable to store generated data results
            System.Collections.Hashtable MySaveDataHash = new System.Collections.Hashtable();

            foreach (System.Collections.DictionaryEntry contactHash in workDataHash)
            {
                // extract GroupDataList from hashtable contents
                GroupDataContact contactData = (GroupDataContact)contactHash.Value;


                // clean up phone number
                string prefix_string = (cfg_prefixNONFB == true) ? contactData.FRITZprefix : string.Empty;
                string CleanUpNumberHome = CleanUpNumber(contactData.home, country_id, prefix_string);
                string CleanUpNumberWork = CleanUpNumber(contactData.work, country_id, prefix_string);
                string CleanUpNumberMobile = CleanUpNumber(contactData.mobile, country_id, prefix_string);

                // check if all relevant phone numbers for this export here are empty
                if (CleanUpNumberHome == string.Empty && CleanUpNumberWork == string.Empty && CleanUpNumberMobile == string.Empty)
                { continue; /* if yes, abort this foreach loop and contine to the next one */ }


                //  limit to 32 chars (unsure if correct value) and XML replacements
                string SaveAsName = Do_XML_replace(LimitNameLength(contactData.combinedname, 32, false, ""));
                string FirstName = Do_XML_replace(LimitNameLength(contactData.firstname, 32, false, ""));
                string LastName = Do_XML_replace(LimitNameLength(contactData.lastname, 32, false, ""));

                string str_1_name = "";
                // if we have chosen to not have a company or if the company string is empty, export lastname and firstname separately
                if (combo_namestyle.SelectedItem.ToString().Contains("Company") == true && contactData.company != "")
                {
                    str_1_name = "    <Contact>\n"
                                  + "        <FirstName></FirstName>\n"
                                  + "        <LastName>" + SaveAsName + "</LastName>\n";
                }
                else
                {
                    str_1_name = "    <Contact>\n"
                                  + "        <FirstName>" + FirstName + "</FirstName>\n"
                                  + "        <LastName>" + LastName + "</LastName>\n";
                }

                string str_3_end = "    </Contact>\n";

                StringBuilder str_2_phones = new StringBuilder();

                // add home phone number
                if (CleanUpNumberHome != "")
                {
                    str_2_phones.Append("        <Phone type=\"Home\">\n"
                                      + "            <phonenumber>" + CleanUpNumberHome + "</phonenumber>\n"
                                      + "            <accountindex>0</accountindex>\n"
                                      + "        </Phone>\n");
                }

                // add work phone number
                if (CleanUpNumberWork != "")
                {
                    str_2_phones.Append("        <Phone type=\"Work\">\n"
                                      + "            <phonenumber>" + CleanUpNumberWork + "</phonenumber>\n"
                                      + "            <accountindex>0</accountindex>\n"
                                      + "        </Phone>\n");
                }

                // add mobile phone number
                if (CleanUpNumberMobile != "")
                {
                    str_2_phones.Append("        <Phone type=\"Mobile\">\n"
                                      + "            <phonenumber>" + CleanUpNumberMobile + "</phonenumber>\n"
                                      + "            <accountindex>0</accountindex>\n"
                                      + "        </Phone>\n");
                }

                // write contact line
                add_nr_to_hash(ref MySaveDataHash, SaveAsName, str_1_name + str_2_phones + str_3_end, contactData.combinedname);

            } // end of foreach loop for the contacts

            foreach (System.Collections.DictionaryEntry saveDataHash in MySaveDataHash)
            { resultSB.Append((string)saveDataHash.Value); }

            // write file footer.
            resultSB.Append("</AddressBook>");

            // Set Encoding to UTF-8 without BOM
            System.Text.Encoding encoding = new System.Text.UTF8Encoding(false);

            // actually write the file to disk
            System.IO.File.WriteAllText(filename, resultSB.ToString(), encoding);

            // tell the user this has been done
            string errorwarning = "";
            if (MySaveDataHash.Count > 500) { errorwarning = Environment.NewLine + "Warning: Over 500 contacts have been exported! This is not supported by the Grandstream GXV 3140. Proceed with care!"; }
            MessageBox.Show(MySaveDataHash.Count + " contacts written to " + filename + " !" + Environment.NewLine + errorwarning);

        }

        private void save_data_GrandstreamGXP(string filename, System.Collections.Hashtable workDataHash)
        {
            // get the country ID from the combobox or from user input
            string country_id = RetrieveCountryID(combo_prefix.SelectedItem.ToString());

            // process with exporting
            StringBuilder resultSB = new StringBuilder();

            // write the header
            resultSB.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<AddressBook>\n");

            // initialize hashtable to store generated data results
            System.Collections.Hashtable MySaveDataHash = new System.Collections.Hashtable();

            foreach (System.Collections.DictionaryEntry contactHash in workDataHash)
            {
                // extract GroupDataList from hashtable contents
                GroupDataContact contactData = (GroupDataContact)contactHash.Value;

                // clean up phone number
                string prefix_string = (cfg_prefixNONFB == true) ? contactData.FRITZprefix : string.Empty;
                string CleanUpNumberHome = CleanUpNumber(contactData.home, country_id, prefix_string);
                string CleanUpNumberWork = CleanUpNumber(contactData.work, country_id, prefix_string);
                string CleanUpNumberMobile = CleanUpNumber(contactData.mobile, country_id, prefix_string);

                //  check if all relevant phone numbers for this export here are empty
                if (CleanUpNumberHome == string.Empty && CleanUpNumberWork == string.Empty && CleanUpNumberMobile == string.Empty)
                { continue; /* if yes, abort this foreach loop and contine to the next one */ }

                bool separate_entries = CheckIfSeparateEntries(CleanUpNumberHome, CleanUpNumberWork, CleanUpNumberMobile);

                // add Home/Work/Mobile to name if separate entries for one name necessary and limit to 20 chars
                string name_home = LimitNameLength(contactData.combinedname, 20, separate_entries, " H");
                string name_work = LimitNameLength(contactData.combinedname, 20, separate_entries, " W");
                string name_mobile = LimitNameLength(contactData.combinedname, 20, separate_entries, " M");

                name_home = Do_XML_replace(name_home);
                name_work = Do_XML_replace(name_work);
                name_mobile = Do_XML_replace(name_mobile);

                string str_1 = "    <Contact>\n"
                             + "        <LastName>";
                string str_2 = "</LastName>\n"
                             + "        <FirstName></FirstName>\n"
                             + "        <Phone>\n"
                             + "            <phonenumber>";

                string str_3 = "</phonenumber>\n"
                             + "            <accountindex>0</accountindex>\n"
                             + "        </Phone>\n"
                             + "    </Contact>\n";

                // write contact line if not empty
                if (CleanUpNumberHome != "")
                { add_nr_to_hash(ref MySaveDataHash, name_home, str_1 + name_home + str_2 + CleanUpNumberHome + str_3, contactData.combinedname); }

                if (CleanUpNumberWork != "")
                { add_nr_to_hash(ref MySaveDataHash, name_work, str_1 + name_work + str_2 + CleanUpNumberWork + str_3, contactData.combinedname); }

                if (CleanUpNumberMobile != "")
                { add_nr_to_hash(ref MySaveDataHash, name_mobile, str_1 + name_mobile + str_2 + CleanUpNumberMobile + str_3, contactData.combinedname); }

            } // end of foreach loop for the contacts

            // retrieve stuff from hastable and put in resultstring:
            foreach (System.Collections.DictionaryEntry saveDataHash in MySaveDataHash)
            { resultSB.Append((string)saveDataHash.Value); }

            // write file footer.
            resultSB.Append("</AddressBook>\n");

            // Set Encoding to UTF-8 without BOM
            System.Text.Encoding encoding = new System.Text.UTF8Encoding(false);

            // actually write the file to disk
            System.IO.File.WriteAllText(filename, resultSB.ToString(), encoding);

            // tell the user this has been done
            MessageBox.Show(MySaveDataHash.Count + " contacts written to " + filename + " !");
        }





        private void addDataHash(ref string duplicates, GroupDataContact theContact)
        {
            try
            {
                myGroupDataHash.Add(theContact.combinedname, theContact);
            }
            catch (ArgumentException)
            {
                if (cfg_DUPren == false)
                {
                    duplicates += "Duplicate entry in source:" + theContact.combinedname + Environment.NewLine;
                }
                else
                { // do something complicated to allow the import of duplicated information
                    int myDupCounter = 0;
                    bool success = false;

                    while (myDupCounter < 100 && success == false)
                    { // try a 100 times to import it before ignoring
                        try
                        {
                            theContact.combinedname = theContact.combinedname.Substring(0, theContact.combinedname.Length - 2) + myDupCounter.ToString("D2");
                            myGroupDataHash.Add(theContact.combinedname, theContact);
                            success = true;
                        }
                        catch
                        { myDupCounter++; }
                    }
                    if (success == false)
                    { duplicates += "Duplicate entry in source even after renaming 100 times:" + theContact.combinedname + Environment.NewLine; }
                }
            }

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
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "cfg_importOther") { cfg_importOther = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "cfg_DUPren") { cfg_DUPren = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "cfg_checkVersion") { cfg_checkVersion = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "clean_brackets") { clean_brackets = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "clean_hashkey") { clean_hashkey = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "clean_slash") { clean_slash = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "clean_hyphen") { clean_hyphen = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "clean_xchar") { clean_xchar = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "clean_space") { clean_space = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "clean_squarebrackets") { clean_squarebrackets = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "clean_letters") { clean_letters = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }
                    if (ParseLine.Substring(0, ParseLine.IndexOf("\t")) == "clean_addzeroprefix") { clean_addzeroprefix = Convert.ToBoolean(ParseLine.Substring(ParseLine.IndexOf("\t") + 1)); continue; }


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
            sb.AppendLine("cfg_importOther" + "\t" + cfg_importOther.ToString());
            sb.AppendLine("cfg_DUPren" + "\t" + cfg_DUPren.ToString());
            sb.AppendLine("cfg_checkVersion" + "\t" + cfg_checkVersion.ToString());
            sb.AppendLine("clean_brackets" + "\t" + clean_brackets.ToString());
            sb.AppendLine("clean_hashkey" + "\t" + clean_hashkey.ToString());
            sb.AppendLine("clean_slash" + "\t" + clean_slash.ToString());
            sb.AppendLine("clean_hyphen" + "\t" + clean_hyphen.ToString());
            sb.AppendLine("clean_xchar" + "\t" + clean_xchar.ToString());
            sb.AppendLine("clean_space" + "\t" + clean_space.ToString());
            sb.AppendLine("clean_squarebrackets" + "\t" + clean_squarebrackets.ToString());
            sb.AppendLine("clean_letters" + "\t" + clean_letters.ToString());
            sb.AppendLine("clean_addzeroprefix" + "\t" + clean_addzeroprefix.ToString());

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

        private void button_website_Click(object sender, EventArgs e)
        {
            if (((Control.ModifierKeys & Keys.Shift) == Keys.Shift) || (this.button_website.ForeColor == System.Drawing.Color.Green))
            { // shift is pressed
                System.Diagnostics.Process.Start("http://software.nv-systems.net/ccw/download");
            }
            else
            { // no shift pressed
                System.Diagnostics.Process.Start("http://software.nv-systems.net/ccw");
            }

        }

        private void button_forum_Click(object sender, EventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            { // shift is pressed
                System.Diagnostics.Process.Start("www.ip-phone-forum.de/showthread.php?t=209976&goto=newpost");
            }
            else
            { // no shift pressed
                System.Diagnostics.Process.Start("www.ip-phone-forum.de/showthread.php?t=209976");
            }
        }

        private void backgroundWorker_updateCheck_DoWork(object sender, DoWorkEventArgs e)
        {
            string website = "";

            try
            {
                website = DoWebRequest("http://software.nv-systems.net/ccw/download");
                website = website.Substring(website.IndexOf("Aktuelle Version"));
                website = website.Substring(website.IndexOf("/downloads/Contact%20Conversion%20Wizard%20v") + "/downloads/Contact%20Conversion%20Wizard%20v".Length);
                string websiteversion = website.Substring(0, website.IndexOf(".zip"));
                string productversion = this.ProductVersion;

                if (websiteversion.CompareTo(productversion) == 1)
                {
                    e.Result = "NEW:"  + websiteversion;
                }
                else
                {
                    if (websiteversion.CompareTo(productversion) == 0) { e.Result = "SAME:" + websiteversion; }
                    else if (websiteversion.CompareTo(productversion) == -1) { e.Result = "OLDER:" + websiteversion; }
                    else { e.Result = "THIS SHOULD NOT HAVE HAPPENED"; }

                }

            }
            catch (Exception ce)
            {
                e.Result = "FAIL:" + ce.Message;
            }


        }

        private void backgroundWorker_updateCheck_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result.ToString().StartsWith("FAIL:"))
            {   // Web Version check has failed.
                // MessageBox.Show("Web Version check has failed for unknown reasons." + Environment.NewLine + e.Result.ToString());
                return;
            }

            if (e.Result.ToString().StartsWith("SAME:"))
            {   // Web Version check has returned the same version currently installed.
                // MessageBox.Show("Web Version check has returned the same version currently installed." + Environment.NewLine + e.Result.ToString());
                return;
            }

            if (e.Result.ToString().StartsWith("OLDER:"))
            {   // Web Version check has returned an older version than you currently have.
                // MessageBox.Show("Web Version check has returned an older version than you currently have." + Environment.NewLine + e.Result.ToString());
                this.button_website.ForeColor = this.button_website.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (e.Result.ToString().StartsWith("NEW:"))
            {   // Web Version check has returned a newer version than you currently have.
                // MessageBox.Show("Web Version check has returned a newer version than you currently have." + Environment.NewLine + e.Result.ToString());
                this.button_website.ForeColor = this.button_website.ForeColor = System.Drawing.Color.Green;
                this.button_website.Text = "Update to: v" + e.Result.ToString().Substring(4);
                return;
            }


        }

        private string DoWebRequest(string url)
        {

            string responseData;
            System.IO.StreamReader responseReader;
            System.Net.HttpWebResponse myResponse;
            System.Net.HttpWebRequest MyRq;

            MyRq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            MyRq.Accept = "text/html";
            MyRq.UserAgent = "User-Agent: Contact Conversion Wizard/" + this.ProductVersion;
            MyRq.Proxy = null;

            myResponse = (System.Net.HttpWebResponse)MyRq.GetResponse();
            responseReader = new System.IO.StreamReader(myResponse.GetResponseStream());
            responseData = responseReader.ReadToEnd();
            responseReader.Close();
            myResponse.Close();

            return responseData;
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
        // public System.Collections.Hashtable importedHash = new System.Collections.Hashtable();

        public ReadDataReturn(string my_duplicates)
        {
            duplicates = my_duplicates;
            // importedHash = my_importedHash;
        }
    }           // small helper class for the return values of the data/file reader methods

}
