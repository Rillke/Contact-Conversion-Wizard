using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using ContactConversionWizard.Core.Import;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;
using Google.Contacts;
using Google.GData.Client;
using Google.GData.Contacts;
using Google.GData.Extensions;
using GoogleContact = Google.Contacts.Contact;

namespace Contact_Conversion_Wizard
{
    /// <summary>Google Contacts access using the (legacy) GData API with ClientLogin.</summary>
    public sealed class GoogleContactsConnector
    {
        private readonly ContactsRequest request;
        private readonly string token;

        private GoogleContactsConnector(ContactsRequest request, string token)
        {
            this.request = request;
            this.token = token;
        }

        /// <summary>Logs in; throws if the login fails.</summary>
        public static GoogleContactsConnector Login(string applicationName, string login, string password)
        {
            var settings = new RequestSettings(applicationName, login, password) { AutoPaging = true };
            var request = new ContactsRequest(settings);
            string token = request.Service.QueryClientLoginToken();
            return new GoogleContactsConnector(request, token);
        }

        /// <summary>All groups as (title, id).</summary>
        public List<string[]> GetGroups()
        {
            var groups = new List<string[]>();
            foreach (Group group in request.GetGroups().Entries) { groups.Add(new[] { group.Title, group.Id }); }
            return groups;
        }

        /// <param name="groupIds">Only import members of these groups; <c>null</c> for all contacts.</param>
        public ImportResult Import(ImportOptions options, bool importMainNumber, ICollection<string> groupIds)
        {
            var result = new ImportResult();

            foreach (GoogleContact entry in request.GetContacts().Entries)
            {
                if (entry.Name == null)
                {
                    result.Warnings.Add("An entry in the google contact database was retrieved that had no name, ignoring.");
                    continue;
                }

                if (groupIds != null && !IsMemberOf(entry, groupIds)) { continue; }

                var contact = new ContactConversionWizard.Core.Model.Contact
                {
                    LastName = entry.Name.FamilyName,
                    FirstName = entry.Name.GivenName,
                };
                if (entry.Organizations.Count > 0) { contact.Company = entry.Organizations[0].Name; }

                string mainNumber = "";
                foreach (PhoneNumber number in entry.Phonenumbers)
                {
                    if (string.IsNullOrEmpty(number.Value)) { continue; }
                    if (number.Rel == ContactsRelationships.IsHome && contact.Home == "") { contact.Home = number.Value; }
                    if (number.Rel == ContactsRelationships.IsWork && contact.Work == "") { contact.Work = number.Value; }
                    if (number.Rel == ContactsRelationships.IsMain) { mainNumber = number.Value; }
                    if (number.Rel == ContactsRelationships.IsHomeFax && contact.HomeFax == "") { contact.HomeFax = number.Value; }
                    if (number.Rel == ContactsRelationships.IsWorkFax && contact.WorkFax == "") { contact.WorkFax = number.Value; }
                    if (number.Rel == ContactsRelationships.IsMobile && contact.Mobile == "") { contact.Mobile = number.Value; }
                }
                if (importMainNumber && mainNumber != "")
                {
                    OutlookConnector.PutIntoFreeSlot(contact, mainNumber, options.TypePreference);
                }

                if (entry.PostalAddresses.Count > 0)
                {
                    contact.Street = entry.PostalAddresses[0].Street;
                    contact.Zip = entry.PostalAddresses[0].Postcode;
                    contact.City = entry.PostalAddresses[0].City;
                }

                if (entry.PrimaryEmail != null) { contact.Email = entry.PrimaryEmail.Address; }

                if (entry.PhotoEtag != null)
                {
                    try
                    {
                        contact.Photo = DownloadPhoto(entry.ContactEntry.PhotoUri);
                    }
                    catch (WebException)
                    {
                        // Google sometimes announces photos that do not exist
                    }
                }

                contact.CombinedName = NameFormatter.GenerateFullName(contact.FirstName, contact.LastName, contact.Company, options.NameStyle);

                string notes = entry.Content ?? "";
                NoteFlags.Apply(contact, entry.ContactEntry.Nickname ?? "", notes, options.VipMode);

                if (contact.CombinedName != "" && contact.HasAnyNumber && !NoteFlags.IsIgnored(notes))
                {
                    result.Contacts.Add(contact);
                }
            }

            return result;
        }

