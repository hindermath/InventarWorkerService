# Erläuterungen zum Einführungscode-Beispiel
## Die wichtigsten Begriffe noch einmal ausführlich erklärt

## 1. Einfaches Konsolenprogramm

```csharp

In C# hat das Schlüsselwort `using` zwei Hauptverwendungen, und beide sind ziemlich wichtig für sauberen und effizienten Code. Hier ist eine klare Übersicht:

---

### 🧹 1. **`using`-Anweisung zur Ressourcenverwaltung**

Diese Form sorgt dafür, dass Ressourcen wie Dateien, Streams oder Datenbankverbindungen **automatisch freigegeben** werden, sobald sie nicht mehr gebraucht werden. Sie funktioniert mit Objekten, die das Interface `IDisposable` implementieren.

    Beispiel:
    using (StreamReader reader = new StreamReader("datei.txt"))
    {
        string inhalt = reader.ReadToEnd();
    }
    // reader.Dispose() wird automatisch aufgerufen

✅ Vorteile:
- Verhindert Speicherlecks
- Garantiert, dass `Dispose()` auch bei Fehlern aufgerufen wird
- Kürzer und lesbarer als `try-finally`

Ab C# 8.0 gibt es auch die **`using`-Deklaration**:
    using var reader = new StreamReader("datei.txt");
    string inhalt = reader.ReadToEnd();
    // Dispose wird am Ende des Gültigkeitsbereichs automatisch aufgerufen

---

### 📦 2. **`using`-Direktive zum Einbinden von Namespaces**

Diese Verwendung steht ganz oben in der Datei und erlaubt dir, Klassen aus anderen Namespaces zu verwenden, ohne den vollständigen Pfad anzugeben.

#### Beispiel:
```csharp
using System;
using System.Collections.Generic;

List<string> namen = new List<string>();
Console.WriteLine("Hallo Welt!");
```

Ohne `using System;` müsstest du schreiben:
```csharp
System.Console.WriteLine("Hallo Welt!");
```

---

```csharp
using ClassLibrary1;
```

Ein Namespace in C# ist wie ein Ordnungshelfer für deinen Code – er hilft dir, Klassen, Interfaces, Enums und andere
Typen logisch zu gruppieren und Namenskonflikte zu vermeiden. Stell dir vor, du hast viele verschiedene Dateien mit
ähnlichen Namen – ohne Ordner wäre das Chaos vorprogrammiert. Genau das verhindert ein Namespace im Code.
🧠 Warum sind Namespaces wichtig?
- Strukturierung: Du kannst deinen Code übersichtlich in logische Bereiche aufteilen.
- Namenskonflikte vermeiden: Zwei Klassen mit dem gleichen Namen können in unterschiedlichen Namespaces existieren.
- Wiederverwendbarkeit: Du kannst Bibliotheken nutzen, ohne dass deren Klassennamen mit deinen kollidieren.

```csharp
namespace ConsoleApp2;
```
/*
In C# ist eine **Class (Klasse)** ein grundlegendes Bauelement der objektorientierten Programmierung. Sie dient als **Bauplan** für Objekte – also Instanzen, die bestimmte Eigenschaften und Verhalten besitzen.

### 🧱 Was macht eine Klasse aus?

Eine Klasse definiert:
- **Felder (Fields)**: Variablen, die den Zustand eines Objekts speichern.
- **Eigenschaften (Properties)**: Zugriffspunkte auf Felder mit optionaler Logik.
- **Methoden (Methods)**: Funktionen, die das Verhalten eines Objekts beschreiben.
- **Konstruktoren (Constructors)**: Spezielle Methoden zur Initialisierung eines Objekts.
- **Zugriffsmodifizierer**: Regeln, wer auf was zugreifen darf (z. B. `public`, `private`).

### 📦 Beispiel für eine einfache Klasse

```csharp
public class Auto
{
    // Feld
    private string farbe;

    // Eigenschaft
    public string Farbe
    {
        get { return farbe; }
        set { farbe = value; }
    }

