using System;
using System.Text;

namespace ContactConversionWizard.Core.Text
{
    /// <summary>Encodings used by the supported file formats.</summary>
    public static class TextEncodings
    {
        static TextEncodings()
        {
            // makes the legacy code pages (Windows-1252, Mac OS Roman, ...) available on .NET (Core)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

        public static readonly Encoding Utf8WithBom = new UTF8Encoding(true);

        public static readonly Encoding Latin1 = Encoding.GetEncoding("ISO-8859-1");

        public static Encoding Windows1252 => Encoding.GetEncoding(1252);

        /// <summary>
        /// Returns the encoding for a (vCard) charset name such as <c>UTF-8</c>, <c>ISO-8859-1</c> or <c>MACINTOSH</c>,
        /// or <c>null</c> if the name is unknown.
        /// </summary>
        public static Encoding GetEncoding(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) { return null; }
            string trimmed = name.Trim();
            if (trimmed.Equals("MACROMAN", StringComparison.OrdinalIgnoreCase) || trimmed.Equals("MAC", StringComparison.OrdinalIgnoreCase))
            {
                trimmed = "macintosh";
            }
            if (trimmed.Equals("UTF-8", StringComparison.OrdinalIgnoreCase) || trimmed.Equals("UTF8", StringComparison.OrdinalIgnoreCase))
            {
                return Encoding.UTF8;
            }

            try
            {
                return Encoding.GetEncoding(trimmed);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}
