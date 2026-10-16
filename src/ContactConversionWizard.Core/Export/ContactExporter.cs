using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace ContactConversionWizard.Core.Export
{
    /// <summary>Base class of all file based export formats.</summary>
    public abstract class ContactExporter
    {
        /// <summary>A short name of the format, for messages.</summary>
        public abstract string FormatName { get; }

        /// <summary>The proposed file name without extension.</summary>
        public abstract string DefaultFileName { get; }

        /// <summary>The file extension without dot.</summary>
        public abstract string FileExtension { get; }

        /// <summary>A file dialog filter like <c>CSV files (*.csv)|*.csv</c>.</summary>
        public virtual string FileFilter => FileExtension.ToUpperInvariant() + " files (*." + FileExtension + ")|*." + FileExtension;

        public ExportResult Export(IEnumerable<Contact> contacts, ExportOptions options)
        {
            if (contacts == null) { throw new ArgumentNullException(nameof(contacts)); }
            if (options == null) { throw new ArgumentNullException(nameof(options)); }

            var cleaner = new PhoneNumberCleaner(options.Cleanup, options.CountryPrefix);
            var context = new ExportContext(options, cleaner);
            ExportResult result = Export(contacts.ToList(), context);
            result.Warnings.InsertRange(0, cleaner.Warnings.Distinct());
            result.Warnings.AddRange(context.Warnings);
            return result;
        }

        protected abstract ExportResult Export(IReadOnlyList<Contact> contacts, ExportContext context);

        /// <summary>State shared by the steps of one export run.</summary>
        protected sealed class ExportContext
        {
            internal ExportContext(ExportOptions options, PhoneNumberCleaner cleaner)
            {
                Options = options;
                Cleaner = cleaner;
            }

            public ExportOptions Options { get; }

            public PhoneNumberCleaner Cleaner { get; }

            public List<string> Warnings { get; } = new List<string>();

            /// <summary>Cleans the numbers of a contact using its prefix if <paramref name="alwaysUsePrefix"/> or the options say so.</summary>
            public CleanNumbers Clean(Contact contact, bool alwaysUsePrefix = false)
            {
                string prefix = alwaysUsePrefix || Options.ApplyPrefixToAllFormats ? contact.FritzPrefix : "";
                return new CleanNumbers(
                    Cleaner.Clean(contact.Home, prefix),
                    Cleaner.Clean(contact.Work, prefix),
                    Cleaner.Clean(contact.Mobile, prefix),
                    Cleaner.Clean(contact.HomeFax, prefix),
                    Cleaner.Clean(contact.WorkFax, prefix));
            }
        }

        /// <summary>The cleaned numbers of one contact.</summary>
        protected sealed class CleanNumbers
        {
            public CleanNumbers(string home, string work, string mobile, string homeFax, string workFax)
            {
                Home = home;
                Work = work;
                Mobile = mobile;
                HomeFax = homeFax;
                WorkFax = workFax;
            }

            public string Home { get; }
            public string Work { get; }
            public string Mobile { get; }
            public string HomeFax { get; }
            public string WorkFax { get; }

            public bool HasVoiceNumber => Home != "" || Work != "" || Mobile != "";

            /// <summary>True if more than one of home, work and mobile is set (requiring separate entries).</summary>
            public bool NeedsSeparateEntries => (Home != "" ? 1 : 0) + (Work != "" ? 1 : 0) + (Mobile != "" ? 1 : 0) > 1;
        }

        /// <summary>Collects output entries and drops (with a warning) entries whose key already exists.</summary>
        protected sealed class EntryCollector
        {
            private readonly Dictionary<string, string> entries = new Dictionary<string, string>(StringComparer.Ordinal);
            private readonly List<KeyValuePair<string, string>> order = new List<KeyValuePair<string, string>>(); // (sort name, key)
            private readonly ExportContext context;

            public EntryCollector(ExportContext context)
            {
                this.context = context;
            }

            public int Count => order.Count;

            public bool Add(string key, string entry, string sourceName)
            {
                if (entries.ContainsKey(key))
                {
                    context.Warnings.Add("Unable to export entry \"" + sourceName + "=>" + key + "\", another entry with this name already exists - ignoring duplicate!");
                    return false;
                }
                entries.Add(key, entry);
                order.Add(new KeyValuePair<string, string>(sourceName ?? key, key));
                return true;
            }

            /// <summary>
            /// Appends the entries sorted alphabetically by the name of their contact; entries of the same contact
            /// (e.g. home, work and mobile lines) keep their order.
            /// </summary>
            public void AppendTo(StringBuilder sb)
            {
                IEnumerable<string> sorted = order
                    .OrderBy(e => e.Key, StringComparer.InvariantCultureIgnoreCase)
                    .ThenBy(e => e.Key, StringComparer.Ordinal)
                    .Select(e => e.Value);
                foreach (string key in sorted) { sb.Append(entries[key]); }
            }
        }
    }
}