    // Konstruktor
    public Auto(string farbe)
    {
        this.farbe = farbe;
    }

    // Methode
    public void Fahren()
    {
        Console.WriteLine("Das Auto fährt.");
    }
}
```

### 🚗 Verwendung der Klasse

```csharp
Auto meinAuto = new Auto("Rot");
meinAuto.Fahren();
Console.WriteLine(meinAuto.Farbe); // Ausgabe: Rot
```

### 🧠 Warum sind Klassen wichtig?

- Sie helfen, **Komplexität zu strukturieren**.
- Sie ermöglichen **Wiederverwendbarkeit** von Code.
- Sie fördern **Modularität** und **Wartbarkeit**.
```csharp
  class Program
  {
```

  In C# ist eine **Methode** ein Block von Code, der eine bestimmte Aufgabe ausführt und bei Bedarf mehrfach aufgerufen werden kann. Sie hilft dabei, Programme übersichtlich, wiederverwendbar und logisch strukturiert zu gestalten.

        ### 🧠 Grundstruktur einer Methode

        Zugriffsmodifizierer Rückgabetyp Methodenname(Parameterliste)
        {
            // Anweisungen
            return Rückgabewert; // optional, je nach Rückgabetyp
        }

        ### 📌 Beispiel

        public int Addiere(int a, int b)
        {
            return a + b;
        }

        - `public`: Zugriffsmodifizierer – bestimmt, wer die Methode aufrufen darf.
        - `int`: Rückgabetyp – hier gibt die Methode eine Ganzzahl zurück.
        - `Addiere`: Name der Methode.
        - `(int a, int b)`: Parameterliste – Eingabewerte für die Methode.
        - `return a + b;`: Rückgabe des Ergebnisses.

        ### 🔄 Methodenaufruf

        ```csharp
        int ergebnis = Addiere(5, 3); // ergebnis ist jetzt 8
        ```

        ### 🧩 Warum Methoden verwenden?

        - **Wiederverwendbarkeit**: Einmal schreiben, beliebig oft verwenden.
        - **Lesbarkeit**: Komplexe Logik wird in kleine, verständliche Einheiten zerlegt.
        - **Wartbarkeit**: Änderungen sind leichter umzusetzen und zu testen.

        Wenn du magst, kann ich dir auch zeigen, wie man Methoden mit verschiedenen Rückgabetypen oder ohne Rückgabe schreibt. Oder willst du gleich selbst eine Methode entwerfen?

  Die Main-Methode in C# ist der Einstiegspunkt eines Programms – quasi das Herzstück, das beim Start zuerst schlägt. 🧠💻
  Hier ist, was du darüber wissen solltest:

        🧩 Bedeutung der Main-Methode
        - Sie ist die erste Methode, die beim Ausführen eines C#-Programms aufgerufen wird.
        - Sie definiert, was das Programm tun soll, sobald es gestartet wird.
        - Ohne eine Main-Methode kann ein Konsolenprogramm nicht ausgeführt werden.

        🛠️ Syntax-Beispiel
        using System;

        class Program
        {
            static void Main(string[] args)
            {
                Console.WriteLine("Hallo Welt!");
            }
        }



        🔍 Wichtige Merkmale
        - static: Die Methode gehört zur Klasse, nicht zu einem Objekt.
        - void oder int: Sie kann entweder nichts zurückgeben (void) oder einen Exit-Code (int) liefern.
        - string[] args: Optionales Argument, das Kommandozeilenparameter enthält.

        🧪 Varianten
        static void Main() { }

        static int Main() { return 0; }

        static void Main(string[] args) { }

        static int Main(string[] args) { return 0; }
```csharp
  static void Main(string[] args)
  {
      Console.WriteLine("Hello, World!");
      Console.WriteLine($"Calculation result:{Calculator.Calculation(1,2, "+")}");
  }
}
```

## 2. Bibliothek
In C# bezeichnet eine **Bibliothek** (englisch: *library*) eine Sammlung von wiederverwendbarem Code, der bestimmte Funktionen oder Dienste bereitstellt. Sie hilft dir, komplexe Aufgaben zu lösen, ohne alles selbst programmieren zu müssen.

### 📚 Arten von Bibliotheken in C#

- **Klassenbibliotheken**  
  Enthalten Klassen, Methoden, Schnittstellen usw., die du in deinen Projekten verwenden kannst. Beispiel: `System.IO` für Dateizugriffe.

- **.NET Framework / .NET Core / .NET Standard Libraries**  
  Microsoft stellt eine riesige Standardbibliothek bereit, die grundlegende Funktionen wie Dateioperationen, Netzwerkkommunikation, Datenbankzugriffe usw. abdeckt.

- **Externe Bibliotheken (Third-Party Libraries)**  
  Von anderen Entwicklern erstellt, z. B. `Newtonsoft.Json` für JSON-Verarbeitung oder `Entity Framework` für Datenbankzugriffe.

- **Eigene Bibliotheken**  
  Du kannst selbst eine Bibliothek schreiben, z. B. zur Wiederverwendung von Code in mehreren Projekten. Diese wird meist als `.dll` (Dynamic Link Library) kompiliert.

### 🛠 Beispiel: Verwendung einer Bibliothek

```csharp
using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Hallo aus der System-Bibliothek!");
    }
}
```

Hier verwendest du die `System`-Bibliothek, um auf die `Console`-Klasse zuzugreifen.

---

Die Eklärungen zu `switch` und `throw` folgen nach dem Beispiel:
```csharp
namespace ClassLibrary1;

