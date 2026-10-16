using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using ContactConversionWizard.Core.Import;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace Contact_Conversion_Wizard
{
    /// <summary>Settings for reading contacts from Outlook.</summary>
    public sealed class OutlookImportOptions
    {
        public ImportOptions Import { get; set; } = new ImportOptions();

        /// <summary>Let the user pick the folder instead of using the default contacts folder.</summary>
        public bool PickFolder { get; set; }

        /// <summary>Only import contacts whose categories match this filter, see <see cref="CategoryFilter"/>.</summary>
        public string CategoryFilter { get; set; } = "";

        /// <summary>Use the "File As" field as combined name instead of generating it.</summary>
        public bool UseFileAs { get; set; }

        /// <summary>Put the "Other" phone number into an empty home or work field.</summary>
        public bool ImportOtherNumber { get; set; }
    }

    /// <summary>
    /// Reads contacts from and writes contacts to Microsoft Outlook using COM automation.
    /// </summary>
    /// <remarks>
    /// Outlook is accessed late bound (<c>dynamic</c>/IDispatch) instead of through the Outlook interop assembly. Early
    /// binding needs the Outlook type library to be registered correctly; on many Office installations (click-to-run,
    /// leftovers of older versions, 32/64 bit mix) it is not, which made the import crash with
    /// "Unable to cast COM object ... TYPE_E_ELEMENTNOTFOUND". Late binding also works with every Outlook version.
    /// </remarks>
    public sealed class OutlookConnector
    {
        private const string ContactPhotoProperty = "http://schemas.microsoft.com/mapi/proptag/0x7FFF000B";
        private const int OlFolderContacts = 10;
        private const int OlContactItem = 2;
        private const int OlContactClass = 40;

        // Outlook stores "no date" as 1 January 4501
        private static readonly DateTime NoDate = new DateTime(4501, 1, 1);

        private readonly dynamic outlook;

        private OutlookConnector(object outlook)
        {
            this.outlook = outlook;
        }

        /// <summary>Connects to a running Outlook or starts it. Throws if Outlook is not installed or cannot be started.</summary>
        public static OutlookConnector Connect()
        {
            Type type = Type.GetTypeFromProgID("Outlook.Application", false);
            if (type == null)
            {
                throw new OutlookNotAvailableException("Outlook is not installed (the COM class \"Outlook.Application\" is not registered).");
            }

            try
            {
                return new OutlookConnector(Activator.CreateInstance(type));
            }
            catch (COMException ex)
            {
                throw new OutlookNotAvailableException("Outlook could not be started: " + ex.Message, ex);
            }
        }

        /// <summary>Contact pictures are only supported from Outlook 2007 (version 12) on.</summary>
        public bool SupportsPictures
        {
            get
            {
                string version = Convert.ToString(outlook.Version, CultureInfo.InvariantCulture) ?? "";
                int dot = version.IndexOf('.');
                return dot > 0 && int.TryParse(version.Substring(0, dot), out int major) && major >= 12;
            }
        }

        public ImportResult Import(OutlookImportOptions options)
        {
            var result = new ImportResult();
            dynamic session = outlook.GetNamespace("MAPI");
            dynamic folder = options.PickFolder ? session.PickFolder() : session.GetDefaultFolder(OlFolderContacts);
            if (folder == null) { return result; } // the user cancelled the folder selection

            var categoryFilter = new CategoryFilter(options.CategoryFilter);
            bool pictures = SupportsPictures;
            dynamic items = folder.Items;
            int count = items.Count;

            for (int i = 1; i <= count; i++)
            {
                dynamic item = items.Item(i);
                try
                {
                    if ((int)item.Class != OlContactClass) { continue; } // distribution lists etc.

                    string messageClass = Text(item.MessageClass);
                    if (!messageClass.StartsWith("IPM.Contact", StringComparison.Ordinal))
                    {
                        result.Warnings.Add("Not a valid contact class " + messageClass);
                        continue;
                    }

                    if (!categoryFilter.IsEmpty && !categoryFilter.Matches(Text(item.Categories))) { continue; }

                    string notes = Text(item.Body);
                    Contact contact = ReadContact(item, options, pictures, result);
                    NoteFlags.Apply(contact, Text(item.NickName), notes, options.Import.VipMode);

                    if (options.Import.Accept(contact, notes))
                    {
                        result.Contacts.Add(contact);
                    }
                }
                catch (Exception ex) when (ex is COMException || ex is Microsoft.CSharp.RuntimeBinder.RuntimeBinderException || ex is InvalidCastException)
                {
                    result.Warnings.Add("Outlook item #" + i + " could not be read: " + ex.Message);
                }
            }

            return result;
        }

        private static string Text(object value) => value == null ? "" : Convert.ToString(value, CultureInfo.InvariantCulture);

        private static Contact ReadContact(dynamic item, OutlookImportOptions options, bool pictures, ImportResult result)
        {
            var contact = new Contact
            {
                LastName = Text(item.LastName),
                FirstName = Text(item.FirstName),
                Company = Text(item.CompanyName),
                Home = Text(item.HomeTelephoneNumber),
                Work = Text(item.BusinessTelephoneNumber),
                HomeFax = Text(item.HomeFaxNumber),
                WorkFax = Text(item.BusinessFaxNumber),
                Mobile = Text(item.MobileTelephoneNumber),
                Street = Text(item.MailingAddressStreet).Replace("\r\n", " - ").Replace("\n", " - "),
                Zip = Text(item.MailingAddressPostalCode),
                City = Text(item.MailingAddressCity),
                Email = Text(item.Email1Address),
            };

            object birthday = item.Birthday;
            if (birthday is DateTime date && date.Year > 1 && date.Date != NoDate)
            {
                contact.Birthday = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }

            string other = Text(item.OtherTelephoneNumber);
            if (options.ImportOtherNumber && other != "")
            {
                ContactSlots.PutIntoFreeSlot(contact, other, options.Import.TypePreference);
            }

            if (pictures && (bool)item.HasPicture)
            {
                contact.Photo = ReadPhoto(item);
            }

            contact.CombinedName = options.UseFileAs
                ? Text(item.FileAs)
                : NameFormatter.GenerateFullName(contact.FirstName, contact.LastName, contact.Company, options.Import.NameStyle);

            return contact;
        }

        private static byte[] ReadPhoto(dynamic item)
        {
            dynamic attachments = item.Attachments;
            int count = attachments.Count;
            for (int i = 1; i <= count; i++)
            {
                try
                {
                    dynamic attachment = attachments.Item(i);
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
                catch (Exception ex) when (ex is COMException || ex is IOException || ex is Microsoft.CSharp.RuntimeBinder.RuntimeBinderException || ex is InvalidCastException)
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
            dynamic folder = outlook.GetNamespace("MAPI").PickFolder();
            if (folder == null) { return -1; }

            bool pictures = exportPictures && SupportsPictures;
            int count = 0;
            foreach (Contact contact in contacts)
            {
                string prefix = applyPrefix ? contact.FritzPrefix : "";
                dynamic item = folder.Items.Add(OlContactItem);

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

                // Outlook needs a year; birthdays without year are not exported
                if (BirthdayParser.TryGetDate(contact.Birthday, out DateTime birthday, out bool hasYear) && hasYear)
                {
                    item.Birthday = birthday;
                }

                string body = (contact.IsVip ? "VIP" + Environment.NewLine : "")
                    + (contact.SpeedDial != "" ? "SPEEDDIAL(" + contact.SpeedDial + ")" : "");
                if (body != "") { item.Body = Text(item.Body) + body; }

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

    /// <summary>Outlook is not installed or cannot be started.</summary>
    public sealed class OutlookNotAvailableException : Exception
    {
        public OutlookNotAvailableException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}
