Die Tests beinhalten:
**Hauptfeatures:**
- `TestInitialize` und `TestCleanup` Methoden für Setup und Teardown
- Tests für Konstruktor-Validierung
- Tests für Service Start/Stop Funktionalität
- Plattformspezifische Tests
- Exception-Handling Tests

**Wichtige Hinweise:**
- Die Tests verwenden realistische Servicenamen, erwarten aber Exceptions bei nicht existierenden Services
- Plattformspezifische Tests werden nur auf der entsprechenden Plattform ausgeführt
- Die Tests sind so konzipiert, dass sie auch in CI/CD-Umgebungen laufen können
- Exception-Handling wird getestet, ohne die Testumgebung zu beeinträchtigen

Du kannst diese Tests nach Bedarf anpassen, je nachdem welche spezifischen Aspekte deiner Implementierung du testen möchtest.
