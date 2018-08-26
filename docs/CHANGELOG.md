# Contact Conversion Wizard - Änderungshistorie

## Änderungshistorie

### v3.5.0.0 [22.11.2013]

* neu: Compilierungs-Profil geändert auf .NET 4.0 Client
* neu: verbesserter Snom V8 Export wenn mehrere Hauptkontakte die selbe primäre Nummer benutzen, als Master ID wird nun zufällige Nummer benutzt
* neu: Der generische CSV Import erlaubt nun das Speichern und Laden der gewählten Zuweisungen
* neu: Google API DDLs von v2.1 auf v2.2 aktualisiert
* neu: Optionales Kopieren der erstellten JPG Bilder direkt auf die \\FRITZ.NAS Freigabe nach Exportvorgang (nur für Boxen mit internem Speicher)
* neu: Filtermöglichkeit für Import einzelner Gruppen beim von Google Contacts-Import
* bugfix: die Konfigurationsoptionen "Keep space characters" und "Keep [] characters" haben nun auch tatsächlich einen Effekt
* neu: Neue Konfigurationsoption "Keep a-z and A-Z" bei den Optionen für die Rufnummernbereinigung
* bugfix: Konfigurationsfenster nun nicht mehr resizebar und maximierbar
* neu: Option um allen exportierten Rufnummern immer eine "0" vorran zu stellen für Telefonanlagen
* neu: Import der "Main" bzw "Festnetz" Nummer aus Google Contacts in Home/Work Felder (sofern noch leer)
* bugfix: Der SimpleInputDialog der nach einzeiligen Eingaben fragt kann nun auch durch ENTER abgeschlossen werden und nicht nur durch schliessen des Dialogs
* neu: Das Google Kennwort muss nicht mehr zwangsweise fest in der Konfiguration eingetragen (und damit gespeichert) werden, falls leer gelassen wird gefragt bei Import/Export.
* bugfix: vCard Dateien wurden bisher gespeichert als 16bit Unicode und nicht im verbreiteteren UTF8. Das war nie Absicht und hat die Kompatibilität unnötig reduziert.
* neu: Spezieller vCard Export für Gigaset Geräte (getestet mit DX600A), vereinfachtes und an Gigaset angepasstes vCard Format als neue Exportoption
* neu: beim Gigaset vCard Export und Grandstream GXV Export werden die separaten Firstname und Lastname Felder statt einem CombinedName benutzt sofern Company leer ist oder ein Combine Name ohne Company Bestandteil ausgewählt wurde
* neu: Konfigurationsoption "Fritz!XML Export: Work Nr. before Home Nr" abgeschafft, Reihenfolge wird jetzt durch Combobox "if in doubt use/store as" mitbestimmt
* bugfix: < und > Zeichen werden beim Fritz!XML Export nun auch im E-Mail Feld escaped und nicht nur in den Namensfeldern
* neu: Import von Faxnummern aus dem Fritz!XML file
* bugfix: keine Endlosschleife mehr bei neuen unbekannten attributen im Fritz!XML file, stattdessen kurzer report nach dem Einlesen was ignoriert wurde
* neu: Export von Faxnummern in die Fritz!XML Datei möglich (aber optional und nur sofern shift taste gedrückt wurde)
* neu: Autodetect des Zeichensatzes beim Fritz!XML Import anhand der ersten Zeile im Quellfile

### v3.4.1.1 [26.08.2012]

* bugfix: Snom V8 und Panasonic - Crash wenn Telefonnummern des Kontakts nach dem Bereinigen leer waren
* bugfix: vCard: korrektes Escapen von CR/LF & LF & : in Adresszeilen
* bugfix: nur leeres file bei Snom V8 Export

### v3.4.0.5 [16.07.2012]

* aktualisiert: Neue v2.1 vom Google Data API integriert
* neu: Grandstream Export für GXP Telefone geschrieben
* neu: Grandstream Export für GXV Telefone deutlich überarbeitett
* neu: Auerwald Export
* neu: Google Contacts Export
* verbessert: Diverse kleine Code Verbesserungen und Aufräumarbeiten an mehreren Stellen
* bugfix: Crash beim holen von Google Kontaktbildern die lt. Google existieren dann plötzlich doch nicht da sind wird nun ignoriert  

