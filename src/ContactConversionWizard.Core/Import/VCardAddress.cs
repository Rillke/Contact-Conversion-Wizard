namespace ContactConversionWizard.Core.Import
{
    /// <summary>Street, ZIP code and city extracted from the value of a vCard <c>ADR</c> property.</summary>
    public sealed class VCardAddress
    {
        private const string EscapedNewLine = "\\n";

        public string Street { get; private set; } = "";
        public string Zip { get; private set; } = "";
        public string City { get; private set; } = "";

        /// <summary>
        /// Parses <c>post office box;extended address;street;locality;region;postal code;country</c>.
        /// If only the extended address is filled (as done by Google), it is split into street and "ZIP city".
        /// </summary>
        public static VCardAddress Parse(string value)
        {
            string[] parts = (value ?? "").Split(';');
            string Part(int index) => parts.Length > index ? parts[index] : "";

            string extended = Part(1);
            string street = Part(2);
            string locality = Part(3);
            string postalCode = Part(5);

            var result = new VCardAddress();
            if (!(street == "" && postalCode == "" && locality == "" && extended != ""))
            {
                result.Street = street;
                result.Zip = postalCode;
                result.City = locality;
                return result;
            }

            while (extended.StartsWith(EscapedNewLine)) { extended = extended.Substring(EscapedNewLine.Length); }
            while (extended.EndsWith(EscapedNewLine)) { extended = extended.Substring(0, extended.Length - EscapedNewLine.Length); }
            while (extended.Contains(EscapedNewLine + EscapedNewLine)) { extended = extended.Replace(EscapedNewLine + EscapedNewLine, EscapedNewLine); }

            int lineBreak = extended.IndexOf(EscapedNewLine);
            if (lineBreak < 0)
            {
                result.Street = extended;
                return result;
            }

            result.Street = extended.Substring(0, lineBreak);
            string remainder = extended.Substring(lineBreak + EscapedNewLine.Length);
            int space = remainder.IndexOf(' ');
            if (space >= 0)
            {
                result.Zip = remainder.Substring(0, space);
                remainder = remainder.Substring(space + 1);
            }
            result.City = remainder.Replace(EscapedNewLine, " - ");
            return result;
        }
    }
}
