using System;
using System.Collections.Generic;
using System.IO;
using ContactConversionWizard.Core.Import;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace Contact_Conversion_Wizard
{
    /// <summary>Settings for reading contacts from Outlook.</summary>
    public sealed class OutlookImportOptions
    {
        public ImportOptions Import { get; set; } = new ImportOptions();

        /// <summary>Let the user pick the folder instead of using the default contacts folder.</summary>
        public bool PickFolder { get; set; }

        /// <summary>Only import contacts whose categories contain this text (if not empty).</summary>
        public string CategoryFilter { get; set; } = "";

        /// <summary>Use the "File As" field as combined name instead of generating it.</summary>
        public bool UseFileAs { get; set; }

        /// <summary>Put the "Other" phone number into an empty home or work field.</summary>
        public bool ImportOtherNumber { get; set; }

        public bool ImportWithoutName { get; set; }

        public bool ImportWithoutNumber { get; set; }
    }

    /// <summary>Reads contacts from and writes contacts to Microsoft Outlook using COM automation.</summary>
    public sealed class OutlookConnector
    {
        private const string ContactPhotoProperty = "http://schemas.microsoft.com/mapi/proptag/0x7FFF000B";

        private readonly Outlook.Application outlook;

        private OutlookConnector(Outlook.Application outlook)
        {
            this.outlook = outlook;
        }

        /// <summary>Connects to Outlook. Throws if Outlook is not installed or cannot be started.</summary>
        public static OutlookConnector Connect()
        {
            return new OutlookConnector(new Outlook.Application());
        }

        /// <summary>Contact pictures are only supported from Outlook 2007 (version 12) on.</summary>
        public bool SupportsPictures
        {
            get
            {
                string version = outlook.Version ?? "";
                int dot = version.IndexOf('.');
                return dot > 0 && int.TryParse(version.Substring(0, dot), out int major) && major >= 12;
            }
        }

        public ImportResult Import(OutlookImportOptions options)
        {
            var result = new ImportResult();
            Outlook.NameSpace session = outlook.GetNamespace("MAPI");
            Outlook.MAPIFolder folder = options.PickFolder
                ? session.PickFolder()
                : session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts);
            if (folder == null) { return result; } // the user cancelled the folder selection

            bool pictures = SupportsPictures;
            Outlook.Items items = folder.Items;
            for (int i = 1; i <= items.Count; i++)
            {
                if (!(items[i] is Outlook.ContactItem item)) { continue; } // distribution lists etc.

                if (!(item.MessageClass ?? "").StartsWith("IPM.Contact", StringComparison.Ordinal))
                {
                    result.Warnings.Add("Not a valid contact class " + item.MessageClass);
                    continue;
                }

                string categories = item.Categories ?? "";
                if (options.CategoryFilter != "" && !categories.Contains(options.CategoryFilter)) { continue; }

                string notes = item.Body ?? "";
                if (NoteFlags.IsIgnored(notes)) { continue; }

                Contact contact = ReadContact(item, options, pictures);
                NoteFlags.Apply(contact, item.NickName ?? "", notes, options.Import.VipMode);

                bool hasName = contact.CombinedName != "";
                bool hasNumber = contact.HasAnyNumber;
                if ((hasName || options.ImportWithoutName) && (hasNumber || options.ImportWithoutNumber))
                {
                    result.Contacts.Add(contact);
                }
            }

            return result;
        }

        private Contact ReadContact(Outlook.ContactItem item, OutlookImportOptions options, bool pictures)
        {
            var contact = new Contact
            {
                LastName = item.LastName,
                FirstName = item.FirstName,
                Company = item.CompanyName,
                Home = item.HomeTelephoneNumber,
                Work = item.BusinessTelephoneNumber,
                HomeFax = item.HomeFaxNumber,
                WorkFax = item.BusinessFaxNumber,
                Mobile = item.MobileTelephoneNumber,
                Street = (item.MailingAddressStreet ?? "").Replace(Environment.NewLine, " - "),
                Zip = item.MailingAddressPostalCode,
                City = item.MailingAddressCity,
                Email = item.Email1Address,
            };

            string other = item.OtherTelephoneNumber;
            if (options.ImportOtherNumber && !string.IsNullOrEmpty(other))
            {
                PutIntoFreeSlot(contact, other, options.Import.TypePreference);
            }

            if (pictures && item.HasPicture)
            {
                contact.Photo = ReadPhoto(item);
            }

            contact.CombinedName = options.UseFileAs
                ? item.FileAs
                : NameFormatter.GenerateFullName(contact.FirstName, contact.LastName, contact.Company, options.Import.NameStyle);

            return contact;
        }

        /// <summary>Stores a number in the empty home or work field, starting with the preferred one.</summary>
        internal static void PutIntoFreeSlot(Contact contact, string number, TypePreference preference)
        {
            if (preference == TypePreference.Home)
            {
                if (contact.Home == "") { contact.Home = number; }
                else if (contact.Work == "") { contact.Work = number; }
            }
            else
            {
                if (contact.Work == "") { contact.Work = number; }
                else if (contact.Home == "") { contact.Home = number; }
            }
        }

        private static byte[] ReadPhoto(Outlook.ContactItem item)
        {
            foreach (Outlook.Attachment attachment in item.Attachments)
            {
                try
                {
                    if (!(bool)attachment.PropertyAccessor.GetProperty(ContactPhotoProperty)) { continue; }

                    string tempFile = Path.GetTempFileName();
                    try
                    {
                        attachment.SaveAsFile(tempFile);
                        return File.ReadAllBytes(tempFile);
                    }
                    finally
                    {
                        File.Delete(tempFile);
                    }
                }
                catch (Exception)
                {
                    // attachment processing fails for some items (mostly Outlook 2003); ignore the picture then
                }
            }
            return null;
        }

        /// <summary>
        /// Lets the user pick a folder and writes the contacts into it.
        /// Returns the number of contacts written, or -1 if the user cancelled.
        /// </summary>
        public int Export(IEnumerable<Contact> contacts, PhoneNumberCleaner cleaner, bool applyPrefix, bool exportPictures)
        {
            Outlook.MAPIFolder folder = outlook.GetNamespace("MAPI").PickFolder();
            if (folder == null) { return -1; }

            bool pictures = exportPictures && SupportsPictures;
            int count = 0;
            foreach (Contact contact in contacts)
            {
                string prefix = applyPrefix ? contact.FritzPrefix : "";
                var item = (Outlook.ContactItem)folder.Items.Add(Outlook.OlItemType.olContactItem);

                item.FirstName = contact.FirstName;
                item.LastName = contact.LastName;
                item.Email1Address = contact.Email;
                item.MailingAddressCity = contact.City;
                item.MailingAddressStreet = contact.Street;
                item.MailingAddressPostalCode = contact.Zip;
                item.HomeTelephoneNumber = cleaner.Clean(contact.Home, prefix);
                item.HomeFaxNumber = cleaner.Clean(contact.HomeFax, prefix);
                item.BusinessTelephoneNumber = cleaner.Clean(contact.Work, prefix);
                item.BusinessFaxNumber = cleaner.Clean(contact.WorkFax, prefix);
                item.MobileTelephoneNumber = cleaner.Clean(contact.Mobile, prefix);
                item.FileAs = contact.CombinedName;
                item.CompanyName = contact.Company;

                string body = (contact.IsVip ? "VIP" + Environment.NewLine : "")
                    + (contact.SpeedDial != "" ? "SPEEDDIAL(" + contact.SpeedDial + ")" : "");
                if (body != "") { item.Body += body; }

                if (pictures && contact.Photo != null)
                {
                    string tempFile = Path.GetTempFileName();
                    try
                    {
                        File.WriteAllBytes(tempFile, contact.Photo);
                        item.AddPicture(tempFile);
                    }
                    finally
                    {
                        File.Delete(tempFile);
                    }
                }

                item.Save();
                count++;
            }

            return count;
        }
    }
}