        /// <summary>Uploads the contacts in batches of 100; returns one status message per batch.</summary>
        public List<string> Export(IEnumerable<ContactConversionWizard.Core.Model.Contact> contacts, PhoneNumberCleaner cleaner, bool applyPrefix, TypePreference preference)
        {
            var entries = new List<GoogleContact>();
            foreach (ContactConversionWizard.Core.Model.Contact contact in contacts)
            {
                entries.Add(ToGoogleContact(contact, cleaner, applyPrefix ? contact.FritzPrefix : "", preference));
            }

            var messages = new List<string>();
            var batchUri = new Uri("https://www.google.com/m8/feeds/contacts/default/full/batch");
            for (int start = 0; start < entries.Count; start += 100)
            {
                List<GoogleContact> batch = entries.GetRange(start, Math.Min(100, entries.Count - start));
                Feed<GoogleContact> response = request.Batch(batch, batchUri, GDataBatchOperationType.Default);

                var errors = new StringBuilder();
                foreach (GoogleContact entry in response.Entries)
                {
                    if (entry.BatchData.Status.Code != 201)
                    {
                        errors.Append(entry.BatchData.Status.Code + " (" + entry.BatchData.Status.Reason + ") / ");
                    }
                }
                messages.Add((start + batch.Count) + "/" + entries.Count + " contacts have been written to the Google Folder 'Other Contacts'!" + Environment.NewLine + errors);
            }
            return messages;
        }

        private static GoogleContact ToGoogleContact(ContactConversionWizard.Core.Model.Contact contact, PhoneNumberCleaner cleaner, string prefix, TypePreference preference)
        {
            var entry = new GoogleContact
            {
                Content = (contact.IsVip ? "VIP" + Environment.NewLine : "") + (contact.SpeedDial != "" ? "SPEEDDIAL(" + contact.SpeedDial + ")" : ""),
                Name = new Name { FullName = contact.CombinedName, GivenName = contact.FirstName, FamilyName = contact.LastName },
            };
            if (contact.Company != "") { entry.Organizations.Add(new Organization { Primary = true, Rel = ContactsRelationships.IsWork, Name = contact.Company }); }
            if (contact.Email != "") { entry.Emails.Add(new EMail { Primary = true, Rel = ContactsRelationships.IsHome, Address = contact.Email }); }

            PhoneType preferred = contact.Preferred;
            AddNumber(entry, cleaner.Clean(contact.Work, prefix), ContactsRelationships.IsWork, preferred == PhoneType.Work);
            AddNumber(entry, cleaner.Clean(contact.Home, prefix), ContactsRelationships.IsHome, preferred == PhoneType.Home);
            AddNumber(entry, cleaner.Clean(contact.Mobile, prefix), ContactsRelationships.IsMobile, preferred == PhoneType.Mobile);
            AddNumber(entry, cleaner.Clean(contact.HomeFax, prefix), ContactsRelationships.IsHomeFax, false);
            AddNumber(entry, cleaner.Clean(contact.WorkFax, prefix), ContactsRelationships.IsWorkFax, false);

            entry.PostalAddresses.Add(new StructuredPostalAddress
            {
                Rel = preference == TypePreference.Home ? ContactsRelationships.IsHome : ContactsRelationships.IsWork,
                Primary = true,
                Street = contact.Street,
                City = contact.City,
                Postcode = contact.Zip,
            });
            return entry;
        }

        private static void AddNumber(GoogleContact entry, string number, string rel, bool primary)
        {
            if (number != "") { entry.Phonenumbers.Add(new PhoneNumber { Primary = primary, Rel = rel, Value = number }); }
        }

        private static bool IsMemberOf(GoogleContact entry, ICollection<string> groupIds)
        {
            foreach (GroupMembership membership in entry.GroupMembership)
            {
                if (groupIds.Contains(membership.HRef)) { return true; }
            }
            return false;
        }

        private byte[] DownloadPhoto(Uri photoUri)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(photoUri);
            webRequest.Headers.Add("Authorization", "GoogleLogin auth=" + token);
            using (var response = (HttpWebResponse)webRequest.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (var buffer = new MemoryStream())
            {
                stream.CopyTo(buffer);
                return buffer.ToArray();
            }
        }
    }
}
