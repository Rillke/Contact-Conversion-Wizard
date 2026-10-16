# Contact-Conversion-Wizard [![CI](https://github.com/Rillke/Contact-Conversion-Wizard/actions/workflows/ci.yml/badge.svg?branch=master)](https://github.com/Rillke/Contact-Conversion-Wizard/actions/workflows/ci.yml) ![Downloads](https://img.shields.io/github/downloads/Rillke/Contact-Conversion-Wizard/total.svg?style=flat) [![Latest Release](https://img.shields.io/github/v/release/Rillke/Contact-Conversion-Wizard.svg?logo=github)](https://github.com/Rillke/Contact-Conversion-Wizard/releases)

Programm, welches auf einfache Weise Kontaktlisten zwischen verschiedenen Programmen/Geräten konvertiert.

Dieses Programm sucht **Mitwirkende**. Auch gelegentliche Patches (Tests, CI, Weiterentwicklung) und Dokumentation sind willkommen.

## Einsatzzweck

Ursprünglich entstand das Programm, da dem Autor die existierenden Methoden das Outlook Telefonbuch in die Fritz!Box zu übernehmen zu umständlich waren.

Aus dem dem hierfür geschriebenen Programm (ursprünglich ein reines Outlook-zu-Fritz!Box Konvertierprogramm) ist im Laufe der Zeit ein universelles Kontaktlisten-Umwandlungs-Tool geworden, das Kontakte aus einer ganzen Reihe von Quellen importieren kann und eine vielzahl [Formate](/docs/FORMATE.md) exportieren kann.

## Quellcode

Das Programm ist in C# geschrieben (Lizenz GPLv3). Binaries (`.exe`) zum Herunterladen gibt es in den [Releases](https://github.com/Rillke/Contact-Conversion-Wizard/releases). Wie bei C#/.NET Programmen üblich, ist es auch unter [Linux](/docs/LINUX.md) lauffähig, sofern man eine aktuelle Mono Runtime installiert hat (natürlich ohne Outlook Support).

### Projektstruktur und Bauen

| Verzeichnis | Inhalt |
|---|---|
| `src/ContactConversionWizard.Core` | Bibliothek (.NET Standard 2.0) mit dem Kontaktmodell, Rufnummernbereinigung sowie allen Datei-Importen und -Exporten. Ohne Abhängigkeit zur Oberfläche und damit testbar. |
| `src/ContactConversionWizard` | Windows-Forms-Anwendung (.NET Framework 4.8) inkl. Outlook-Anbindung. |
| `tests/ContactConversionWizard.Core.Tests` | xUnit-Tests der Bibliothek. |

Voraussetzung ist das [.NET SDK 10](https://dotnet.microsoft.com/download).

```sh
dotnet test                                   # Tests ausführen (Windows, Linux, macOS)
msbuild src/ContactConversionWizard -restore -p:Configuration=Release   # Windows: einzelne .exe erzeugen
```

Release-Builds unter Windows fassen alle Bibliotheken mit [ILRepack](https://github.com/gluck/il-repack) zu einer einzigen `.exe` zusammen.
Jeder Pull Request wird per [GitHub Actions](.github/workflows/ci.yml) gebaut und getestet. Beim Veröffentlichen eines Releases wird die `.exe` automatisch gebaut und angehängt.

## Screenshot

Windows 8.1 Screenshot, das den Contact Conversion Wizard v3.5.0.2 zeigt: Ein GUI Fenster mit 3 Bereichen: Links: Daten laden; Mitte: Daten ansehen; Rechts: Daten abspeichern

<img src="img/Contact Conversion Wizard v3.5.0.2 - Windows.png" alt="GUI" width="625"/>

## Abhängigkeiten

Windows: [.NET Framework 4.8](https://dotnet.microsoft.com/download/dotnet-framework/net48) (in Windows 10 und 11 enthalten)
Andere: Siehe [Linux](/docs/LINUX.md)

## Autor, Contributing, Mitmachen

Gerald Hopf entwickelte und stellte das Tool bis Mitte 2016 auf seiner Website zur Verfügung. Im [ip-phone-forum](https://www.ip-phone-forum.de/threads/contact-conversion-wizard.209976/page-44#post-2209489) wurde über einen Fork nachgedacht. Here it is.

## Weitere Tools

| Tool                                                                             | Programmiersprache / Plattform                                          | Web-Version                                  | Import                                                         | Export                                                                                                                             | Lizenz |
|----------------------------------------------------------------------------------|-------------------------------------------------------------------------|----------------------------------------------|----------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------------|--------|
| [Contact-Conversion-Wizard](https://github.com/Rillke/Contact-Conversion-Wizard) | C# bevorzugt Windows; MacOS und Linux ggf. möglich                      | -                                            | Outlook, Fritz!Box XML, vCard, Fritz!Adr, CSV, Google Contacts | Outlook, Fritz!Box XML, vCard, Fritz!Adr, Snom v7, Snom v8, Gigaset Talk&Surf CSV, Asstra, Grandstream, Auerswald, Google Contacts | GPLv3  |
| [fritzXML2vcard](https://github.com/Rillke/fritzXML2vcard)                       | JavaScript - Node.js Nahezu alle Betriebssysteme                        | [verfügbar](https://blog.rillke.com/fritzXML2vcard/) | Fritz!Box XML                                                  | vCard(s)                                                                                                                           | MIT    |
| [vcard2fritzXML](https://github.com/berkholz/vcard2fritzXML)                     | Java - Nahezu alle Betriebssysteme                                        | -                                            | vCard                                                          | Fritz!Box XML                                                                                                                      | GPLv2  |
