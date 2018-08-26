# Contact Conversion Wizard - Linux/MacOS X

Wie bei C#/.NET Programmen üblich ist der Contact Conversion Wizard auch unter Linux & MacOS lauffähig, sofern man eine aktuelle Mono Runtime installiert hat (Unter diesen Betriebssystemen gibt es natürlich keinen Outlook Support).

## Linux am Beispiel von openSUSE 13.1

Anleitung Stand 11/2013:

* Pakete "mono-core" und "mono-winforms" nachinstallieren über Suse Software-Verwaltung
* Im Terminal: Mit "mozroots --import --ask-remove" SSL-Zertifikate für Mono holen und installieren (nur nötig für direkten Google Mail Import, sonst kann dieser Schritt übergangen werden)
* Im Terminal: exe file im Terminal starten mit "mono /path/to/ccw.exe"

Das WinForms Fenster ohne ordentliches Theme ist zwar nicht unbedingt schön anzusehen, aber volle Funktionalität scheint mir gegeben zu sein. "&" Zeichen in der Tabelle wird nicht korrekt dargestellt (stattdessen "_"), im XML Ausgabefile war es allerdings ok. Sollte also eigentlich alles gehen. Outlook Import natürlich nicht mangels Outlook unter Linux.

![OpenSuse Screenshot, das den Contact Conversion Wizard v3.5.0.0 zeigt: Ein GUI Fenster mit 3 Bereichen: Links: Daten laden; Mitte: Daten ansehen; Rechts: Daten abspeichern](../img/Contact%20Conversion%20Wizard%20v3.5.0.0%20-%20OpenSuse%2013.1.jpg)

## MacOS X

Anleitung Stand 06/2014:

* Er geht wieder! Man braucht:
  * CCW 3.5.0.0 OS X
  * XQuartz 2.7.6 (http://xquartz.macosforge.org/trac/wiki) - muss man vor Mono installieren!
  * Mono MRE von http://www.go-mono.com/mono-downloads/download.html
* Der erste Start vom CCW dauert ewig (>1 min) und er meint ständig er würde hängen im Activity Monitor. Irgendwann kommt er dann und ab dann geht er schnell auch über Beenden und wieder Starten hinweg.

![MacOS Screenshot, das den Contact Conversion Wizard v3.5.0.0 zeigt: Ein GUI Fenster mit 3 Bereichen: Links: Daten laden; Mitte: Daten ansehen; Rechts: Daten abspeichern](../img/Contact%20Conversion%20Wizard%20v3.5.0.0%20-%20MacOSX.png)

