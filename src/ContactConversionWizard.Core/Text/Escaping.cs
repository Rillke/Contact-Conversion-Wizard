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
