# Contact Conversion Wizard - Export/Import Formate

## Unterstützte Import-Formate

* Outlook: Versionen 2003, 2007, 2010 offiziell unterstützt
* Fritz!Box XML: Dateien des Fritz!Box Telefonbuchs
* vCard: (getestet mit vCard Export Dateien von "MacOS 10.6 Adressbuch" /  "Google Mail/Contacts" / "jFritz Adressbuch"
* Fritz!Adr Adressbuche-Exportdateien (Erstellen bitte über Datei => Exportieren => in Datei exportieren => Text-Tabstopp-getrennt)
* CSV Dateien aller Art  über den Generic CSV Import (z.B. für das Addressbuch von Windows Live Mail oder Export-CSV Dateien von Webmail Diensten wie GMX)
* Google Contacts: Direkter Import von GMail Konten (direkt über Google API v3)

## Unterstützte Export-Formate

* Outlook
* Fritz!Box XML: XML Dateien des Fritz!Box Telefonbuchs (generierte Datei im Fritz!Box WebIF dann als Telefonbuch "wiederherstellen")
* vCard
* vCard - Spezialversion für Gigaset Telefone (getestet mit DX600A)
* Fritz!Adr - Fritz!Adressbuch von Fritz!Fax & Fritz!Fon (über Text mit Tabstopp Datei - generierte Datei dort dann manuell importieren)
* Snom v7: CSV Dateien für Snom Telefone mit Firmware v7.x
* Snom v8: CSV Dateien für Snom Telefone mit Firmware v8.x (mit mehreren Nummern pro Kontakt)
* Gigaset Talk&Surf: CSV Dateien für die Gigaset Talk&Surf Software
* Aastra: CSV Dateien für Aastra Telefone
* Grandstream: XML Dateien für Grandstream Telefone (in 2 Varianten: 1 Telefonnummer pro Kontakteintrag für GXP Telefone, mehrere Telefonnummern pro Kontakteintrag für GXV Telefone)
* Auerswald: CSV Dateien für Auerswald Telefonanlagen
* Google Contacts Export, exportiert direkt per Google API in die Kontaktdatenbank

## Hinweise

Namensgenerierung: Beim Export in Zielformate (Fritz!Box XML, Fritz!Adr, Snom v7) die keine getrennten Felder für Vor und Nachnahmen vorsehen, stehen viele Möglichkeiten zur Auswahl auf welche Weise der nötige Gesamtname aus den vorhanden Einzelteilen (Vorname, Nachname, Firma) zusammengesetzt werden soll.

Spezialoptionen: Desweiteren gibt es Spezialoptionen für Outlook (Feld "Speichern Unter" nutzen für den Gesamtnamen) sowie Möglichkeiten Kontake in den Quelldaten als VIP zu definieren und Kurzwahlnummern festzulegen.

