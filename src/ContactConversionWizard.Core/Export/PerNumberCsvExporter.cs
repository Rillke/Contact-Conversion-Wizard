using System.Collections.Generic;
using System.Text;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace ContactConversionWizard.Core.Export
{
    /// <summary>
    /// Base class for simple formats that write one line per number. If a contact has more than one of
    /// home, work and mobile, the names get the suffixes " H", " W" and " M".
    /// </summary>
    public abstract class PerNumberCsvExporter : ContactExporter
    {
        protected abstract int MaxNameLength { get; }

        protected abstract string Header { get; }

        protected abstract Encoding FileEncoding { get; }

        /// <summary>Adjusts the (already shortened and suffixed) name, e.g. removes separator characters.</summary>
        protected virtual string AdjustName(string name) => name;

        /// <summary>Removes characters before shortening the name.</summary>
        protected virtual string PrepareName(string combinedName) => combinedName;

        /// <summary>The name that is shortened and suffixed for the lines of a contact, by default the combined name.</summary>
        protected virtual string GetNameBase(Contact contact) => contact.CombinedName;

        /// <summary>The key used to detect duplicate lines.</summary>
        protected virtual string GetKey(string name, PhoneType type, Contact contact) => name;

        protected abstract string FormatLine(string name, string number, PhoneType type, Contact contact, string speedDialNumber);

        /// <summary>Called once per contact before its lines are written; returns the quick dial number for the preferred line.</summary>
        protected virtual string GetSpeedDialNumber(Contact contact) => "";

        protected override ExportResult Export(IReadOnlyList<Contact> contacts, ExportContext context)
        {
            var entries = new EntryCollector(context);

            foreach (Contact contact in contacts)
            {
                CleanNumbers numbers = context.Clean(contact);
                if (!numbers.HasVoiceNumber) { continue; }

                bool separate = numbers.NeedsSeparateEntries;
                string baseName = PrepareName(GetNameBase(contact));
                string speedDial = GetSpeedDialNumber(contact);
                PhoneType preferred = contact.Preferred;

                AddLine(entries, contact, baseName, separate, " H", numbers.Home, PhoneType.Home, preferred == PhoneType.Home ? speedDial : "");
                AddLine(entries, contact, baseName, separate, " W", numbers.Work, PhoneType.Work, preferred == PhoneType.Work ? speedDial : "");
                AddLine(entries, contact, baseName, separate, " M", numbers.Mobile, PhoneType.Mobile, preferred == PhoneType.Mobile ? speedDial : "");
            }

            var document = new StringBuilder(Header);
            entries.AppendTo(document);
            return new ExportResult(document.ToString(), FileEncoding, entries.Count);
        }

        private void AddLine(EntryCollector entries, Contact contact, string baseName, bool separate, string suffix, string number, PhoneType type, string speedDial)
        {
            if (number == "") { return; }
            string name = AdjustName(NameFormatter.LimitLength(baseName, MaxNameLength, separate, suffix));
            entries.Add(GetKey(name, type, contact), FormatLine(name, number, type, contact, speedDial), contact.CombinedName);
        }
    }

    /// <summary>Snom phones with v7 firmware: <c>"Name","Number"</c> (UTF-8 without BOM).</summary>
    public sealed class SnomV7CsvExporter : PerNumberCsvExporter
    {
        public override string FormatName => "Snom v7 CSV";
        public override string DefaultFileName => "tbook_v7";
        public override string FileExtension => "csv";
        protected override int MaxNameLength => 31;
        protected override string Header => "\"Name\",\"Number\"\r\n";
        protected override Encoding FileEncoding => TextEncodings.Utf8NoBom;

        protected override string FormatLine(string name, string number, PhoneType type, Contact contact, string speedDialNumber)
            => Escaping.CsvQuoted(name) + "," + Escaping.CsvQuoted(number) + "\r\n";
    }

    /// <summary>Gigaset Talk&amp;Surf: <c>Name,Number</c>, names limited to 16 characters (ISO-8859-1).</summary>
    public sealed class TalkSurfCsvExporter : PerNumberCsvExporter
    {
        public override string FormatName => "Talk&Surf CSV";
        public override string DefaultFileName => "export_talksurf";
        public override string FileExtension => "csv";
        protected override int MaxNameLength => 16;
        protected override string Header => "Name,Number\r\n";
        protected override Encoding FileEncoding => TextEncodings.Latin1;

        // Talk&Surf cannot handle commas in names
        protected override string PrepareName(string combinedName) => Escaping.SingleLine(combinedName).Replace(",", "").Replace("\"", "");

        protected override string FormatLine(string name, string number, PhoneType type, Contact contact, string speedDialNumber)
            => name + "," + number + "\r\n";
    }

    /// <summary>Aastra phones: <c>Name,Number,1,Type,public</c> (UTF-8 without BOM).</summary>
    public sealed class AastraCsvExporter : PerNumberCsvExporter
    {
        public override string FormatName => "Aastra CSV";
        public override string DefaultFileName => "aastra_csv";
        public override string FileExtension => "csv";
        protected override int MaxNameLength => 31;
        protected override string Header => "";
        protected override Encoding FileEncoding => TextEncodings.Utf8NoBom;

        protected override string AdjustName(string name) => Escaping.SingleLine(name).Replace(",", " ").Replace("\"", " ");

        protected override string GetKey(string name, PhoneType type, Contact contact) => name + "#" + type;

        protected override string FormatLine(string name, string number, PhoneType type, Contact contact, string speedDialNumber)
            => name + "," + number + ",1," + type + ",public\n";
    }

    /// <summary>Auerswald PBX: <c>Kurzwahl;Rufnummer;Name</c> (ISO-8859-1).</summary>
    public sealed class AuerswaldCsvExporter : PerNumberCsvExporter
    {
        private SpeedDialAllocator speedDials;

        public override string FormatName => "Auerswald CSV";
        public override string DefaultFileName => "auerswald_csv";
        public override string FileExtension => "csv";
        protected override int MaxNameLength => 31;
        protected override string Header => "Kurzwahl;Rufnummer;Name\n";
        protected override Encoding FileEncoding => TextEncodings.Latin1;

        protected override ExportResult Export(IReadOnlyList<Contact> contacts, ExportContext context)
        {
            speedDials = new SpeedDialAllocator();
            return base.Export(contacts, context);
        }

        protected override string AdjustName(string name) => Escaping.SingleLine(name).Replace(";", " ").Replace("\"", " ");

        protected override string GetSpeedDialNumber(Contact contact)
        {
            speedDials.Resolve(contact.SpeedDial, out string number, out _);
            return number;
        }

        protected override string FormatLine(string name, string number, PhoneType type, Contact contact, string speedDialNumber)
            => speedDialNumber + ";" + number + ";" + name + "\n";
    }
}
