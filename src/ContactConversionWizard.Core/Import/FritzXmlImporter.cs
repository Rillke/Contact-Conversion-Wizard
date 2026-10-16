using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using ContactConversionWizard.Core.Model;

namespace ContactConversionWizard.Core.Import
{
    /// <summary>Reads phone books exported from an AVM FRITZ!Box (XML).</summary>
    public sealed class FritzXmlImporter
    {
        private readonly ImportOptions options;

        public FritzXmlImporter(ImportOptions options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>Reads the phone book. The encoding is taken from the XML declaration (UTF-8 if missing).</summary>
        public ImportResult Import(Stream input)
        {
            if (input == null) { throw new ArgumentNullException(nameof(input)); }

            var result = new ImportResult();
            var unknownTypes = new SortedSet<string>(StringComparer.Ordinal);

            try
            {
                using (XmlReader reader = SecureXml.CreateReader(input))
                {
                    reader.Read();
                    while (!reader.EOF)
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "contact")
                        {
                            // ReadFrom positions the reader on the node following the contact
                            var element = (XElement)XNode.ReadFrom(reader);
                            result.Contacts.Add(ParseContact(element, unknownTypes));
                        }
                        else
                        {
                            reader.Read();
                        }
                    }
                }
            }
            catch (XmlException ex)
            {
                result.Warnings.Add("error occured: " + ex.Message);
            }

            foreach (string unknown in unknownTypes)
            {
                result.Warnings.Add("Unknown XML Attribute: " + unknown);
            }

            return result;
        }

        private Contact ParseContact(XElement element, ISet<string> unknownTypes)
        {
            var contact = new Contact();

            contact.IsVip = (string)element.Element("category") == "1";
            contact.LastName = ((string)element.Element("person")?.Element("realName") ?? "").Trim();
            contact.CombinedName = contact.LastName;
            contact.Email = ((string)element.Element("services")?.Elements("email").FirstOrDefault(e => !string.IsNullOrWhiteSpace(e.Value)) ?? "").Trim();

            IEnumerable<XElement> numbers = element.Element("telephony")?.Elements("number") ?? Enumerable.Empty<XElement>();
            foreach (XElement number in numbers)
            {
                string quickDial = (string)number.Attribute("quickdial");
                string vanity = (string)number.Attribute("vanity");
                if (!string.IsNullOrEmpty(quickDial))
                {
                    contact.SpeedDial = string.IsNullOrEmpty(vanity) ? contact.SpeedDial + quickDial : quickDial + "," + vanity;
                }

                string type = (string)number.Attribute("type") ?? "";
                bool preferred = (string)number.Attribute("prio") == "1";
                string value = number.Value.Trim();

                switch (type)
                {
                    case "home":
                        contact.Home = value;
                        if (preferred) { contact.ExplicitPreferred = PhoneType.Home; }
                        break;
                    case "work":
                        contact.Work = value;
                        if (preferred) { contact.ExplicitPreferred = PhoneType.Work; }
                        break;
                    case "mobile":
                        contact.Mobile = value;
                        if (preferred) { contact.ExplicitPreferred = PhoneType.Mobile; }
                        break;
                    case "fax_work":
                        if (options.TypePreference == TypePreference.Home) { contact.HomeFax = value; }
                        else { contact.WorkFax = value; }
                        break;
                    default:
                        unknownTypes.Add(type + "=" + value);
                        break;
                }
            }

            return contact;
        }
    }
}
