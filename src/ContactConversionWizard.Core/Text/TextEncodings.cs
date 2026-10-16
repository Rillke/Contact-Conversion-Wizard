using System.Text;

namespace ContactConversionWizard.Core.Text
{
    /// <summary>Encodings used by the supported file formats.</summary>
    public static class TextEncodings
    {
        public static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

        public static readonly Encoding Utf8WithBom = new UTF8Encoding(true);

        public static readonly Encoding Latin1 = Encoding.GetEncoding("ISO-8859-1");
    }
}
