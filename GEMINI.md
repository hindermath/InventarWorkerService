# GEMINI.md - Projektkontext für InventarWorkerService

Dieses Dokument dient als zentrale Orientierungshilfe für die Arbeit an diesem Repository. Es ergänzt die `README.md` und `CLAUDE.md`.

## 🚀 Projektübersicht
**InventarWorkerService** ist eine plattformübergreifende Inventarisierungslösung für IT-Infrastrukturen, entwickelt mit **.NET 10.0** und **C# 14.0**. Das System erfasst Hardware- und Software-Informationen von Windows-, macOS- und Linux-Systemen.

### Kernkomponenten:
1.  **InventarWorkerService**: Ein ASP.NET Core "Agent", der auf jedem zu überwachenden Rechner läuft. Er erfasst lokale Daten und stellt sie über eine REST-API bereit.
2.  **HarvesterWorkerService**: Der zentrale Dienst, der die Agenten abfragt und die Daten in **SQLite**, **MongoDB** oder **PostgreSQL** konsolidiert.
3.  **InventarViewerApp**: Eine interaktive Terminal-Benutzeroberfläche (TUI) basierend auf `Terminal.Gui` zur Anzeige und Verwaltung des Inventars.
4.  **InventarWorkerCommon**: Die zentrale Bibliothek mit Domänenmodellen, Datenbank-Services und API-Logik.
5.  **Steuerungstools**: Verschiedene Projekte (`CtrlWorker...`) zur Verwaltung der Dienste als Windows-Service oder via PowerShell.

## 🛠 Build & Run
Das Projekt nutzt die Standard .NET-CLI.

- **Gesamte Lösung bauen**: `dotnet build InventarWorkerService.sln`
- **Agent starten**: `dotnet run --project InventarWorkerService/InventarWorkerService.csproj`
- **Sammler starten**: `dotnet run --project HarvesterWorkerService/HarvesterWorkerService.csproj`
- **TUI-App starten**: `dotnet run --project InventarViewerApp/InventarViewerApp.csproj`
- **Coverage messen (CI-Grenze >=70%, Ziel >=80%)**: `dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults`
- **Veraltete NuGet-Pakete prüfen**: `dotnet list package --outdated`
- **Dokumentation neu erzeugen (bei API/XML-Doku-Änderungen)**: `docfx docfx.json`

## 🧪 Testing
- **Alle Tests**: `dotnet test`
- **Unit Tests**: `dotnet test InventarWorkerCommonTest/InventarWorkerCommonTest.csproj`
- **Integration Tests**: Erfordern einen laufenden Agenten auf Port 5000.
  `dotnet test InventarWorkerServiceIntegrationTest/InventarWorkerServiceIntegrationTest.csproj`

## 🌿 Branching-Workflow (Verbindlich)
- Der Branch `main` ist geschützt und darf nicht direkt für Feature-Commits genutzt werden.
- Für jedes Feature/Fix muss ein neuer Branch erstellt werden.
- Änderungen gelangen ausschließlich per Pull Request nach `main` (inkl. Testnachweis).

## 🏗 Architektur & Konventionen
- **Plattform-Support**: Implementiert als Windows Service, systemd (Linux) und launchd (macOS).
- **Datenhaltung**: 
  - Primär: **SQLite** (via Dapper).
  - Sekundär (Harvester): **MongoDB** & **PostgreSQL**.
- **API**: System.Text.Json (camelCase), RestSharp für Clients. Swagger/OpenAPI ist in Development unter `/swagger` verfügbar.
- **Sprache**: 
  - Erklärende Texte in Kommentaren/Dokumentation: zweisprachig (Deutsch zuerst, dann Englisch) auf CEFR-B2-Niveau.
  - UI-Labels & Logs: Deutsch.
- **Coding Style**:
  - Toolchain-Basis: `.NET 10` und `C# 14.0`.
  - Nullable Reference Types sind aktiviert.
  - Asynchrone Programmierung (`async/await`) ist Standard für I/O.
  - Test-Namensschema: `<UnitUnderTest>_<Scenario>_<ExpectedOutcome>`.
  - Testabdeckung in CI: mindestens 70%, Zielbereich ab 80%.
  - NuGet-Pakete auf aktuellem stabilen Stand halten; Ausnahmen dokumentieren.
  - XML-Dokumentation ist für öffentliche APIs verpflichtend (CS1591 nicht global unterdrücken).
  - Für nicht-öffentliche Member/Variablen sind an didaktisch relevanten Stellen zweisprachige Block- oder Zeilen-Kommentare zu nutzen.
  - Bei API- oder XML-Doku-Änderungen `docfx docfx.json` ausführen.
- **Worker-Intervalle**:
  - Debug: 30 Sekunden.
  - Release: 24 Stunden.

## 📁 Wichtige Verzeichnisse
- `InventarWorkerCommon/Models/`: Domänenmodelle (Hardware, Software, SQL, etc.).
- `InventarWorkerCommon/Services/`: Geschäftslogik für DB, API und Inventarisierung.
- `api/`: Enthält statische API-Dokumentation (DocFX).
- `docs/`: Zusätzliche Dokumentation und PDFs.

## 👤 Benutzer-Präferenzen (Thorsten)
- **IDE**: JetBrains Rider.
- **Shell**: Bevorzugt PowerShell (plattformübergreifend).
- **Datenbanken**: SQLite, PostgreSQL, MongoDB.
- **Interessen**: C64-Entwicklung (cc65, ACME), VST3-Plugins, Arbeitsrecht (Betriebsrat).
- **Kommunikation**: Deutsch (Du-Form).
