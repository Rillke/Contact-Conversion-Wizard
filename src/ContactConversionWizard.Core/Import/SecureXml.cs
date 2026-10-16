using System.IO;
using System.Xml;

namespace ContactConversionWizard.Core.Import
{
    /// <summary>Creates XML readers that neither process DTDs nor resolve external resources.</summary>
    internal static class SecureXml
    {
        public static XmlReader CreateReader(Stream input, ConformanceLevel conformance = ConformanceLevel.Document)
        {
            var settings = new XmlReaderSettings
            {
                ConformanceLevel = conformance,
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                IgnoreWhitespace = true,
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
            };
            return XmlReader.Create(input, settings);
        }
    }
}
