# Algorithmen und Datenstrukturen

In Programmier-Übung habe ich ja viel über Algorithmen und Datenstrukturen erzählt. Um diese Begriffe noch mal etwas mit mehr Inhalt zum in Ruhe nachlesen zu unterfüttern, hier eine weitere README-Datei.
Und am Schluss nochmal die Lösung für die 3 Ablage-Orte im Dateisystem der Betriebssysteme Windows, macOS und Linux
## Algorithmus
Ein Algorithmus ist eine systematische Schritt-für-Schritt-Anleitung, mit der ein bestimmtes Problem gelöst oder eine Aufgabe ausgeführt wird. Man kann sich einen Algorithmus wie ein Rezept vorstellen: Es gibt eine klare Reihenfolge von Anweisungen, die – wenn korrekt befolgt – zu einem gewünschten Ergebnis führen.
### 🧠 Merkmale eines Algorithmus
- Eindeutigkeit: Jeder Schritt ist klar und unmissverständlich.
- Endlichkeit: Der Algorithmus besteht aus einer endlichen Anzahl von Schritten.
- Ausführbarkeit: Jeder Schritt ist so formuliert, dass er tatsächlich ausgeführt werden kann.
- Zielgerichtetheit: Er führt zu einem bestimmten Ergebnis oder löst ein Problem.

### 📌 Beispiel aus dem Alltag
  Algorithmus zum Zähneputzen:
- Zahnbürste nehmen.
- Zahnpasta auftragen.
- Wasser auf die Bürste geben.
- Zähne 2 Minuten lang putzen.
- Mund ausspülen.

### 💻 Beispiel aus der Informatik
  Ein einfacher Algorithmus zum Addieren zweier Zahlen in Pseudocode:
```pseudocode
  Eingabe: Zahl A, Zahl B
  Verarbeite: Summe = A + B
  Ausgabe: Summe
 ```
Algorithmen sind das Herzstück jeder Software – egal ob beim Sortieren von Daten, beim Navigieren mit Google Maps oder beim Empfehlen von Videos auf YouTube.

