# InventarWorkerService Integration Tests

Dieses Projekt enthält Integration-Tests für den InventarWorkerService basierend auf den `.http`-Dateien.

## Voraussetzungen

### Playwright Installation
Führen Sie nach dem ersten Build aus:
```bash
pwsh bin/Debug/net9.0/playwright.ps1 install
```
### Service läuft
Stellen Sie sicher, dass der InventarWorkerService unter
> http://localhost:5000

läuft.

## Test-Kategorien

### InventarWorkerServiceApiTests
* Service Status Tests
* Hardware/Software Inventar Tests
* Komplettes Inventar Tests
* Custom Header Tests
* Swagger Dokumentation Tests
* Performance Tests

### InventarWorkerServiceRemoteTests
* Remote Server Tests (192.168.1.100:5000)
* Tests verwenden `Assert.Inconclusive()` wenn Remote-Server nicht erreichbar

## Ausführung
### Alle Tests ausführen
> dotnet test
### Nur lokale Tests (ohne Remote-Tests)
> dotnet test --filter "TestCategory!=Remote"
### Nur API Tests
> dotnet test --filter "ClassName~ApiTests"

## Konfiguration
* **BaseUrl**: `http://localhost:5000`
* **RemoteUrl**: `http://192.168.1.100:5000` (anpassbar)
* **Timeout**: 10 Sekunden für Remote-Tests

## Die erstellten Tests decken alle Szenarien aus dem `.http`-File ab:
1. **Grundlegende API-Tests**: Status, Hardware, Software, vollständiges Inventar
2. **Verschiedene IP-Adressen**: localhost, 127.0.0.1, Remote-IP
3. **Custom Headers**: User-Agent, X-Request-ID mit Timestamp
4. **Swagger-Tests**: HTML-Dokumentation und JSON-API-Spec
5. **Performance-Tests**: Mehrere parallele Anfragen
6. **Remote-Tests**: Separate Testklasse für Remote-Server

## Die Tests verwenden **MSTest.Playwright** für HTTP-API-Tests und enthalten:
* Proper Setup/Cleanup
* JSON-Validierung
* Error Handling für nicht erreichbare Remote-Server
* Konfigurierbare Timeouts
* Aussagekräftige Fehlermeldungen
