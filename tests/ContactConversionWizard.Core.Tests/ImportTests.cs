using System.IO;
using System.Linq;
using System.Text;
using ContactConversionWizard.Core.Config;
using ContactConversionWizard.Core.Import;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace ContactConversionWizard.Core.Tests
{
    public class ContactBookTests
    {
        [Fact]
        public void Add_RejectsDuplicates()
        {
            var book = new ContactBook();
            Assert.Null(book.Add(new Contact { CombinedName = "Doe John" }));
            Assert.Equal("Duplicate entry in source:Doe John", book.Add(new Contact { CombinedName = "Doe John" }));
            Assert.Single(book);
        }

        [Fact]
        public void Add_RenamesDuplicatesWhenEnabled()
        {
            var book = new ContactBook { RenameDuplicates = true };
            book.Add(new Contact { CombinedName = "Doe John" });
            var duplicate = new Contact { CombinedName = "Doe John" };
            var second = new Contact { CombinedName = "Doe John" };

            Assert.Null(book.Add(duplicate));
            Assert.Null(book.Add(second));
            Assert.Equal("Doe Jo00", duplicate.CombinedName);
            Assert.Equal("Doe Jo01", second.CombinedName);
            Assert.Equal(3, book.Count);
        }

        [Fact]
        public void Preferred_FallsBackToFirstNumber()
        {
            Assert.Equal(PhoneType.Work, new Contact { Work = "1", Mobile = "2" }.Preferred);
            Assert.Equal(PhoneType.Mobile, new Contact { Work = "1", Mobile = "2", ExplicitPreferred = PhoneType.Mobile }.Preferred);
            Assert.Equal(PhoneType.None, new Contact { HomeFax = "1" }.Preferred);
        }

        [Fact]
        public void Clone_CopiesPhoto()
        {
            var contact = new Contact { CombinedName = "x", Photo = new byte[] { 1 } };
            Contact clone = contact.Clone();
            clone.Photo[0] = 2;
            Assert.Equal(1, contact.Photo[0]);
        }
    }

    public class VCardImporterTests
    {
        private static ImportResult Import(string vcard, ImportOptions options = null)
            => new VCardImporter(options ?? new ImportOptions()).Import(vcard);

        [Fact]
        public void Import_Version30Card()
        {
            const string vcard =
                "BEGIN:VCARD\r\nVERSION:3.0\r\nN:Doe;John;;;\r\nFN:John Doe\r\nORG:ACME;Sales\r\n" +
                "TEL;TYPE=HOME,VOICE:030 1234\r\nTEL;TYPE=CELL:0170 1\r\nTEL;TYPE=WORK;TYPE=FAX:030 9\r\n" +
                "ADR;TYPE=HOME:;;Main St. 1;Berlin;;10115;Germany\r\nEMAIL;TYPE=INTERNET:john@example.com\r\n" +
                "NOTE:VIP SPEEDDIAL(5) PREFIX(#31#)\r\nEND:VCARD\r\n";

            var result = Import(vcard, new ImportOptions { VipMode = VipMode.IfNotesContainVip });

            Contact contact = Assert.Single(result.Contacts);
            Assert.Empty(result.Warnings);
            Assert.Equal("Doe", contact.LastName);
            Assert.Equal("John", contact.FirstName);
            Assert.Equal("ACME Sales", contact.Company);
            Assert.Equal("Doe John [ACME Sales]", contact.CombinedName);
            Assert.Equal("030 1234", contact.Home);
            Assert.Equal("0170 1", contact.Mobile);
            Assert.Equal("030 9", contact.WorkFax);
            Assert.Equal("Main St. 1", contact.Street);
            Assert.Equal("10115", contact.Zip);
            Assert.Equal("Berlin", contact.City);
            Assert.Equal("john@example.com", contact.Email);
            Assert.True(contact.IsVip);
            Assert.Equal("05", contact.SpeedDial);
            Assert.Equal("#31#", contact.FritzPrefix);
        }

        [Fact]
        public void Import_Version21WithFoldingAndGroups()
        {
            const string vcard =
                "BEGIN:VCARD\nVERSION:2.1\nN:Mustermann;Erika\nitem1.TEL;WORK;PREF:0301\n" +
                "TEL;WORK:0302\nTEL;HOME:040\n 123\nEND:VCARD\n";

            Contact contact = Assert.Single(Import(vcard).Contacts);
            Assert.Equal("0301", contact.Work);
            Assert.Equal("040123", contact.Home);
            Assert.Equal(PhoneType.Work, contact.Preferred);
        }

        [Fact]
        public void Import_SkipsContactsWithoutNumberOrIgnored()
        {
            const string vcard =
                "BEGIN:VCARD\nN:NoNumber;A\nEND:VCARD\n" +
                "BEGIN:VCARD\nN:Ignored;B\nTEL;HOME:1\nNOTE:CCW-IGNORE\nEND:VCARD\n" +
                "BEGIN:VCARD\nN:Kept;C\nTEL;HOME:1\nEND:VCARD\n";

            Assert.Equal("Kept C", Import(vcard).Contacts.Single().CombinedName);
        }

        [Fact]
        public void Import_UsesFullNameIfNoNameAndDropsDuplicateCompanyName()
        {
            const string vcard =
                "BEGIN:VCARD\nFN:Only Full Name\nTEL;CELL:1\nEND:VCARD\n" +
                "BEGIN:VCARD\nN:Ignored;Name\nFN:ACME\nORG:ACME\nTEL;CELL:2\nEND:VCARD\n";

            var contacts = Import(vcard).Contacts;
            Assert.Equal("Only Full Name", contacts[0].CombinedName);
            Assert.Equal("ACME", contacts[1].CombinedName);
            Assert.Equal("", contacts[1].LastName);
        }

        [Fact]
        public void Import_NextcloudQuotedTypes()
        {
            const string vcard = "BEGIN:VCARD\nN:Doe;J\nTEL;TYPE=\"cell\":0170\nEND:VCARD\n";
            Assert.Empty(Import(vcard).Contacts);
            Assert.Equal("0170", Import(vcard, new ImportOptions { NextcloudVCardFix = true }).Contacts.Single().Mobile);
        }

        [Fact]
        public void Import_PrefersAddressAndEmailByTypePreference()
        {
            const string vcard =
                "BEGIN:VCARD\nN:Doe;J\nTEL;HOME:1\nADR;TYPE=HOME:;;Home St;Home City;;1;\nADR;TYPE=WORK:;;Work St;Work City;;2;\n" +
                "EMAIL;TYPE=HOME:home@example.com\nEMAIL;TYPE=WORK:work@example.com\nEND:VCARD\n";

            Contact home = Import(vcard).Contacts.Single();
            Assert.Equal("Home St", home.Street);
            Assert.Equal("home@example.com", home.Email);

            Contact work = Import(vcard, new ImportOptions { TypePreference = TypePreference.Work }).Contacts.Single();
            Assert.Equal("Work St", work.Street);
            Assert.Equal("work@example.com", work.Email);
        }

        [Fact]
        public void Import_Photo()
        {
            const string vcard =
                "BEGIN:VCARD\nN:Doe;J\nTEL;HOME:1\nPHOTO;ENCODING=b;TYPE=JPEG:AQID\nEND:VCARD\n" +
                "BEGIN:VCARD\nN:Doe;K\nTEL;HOME:1\nPHOTO:data:image/jpeg;base64,BAUG\nEND:VCARD\n" +
                "BEGIN:VCARD\nN:Doe;L\nTEL;HOME:1\nPHOTO;BASE64:\n  BwgJ\nEND:VCARD\n";

            var contacts = Import(vcard).Contacts;
            Assert.Equal(new byte[] { 1, 2, 3 }, contacts[0].Photo);
            Assert.Equal(new byte[] { 4, 5, 6 }, contacts[1].Photo);
            Assert.Equal(new byte[] { 7, 8, 9 }, contacts[2].Photo);
        }

        [Fact]
        public void Import_ReportsBrokenLines()
        {
            const string vcard = "BEGIN:VCARD\nN:Doe;J\nGARBAGE\nTEL;HOME:1\nPHOTO;BASE64:!!!\nEND:VCARD\n";
            var result = Import(vcard);
            Assert.Single(result.Contacts);
            Assert.Equal(2, result.Warnings.Count);
        }

        [Fact]
        public void Import_FromBytesStripsBom()
        {
            byte[] data = TextEncodings.Utf8WithBom.GetPreamble().Concat(Encoding.UTF8.GetBytes("BEGIN:VCARD\nN:Müller;Jörg\nTEL;HOME:1\nEND:VCARD\n")).ToArray();
            Assert.Equal("Müller Jörg", new VCardImporter(new ImportOptions()).Import(data, Encoding.UTF8).Contacts.Single().CombinedName);
        }

        [Theory]
        [InlineData(";;Street 1;City;;12345;", "Street 1", "12345", "City")]
        [InlineData(";\\nStreet 2\\n\\n12345 Big City\\nGermany\\n;;;;;", "Street 2", "12345", "Big City - Germany")]
        [InlineData(";Only Street;;;;;", "Only Street", "", "")]
        [InlineData(";Street\\nCity;;;;;", "Street", "", "City")]
        public void VCardAddress_Parse(string value, string street, string zip, string city)
        {
            VCardAddress address = VCardAddress.Parse(value);
            Assert.Equal(street, address.Street);
            Assert.Equal(zip, address.Zip);
            Assert.Equal(city, address.City);
        }
    }

    public class FritzXmlImporterTests
    {
        private const string Phonebook =
            "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>\n<phonebooks>\n<phonebook>" +
            "<contact><category>1</category><person><realName>Müller Jörg</realName></person><telephony>" +
            "<number type=\"home\" prio=\"1\" quickdial=\"7\" vanity=\"JOERG\">030123</number>" +
            "<number type=\"work\" />" +
            "<number type=\"mobile\">0170</number>" +
            "<number type=\"fax_work\">0309</number>" +
            "</telephony><services><email classifier=\"private\">j@example.com</email><email /></services></contact>" +
            "<contact><category>0</category><person><realName>Second</realName></person><telephony>" +
            "<number type=\"intern\">**1</number><number type=\"work\">040</number>" +
            "</telephony></contact>" +
            "</phonebook>\n</phonebooks>";

        [Fact]
        public void Import_ReadsContactsInDeclaredEncoding()
        {
            var options = new ImportOptions { TypePreference = TypePreference.Work };
            var result = new FritzXmlImporter(options).Import(new MemoryStream(TextEncodings.Latin1.GetBytes(Phonebook)));

            Assert.Equal(2, result.Contacts.Count);
            Contact first = result.Contacts[0];
            Assert.Equal("Müller Jörg", first.CombinedName);
            Assert.True(first.IsVip);
            Assert.Equal("030123", first.Home);
            Assert.Equal("", first.Work);
            Assert.Equal("0170", first.Mobile);
            Assert.Equal("0309", first.WorkFax);
            Assert.Equal("7,JOERG", first.SpeedDial);
            Assert.Equal(PhoneType.Home, first.ExplicitPreferred);
            Assert.Equal("j@example.com", first.Email);

            Assert.False(result.Contacts[1].IsVip);
            Assert.Equal("040", result.Contacts[1].Work);
            Assert.Equal("Unknown XML Attribute: intern=**1", Assert.Single(result.Warnings));
        }

        [Fact]
        public void Import_RejectsDtd()
        {
            const string xml = "<?xml version=\"1.0\"?><!DOCTYPE x [<!ENTITY e SYSTEM \"file:///etc/passwd\">]><phonebooks><phonebook><contact><person><realName>&e;</realName></person></contact></phonebook></phonebooks>";
            var result = new FritzXmlImporter(new ImportOptions()).Import(new MemoryStream(Encoding.UTF8.GetBytes(xml)));
            Assert.Empty(result.Contacts);
            Assert.Single(result.Warnings);
        }
    }

    public class FritzAdrImporterTests
    {
        private static string Line(params string[] values)
        {
            var columns = Enumerable.Repeat("", FritzAdrImporter.ColumnCount).ToArray();
            for (int i = 0; i + 1 < values.Length; i += 2) { columns[int.Parse(values[i])] = values[i + 1]; }
            return string.Join("\t", columns);
        }

        [Fact]
        public void Import_ParsesColumnsAndTypeSuffix()
        {
            string text = string.Join("\r\n",
                "BEZCHNG\tFIRMA\tNAME\tVORNAME\tABTEILUNG\tSTRASSE\tPLZ\tORT\tKOMMENT\tTELEFON\tTELEFAX\tTRANSFER\tTERMINAL\tBENUTZER\tPASSWORT\tTRANSPROT\tTERMMODE\tNOTIZEN\tMOBILFON\tEMAIL\tHOMEPAGE",
                Line("0", "Doe John", "2", "Doe", "3", "John", "8", "VIP SPEEDDIAL(3)", "9", "030", "10", "039", "18", "0170", "19", "j@example.com"),
                Line("0", "Doe Jane (gesch.)", "9", "040"),
                Line("0", "NoNumber"),
                "broken line");

            var result = new FritzAdrImporter(new ImportOptions { VipMode = VipMode.IfNotesContainVip }).Import(new StringReader(text));

            Assert.Equal(2, result.Contacts.Count);
            Contact john = result.Contacts[0];
            Assert.Equal("030", john.Home);
            Assert.Equal("039", john.HomeFax);
            Assert.Equal("0170", john.Mobile);
            Assert.True(john.IsVip);
            Assert.Equal("03", john.SpeedDial);
            Assert.Equal("040", result.Contacts[1].Work);
            Assert.Single(result.Warnings);
        }
    }

    public class CsvContactMapperTests
    {
        [Fact]
        public void Map_AssignsColumns()
        {
            var rows = new[]
            {
                new[] { "Doe", "John", "0301", "ignored", "VIP" },
                new[] { "NoNumber", "X", "", "", "" },
            };
            var assignment = new[] { CsvField.LastName, CsvField.FirstName, CsvField.Work, CsvField.Ignore, CsvField.Notes };

            var result = new CsvContactMapper(new ImportOptions { VipMode = VipMode.IfNotesContainVip, NameStyle = NameStyle.FirstLast }).Map(rows, assignment);

            Contact contact = Assert.Single(result.Contacts);
            Assert.Equal("John Doe", contact.CombinedName);
            Assert.Equal("0301", contact.Work);
            Assert.True(contact.IsVip);
        }

        [Fact]
        public void FieldNames_MatchEnum()
        {
            Assert.Equal(14, CsvContactMapper.FieldNames.Count);
            Assert.Equal("Notes", CsvContactMapper.FieldNames[(int)CsvField.Notes + 1]);
        }
    }

    public class ConfigFileTests
    {
        [Fact]
        public void ParseAndSerialize()
        {
            ConfigFile config = ConfigFile.Parse("cfg_a\tTrue\nnoTab\ncfg_b\tsome value\n\ncfg_c\tnot a bool\n");

            Assert.True(config.GetBool("cfg_a", false));
            Assert.False(config.GetBool("cfg_c", false));
            Assert.True(config.GetBool("missing", true));
            Assert.Equal("some value", config.GetString("cfg_b"));
            Assert.Null(config.GetString("noTab"));

            config.Set("cfg_d", "multi\nline");
            config.Remove("cfg_c");
            Assert.Equal(new[] { "cfg_a", "cfg_b", "cfg_d" }, config.Keys);
            Assert.Equal("multi line", ConfigFile.Parse(config.ToString()).GetString("cfg_d"));
        }
    }
}
