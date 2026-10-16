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
                            Contact contact = ParseContact(element, result);
                            if (options.Accept(contact)) { result.Contacts.Add(contact); }
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

            return result;
        }

        private Contact ParseContact(XElement element, ImportResult result)
        {
            var contact = new Contact();

            contact.IsVip = (string)element.Element("category") == "1";
            contact.LastName = ((string)element.Element("person")?.Element("realName") ?? "").Trim();
            contact.CombinedName = contact.LastName;
            contact.Email = ((string)element.Element("services")?.Elements("email").FirstOrDefault(e => !string.IsNullOrWhiteSpace(e.Value)) ?? "").Trim();

            // numbers that do not fit into the field of their type are kept until all typed numbers are known
            var otherNumbers = new List<string>();

            IEnumerable<XElement> numbers = element.Element("telephony")?.Elements("number") ?? Enumerable.Empty<XElement>();
            foreach (XElement number in numbers)
            {
                string value = number.Value.Trim();
                if (value == "") { continue; }

                string quickDial = (string)number.Attribute("quickdial");
                string vanity = (string)number.Attribute("vanity");
                if (!string.IsNullOrEmpty(quickDial))
                {
                    contact.SpeedDial = string.IsNullOrEmpty(vanity) ? contact.SpeedDial + quickDial : quickDial + "," + vanity;
                }

                string type = (string)number.Attribute("type") ?? "";
                bool preferred = (string)number.Attribute("prio") == "1";
                contact.AddNumber(KindOf(type), value, preferred, type);

                switch (type)
                {
                    case "home":
                        SetNumber(contact, PhoneType.Home, value, preferred, otherNumbers);
                        break;
                    case "work":
                        SetNumber(contact, PhoneType.Work, value, preferred, otherNumbers);
                        break;
                    case "mobile":
                        SetNumber(contact, PhoneType.Mobile, value, preferred, otherNumbers);
                        break;
                    case "fax_work":
                        if (options.TypePreference == TypePreference.Home && contact.HomeFax == "") { contact.HomeFax = value; }
                        else if (contact.WorkFax == "") { contact.WorkFax = value; }
                        else if (contact.HomeFax == "") { contact.HomeFax = value; }
                        else { Dropped(result, contact, value, type); }
                        break;
                    case "other":   // "Sonstige"
                    case "intern":
                    case "memo":
                    default:
                        otherNumbers.Add(value);
                        if (type != "other" && type != "intern" && type != "memo")
                        {
                            result.Warnings.Add("Unknown number type \"" + type + "\" of " + contact.CombinedName + " was imported like \"other\": " + value);
                        }
                        break;
                }
            }

            foreach (string other in otherNumbers)
            {
                if (!ContactSlots.PutIntoFreeSlot(contact, other, options.TypePreference, includeMobile: true))
                {
                    Dropped(result, contact, other, "other");
                }
            }

            return contact;
        }

        private NumberKind KindOf(string type)
        {
            switch (type)
            {
                case "home": return NumberKind.Home;
                case "work": return NumberKind.Work;
                case "mobile": return NumberKind.Mobile;
                case "fax_work": return options.TypePreference == TypePreference.Home ? NumberKind.HomeFax : NumberKind.WorkFax;
                default: return NumberKind.Other;
            }
        }

        private static void SetNumber(Contact contact, PhoneType type, string value, bool preferred, List<string> overflow)
        {
            string current = ContactSlots.Get(contact, type);
            if (current != "" && current != value)
            {
                // only one number per type is supported, the additional one is treated like "other"
                overflow.Add(value);
                return;
            }

            ContactSlots.Set(contact, type, value);
            if (preferred) { contact.ExplicitPreferred = type; }
        }

        private static void Dropped(ImportResult result, Contact contact, string number, string type)
        {
            result.Warnings.Add(contact.CombinedName + ": the number " + number + " (" + type + ") was not imported, all number fields are in use.");
        }
    }
}