### v3.3.0.1 [13.08.2011]

* aktualisiert: integrierte LumenWorks.Framework.IO.dll für CSV Import auf v3.8.0.0 (von v3.7.0.0)
* aktualisiert: integrierte Google GData DLLs auf v1.8.0.0 (von v1.7.0.1)

### v3.3.0.0 [11.07.2011]

* neu: Buttons zum Aufrufen der Homepage und des Support Forums
* neu: automatischer Versionscheck beim Starten (sofern in Optionen aktiviert, Standard ist AUS). Bei verfügbarer neuer Version wird der Button für die Homepage farbig hervorgehoben und ändert seinen Text.
* neu: restriktive Rufnummern-Bereinigung: Alle Zeichen werden nun entfernt die nicht explizit erlaubt sind (0-9, +, *, sowie die im Optionsmenü konfigurierbaren sind erlaubt)

### v3.2.0.1 [21.06.2011]

* nun automatische Erkennung der Outlook Version, früher eingeführte Option "Import/Export Outlook contact pictures" nun wieder entfernt
* Bilderimport aus Outlook geht nun offiziell erst ab Outlook 2007. Ob es vorher jemals mit Outlook 2003 ging ist unklar, vermutlich nicht. Gründe sind dubios und waren auch nach 2 Stunden Fehlersuche nicht klar erkennbar.

### v3.2.0.0 [09.06.2011]

* Möglichkeit (Konfigurationsoption) um Duplikate importieren durch automatisches umbenennen zu erlauben
* Duplikatserkennung neu geschrieben und umstrukturiert
* Neue Möglichkeit über PREFER(HOME), PREFER(WORK), PREFER(MOBILE) im Notizfeld der Kontakte die primäre Nummer festzulegen [beeinflusst unter anderem auch für die Speeddial-Funktion der Fritz!Box]
* vCard Parser deutlich überarbeitet&verbessert.

### v3.0.0.4

* neue Option "Import/Export Outlook contact pictures" (Standardmässig an, evtl. Abschalten für Outlook 2000/2002 Kompatibilität)

### v3.0.0.3

* Funktion "Outlook Import: es kann nun auch das Feld "Other (Weitere Tel Nr)" importiert werden (konfigurierbar, benutzt Zielfelder Home/Work sofern sie leer sind)" geht nun auch tatsächlich [Smile]

### v3.0.0.2

* Probleme beim Laden der gespeicherten Konfiguration unter Mono/MacOS beseitigt (Thx Hachre!)

### v3.0.0.1

* Direkter Import von GoogleMail Konten ohne Umweg über vCard, inclusive Photo Import !!!
* Outlook Import: es kann nun auch das Feld "Other (Weitere Tel Nr)" importiert werden (konfigurierbar, benutzt Zielfelder Home/Work sofern sie leer sind)
* Vorhandene "/" Zeichen können nun auch aus den Telefonnummern entfernt werden (konfigurierbar)
* Grandstream XML Export (Implementiert von bernd.hardung - DANKE!)
* Möglichkeit die Sortierreihenfolge der Telefonbucheinträge im Fritz!Box XML Export zu beeinflussen (optional statt Home/Work jetzt auch Work/Home)

### v2.9.0.5

