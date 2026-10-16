using System;
using System.Linq;
using ContactConversionWizard.Core.Model;
using ContactConversionWizard.Core.Text;

namespace ContactConversionWizard.Core.Tests
{
    public class NameFormatterTests
    {
        [Theory]
        [InlineData(NameStyle.LastFirstBracketCompany, "Doe John", "Doe [ACME]", "John [ACME]", "Doe John [ACME]")]
        [InlineData(NameStyle.LastCommaFirstBracketCompany, "Doe, John", "Doe [ACME]", "John [ACME]", "Doe, John [ACME]")]
        [InlineData(NameStyle.LastCommaFirstCommaCompany, "Doe, John", "Doe, ACME", "John, ACME", "Doe, John, ACME")]
        [InlineData(NameStyle.CompanyBracketLastFirst, "Doe John", "ACME [Doe]", "ACME [John]", "ACME [Doe John]")]
        [InlineData(NameStyle.CompanyBracketLastCommaFirst, "Doe, John", "ACME [Doe]", "ACME [John]", "ACME [Doe, John]")]
        [InlineData(NameStyle.CompanyCommaLastCommaFirst, "Doe, John", "ACME, Doe", "ACME, John", "ACME, Doe, John")]
        [InlineData(NameStyle.FirstLastBracketCompany, "John Doe", "Doe [ACME]", "John [ACME]", "John Doe [ACME]")]
        [InlineData(NameStyle.FirstCommaLastBracketCompany, "John, Doe", "Doe [ACME]", "John [ACME]", "John, Doe [ACME]")]
        [InlineData(NameStyle.FirstCommaLastCommaCompany, "John, Doe", "Doe, ACME", "John, ACME", "John, Doe, ACME")]
        [InlineData(NameStyle.CompanyBracketFirstLast, "John Doe", "ACME [Doe]", "ACME [John]", "ACME [John Doe]")]
        [InlineData(NameStyle.CompanyBracketFirstCommaLast, "John, Doe", "ACME [Doe]", "ACME [John]", "ACME [John, Doe]")]
        [InlineData(NameStyle.CompanyCommaFirstCommaLast, "John, Doe", "ACME, Doe", "ACME, John", "ACME, John, Doe")]
        [InlineData(NameStyle.LastFirst, "Doe John", "Doe", "John", "Doe John")]
        [InlineData(NameStyle.LastCommaFirst, "Doe, John", "Doe", "John", "Doe, John")]
        [InlineData(NameStyle.FirstLast, "John Doe", "Doe", "John", "John Doe")]
        [InlineData(NameStyle.FirstCommaLast, "John, Doe", "Doe", "John", "John, Doe")]
        public void GenerateFullName_AllStyles(NameStyle style, string bothNames, string lastAndCompany, string firstAndCompany, string all)
        {
            Assert.Equal(bothNames, NameFormatter.GenerateFullName("John", "Doe", "", style));
            Assert.Equal(lastAndCompany, NameFormatter.GenerateFullName("", "Doe", "ACME", style));
            Assert.Equal(firstAndCompany, NameFormatter.GenerateFullName("John", "", "ACME", style));
            Assert.Equal(all, NameFormatter.GenerateFullName("John", "Doe", "ACME", style));

            // a single part is always used as is
            Assert.Equal("Doe", NameFormatter.GenerateFullName("", "Doe", "", style));
            Assert.Equal("John", NameFormatter.GenerateFullName("John", "", "", style));
            Assert.Equal("ACME", NameFormatter.GenerateFullName("", "", "ACME", style));
            Assert.Equal("", NameFormatter.GenerateFullName("", "", "", style));
        }

        [Fact]
        public void IncludesCompany_OnlyForStylesWithCompany()
        {
            foreach (NameStyle style in Enum.GetValues(typeof(NameStyle)))
            {
                Assert.Equal(style <= NameStyle.CompanyCommaFirstCommaLast, NameFormatter.IncludesCompany(style));
            }
        }

