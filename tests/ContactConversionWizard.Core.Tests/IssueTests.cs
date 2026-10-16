using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ContactConversionWizard.Core.Export;
using ContactConversionWizard.Core.Import;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace ContactConversionWizard.Core.Tests
{
    /// <summary>Regression tests for reported GitHub issues.</summary>
    public class IssueTests
    {
        private static ExportOptions Options() => new ExportOptions { CountryPrefix = "0049" };

        [Fact]
        public void Issue17_MacintoshCharsetIsDecoded()
        {
            // "Küchle" in Mac OS Roman: ü = 0x9F
            byte[] data = TextEncodings.Latin1.GetBytes("BEGIN:VCARD\r\nVERSION:2.1\r\nN;CHARSET=MACINTOSH:K\u009Fchle;Erika;;;\r\nFN;CHARSET=MACINTOSH:Erika K\u009Fchle\r\nTEL;WORK;VOICE;pref:00000 - 00 0 00\r\nEND:VCARD\r\n");

            Contact contact = new VCardImporter(new ImportOptions()).Import(data).Contacts.Single();

            Assert.Equal("Küchle", contact.LastName);
            Assert.Equal("Erika", contact.FirstName);
        }

        [Fact]
        public void Issue21_QuotedPrintableUtf8IsDecoded()
        {
            // the example of the issue, including the soft line break in the middle of a UTF-8 sequence
            const string vcard =
                "BEGIN:VCARD\r\nVERSION:2.1\r\n" +
                "N;CHARSET=UTF-8;ENCODING=QUOTED-PRINTABLE:=C3=96=C3=A4=C3=BC;=54=65=73=74=20=45=6E=74=72=79=20=C3=B6=C3=A4=C3=BC;;;\r\n" +
                "FN;CHARSET=UTF-8;ENCODING=QUOTED-PRINTABLE:=54=65=73=74=20=45=6E=74=72=79=20=C3=B6=C3=A4=C3=BC=20=C3=96=C3=A4=C3=\r\n" +
                "=BC\r\n" +
                "TEL;CELL:123\r\nEND:VCARD\r\n";

            Contact contact = new VCardImporter(new ImportOptions()).Import(Encoding.ASCII.GetBytes(vcard)).Contacts.Single();

            Assert.Equal("Öäü", contact.LastName);
            Assert.Equal("Test Entry öäü", contact.FirstName);
            Assert.Equal("123", contact.Mobile);
        }

        [Fact]
        public void Issue21_QuotedPrintableWithLatin1Charset()
        {
            byte[] data = Encoding.ASCII.GetBytes("BEGIN:VCARD\nN;CHARSET=ISO-8859-1;ENCODING=QUOTED-PRINTABLE:M=FCller;J=F6rg\nTEL:1\nEND:VCARD\n");
            Assert.Equal("Müller Jörg", new VCardImporter(new ImportOptions()).Import(data).Contacts.Single().CombinedName);
        }

        [Fact]
        public void DetectsWindows1252WhenNotUtf8()
        {
            byte[] data = TextEncodings.Latin1.GetBytes("BEGIN:VCARD\nN:Müller;Jörg\nTEL:1\nEND:VCARD\n");
            Assert.Equal("Müller Jörg", new VCardImporter(new ImportOptions()).Import(data).Contacts.Single().CombinedName);
        }

        [Fact]
        public void ReadsUtf16Files()
        {
            byte[] data = Encoding.Unicode.GetPreamble().Concat(Encoding.Unicode.GetBytes("BEGIN:VCARD\r\nN:Müller;Jörg\r\nTEL:1\r\nEND:VCARD\r\n")).ToArray();
            Assert.Equal("Müller Jörg", new VCardImporter(new ImportOptions()).Import(data).Contacts.Single().CombinedName);
        }

        [Fact]
        public void Issue18_GenericCsvExportCanBeImportedAgain()
        {
            var contacts = new List<Contact>
            {
                new Contact { FirstName = "Jörg", LastName = "Müller", Company = "A, \"B\"", CombinedName = "Müller Jörg", Home = "030 1", Email = "j@example.com", Birthday = "1980-01-19", IsVip = true, SpeedDial = "05" },
            };

            ExportResult result = new GenericCsvExporter().Export(contacts, Options());
            byte[] bytes = result.GetBytes();
            Assert.Equal(new byte[] { 0xEF, 0xBB, 0xBF }, bytes.Take(3).ToArray()); // BOM for Excel

            List<string[]> rows = CsvParser.Parse(Encoding.UTF8.GetString(bytes).TrimStart('﻿'), ',');
            Assert.Equal(GenericCsvExporter.Columns, rows[0]);

            // map the columns by header like the CSV row assigner does
            CsvField[] assignment = rows[0].Select(h => CsvContactMapper.FieldNames.Contains(h) ? (CsvField)(CsvContactMapper.FieldNames.ToList().IndexOf(h) - 1) : CsvField.Ignore).ToArray();
            Contact imported = new CsvContactMapper(new ImportOptions { VipMode = VipMode.IfNotesContainVip }).Map(rows.Skip(1), assignment).Contacts.Single();

            Assert.Equal("Müller", imported.LastName);
            Assert.Equal("Jörg", imported.FirstName);
            Assert.Equal("A, \"B\"", imported.Company);
            Assert.Equal("0301", imported.Home);
            Assert.Equal("1980-01-19", imported.Birthday);
            Assert.True(imported.IsVip);
            Assert.Equal("05", imported.SpeedDial);
        }

        [Fact]
        public void Issue20_ExportsAreSortedAlphabetically()
        {
            var contacts = new List<Contact>
            {
                new Contact { CombinedName = "Zimmer", Home = "1" },
                new Contact { CombinedName = "Ärger", Home = "2" },
                new Contact { CombinedName = "bauer", Home = "3", Mobile = "4" },
                new Contact { CombinedName = "Adler", Home = "5" },
            };

            foreach (ContactExporter exporter in Exporters.All)
            {
                string content = exporter.Export(contacts, Options()).Content;
                int adler = content.IndexOf("Adler", StringComparison.Ordinal);
                int aerger = content.IndexOf("rger", StringComparison.Ordinal);
                int bauer = content.IndexOf("bauer", StringComparison.Ordinal);
                int zimmer = content.IndexOf("Zimmer", StringComparison.Ordinal);
                Assert.True(adler < aerger && aerger < bauer && bauer < zimmer, exporter.FormatName);
            }

            // the home line of a contact still comes before its mobile line
            string snom = new SnomV7CsvExporter().Export(contacts, Options()).Content;
            Assert.True(snom.IndexOf("bauer H", StringComparison.Ordinal) < snom.IndexOf("bauer M", StringComparison.Ordinal));
        }

        [Fact]
        public void Issue22_GrandstreamGxpUsesFirstAndLastName()
        {
            var contacts = new List<Contact>
            {
                new Contact { FirstName = "John", LastName = "Doe", CombinedName = "Doe John", Home = "1", Mobile = "2" },
                new Contact { FirstName = "Jane", LastName = "Doe", CombinedName = "Doe Jane", Work = "3" },
                new Contact { Company = "ACME", CombinedName = "ACME", Work = "4" },
                new Contact { FirstName = "Max", LastName = "Muster", Company = "Corp", CombinedName = "Muster Max [Corp]", Work = "5" },
            };

            var options = Options();
            options.NameStyle = NameStyle.LastFirst;
            string content = new GrandstreamGxpExporter().Export(contacts, options).Content;

            Assert.Contains("<LastName>Doe H</LastName>\n        <FirstName>John</FirstName>", content);
            Assert.Contains("<LastName>Doe M</LastName>\n        <FirstName>John</FirstName>", content);
            Assert.Contains("<LastName>Doe</LastName>\n        <FirstName>Jane</FirstName>", content);
            Assert.Contains("<LastName>ACME</LastName>\n        <FirstName></FirstName>", content);
            Assert.Contains("<LastName>Muster</LastName>\n        <FirstName>Max</FirstName>", content);

            // with a name style that includes the company, contacts with company keep the combined name
            options.NameStyle = NameStyle.LastFirstBracketCompany;
            Assert.Contains("<LastName>Muster Max [Corp]</LastName>\n        <FirstName></FirstName>", new GrandstreamGxpExporter().Export(contacts, options).Content);
        }

        [Fact]
        public void Issue23_FritzBoxOtherNumbersAreImported()
        {
            const string xml =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><phonebooks><phonebook><contact><category>0</category><person><realName>Doe</realName></person><telephony>" +
                "<number type=\"home\">1</number><number type=\"other\">2</number><number type=\"mobile\">3</number><number type=\"other\">4</number><number type=\"other\">5</number>" +
                "</telephony></contact></phonebook></phonebooks>";

            ImportResult result = new FritzXmlImporter(new ImportOptions()).Import(new MemoryStream(Encoding.UTF8.GetBytes(xml)));

            Contact contact = result.Contacts.Single();
            Assert.Equal("1", contact.Home);
            Assert.Equal("2", contact.Work);
            Assert.Equal("3", contact.Mobile);
            Assert.Equal(2, result.Warnings.Count); // 4 and 5 do not fit and are reported instead of silently dropped
        }

        [Fact]
        public void Issue1_SecondNumberOfATypeIsKept()
        {
            const string xml =
                "<phonebooks><phonebook><contact><person><realName>Doe</realName></person><telephony>" +
                "<number type=\"mobile\">1</number><number type=\"mobile\" prio=\"1\">2</number>" +
                "</telephony></contact></phonebook></phonebooks>";

            Contact contact = new FritzXmlImporter(new ImportOptions()).Import(new MemoryStream(Encoding.UTF8.GetBytes(xml))).Contacts.Single();
            Assert.Equal("1", contact.Mobile);
            Assert.Equal("2", contact.Home);
        }

        [Theory]
        [InlineData("BDAY:1980-01-19", "1980-01-19")]
        [InlineData("BDAY:19800119", "1980-01-19")]
        [InlineData("BDAY:--0119", "--01-19")]
        [InlineData("BDAY:--01-19", "--01-19")]
        [InlineData("BDAY;VALUE=date:1980-01-19T00:00:00Z", "1980-01-19")]
        [InlineData("BDAY;X-APPLE-OMIT-YEAR=1604:1604-01-19", "--01-19")]
        public void Issue28_BirthdayIsImported(string line, string expected)
        {
            string vcard = "BEGIN:VCARD\nN:Doe;J\nTEL:1\n" + line + "\nEND:VCARD\n";
            Assert.Equal(expected, new VCardImporter(new ImportOptions()).Import(vcard).Contacts.Single().Birthday);
        }

        [Theory]
        [InlineData("1980-01-19", "BDAY:1980-01-19")]
        [InlineData("--01-19", "BDAY:1900-01-19")]
        [InlineData("--02-29", "BDAY:1904-02-29")]
        public void Issue28_BirthdayIsExportedWithPlaceholderYear(string birthday, string expected)
        {
            var contacts = new List<Contact> { new Contact { CombinedName = "Doe", Home = "1", Birthday = birthday } };
            Assert.Contains(expected + "\r\n", new VCardExporter().Export(contacts, Options()).Content);
        }

        [Theory]
        [InlineData("19.01.1980", "1980-01-19")]
        [InlineData("19.1.", "--01-19")]
        [InlineData("1980-02-30", null)]
        [InlineData("tomorrow", null)]
        [InlineData("", "")]
        public void BirthdayParser_Normalize(string input, string expected)
        {
            Assert.Equal(expected, BirthdayParser.Normalize(input));
        }

        [Fact]
        public void Issue30_ContactsWithoutNumberCanBeImported()
        {
            const string vcard = "BEGIN:VCARD\nN:Doe;J\nEMAIL:j@example.com\nEND:VCARD\nBEGIN:VCARD\nEMAIL:nobody@example.com\nEND:VCARD\n";

            Assert.Empty(new VCardImporter(new ImportOptions()).Import(vcard).Contacts);

            var withoutNumber = new VCardImporter(new ImportOptions { ImportWithoutNumber = true }).Import(vcard).Contacts;
            Assert.Equal("Doe J", withoutNumber.Single().CombinedName);

            var all = new VCardImporter(new ImportOptions { ImportWithoutNumber = true, ImportWithoutName = true }).Import(vcard).Contacts;
            Assert.Equal(new[] { "Doe J", "nobody@example.com" }, all.Select(c => c.CombinedName));
        }

        [Fact]
        public void Issue30_IgnoredContactsStayIgnored()
        {
            var options = new ImportOptions { ImportWithoutNumber = true, ImportWithoutName = true };
            Assert.False(options.Accept(new Contact { CombinedName = "x" }, "CCW-IGNORE"));
        }

        [Fact]
        public void Issue32_SnomMasterIsReferencedByItsFirstNumber()
        {
            // the example of the issue
            var contacts = new List<Contact>
            {
                new Contact { FirstName = "Max", LastName = "Mustermann", CombinedName = "Max Mustermann", Email = "max@mustermann.de", Home = "0123456789", Mobile = "00112233", ExplicitPreferred = PhoneType.Mobile },
            };

            string content = new SnomV8CsvExporter().Export(contacts, new ExportOptions()).Content;

            Assert.Equal(
                "\"Max Mustermann\",\"00112233\",\"MASTER\",\"\",\"Max\",\"Mustermann\",\"\",\"\",\"max@mustermann.de\",\"\",\"\",\"\",\"false\",\"\",\"sip\",\"\"\r\n" +
                "\"\",\"00112233\",\"none\",\"\",\"Member_Alias\",\"00112233\",\"\",\"\",\"\",\"\",\"\",\"\",\"false\",\"\",\"mobile\",\"\"\r\n" +
                "\"\",\"0123456789\",\"none\",\"\",\"Member_Alias\",\"00112233\",\"\",\"\",\"\",\"\",\"\",\"\",\"false\",\"\",\"home\",\"\"\r\n",
                content);
        }

        [Fact]
        public void Issue32_SharedNumbersGetUniqueMasterKeys()
        {
            var contacts = new List<Contact>
            {
                new Contact { CombinedName = "A", Home = "1", Work = "2" },
                new Contact { CombinedName = "B", Home = "1", Work = "2" },
                new Contact { CombinedName = "C", Home = "1", Work = "2" },
            };

            ExportResult result = new SnomV8CsvExporter().Export(contacts, new ExportOptions());
            List<string[]> rows = CsvParser.Parse(result.Content, ',');
            string[] masters = rows.Where(r => r[2] == "MASTER").Select(r => r[1]).ToArray();

            Assert.Equal(new[] { "1", "2", "00100001" }, masters);
            Assert.Single(result.Warnings);
            Assert.Contains(rows, r => r[1] == "2" && r[5] == "2" && r[14] == "business");
        }

        [Theory]
        [InlineData("Family", "Family", true)]
        [InlineData("Family", "Friends; Family", true)]
        [InlineData("Family", "Business", false)]
        [InlineData("family", "Family", true)]
        [InlineData("Family; Friends", "Friends", true)]
        [InlineData("!Business", "Friends", true)]
        [InlineData("!Business", "Friends, Business", false)]
        [InlineData("Friends; -Business", "Friends; Business", false)]
        [InlineData("!Business", "", true)]
        [InlineData("Family", "", false)]
        public void Issue34_CategoryFilter(string filter, string categories, bool expected)
        {
            Assert.Equal(expected, new CategoryFilter(filter).Matches(categories));
        }

        [Fact]
        public void UntypedTelephoneNumbersAreKept()
        {
            const string vcard = "BEGIN:VCARD\nN:Doe;J\nTEL:030 1\nTEL;VOICE:030 2\nTEL;TYPE=CELL:0170\nTEL;TYPE=OTHER:4\nEND:VCARD\n";
            ImportResult result = new VCardImporter(new ImportOptions { TypePreference = TypePreference.Work }).Import(vcard);

            Contact contact = result.Contacts.Single();
            Assert.Equal("030 1", contact.Work);
            Assert.Equal("030 2", contact.Home);
            Assert.Equal("0170", contact.Mobile);
            Assert.Single(result.Warnings); // the fourth number has no free field
        }

        [Fact]
        public void VCard4PreferenceAndUris()
        {
            const string vcard = "BEGIN:VCARD\nVERSION:4.0\nN:Doe;J;;;\nTEL;TYPE=home:tel:+49301\nTEL;TYPE=work;PREF=1:tel:+49302\nEMAIL:mailto:j@example.com\nEND:VCARD\n";
            Contact contact = new VCardImporter(new ImportOptions()).Import(vcard).Contacts.Single();
            Assert.Equal("+49301", contact.Home);
            Assert.Equal("+49302", contact.Work);
            Assert.Equal(PhoneType.Work, contact.Preferred);
            Assert.Equal("j@example.com", contact.Email);
        }

        [Fact]
        public void QuotedPrintable_Decode()
        {
            Assert.Equal(new byte[] { 0xC3, 0x96, (byte)'a', (byte)'=' , (byte)'x' }, QuotedPrintable.Decode("=C3=96a=x"));
            Assert.Equal(Encoding.ASCII.GetBytes("ab"), QuotedPrintable.Decode("a=\r\nb="));
        }

        [Theory]
        [InlineData("MACINTOSH", 10000)]
        [InlineData("macroman", 10000)]
        [InlineData("UTF-8", 65001)]
        [InlineData("ISO-8859-1", 28591)]
        [InlineData("windows-1252", 1252)]
        public void TextEncodings_GetEncoding(string name, int codePage)
        {
            Assert.Equal(codePage, TextEncodings.GetEncoding(name).CodePage);
        }

        [Fact]
        public void TextEncodings_UnknownCharset()
        {
            Assert.Null(TextEncodings.GetEncoding("no-such-charset"));
            Assert.Null(TextEncodings.GetEncoding(""));
        }
    }
}
