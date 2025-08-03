# Erklärung der MSTest-Attribute
## Auf Testmethoden-Ebene:
* `[TestInitialize]` - Wird vor jedem einzelnen Test ausgeführt
* `[TestCleanup]` - Wird nach jedem einzelnen Test ausgeführt

## Auf Testklassen-Ebene:
* `[ClassInitialize]` - Wird einmal vor allen Tests in der Klasse ausgeführt (muss static sein)
* `[ClassCleanup]` - Wird einmal nach allen Tests in der Klasse ausgeführt (muss static sein)

## Zusätzliche Möglichkeiten:
* `[AssemblyInitialize]` - Wird einmal vor allen Tests in der Assembly ausgeführt
* `[AssemblyCleanup]` - Wird einmal nach allen Tests in der Assembly ausgeführt

## Diese Struktur ermöglicht es Ihnen:
* Testdaten vor jedem Test zu initialisieren
* Ressourcen nach jedem Test aufzuräumen
* Einmalige Setup-Operationen für die gesamte Testklasse durchzuführen
* Debug-Ausgaben zur Nachverfolgung der Testausführung zu verwenden