## Algorithmus vs. Programm
Die Unterscheidung zwischen einem Algorithmus und einem Programm ist grundlegend, aber oft subtil – besonders in der Informatik. Hier ist eine klare Gegenüberstellung:
### 🧩 Algorithmus vs. Programm
| Merkmal          | Algorithmus                                                       | Programm |
|------------------|-------------------------------------------------------------------|--|
| Definition       | Eine abstrakte, schrittweise Anleitung zur Lösung eines Problems  | Eine konkrete Umsetzung eines Algorithmus in einer Programmiersprache |
| Form             | Konzeptuell, oft in Pseudocode oder als Flussdiagramm dargestellt | In einer Programmiersprache geschrieben (z. B. C#, R, Python) |
| Ausführbarkeit   | Nicht direkt ausführbar – muss erst implementiert werden          | Direkt ausführbar auf einem Computer |
| Abstraktionsgrad | Höher – beschreibt was getan werden soll                          | Konkreter – beschreibt wie es getan wird|
| Beispiel         |  „Sortiere eine Liste durch wiederholten Vergleich benachbarter Elemente“|Ein C#-Programm, das Bubble Sort implementiert  |

### 🔧 Beispiel zur Veranschaulichung
Algorithmus (Pseudocode):
```pseudocode
Für jedes Element in der Liste:
Vergleiche mit dem nächsten Element
Wenn größer, tausche sie
Wiederhole, bis keine Vertauschung mehr nötig ist
```
### Programm (C#):
```csharp
void BubbleSort(int[] arr) {
    for (int i = 0; i < arr.Length - 1; i++) {
        for (int j = 0; j < arr.Length - i - 1; j++) {
            if (arr[j] > arr[j + 1]) {
                int temp = arr[j];
                arr[j] = arr[j + 1];
                arr[j + 1] = temp;
            }
        }
    }
}
```
### 🧠 Fazit
Ein Algorithmus ist die Idee, ein Programm ist die Umsetzung. Du kannst dir den Algorithmus wie den Bauplan vorstellen – das Programm ist das fertige Haus.

## Datenstruktur
Datenstrukturen sind organisierte Speicherformen für Daten, die es ermöglichen, Informationen effizient zu speichern, zu verwalten und zu verarbeiten. Sie sind das Fundament jeder Software – ohne sie wäre Programmieren wie Schreiben ohne Grammatik.
### 🧱 Warum sind Datenstrukturen wichtig?
- Sie bestimmen, wie Daten abgelegt werden.
- Sie beeinflussen die Leistung von Algorithmen.
- Sie helfen, komplexe Probleme elegant zu lösen.

### 📦 Typische Datenstrukturen im Überblick
| Datenstruktur         | Beschreibung | Beispielanwendung |
|-----------------------|--------------|-------------------|
| Array (Feld)          | Feste Größe, indexbasierter Zugriff| Liste von Temperaturen                 |
| Liste (Linked list)   | Dynamisch, Elemente sind verkettet            |  Musik-Playlist                |
| Stack (Stapel)        | LIFO-Prinzip: Last In, First Out       |  Rückgängig-Funktion in Texteditoren                |
| Queue (Warteschlange) | FIFO-Prinzip: First In, First Out            | Druckerwarteschlange                 |
| Hash/Dictionary       | Schlüssel-Wert-Paare für schnellen Zugriff|  Wörterbuch, Konfigurationen|
| Tree (Baum)           | Hierarchische Struktur mit Eltern-Kind-Beziehungen             |  Dateisystem, Entscheidungsbäume                 |
| Graph                 | Knoten und Kanten zur Darstellung von Beziehungen             |  Soziale Netzwerke, Routenplanung                 |

### 🧠 Beispiel in C# – Dictionary
```csharp
var telefonbuch = new Dictionary<string, string>();
telefonbuch["Thorsten"] = "+49 123 456789";
Console.WriteLine(telefonbuch["Thorsten"]); // Gibt die Nummer aus
```
### 📊 Beispiel in R – Data Frame
```R
daten <- data.frame(
    Name = c("Anna", "Ben", "Clara"),
    Alter = c(23, 31, 27)
)
print(daten)
```

## Ablage-Orte im Dateisystem der gängisten Desktop-Betriebssysteme
Mit ausführlicher Erklärung
```csharp
namespace InventarWorkerCommon.Services.Paths;

/// <summary>
/// Provides functionality to retrieve the base directory path for application data across different operating systems.
/// This class determines the base path depending on the operating system where the application is running.
/// </summary>
public class BasePath
{
    /// <summary>
    /// Retrieves the base directory path for application data based on the operating system.
    /// This method returns a path that varies depending on whether the application is running
    /// on Windows, MacOS, or other Unix-based systems.
    /// </summary>
    /// <returns>
    /// A string representing the base directory path:
    /// - On Windows, the path to the common application data folder.
    /// - On MacOS, the "/Users/Shared" directory.
    /// - On other Unix-based systems, the "/var/tmp" directory.
    /// </returns>
    public static string GetBasePath()
    {
        return OperatingSystem.IsWindows()
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData))
            : OperatingSystem.IsMacOS()
                ? "/Users/Shared"
                // 🗂️ 2. /var/tmp – Persistent temporary storage
                // - Description: Similar to /tmp, but files persist even after a reboot.
                // - Access: Writable by all users.
                // - Security: Each user can only access their own files due to the sticky bit being set.
                // - Security: Each user can only access their own files, as the sticky bit is set.
                // - Sticky-Bit: Usually also set.
                : "/var/tmp";
                /*
                  🧷 Das Sticky-Bit unter Unix – kurz erklärt
                  Das Sticky-Bit ist ein spezielles Dateisystem-Attribut, das vor allem bei gemeinsam genutzten Verzeichnissen eine wichtige Rolle spielt. Es sorgt dafür, dass nur der Eigentümer einer Datei (oder der Superuser) diese löschen oder umbenennen kann – selbst wenn andere Benutzer Schreibrechte für das Verzeichnis haben.
                  📌 Typisches Einsatzszenario
                  Ein klassisches Beispiel ist das Verzeichnis /tmp, das von vielen Prozessen und Benutzern gemeinsam genutzt wird. Ohne Sticky-Bit könnten Benutzer fremde Dateien löschen, wenn sie Schreibrechte auf /tmp haben. Mit gesetztem Sticky-Bit wird das verhindert.
                  🔧 Technische Details
                    - Das Sticky-Bit wird auf Verzeichnisse gesetzt, nicht auf einzelne Dateien.
                    - Es wird mit dem Befehl chmod +t aktiviert.
                    - In der Dateiausgabe von ls -ld sieht man es als ein „t“ am Ende der Berechtigungen:
                    drwxrwxrwt  10 root  root  4096 Oct  7 14:00 /var/tmp
                 */
    }
}
```
