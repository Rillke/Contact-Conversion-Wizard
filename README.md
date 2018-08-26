# Contact-Conversion-Wizard

Programm um auf einfache Weise Kontaktlisten zwischen verschiedenen Programmen/Geräten zu konvertieren

## Einsatzzweck

Ursprünglich entstand das Programm da dem Autor die existierenden Methoden das Outlook Telefonbuch in die Fritz!Box zu übernehmen zu umständlich waren.

Aus dem dem hierfür geschriebenen Programm (ursprünglich ein reines Outlook-zu-Fritz!Box Konvertierprogramm) im Laufe der Zeit ein universelles Kontaktlisten-Umwandlungs-Tool geworden das Kontakte aus einer ganzen Reihe von Quellen importieren kann und eine vielzahl [interessanter Formate](/docs/FORMATE.md) wieder exportieren kann.

## Quellcode

Das Programm ist in C# geschrieben (Lizenz GPLv3). Binaries (`.exe`) zum Herunterladen gibt es in den [Releases](https://github.com/Rillke/Contact-Conversion-Wizard/releases). Wie bei C#/.NET Programmen üblich ist es auch unter [Linux](/docs/LINUX.md) lauffähig, sofern man eine aktuelle Mono Runtime installiert hat (natürlich ohne Outlook Support).

## Screenshot

![Windows 7 Screenshot, das den Contact Conversion Wizard v3.0.0.4 zeigt: Ein GUI Fenster mit 3 Bereichen: Links: Daten laden; Mitte: Daten ansehen; Rechts: Daten abspeichern](img/Contact%20Conversion%20Wizard%20v3.0.0.4%20-%20Windows.jpg)

## Abhängigkeiten

Windows: [.NET Runtime v4](https://www.microsoft.com/de-de/download/details.aspx?id=17718)
Andere: Siehe [Linux](/docs/LINUX.md)

## Weitere Tool

| Tool                                                                             | Programmiersprache / Plattform                                          | Web-Version                                  | Import                                                         | Export                                                                                                                             | Lizenz |
|----------------------------------------------------------------------------------|-------------------------------------------------------------------------|----------------------------------------------|----------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------|--------|
| [Contact-Conversion-Wizard](https://github.com/Rillke/Contact-Conversion-Wizard) | C# bevorzugt Windows; MacOS und Linux ggf. möglich                      | -                                            | Outlook, Fritz!Box XML, vCard, Fritz!Adr, CSV, Google Contacts | Outlook, Fritz!Box XML, vCard, Fritz!Adr, Snom v7, Snom v8, Gigaset Talk&Surf CSV, Asstra, Grandstream, Auerswald, Google Contacts | GPLv3  |
| [fritzXML2vcard](https://github.com/Rillke/fritzXML2vcard)                       | JavaScript - Node.js Nahezu alle Betriebssysteme                        | [verfügbar](https://blog.rillke.com/fritzXML2vcard/) | Fritz!Box XML                                                  | vCard(s)                                                                                                                           | MIT    |
| [vcard2fritzXML](https://github.com/berkholz/vcard2fritzXML)                     | Java Nahezu alle Betriebssysteme                                        | -                                            | vCard                                                          | Fritz!Box XML                                                                                                                      | GPLv2  |
