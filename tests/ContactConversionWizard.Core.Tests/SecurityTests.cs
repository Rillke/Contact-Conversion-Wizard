using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ContactConversionWizard.Core.Export;
using ContactConversionWizard.Core.Import;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;
using ContactConversionWizard.Core.Updates;

namespace ContactConversionWizard.Core.Tests
{
    /// <summary>Contact data must never be able to change the structure of an exported file.</summary>
    public class OutputInjectionTests
    {
        private static readonly ExportOptions Options = new ExportOptions { CountryPrefix = "0049", ExportPictures = true, PicturePath = "file:///a&b/" };

        private static List<Contact> Malicious() => new List<Contact>
        {
            new Contact
            {
                CombinedName = "Evil \"Name\"\r\n<x>", FirstName = "F\"", LastName = "L\t", Company = "C",
                Home = "</number><number type=\"home\">123@sip.example", Mobile = "123@a\r\nb", Email = "a&b@example.com",
                Photo = new byte[] { 1 }, SpeedDial = "XX,A\"B",
            },
        };

        [Theory]
        [InlineData(typeof(FritzXmlExporter))]
        [InlineData(typeof(GrandstreamGxvExporter))]
        [InlineData(typeof(GrandstreamGxpExporter))]
        public void XmlExportsStayWellFormed(Type exporterType)
        {
            var exporter = (ContactExporter)Activator.CreateInstance(exporterType);
            string content = exporter.Export(Malicious(), Options).Content;

            XDocument document = XDocument.Parse(content);
            Assert.DoesNotContain(document.Descendants(), e => e.Name.LocalName == "x");
            Assert.Contains(document.Descendants(), e => e.Value.Contains("</number><number type=\"home\">123@sip.example"));
        }

        [Fact]
        public void FritzXmlImportReadsBackTheSameNumber()
        {
            string content = new FritzXmlExporter().Export(Malicious(), Options).Content;
            var imported = new FritzXmlImporter(new ImportOptions()).Import(new MemoryStream(TextEncodings.Latin1.GetBytes(content)));
            Assert.Equal("</number><number type=\"home\">123@sip.example", imported.Contacts.Single().Home);
        }

        [Theory]
        [InlineData(typeof(SnomV7CsvExporter), ',')]
        [InlineData(typeof(SnomV8CsvExporter), ',')]
        [InlineData(typeof(PanasonicCsvExporter), ',')]
        [InlineData(typeof(AastraCsvExporter), ',')]
        [InlineData(typeof(TalkSurfCsvExporter), ',')]
        [InlineData(typeof(AuerswaldCsvExporter), ';')]
        public void CsvExportsKeepTheirColumns(Type exporterType, char separator)
        {
            var exporter = (ContactExporter)Activator.CreateInstance(exporterType);
            string content = exporter.Export(Malicious(), Options).Content;
            if (exporter is PanasonicCsvExporter) { content = content.Substring(content.IndexOf('\n') + 1); }

            List<string[]> rows = CsvParser.Parse(content, separator);
            Assert.Single(rows.Select(r => r.Length).Distinct());
            Assert.All(rows, row => Assert.DoesNotContain(row, field => field.Contains('\n') || field.Contains('\r')));
        }

        [Fact]
        public void FritzAdrKeepsItsColumns()
        {
            string content = new FritzAdrExporter().Export(Malicious(), Options).Content;
            string[] lines = content.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            Assert.All(lines, line => Assert.Equal(FritzAdrImporter.ColumnCount, line.Split('\t').Length));
        }

        [Fact]
        public void VCardKeepsOneLinePerProperty()
        {
            string content = new VCardExporter().Export(Malicious(), Options).Content;
            var contact = new VCardImporter(new ImportOptions()).Import(content).Contacts.Single();
            Assert.Equal("123@ab", contact.Mobile);
        }

        [Fact]
        public void NumberCleanerRemovesControlCharactersFromAddresses()
        {
            var cleaner = new PhoneNumberCleaner(new NumberCleanupOptions(), "0049");
            Assert.Equal("100@sip.example", cleaner.Clean("100@sip.\r\nexample\t"));
        }

        [Fact]
        public void CsvQuoted()
        {
            Assert.Equal("\"a \"\"b\"\" c\"", Escaping.CsvQuoted("a \"b\"\nc"));
        }
    }

    public class ReleaseVersionsTests
    {
        [Theory]
        [InlineData("{\"url\":\"x\",\"tag_name\":\"3.6.0.0\",\"name\":\"x\"}", "3.6.0.0")]
        [InlineData("{\"tag_name\" : \"v3.6\"}", "3.6.0.0")]
        [InlineData("{\"tag_name\":\"release-3.5.1-beta\"}", "3.5.1.0")]
        public void ParseLatestRelease(string json, string expected)
        {
            Assert.Equal(Version.Parse(expected), ReleaseVersions.ParseLatestRelease(json));
        }

        [Theory]
        [InlineData("{}")]
        [InlineData("{\"tag_name\":\"latest\"}")]
        [InlineData("<html>")]
        public void ParseLatestRelease_RejectsInvalidDocuments(string json)
        {
            Assert.Throws<FormatException>(() => ReleaseVersions.ParseLatestRelease(json));
        }

        [Theory]
        [InlineData("3.10.0.0", "3.5.0.2", true)]
        [InlineData("3.5.0.2", "3.5.0.2", false)]
        [InlineData("3.5", "3.5.0.0", false)]
        [InlineData("3.5.0.1", "3.5.0.2", false)]
        [InlineData("3.6.0.0", "not a version", false)]
        public void IsNewer(string candidate, string current, bool expected)
        {
            Assert.Equal(expected, ReleaseVersions.IsNewer(Version.Parse(candidate), current));
        }
    }
}