public class Calculator
{
    public static double Calculation(double number1, double number2, string operation)
    { 
        
        switch (operation)
        {             
            case "+":
                return number1 + number2;
            case "-":
                return number1 - number2;
            case "*":
                return number1 * number2;
            case "/":
                return number1 / number2;
            default:
                throw new ArgumentException("Invalid operation");
        }
    }
}
```
---
## `switch`-Anweisung
Die `switch`-Anweisung in C# ist eine Kontrollstruktur, mit der du einen Ausdruck gegen mehrere mögliche Werte vergleichen kannst – eine elegante Alternative zu vielen `if-else`-Blöcken. Sie macht deinen Code übersichtlicher, besonders wenn du viele Bedingungen prüfen musst.

### 🧠 Aufbau einer `switch`-Anweisung

```csharp
int zahl = 2;

switch (zahl)
{
    case 1:
        Console.WriteLine("Eins");
        break;
    case 2:
        Console.WriteLine("Zwei");
        break;
    case 3:
        Console.WriteLine("Drei");
        break;
    default:
        Console.WriteLine("Unbekannt");
        break;
}
```

### 🔍 Erklärung der Bestandteile

- **`switch (Ausdruck)`**: Der Ausdruck, der ausgewertet wird (z. B. eine Variable).
- **`case`**: Jeder mögliche Wert, den der Ausdruck annehmen kann.
- **`break`**: Beendet den aktuellen Fall, damit nicht automatisch der nächste ausgeführt wird.
- **`default`**: Optionaler Block, der ausgeführt wird, wenn kein `case` zutrifft.

### ✨ Moderne Variante mit Musterabgleich (ab C# 8.0)

```csharp
object obj = 42;

