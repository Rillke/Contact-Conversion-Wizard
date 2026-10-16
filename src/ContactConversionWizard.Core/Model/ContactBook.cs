using System;
using System.Collections;
using System.Collections.Generic;

namespace ContactConversionWizard.Core.Model
{
    /// <summary>
    /// The list of loaded contacts, keyed by <see cref="Contact.CombinedName"/>.
    /// </summary>
    public sealed class ContactBook : IReadOnlyCollection<Contact>
    {
        private const int MaxRenameAttempts = 100;

        private readonly Dictionary<string, Contact> byName = new Dictionary<string, Contact>(StringComparer.Ordinal);
        private readonly List<Contact> contacts = new List<Contact>();

        public int Count => contacts.Count;

        /// <summary>When set, duplicates are imported with the last two characters of the name replaced by a counter.</summary>
        public bool RenameDuplicates { get; set; }

        public bool Contains(string combinedName) => byName.ContainsKey(combinedName);

        /// <summary>
        /// Adds a contact. Returns <c>null</c> on success, otherwise a message describing why the contact was not added.
        /// </summary>
        public string Add(Contact contact)
        {
            if (contact == null) { throw new ArgumentNullException(nameof(contact)); }

            if (!byName.ContainsKey(contact.CombinedName))
            {
                Insert(contact);
                return null;
            }

            if (!RenameDuplicates)
            {
                return "Duplicate entry in source:" + contact.CombinedName;
            }

            string baseName = contact.CombinedName.Length >= 2
                ? contact.CombinedName.Substring(0, contact.CombinedName.Length - 2)
                : "";
            for (int counter = 0; counter < MaxRenameAttempts; counter++)
            {
                string candidate = baseName + counter.ToString("D2");
                if (!byName.ContainsKey(candidate))
                {
                    contact.CombinedName = candidate;
                    Insert(contact);
                    return null;
                }
            }

            return "Duplicate entry in source even after renaming 100 times:" + contact.CombinedName;
        }

        /// <summary>Adds all contacts and returns the messages for those that could not be added.</summary>
        public IList<string> AddRange(IEnumerable<Contact> toAdd)
        {
            var messages = new List<string>();
            foreach (Contact contact in toAdd)
            {
                string message = Add(contact);
                if (message != null) { messages.Add(message); }
            }
            return messages;
        }

        public void Clear()
        {
            byName.Clear();
            contacts.Clear();
        }

        public IEnumerator<Contact> GetEnumerator() => contacts.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void Insert(Contact contact)
        {
            byName.Add(contact.CombinedName, contact);
            contacts.Add(contact);
        }
    }
}