* Neue Konfigurationsoption um zu beeinflussen ob PREFIX nur die Fritz!Box XML ausgabe betrifft oder alle Ausgabeformate.
* Möglichkeit bei den Notizen eines Kontakts mittels z.B. PREFIX(#31#) allen Rufnummern ein #31# vorranzustellen (wird nur beim Fritz!Box XML Export gemacht)
* kleine Änderung am vCard Parser um aus den von vCardIO (Android) erstellten vCards die Bilder zu importieren
* Vorwahlen, Bindestriche und "x" wird aus Rufnummern grundsätzlich nur noch dann entfernt wenn kein "@" in der Rufnummer vorkommt (Sonderbehandlung eMail Adressen als Rufnummer für HD Telefonie)
* eine Info/Debug Meldung beim Einlesen der Kontakte abgeschaltet

### v2.9.0.0

* Konfigurationsoptionen werden nun in einem .config file dauerhaft gespeichert
* Zusätzliche Konfigurationsoptionen hinzugefügt mittels derer man beeinflussen kann welche Zeichen aus den Telefonnummern entfernt werden
* kleine Buttons hinzugefügt die den Standard-Bilderpfad für 7270 und 7390 ins Textfeld eintragen

### v2.8.6.0

* Outlook Import so geändert das nun auch Kontakte importiert werden die ein Custom Form als Messageclass zugewiesen haben.
* es gibt nun ein Konfigurationsmenü
* Konfigurationsoption: Spaltengröße anpassbar
* Konfigurationsoption: Spalten ausblenden die nichts enthalten
* Neue Output Prefix Option "Keep '+XX' prefixes intact (nicht so sinnvoll bei Export in Fritz!Box, aber vielleicht bei anderen Exportzielen!)
* Probleme beim Fritz!XML Export behoben wenn im Namen "<" oder ">" vorkamen
* Berücksichtigung von eMail Addressen beim Fritz!XML Export
* Character Encoding geändert beim Export für die Telefone SnomV7, SnomV8, Aastra von UTF8 auf UTF8-without-BOM (Byte-Order-Mark)
* Möglichkeit nach Kategorien zu filtern beim Outlook Import (CTRL-Taste gedrückt halten)
* Deutlich verbessert: Generic CSV Import Feature
* Export CSV für AAstra Telefone

### v2.7.0.0

* Neu: Generic CSV Import Feature

### v2.6.0.0

* Neu: vCard export nun implementiert
* kleine Verbesserungen beim vCard Import

### v2.5.5.1

* Entfernt nun auch "-" aus Rufnummern vor dem Exportieren
* Exportfunktion für Gigaset Talk & Surf CSV Dateien
* Vanity Nummer Unterstützung, Achtung: neuer Syntax für SPEEDDIAL Keyword
* Bug nun endgültig behoben der weiterhin manchmal die Ländervorwahl durch 0 ersetzt hat wenn sie mitten in der Rufnummer vorkam

### v2.4.0.5

* '*' Zeichen werden nicht mehr aus der Rufnummer entfernt, da diese ja für Interne Teilnehmer an der Fritz!Box verwendet werden
* Abstürze beim Import mancher Kontakte aus Outlook behoben
* Neue Exportfunktion für Snom v8 mit voller Unterstützung für Subcontacts und VIP flags
* vCard Import nun noch toleranter - akzeptiert nun auch mit jFritz v0.7.3.10 erstellte Dateien (ich hatte es bisher nur mit der jFritz Beta getestet, die aber anders formatierte vCard Dateien erstellt)
* Potentielle Probleme behoben bei mehrfachem Export der Daten

### v2.3.0.1

* Datenexport nach Outlook, incl. evtl. vorhandener Kontaktbilder
* Import von BASE64 codierten Bildern aus vCards (Apple Adressbook)
* Import von Kontaktbildern aus Outlook
* Speichern von Kontaktbildern in Unterordner beim Fritz!XML Export, Fritz!Box Bilder Pfad konfigurierbar über Textfeld im Hauptdialog

### v2.1.0.3

* Unterstützung für "MIME Unfolding" zur Verarbeitung von auf mehreren Zeilen verteilten vCard Einträgen
* Speeddial & VIP Funktionalität
* Neues Keyword "CCW-IGNORE", dessen Vorhandensein im Kommentar/Notiz-Feld bei Import aus Outlook, vCard und Fritz!Adr Daten geprüft wird. Wird es gefunden, wird der gesamte Kontakt ignoriert und nicht importiert


### v2.0.0.x im Vergleich zu v1.x

* Namensänderung von Fritz!XML Wizard zu Contact Conversion Wizard
* Import Fritz!Box XML Dateien, vCard Import nun deutlich toleranter, akzeptiert auch jFritz vCard Dateien
* Einlesen nicht nur von Telefonnummern sondern auch von Adressen und eMail
* Mehr Möglichkeiten den Ausgabenamen zu gestalten (Vorname vorne)
* Fritz!Adr Adressbuch Import ist implementiert (über Fritz!Adr Text mit Tabstopp-Export Datei) sowie auch Export mit allen Feldern
* Großes Vorschaufenster (readonly), Tooltips für die Buttons die erklären was sie machen
* Erweiterte Importoptionen