switch (obj)
{
    case int i when i > 0:
        Console.WriteLine("Positive Zahl");
        break;
    case string s:
        Console.WriteLine($"Ein String: {s}");
        break;
    default:
        Console.WriteLine("Etwas anderes");
        break;
}
```
Damit kannst du sogar Typen und Bedingungen kombinieren – ziemlich mächtig!

---
## `throw`-Anweisung
Die `throw`-Anweisung in C# ist dein Werkzeug, um **gezielt Ausnahmen (Exceptions)** auszulösen – also Fehler bewusst zu melden, wenn etwas Unerwartetes passiert. Sie ist ein zentraler Bestandteil der Fehlerbehandlung in .NET.

### ⚡ Grundsyntax

```csharp
throw new Exception("Da ist etwas schiefgelaufen!");
```

### 🧠 Was passiert dabei?

- Du **erzeugst ein neues Ausnahmeobjekt** (z. B. `Exception`, `ArgumentNullException`, `InvalidOperationException` usw.).
- Mit `throw` **wirfst du diese Ausnahme**, wodurch der normale Programmfluss unterbrochen wird.
- Die Ausnahme kann dann von einem `try-catch`-Block **abgefangen und behandelt** werden.

### 🛠 Beispiel mit `try-catch`

```csharp
try
{
    int zahl = 0;
    if (zahl == 0)
        throw new DivideByZeroException("Division durch Null ist nicht erlaubt.");
    
    int ergebnis = 10 / zahl;
}
catch (DivideByZeroException ex)
{
    Console.WriteLine($"Fehler: {ex.Message}");
}
```

### 🔁 Re-Throw einer Ausnahme

Wenn du eine Ausnahme im `catch`-Block abfängst, aber sie **weiterreichen** willst:

```csharp
catch (Exception)
{
    // Logging oder andere Aktionen
    throw; // wirft die ursprüngliche Ausnahme erneut
}
```

### 💡 Typische Einsatzszenarien

- Validierung von Eingaben
- Schutz vor ungültigen Zuständen
- Implementierung eigener Fehlerlogik in Bibliotheken

Wenn du magst, kann ich dir zeigen, wie man eigene Exception-Klassen erstellt oder wie man `throw` sinnvoll in einem Projekt einsetzt. Sag einfach Bescheid!

------

## `public`-Zugriffsmodifizierer

In C# steuert der **Zugriffsmodifizierer** die Sichtbarkeit von Klassen, Methoden, Eigenschaften und anderen Membern. Der Modifizierer `public` ist dabei der offenste – er erlaubt **vollen Zugriff von überall**, also aus jedem Namespace, jeder Klasse und jedem Assembly.

---

### 🔓 `public` in C#

- Bedeutet: Die Methode ist **für alle anderen Klassen und Komponenten sichtbar**.
- Wird verwendet, wenn du möchtest, dass andere Teile deines Programms oder sogar externe Programme auf die Methode zugreifen können.

```csharp
public void StarteMotor()
{
    Console.WriteLine("Motor gestartet.");
}
```

Diese Methode kann von jeder anderen Klasse aufgerufen werden, solange die Klasse selbst auch öffentlich ist.

---

### 🧰 Weitere Zugriffsmodifizierer in C#

| Modifizierer     | Beschreibung                                                                 |
|------------------|------------------------------------------------------------------------------|
| `public`         | Voller Zugriff von überall                                                   |
| `private`        | Nur innerhalb der eigenen Klasse sichtbar                                    |
| `protected`      | Sichtbar in der eigenen Klasse und in abgeleiteten Klassen                   |
| `internal`       | Sichtbar **nur innerhalb desselben Assemblys**                               |
| `protected internal` | Sichtbar in abgeleiteten Klassen **oder** im selben Assembly             |
| `private protected`  | Sichtbar in abgeleiteten Klassen **und** nur im selben Assembly          |

---

### 🧠 Beispiel zur Veranschaulichung

```csharp
class Fahrzeug
{
    private void TestPrivat() { }               // Nur in Fahrzeug sichtbar
    protected void TestGeschützt() { }          // In Fahrzeug und abgeleiteten Klassen
    internal void TestIntern() { }              // Nur im selben Assembly
    public void TestÖffentlich() { }            // Überall sichtbar
}
```

---
## 3. `.Net` Assembly - was ist das?

Ein C# Assembly ist im Grunde das Herzstück einer .NET-Anwendung — eine kompilierte Einheit von Code, die vom .NET-Runtime ausgeführt werden kann. Hier ist eine klare Übersicht, was ein Assembly ist und warum es wichtig ist:

---

### 🧩 Was ist ein Assembly?

Ein **Assembly** ist eine Datei, die von einem C#-Compiler erzeugt wird. Sie enthält:
- **Kompilierten Code** (in Intermediate Language, IL)
- **Metadaten** über die enthaltenen Typen, Methoden, Eigenschaften usw.
- Optional: **Ressourcen** wie Bilder, XML-Dateien oder andere eingebettete Inhalte

Assemblies können zwei Formen haben:

| Typ            | Dateiendung | Beschreibung |
|----------------|-------------|--------------|
| **Executable (EXE)** | `.exe`        | Startbare Anwendung |
| **Library (DLL)**    | `.dll`        | Wiederverwendbare Bibliothek, z.B. für Klassen oder Funktionen |

---

### 🔐 Weitere Merkmale

- **Starke Namen**: Assemblies können mit einem kryptografischen Schlüssel signiert werden, um ihre Identität zu sichern.
- **Versionierung**: Jede Assembly kann eine eigene Versionsnummer haben, was wichtig für Updates und Kompatibilität ist.
- **Manifest**: Enthält Informationen über die Assembly selbst, wie Name, Version, Kultur und Abhängigkeiten.

---

### 🛠 Beispiel

Wenn du in `Rider` ein Projekt kompilierst, entsteht z.B. eine Datei namens `MeineApp.dll` oder `MeineApp.exe`. Diese Datei ist das Assembly, das du dann ausführen oder in anderen Projekten verwenden kannst.

---


## 4. Beispiel Projektformat `ConsoleApp1.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net9.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
    </PropertyGroup>

    <ItemGroup>
      <ProjectReference Include="..\ClassLibrary1\ClassLibrary1.csproj" />
    </ItemGroup>

