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
    /// <summary>Issue #1: several numbers of the same type.</summary>
    public class MultipleNumbersTests
    {
        private const string FritzPhonebook =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><phonebooks><phonebook><contact><category>0</category><person><realName>Doe</realName></person><telephony>" +
            "<number type=\"mobile\">0170 1</number>" +
            "<number type=\"home\">030 1</number>" +
            "<number type=\"mobile\" prio=\"1\">0170 2</number>" +
            "<number type=\"mobile\">0170 3</number>" +
            "<number type=\"intern\">**610</number>" +
            "<number type=\"fax_work\">030 9</number>" +
            "</telephony></contact></phonebook></phonebooks>";

        private static Contact ImportFritz(string xml = FritzPhonebook)
            => new FritzXmlImporter(new ImportOptions()).Import(new MemoryStream(Encoding.UTF8.GetBytes(xml))).Contacts.Single();

        private static ExportOptions Options(bool multiple, bool fax = false)
            => new ExportOptions { CountryPrefix = "0049", MultipleNumbersPerType = multiple, IncludeFax = fax };

        [Fact]
        public void FritzImportKeepsAllNumbersWithTheirType()
        {
            Contact contact = ImportFritz();

            Assert.Equal(new[] { "Mobile: 0170 1", "Home: 030 1", "Mobile: 0170 2", "Mobile: 0170 3", "Other: **610", "HomeFax: 030 9" }, contact.Numbers.Select(n => n.ToString()));
            Assert.True(contact.Numbers[2].Preferred);
            Assert.Equal("intern", contact.Numbers[4].SourceType);

            // the single number fields are filled as before
            Assert.Equal("030 1", contact.Home);
            Assert.Equal("0170 2", contact.Work);
            Assert.Equal("0170 1", contact.Mobile);
            Assert.Equal("030 9", contact.HomeFax);
        }

        [Fact]
        public void VCardImportKeepsAllNumbersWithTheirType()
        {
            const string vcard = "BEGIN:VCARD\nN:Doe;J\nTEL;TYPE=CELL:1\nTEL;TYPE=CELL,PREF:2\nTEL;TYPE=CELL:3\nTEL:4\nTEL;TYPE=WORK,FAX:5\nTEL;TYPE=WORK,FAX:6\nEND:VCARD\n";
            Contact contact = new VCardImporter(new ImportOptions()).Import(vcard).Contacts.Single();

            Assert.Equal(new[] { "Mobile: 1", "Mobile: 2", "Mobile: 3", "Other: 4", "WorkFax: 5", "WorkFax: 6" }, contact.Numbers.Select(n => n.ToString()));
            Assert.True(contact.Numbers[1].Preferred);
            Assert.Equal("2", contact.Mobile); // preferred one, as before
        }

        [Fact]
        public void SwitchOffKeepsTheOutputOfEveryFormat()
        {
            Contact contact = ImportFritz();
            Contact withoutList = contact.Clone();
            withoutList.Numbers.Clear();

            foreach (ContactExporter exporter in Exporters.All)
            {
                Assert.Equal(
                    exporter.Export(new[] { withoutList }, Options(false, fax: true)).Content,
                    exporter.Export(new[] { contact }, Options(false, fax: true)).Content);
            }
        }

        [Fact]
        public void SwitchOnDoesNotChangeFormatsWithoutSupport()
        {
            Contact contact = ImportFritz();
            foreach (ContactExporter exporter in Exporters.All.Where(e => !(e is FritzXmlExporter) && !(e is VCardExporter)))
            {
                Assert.Equal(
                    exporter.Export(new[] { contact }, Options(false)).Content,
                    exporter.Export(new[] { contact }, Options(true)).Content);
            }
        }

        [Fact]
        public void SwitchOnDoesNotChangeContactsWithOneNumberPerType()
        {
            var contacts = new List<Contact>
            {
                new Contact { CombinedName = "A", Home = "1", Work = "2", Mobile = "3", HomeFax = "4", WorkFax = "5", SpeedDial = "07" },
                new Contact { CombinedName = "B", Work = "6", Mobile = "7", ExplicitPreferred = PhoneType.Mobile, SpeedDial = "XX,B" },
            };
            // the same contacts as read from a file, i.e. with the list of numbers
            const string vcard = "BEGIN:VCARD\nN:C;\nTEL;TYPE=HOME:8\nTEL;TYPE=CELL,PREF:9\nEND:VCARD\n";
            contacts.Add(new VCardImporter(new ImportOptions()).Import(vcard).Contacts.Single());

            foreach (ContactExporter exporter in Exporters.All)
            {
                foreach (TypePreference preference in new[] { TypePreference.Home, TypePreference.Work })
                {
                    ExportOptions off = Options(false, fax: true);
                    ExportOptions on = Options(true, fax: true);
                    off.TypePreference = on.TypePreference = preference;
                    Assert.Equal(exporter.Export(contacts, off).Content, exporter.Export(contacts, on).Content);
                }
            }
        }

        [Fact]
        public void FritzExportWritesAllNumbers()
        {
            string content = new FritzXmlExporter().Export(new[] { ImportFritz() }, Options(true, fax: true)).Content;

            Assert.Contains(
                "<number type=\"home\">0301</number>\n" +
                "<number type=\"work\" />\n" +
                "<number type=\"mobile\">01701</number>\n" +
                "<number type=\"mobile\" prio=\"1\">01702</number>\n" +
                "<number type=\"mobile\">01703</number>\n" +
                "<number type=\"intern\">**610</number>\n" +
                "<number type=\"fax_work\">0309</number>\n",
                content);

            // and can be read back without losing a number
            Contact reimported = ImportFritz(content);
            Assert.Equal(6, reimported.Numbers.Count);
            Assert.Equal("01702", reimported.Numbers.Single(n => n.Preferred).Number);
        }

        [Fact]
        public void VCardExportWritesAllNumbers()
        {
            string content = new VCardExporter().Export(new[] { ImportFritz() }, Options(true)).Content;

            Assert.Contains(
                "TEL;type=CELL:01701\r\n" +
                "TEL;type=HOME:0301\r\n" +
                "TEL;type=CELL;type=pref:01702\r\n" +
                "TEL;type=CELL:01703\r\n" +
                "TEL;type=VOICE:**610\r\n" +
                "TEL;type=HOME;type=FAX:0309\r\n",
                content);

            Contact reimported = new VCardImporter(new ImportOptions()).Import(content).Contacts.Single();
            Assert.Equal(new[] { "Mobile: 01701", "Home: 0301", "Mobile: 01702", "Mobile: 01703", "Other: **610", "HomeFax: 0309" }, reimported.Numbers.Select(n => n.ToString()));
        }

        [Fact]
        public void PreferKeywordInNotesWins()
        {
            Contact contact = ImportFritz();
            contact.ExplicitPreferred = PhoneType.Home;
            string content = new FritzXmlExporter().Export(new[] { contact }, Options(true)).Content;
            Assert.Contains("<number type=\"home\" prio=\"1\">0301</number>", content);
            Assert.DoesNotContain("<number type=\"mobile\" prio", content);
        }

        [Fact]
        public void CloneCopiesTheList()
        {
            Contact contact = ImportFritz();
            Contact clone = contact.Clone();
            clone.Numbers.Clear();
            Assert.Equal(6, contact.Numbers.Count);
        }
    }
}