        [Fact]
        public void GenerateFullName_RejectsUnknownStyle()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => NameFormatter.GenerateFullName("a", "b", "c", (NameStyle)99));
        }

        [Theory]
        [InlineData("Short", 10, false, "", "Short")]
        [InlineData("A very long name", 6, false, "", "A very")]
        [InlineData("Doe John [ACME Corporation]", 12, false, "", "Doe John [A]")]
        [InlineData("Doe John", 31, true, " H", "Doe John H")]
        [InlineData("1234567890", 8, true, " W", "123456 W")]
        public void LimitLength(string name, int limit, bool appendSuffix, string suffix, string expected)
        {
            Assert.Equal(expected, NameFormatter.LimitLength(name, limit, appendSuffix, suffix));
        }
    }

    public class NoteFlagsTests
    {
        [Theory]
        [InlineData("SPEEDDIAL(5)", "05")]
        [InlineData("SPEEDDIAL(99)", "99")]
        [InlineData("SPEEDDIAL(100)", "")]
        [InlineData("SPEEDDIAL(MAMA)", "XX,MAMA")]
        [InlineData("SPEEDDIAL(TOOLONGVANITY)", "")]
        [InlineData("x SPEEDDIAL(7,MAMA) y", "07,MAMA")]
        [InlineData("SPEEDDIAL(XX,PAPA)", "XX,PAPA")]
        [InlineData("SPEEDDIAL(7,MAMA,extra)", "07,MAMA")]
        [InlineData("SPEEDDIAL(7", "")]
        [InlineData("", "")]
        [InlineData(null, "")]
        public void ParseSpeedDial(string notes, string expected)
        {
            Assert.Equal(expected, NoteFlags.ParseSpeedDial(notes));
        }

        [Theory]
        [InlineData("PREFER(home)", PhoneType.Home)]
        [InlineData("PREFER(Work)", PhoneType.Work)]
        [InlineData("PREFER(MOBILE)", PhoneType.Mobile)]
        [InlineData("PREFER(fax)", PhoneType.None)]
        [InlineData("prefer(home)", PhoneType.None)]
        [InlineData(null, PhoneType.None)]
        public void ParsePrefer(string notes, PhoneType expected)
        {
            Assert.Equal(expected, NoteFlags.ParsePrefer(notes));
        }

        [Fact]
        public void ParsePrefix()
        {
            Assert.Equal("#31#", NoteFlags.ParsePrefix("Call with PREFIX(#31#) please"));
            Assert.Equal("", NoteFlags.ParsePrefix("no prefix"));
        }

        [Fact]
        public void IsVip_DependsOnMode()
        {
            Assert.False(NoteFlags.IsVip("nick", "VIP", VipMode.Never));
            Assert.True(NoteFlags.IsVip("nick", "", VipMode.IfNickname));
            Assert.False(NoteFlags.IsVip("", "VIP", VipMode.IfNickname));
            Assert.True(NoteFlags.IsVip("", "is a VIP", VipMode.IfNotesContainVip));
            Assert.False(NoteFlags.IsVip("nick", "nothing", VipMode.IfNotesContainVip));
        }

        [Fact]
        public void Apply_KeepsExplicitPreferenceWithoutPreferKeyword()
        {
            var contact = new Contact { Home = "1", Mobile = "2", ExplicitPreferred = PhoneType.Mobile };
            NoteFlags.Apply(contact, "", "SPEEDDIAL(3)", VipMode.Never);
            Assert.Equal(PhoneType.Mobile, contact.Preferred);
            Assert.Equal("03", contact.SpeedDial);

            NoteFlags.Apply(contact, "", "PREFER(home)", VipMode.Never);
            Assert.Equal(PhoneType.Home, contact.Preferred);
        }

        [Fact]
        public void IsIgnored()
        {
            Assert.True(NoteFlags.IsIgnored("please CCW-IGNORE this"));
            Assert.False(NoteFlags.IsIgnored(null));
        }
    }

    public class PhoneNumberCleanerTests
    {
        [Theory]
        [InlineData("+49 (30) 123-456", "030123456")]
        [InlineData("0049 30 123456", "030123456")]
        [InlineData("+1 555 1234", "0015551234")]
        [InlineData("**610", "**610")]
        [InlineData("030/123 45 x6", "030123456")]
        [InlineData("", "")]
        public void Clean_Germany(string input, string expected)
        {
            var cleaner = new PhoneNumberCleaner(new NumberCleanupOptions(), "0049");
            Assert.Equal(expected, cleaner.Clean(input));
        }

        [Fact]
        public void Clean_KeepCountryPrefix()
        {
            var cleaner = new PhoneNumberCleaner(new NumberCleanupOptions(), PhoneNumberCleaner.KeepCountryPrefix);
            Assert.Equal("+4930123", cleaner.Clean("+49 30 123"));
        }

        [Fact]
        public void Clean_KeepsConfiguredCharacters()
        {
            var options = new NumberCleanupOptions
            {
                RemoveBrackets = false,
                RemoveHashKey = false,
                RemoveHyphen = false,
                RemoveSlash = false,
                RemoveSpace = false,
                RemoveSquareBrackets = false,
                RemoveLetters = false,
            };
            var cleaner = new PhoneNumberCleaner(options, PhoneNumberCleaner.KeepCountryPrefix);
            Assert.Equal("(030) 12-34/5 [6] #ab", cleaner.Clean("(030) 12-34/5 [6] #ab"));
        }

        [Fact]
        public void Clean_AddsDialAndZeroPrefix()
        {
            var cleaner = new PhoneNumberCleaner(new NumberCleanupOptions { AddZeroPrefix = true }, "0049");
            Assert.Equal("0#31#030123", cleaner.Clean("+49 30 123", "#31#"));
            Assert.Equal("", cleaner.Clean("abc", "#31#"));
        }

        [Fact]
        public void Clean_LeavesSipAddressesAndWarnsAboutBrackets()
        {
            var cleaner = new PhoneNumberCleaner(new NumberCleanupOptions(), "0049");
            Assert.Equal("100@hd-telefonie.avm.de", cleaner.Clean("100@hd-telefonie.avm.de"));
            Assert.Empty(cleaner.Warnings);
            Assert.Equal("<a@b>", cleaner.Clean("<a@b>"));
            Assert.Single(cleaner.Warnings);
        }
    }

    public class EscapingTests
    {
        [Fact]
        public void Xml() => Assert.Equal("a &amp; b &lt;c&gt; &quot;d&quot; &apos;e&apos;", Escaping.Xml("a & b <c> \"d\" 'e'"));

        [Fact]
        public void VCard() => Assert.Equal(@"a\\b\;c\,d\:e\nf\ng", Escaping.VCard("a\\b;c,d:e\r\nf\ng"));

        [Fact]
        public void VCardGigaset() => Assert.Equal(@"a\\b\;c,d:efg", Escaping.VCardGigaset("a\\b;c,d:e\r\nf\ng"));
    }

    public class CsvParserTests
    {
        [Fact]
        public void Parse_QuotedFieldsSeparatorsAndLineBreaks()
        {
            string csv = "Name,Number\r\n\"Doe, John\",\"0301\"\r\n\r\n\"Multi\nLine\",\"say \"\"hi\"\"\"\n  plain  , value ";
            var rows = CsvParser.Parse(csv, ',');

            Assert.Equal(4, rows.Count);
            Assert.Equal(new[] { "Name", "Number" }, rows[0]);
            Assert.Equal(new[] { "Doe, John", "0301" }, rows[1]);
            Assert.Equal(new[] { "Multi\nLine", "say \"hi\"" }, rows[2]);
            Assert.Equal(new[] { "plain", "value" }, rows[3]);
        }

        [Fact]
        public void Parse_OtherSeparatorsAndEmptyFields()
        {
            var rows = CsvParser.Parse("a;;c\n;\n", ';');
            Assert.Equal(new[] { "a", "", "c" }, rows[0]);
            Assert.Equal(new[] { "", "" }, rows[1]);

            rows = CsvParser.Parse("a\tb", '\t');
            Assert.Equal(new[] { "a", "b" }, rows.Single());
        }

        [Fact]
        public void Parse_UnterminatedQuoteThrows()
        {
            Assert.Throws<FormatException>(() => CsvParser.Parse("\"abc", ','));
        }
    }
}
