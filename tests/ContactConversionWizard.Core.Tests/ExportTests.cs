using System.Collections.Generic;
using System.Linq;
using ContactConversionWizard.Core.Export;
using ContactConversionWizard.Core.Import;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace ContactConversionWizard.Core.Tests
{
    public class ExportTests
    {
        private static List<Contact> SampleContacts() => new List<Contact>
        {
            new Contact
            {
                FirstName = "John", LastName = "Doe", Company = "A&B", CombinedName = "Doe John [A&B]",
                Home = "+49 30 1234", Work = "040-5678", Mobile = "0170 999", WorkFax = "040 1",
                Street = "Main St. 1", Zip = "10115", City = "Berlin", Email = "john@example.com",
                IsVip = true, SpeedDial = "05", FritzPrefix = "#31#",
            },
            new Contact { FirstName = "Jane", LastName = "Roe", CombinedName = "Roe Jane", Mobile = "0171 1", ExplicitPreferred = PhoneType.Mobile, SpeedDial = "XX,JANE" },
            new Contact { LastName = "OnlyFax", CombinedName = "OnlyFax", HomeFax = "030 2" },
        };

        private static ExportOptions Options() => new ExportOptions { CountryPrefix = "0049" };

        private static ExportResult Run(ContactExporter exporter, ExportOptions options = null)
            => exporter.Export(SampleContacts(), options ?? Options());

        [Fact]
        public void FritzXml()
        {
            var options = Options();
            options.IncludeFax = true;
            options.ExportPictures = true;
            options.PicturePath = "file:///var/media/";
            options.PictureFileNameGenerator = () => "pic.jpg";
            var contacts = SampleContacts();
            contacts[1].Photo = new byte[] { 1 };

            ExportResult result = new FritzXmlExporter().Export(contacts, options);

            Assert.Equal(2, result.EntryCount);
            Assert.Equal("iso-8859-1", result.Encoding.WebName);
            Assert.Equal(
                "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>\n<phonebooks>\n<phonebook>" +
                "<contact>\n<category>1</category>\n<person>\n<realName>Doe John [A&amp;B]</realName>\n</person>\n<telephony>\n" +
                "<number type=\"home\" prio=\"1\" quickdial=\"05\">#31#0301234</number>\n" +
                "<number type=\"work\">#31#0405678</number>\n" +
                "<number type=\"mobile\">#31#0170999</number>\n" +
                "<number type=\"fax_work\">#31#0401</number>\n" +
                "</telephony>\n<services>\n<email classifier=\"private\">john@example.com</email><email />\n</services>\n<setup>\n<ringTone />\n<ringVolume />\n</setup>\n</contact>" +
                "<contact>\n<category>0</category>\n<person>\n<realName>Roe Jane</realName>\n<imageURL>file:///var/media/pic.jpg</imageURL>\n</person>\n<telephony>\n" +
                "<number type=\"home\" />\n<number type=\"work\" />\n" +
                "<number type=\"mobile\" prio=\"1\" quickdial=\"97\" vanity=\"JANE\">01711</number>\n" +
                "</telephony>\n<services>\n<email />\n</services>\n<setup>\n<ringTone />\n<ringVolume />\n</setup>\n</contact>" +
                "</phonebook>\n</phonebooks>",
                result.Content);
            Assert.Equal("pic.jpg", Assert.Single(result.AdditionalFiles).FileName);
        }

        [Fact]
        public void FritzXml_WorkFirstWhenPreferred()
        {
            var options = Options();
            options.TypePreference = TypePreference.Work;
            string content = Run(new FritzXmlExporter(), options).Content;
            Assert.True(content.IndexOf("type=\"work\"") < content.IndexOf("type=\"home\""));
        }

        [Fact]
        public void VCard_RoundTrip()
        {
            ExportResult result = Run(new VCardExporter());
            Assert.Equal(3, result.EntryCount);
            Assert.Contains("N:Doe;John;;;\r\n", result.Content);
            Assert.Contains("TEL;type=HOME;type=pref:0301234\r\n", result.Content);
            Assert.Contains("TEL;type=CELL;type=pref:01711\r\n", result.Content);
            Assert.Contains("ADR;type=HOME;type=pref:;;Main St. 1;Berlin;;10115;\r\n", result.Content);
            Assert.Contains("NOTE:VIP SPEEDDIAL(05)\r\n", result.Content);

            var imported = new VCardImporter(new ImportOptions { VipMode = VipMode.IfNotesContainVip }).Import(result.Content).Contacts;
            Assert.Equal(3, imported.Count);
            Contact john = imported[0];
            Assert.Equal("Doe John [A&B]", john.CombinedName);
            Assert.Equal("0301234", john.Home);
            Assert.Equal("0405678", john.Work);
            Assert.Equal("0170999", john.Mobile);
            Assert.Equal("0401", john.WorkFax);
            Assert.Equal("Berlin", john.City);
            Assert.True(john.IsVip);
            Assert.Equal("05", john.SpeedDial);
            Assert.Equal("0302", imported[2].HomeFax);
        }

        [Fact]
        public void VCard_OnlyWithPhoneNumber()
        {
            var options = Options();
            options.OnlyWithPhoneNumber = true;
            Assert.Equal(2, Run(new VCardExporter(), options).EntryCount);
        }

        [Fact]
        public void GigasetVCard()
        {
            string content = Run(new GigasetVCardExporter()).Content;
            Assert.Equal(
                "BEGIN:VCARD\r\nVERSION:2.1\r\nN:Doe John [A&B];\r\nTEL;HOME:0301234\r\nTEL;WORK:0405678\r\nTEL;CELL:0170999\r\nEMAIL:john@example.com\r\nEND:VCARD\r\n" +
                "BEGIN:VCARD\r\nVERSION:2.1\r\nN:Roe;Jane\r\nTEL;CELL:01711\r\nEND:VCARD\r\n",
                content);
        }

        [Fact]
        public void FritzAdr()
        {
            ExportResult result = Run(new FritzAdrExporter());
            string[] lines = result.Content.Split("\r\n");
            Assert.Equal(FritzAdrExporter.Header, lines[0] + "\r\n");
            Assert.Equal(4, result.EntryCount);
            Assert.StartsWith("Doe John [A&B] H\tA&B\tDoe\tJohn\t\tMain St. 1\t10115\tBerlin\t\t0301234\t\t", lines[1]);
            Assert.Contains("\tVIP SPEEDDIAL(05)\t0170999\tjohn@example.com\t", lines[1]);
            Assert.StartsWith("Doe John [A&B] W\t", lines[2]);
            Assert.Contains("\t0405678\t0401\t", lines[2]);
            Assert.StartsWith("OnlyFax\t", lines[4]);

            var options = Options();
            options.OnlyWithFaxNumber = true;
            Assert.Equal(3, Run(new FritzAdrExporter(), options).EntryCount);
        }

        [Fact]
        public void SnomV7()
        {
            Assert.Equal(
                "\"Name\",\"Number\"\r\n\"Doe John [A&B] H\",\"0301234\"\r\n\"Doe John [A&B] W\",\"0405678\"\r\n\"Doe John [A&B] M\",\"0170999\"\r\n\"Roe Jane\",\"01711\"\r\n",
                Run(new SnomV7CsvExporter()).Content);
        }

        [Fact]
        public void SnomV8()
        {
            Assert.Equal(
                "\"Doe John [A&B]\",\"00100001\",\"MASTER\",\"\",\"John\",\"Doe\",\"\",\"\",\"john@example.com\",\"\",\"\",\"\",\"false\",\"\",\"home\",\"\"\r\n" +
                "\"\",\"0301234\",\"vip\",\"\",\"Member_Alias\",\"00100001\",\"\",\"\",\"\",\"\",\"\",\"\",\"false\",\"\",\"home\",\"\"\r\n" +
                "\"\",\"0405678\",\"vip\",\"\",\"Member_Alias\",\"00100001\",\"\",\"\",\"\",\"\",\"\",\"\",\"false\",\"\",\"work\",\"\"\r\n" +
                "\"\",\"0170999\",\"vip\",\"\",\"Member_Alias\",\"00100001\",\"\",\"\",\"\",\"\",\"\",\"\",\"false\",\"\",\"mobile\",\"\"\r\n" +
                "\"Roe Jane\",\"01711\",\"\",\"\",\"Jane\",\"Roe\",\"\",\"\",\"\",\"\",\"\",\"\",\"false\",\"\",\"mobile\",\"\"\r\n",
                Run(new SnomV8CsvExporter()).Content);
        }

        [Fact]
        public void TalkSurf()
        {
            var contacts = new List<Contact> { new Contact { CombinedName = "Doe, John Maximilian", Home = "1" } };
            Assert.Equal("Name,Number\r\nDoe John Maximil,1\r\n", new TalkSurfCsvExporter().Export(contacts, Options()).Content);
        }

        [Fact]
        public void Aastra()
        {
            string content = Run(new AastraCsvExporter()).Content;
            Assert.StartsWith("Doe John [A&B] H,0301234,1,Home,public\nDoe John [A&B] W,0405678,1,Work,public\n", content);
            Assert.EndsWith("Roe Jane,01711,1,Mobile,public\n", content);
        }

        [Fact]
        public void Auerswald()
        {
            Assert.Equal(
                "Kurzwahl;Rufnummer;Name\n05;0301234;Doe John [A&B] H\n;0405678;Doe John [A&B] W\n;0170999;Doe John [A&B] M\n97;01711;Roe Jane\n",
                Run(new AuerswaldCsvExporter()).Content);
        }

        [Fact]
        public void GrandstreamGxv()
        {
            string content = Run(new GrandstreamGxvExporter()).Content;
            Assert.StartsWith("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<AddressBook>\n    <version>1</version>\n    <Contact>\n        <FirstName></FirstName>\n        <LastName>Doe John [A&amp;B]</LastName>\n", content);
            Assert.Contains("        <FirstName>Jane</FirstName>\n        <LastName>Roe</LastName>\n", content);
            Assert.Contains("<Phone type=\"Mobile\">\n            <phonenumber>01711</phonenumber>", content);
            Assert.EndsWith("</AddressBook>", content);
        }

        [Fact]
        public void GrandstreamGxp()
        {
            ExportResult result = Run(new GrandstreamGxpExporter());
            Assert.Equal(4, result.EntryCount);
            Assert.StartsWith("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<AddressBook>\n    <Contact>\n        <LastName>Doe John [A&amp;B] H</LastName>\n", result.Content);
            Assert.EndsWith("</AddressBook>\n", result.Content);
        }

        [Fact]
        public void Panasonic()
        {
            Assert.Equal(
                "; Panasonic Communications Co., Ltd. Phonebook DECT Version 2.00\r\n\"Doe John [A&B]\",1,0301234,0405678,0170999\n\"Roe Jane\",1,01711\n",
                Run(new PanasonicCsvExporter()).Content);
        }

        [Fact]
        public void DuplicateEntriesAreReported()
        {
            var contacts = new List<Contact>
            {
                new Contact { CombinedName = "A very long name that is cut off 1", Home = "1" },
                new Contact { CombinedName = "A very long name that is cut off 2", Home = "2" },
            };
            ExportResult result = new SnomV7CsvExporter().Export(contacts, Options());
            Assert.Equal(1, result.EntryCount);
            Assert.Single(result.Warnings);
        }

        [Fact]
        public void PrefixAppliedToAllFormatsWhenConfigured()
        {
            var options = Options();
            options.ApplyPrefixToAllFormats = true;
            Assert.Contains("#31#0301234", Run(new SnomV7CsvExporter(), options).Content);
            Assert.DoesNotContain("#31#", Run(new SnomV7CsvExporter()).Content);
        }

        [Fact]
        public void AllExportersHandleEmptyInput()
        {
            foreach (ContactExporter exporter in Exporters.All)
            {
                ExportResult result = exporter.Export(Enumerable.Empty<Contact>(), Options());
                Assert.Equal(0, result.EntryCount);
                Assert.NotNull(result.Content);
                Assert.NotEmpty(exporter.FileFilter);
            }
        }
    }
}
