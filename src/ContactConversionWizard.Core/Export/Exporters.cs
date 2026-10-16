using System.Collections.Generic;

namespace ContactConversionWizard.Core.Export
{
    /// <summary>All file based export formats.</summary>
    public static class Exporters
    {
        public static IReadOnlyList<ContactExporter> All { get; } = new ContactExporter[]
        {
            new FritzXmlExporter(),
            new VCardExporter(),
            new GigasetVCardExporter(),
            new FritzAdrExporter(),
            new SnomV7CsvExporter(),
            new SnomV8CsvExporter(),
            new TalkSurfCsvExporter(),
            new AastraCsvExporter(),
            new GrandstreamGxvExporter(),
            new GrandstreamGxpExporter(),
            new AuerswaldCsvExporter(),
            new PanasonicCsvExporter(),
        };
    }
}
