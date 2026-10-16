namespace ContactConversionWizard.Core.Text
{
    /// <summary>Escaping rules of the supported output formats.</summary>
    public static class Escaping
    {
        public static string Xml(string value)
        {
            return (value ?? "")
                .Replace("&", "&amp;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }

        public static string VCard(string value)
        {
            return (value ?? "")
                .Replace("\\", "\\\\")
                .Replace(";", "\\;")
                .Replace(",", "\\,")
                .Replace(":", "\\:")
                .Replace("\r\n", "\\n")
                .Replace("\n", "\\n");
        }

        /// <summary>Encloses a CSV field in double quotes, doubling quotes inside the value.</summary>
        public static string CsvQuoted(string value)
        {
            return "\"" + SingleLine(value).Replace("\"", "\"\"") + "\"";
        }

        /// <summary>Replaces tabs and line breaks by spaces, for formats that cannot contain them.</summary>
        public static string SingleLine(string value)
        {
            return (value ?? "").Replace("\r\n", " ").Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ');
        }

        /// <summary>Gigaset phones (e.g. DX600A) must not get escaped commas/colons and do not support line breaks.</summary>
        public static string VCardGigaset(string value)
        {
            return (value ?? "")
                .Replace("\\", "\\\\")
                .Replace(";", "\\;")
                .Replace("\r\n", "")
                .Replace("\n", "");
        }
    }
}