</Project>
```

## 5. Beispiel Solutionformat `ConsoleApp1.sln`
```ini

Microsoft Visual Studio Solution File, Format Version 12.00
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "ConsoleApp1", "ConsoleApp1\ConsoleApp1.csproj", "{BA0A24C4-98A2-4537-ADF5-43077454AB69}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "ConsoleApp2", "ConsoleApp2\ConsoleApp2.csproj", "{117CC101-D584-4D21-906C-B46E468F958D}"
EndProject
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "ClassLibrary1", "ClassLibrary1\ClassLibrary1.csproj", "{88296F48-B5ED-4528-8695-52146C64EB31}"
EndProject
Global
    GlobalSection(SolutionConfigurationPlatforms) = preSolution
        Debug|Any CPU = Debug|Any CPU
        Release|Any CPU = Release|Any CPU
    EndGlobalSection
    GlobalSection(ProjectConfigurationPlatforms) = postSolution
        {BA0A24C4-98A2-4537-ADF5-43077454AB69}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
        {BA0A24C4-98A2-4537-ADF5-43077454AB69}.Debug|Any CPU.Build.0 = Debug|Any CPU
        {BA0A24C4-98A2-4537-ADF5-43077454AB69}.Release|Any CPU.ActiveCfg = Release|Any CPU
        {BA0A24C4-98A2-4537-ADF5-43077454AB69}.Release|Any CPU.Build.0 = Release|Any CPU
        {117CC101-D584-4D21-906C-B46E468F958D}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
        {117CC101-D584-4D21-906C-B46E468F958D}.Debug|Any CPU.Build.0 = Debug|Any CPU
        {117CC101-D584-4D21-906C-B46E468F958D}.Release|Any CPU.ActiveCfg = Release|Any CPU
        {117CC101-D584-4D21-906C-B46E468F958D}.Release|Any CPU.Build.0 = Release|Any CPU
        {88296F48-B5ED-4528-8695-52146C64EB31}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
        {88296F48-B5ED-4528-8695-52146C64EB31}.Debug|Any CPU.Build.0 = Debug|Any CPU
        {88296F48-B5ED-4528-8695-52146C64EB31}.Release|Any CPU.ActiveCfg = Release|Any CPU
        {88296F48-B5ED-4528-8695-52146C64EB31}.Release|Any CPU.Build.0 = Release|Any CPU
    EndGlobalSection
