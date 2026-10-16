using System;
using System.Collections.Generic;
using System.Text;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace ContactConversionWizard.Core.Export
{
    /// <summary>Phone book XML that can be imported into an AVM FRITZ!Box.</summary>
    public sealed class FritzXmlExporter : ContactExporter
    {
        public const int MaxNameLength = 32;
        public const int RecommendedMaxContacts = 300;

        public override string FormatName => "Fritz!Box XML";
        public override string DefaultFileName => "FritzExport";
        public override string FileExtension => "xml";

        protected override ExportResult Export(IReadOnlyList<Contact> contacts, ExportContext context)
        {
            ExportOptions options = context.Options;
            var entries = new EntryCollector(context);
            var pictures = new List<ExportedFile>();
            var speedDials = new SpeedDialAllocator();

            foreach (Contact contact in contacts)
            {
                CleanNumbers numbers = context.Clean(contact, alwaysUsePrefix: true);
                if (!numbers.HasVoiceNumber) { continue; }

                string name = Escaping.Xml(NameFormatter.LimitLength(contact.CombinedName, MaxNameLength));
                var sb = new StringBuilder();

                string imageUrl = "";
                if (options.ExportPictures && contact.Photo != null)
                {
                    string pictureName = options.PictureFileNameGenerator();
                    pictures.Add(new ExportedFile(pictureName, contact.Photo));
                    imageUrl = "<imageURL>" + options.PicturePath + pictureName + "</imageURL>\n";
                }

                sb.Append("<contact>\n<category>").Append(contact.IsVip ? "1" : "0").Append("</category>\n<person>\n<realName>")
                  .Append(name).Append("</realName>\n").Append(imageUrl).Append("</person>\n<telephony>\n");

                string preferredAttributes = "prio=\"1\"" + speedDials.GetAttributes(contact.SpeedDial);
                PhoneType preferred = contact.Preferred;

                string home = NumberElement("home", numbers.Home, preferred == PhoneType.Home ? preferredAttributes : null);
                string work = NumberElement("work", numbers.Work, preferred == PhoneType.Work ? preferredAttributes : null);
                if (options.TypePreference == TypePreference.Home) { sb.Append(home).Append(work); }
                else { sb.Append(work).Append(home); }
                sb.Append(NumberElement("mobile", numbers.Mobile, preferred == PhoneType.Mobile ? preferredAttributes : null));

                if (options.IncludeFax)
                {
                    if (numbers.HomeFax != "") { sb.Append("<number type=\"fax_work\">").Append(numbers.HomeFax).Append("</number>\n"); }
                    if (numbers.WorkFax != "") { sb.Append("<number type=\"fax_work\">").Append(numbers.WorkFax).Append("</number>\n"); }
                }

                sb.Append("</telephony>\n<services>\n");
                if (contact.Email != "")
                {
                    sb.Append("<email classifier=\"private\">").Append(Escaping.Xml(contact.Email)).Append("</email>");
                }
                sb.Append("<email />\n");
                sb.Append("</services>\n<setup>\n<ringTone />\n<ringVolume />\n</setup>\n</contact>");

                entries.Add(name, sb.ToString(), name);
            }

            var document = new StringBuilder("<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>\n<phonebooks>\n<phonebook>");
            entries.AppendTo(document);
            document.Append("</phonebook>\n</phonebooks>");

            var result = new ExportResult(document.ToString(), TextEncodings.Latin1, entries.Count);
            result.AdditionalFiles.AddRange(pictures);
            if (entries.Count > RecommendedMaxContacts)
            {
                result.Notices.Add("Warning: Over 300 contacts have been exported! This might or might not be officially supported by AVM's Fritz!Box and may cause problems.");
            }
            return result;
        }

        private static string NumberElement(string type, string number, string preferredAttributes)
        {
            if (number == "") { return "<number type=\"" + type + "\" />\n"; }
            string attributes = preferredAttributes == null ? "" : " " + preferredAttributes;
            return "<number type=\"" + type + "\"" + attributes + ">" + number + "</number>\n";
        }
    }
}