EndGlobal
```

## 6. Nachträgliches einchecken des Projekts in Git(Lab) der GWDG.

### 6.1 `.gitignore`-Datei anlegen
Damit nicht alle temporären Dateien und Ordner mit in das Repository aufgenommen werden,
sollte eine `.gitignore`-Datei angelegt werden. Diese Datei enthält eine Liste von Dateien und Ordner.
Von der _Solution_-Ansicht auf die _Files_-Ansicht wechseln.
Nun die oberste Ebene anklicken und dann einen Mausklick mit der rechten Maustaste.
Im Kontextmenü `Add` -> `File` klicken.

![img_5.png](img_5.png)

Die Datei im Dialog-Feld `.gitignore` benennen. Die Datei wird automatisch geöffnet. Die Frage zum Hinzufügen zum
Repository mit `Yes` beantworten.

#### 6.1.1 `.gitignore`-Datei mit Inhalt befüllen
In einem Web-Browser die Webseite https://gitignore.io aufrufen.
In das Eingabefeld den die folgenden Schlüsselworte eingeben wie in der folgenden Abbildung zu sehen
und auf den Button `Create` klicken.

![img_6.png](img_6.png)

Dies ergibt den neuen URL https://www.toptal.com/developers/gitignore/api/csharp,dotnetcore,rider,visualstudio,visualstudiocode,jetbrains,jetbrains+all,jetbrains+iml
und der Inhalt wird im Browser-Fenster angezeigt. Diesen markieren, kopieren und in die gerade angelegte Datei einfügen.

### 6.2 Git-Repository anlegen
Über den Menüpunkt `VCS` oder `Git` den Punkt `Enable Version Control Integration` auswählen und `Git` 
als Versionskontrollsystem auswählen.

![img.png](img.png)

![img_1.png](img_1.png)

Dann sind erst einmal alle Teilprojekte und die Solution rot markiert, da sie noch nicht im Repository sind.

![img_2.png](img_2.png)

### 6.3 Solution und Projekte dem Repository hinzufügen
Nach dieser Aktion re. Mausklick auf die Solution und `Git` -> `Add` auswählen.

![img_3.png](img_3.png)

Danach sind die Projekte und die Solution grün markiert, da sie jetzt im Repository sind.

![img_4.png](img_4.png)


### 6.4 Ersten `Git Commit` durchführen
Jetzt können die Änderungen 0committet werden. Dazu in die `Commit`-Ansicht wechseln.
Den in den Kasten vor dem obersten Knoten `Changes` klicken. Alles zu committen anstehenden Dateien werden markiert
Eine Commit-Message eingeben und auf den Button `Commit` klicken.

![img_7.png](img_7.png)

### 6.5 Ersten `Git Push` durchführen
Dazu den Menüpunkt `Git` -> `Push` auswählen.

![img_8.png](img_8.png)

In dem Dialog `Define Remote`anklicken.

![img_9.png](img_9.png)

#### 6.5.1
In einem Browser zu eurem Gitlab-Repo wechseln, das dem folgen URL-Format entspricht:
https://gitlab-ce.gwdg.de/ausbildung/`euer_Vorname`

Oben rechts die Schaltfläche "Neues Projekt" anklicken. Die Kachel `Leeres Projekt anlegen` anklicken.
Das Projekt benennen durch eingeben eines Namens in dem Eingabefeld `Projektname`. Den Haken vor "Repository mit 
einem README initialisieren" anklicken und somit abwählen. Jetzt auf `Projekt anlegen` klicken.

![img_10.png](img_10.png)

Auf die Schaltfläche `Code` klicken und den URL `Mit HTTPS klonen` kopieren.

![img_11.png](img_11.png)

Zurück in in der IDE `Rider` den gerade kopierten URL in das Eingabefeld `URL:` einfügen im Dialog `Define Remote` und abschließend auf `OK` klicken.

![img_12.png](img_12.png)

Abschließend auf `Push` klicken.

![img_13.png](img_13.png)

### 6.6 Abschließende Information
Ab diesem Zeitpunkt können nun alle geänderten oder neu hinzugefügten Dateien in das Repository aufgenommen werden.
Dazu in der `Commit`-Ansicht den Button `Commit And Push` anklicken, nachdem zuvor die Dateien für den `Git Commit`
markiert und eine `Commit Message` eingegeben wurden.
