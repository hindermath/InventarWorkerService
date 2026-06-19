# Lastenheft-Abarbeitungsreihenfolge / Requirements Processing Order

Stand: 2026-06-19

## Zweck / Purpose

Diese Datei haelt die sinnvolle Abarbeitungsreihenfolge der vorhandenen
Lastenhefte fest. Sie ist eine Vorbereitung fuer spaetere Spec-Kit-Laeufe und
startet selbst keinen Lauf. Die Reihenfolge ist so gewaehlt, dass
Governance-Baselines vor grossen Codeaenderungen geklaert werden, danach die
Terminal.Gui-Migration gebuendelt laeuft und datenbanknahe Erweiterungen erst
nach den stabilisierenden Vorarbeiten folgen.

This file records the proposed processing order for the existing requirements
files. It prepares later Spec Kit runs and does not start a run by itself. The
order puts governance baselines before large code changes, then groups the
Terminal.Gui migration work, and schedules database-facing extensions after
the stabilizing preparation work.

## Aktive Reihenfolge / Active Order

| Rang | Lastenheft | Naechster Status / Next Status | Begruendung / Rationale |
|---:|---|---|---|
| 1 | `Lastenheft_Secure-Development-Hardening.md` | Als erstes spezifizieren / specify first | Sicherheits-, Architektur- und Evidenzregeln bilden die Basis fuer alle folgenden Laeufe.<br>Security, architecture, and evidence rules are the baseline for all later runs. |
| 2 | `Lastenheft_Didactic-Inline-Code-Comment-Hardening.md` | Nach Security-Hardening spezifizieren / specify after security hardening | Didaktische Kommentarregeln sollten vor den groesseren Migrations- und Datenbankaenderungen stabil sein.<br>Didactic comment rules should be stable before larger migration and database changes. |
| 3 | `Lastenheft_TG_Elmish_Entscheidung.md` | Vor Terminal.Gui-Codeaenderungen entscheiden / decide before Terminal.Gui code changes | Die Elmish-Entscheidung beeinflusst alle Terminal.Gui-v2-Migrationslaeufe und sollte nicht mehrfach getroffen werden.<br>The Elmish decision affects all Terminal.Gui v2 migration runs and should not be made repeatedly. |
| 4 | `Lastenheft_TG_Migration_InventarViewerApp.md` | Erste operative TUI-Migration / first operative TUI migration | Der Viewer ist die fachlich komplexeste TUI-Flaeche und liefert Muster fuer MainLoop-, Dialog- und Testanpassungen.<br>The viewer is the most complex TUI surface and creates patterns for MainLoop, dialog, and test changes. |
| 5 | `Lastenheft_TG_Migration_CtrlWorkerServiceCmdlet.md` | Nach Viewer-Muster migrieren / migrate after viewer pattern | Das Cmdlet hat PowerShell- und Terminal.Gui-Lebenszyklusgrenzen, profitiert aber von den Viewer-Erkenntnissen.<br>The cmdlet has PowerShell and Terminal.Gui lifecycle boundaries, but benefits from the viewer findings. |
| 6 | `Lastenheft_TG_Migration_CtrlWorkerServiceApp.md` | Letzte operative TUI-Migration / final operative TUI migration | Die Service-App ist kleiner und kann die vorher festgelegten v2- und Elmish-Entscheidungen uebernehmen.<br>The service app is smaller and can reuse the v2 and Elmish decisions from the earlier runs. |
| 7 | `Lastenheft_A11Y_TUI_API.md` | Nach TUI-Migration spezifizieren / specify after TUI migration | A11Y-Pruefungen fuer TUI und API sind belastbarer, wenn die Ziel-TUI-API bereits stabilisiert ist.<br>A11y checks for TUI and API are more reliable after the target TUI API is stabilized. |
| 8 | `Lastenheft_Statistik_View_Lesemethoden.md` | Vor Interface-Schnitt spezifizieren / specify before interface cut | Die relationalen View-Lesemethoden erweitern die tatsaechliche Service-Oberflaeche und sollten vor dem Interface feststehen.<br>The relational view read methods extend the actual service surface and should be known before the interface is cut. |
| 9 | `Lastenheft_IDbService_Interface.md` | Nach gereiftem relationalem Umfang spezifizieren / specify after relational scope matures | Das Interface sollte die bereinigten SQLite/PostgreSQL- und Statistik-Lesepfade abbilden, nicht einen Zwischenstand.<br>The interface should reflect the cleaned SQLite/PostgreSQL and statistics read paths, not an intermediate state. |
| 10 | `Lastenheft_MongoDB_Paritaet.md` | Nach relationalem Interface pruefen / review after relational interface | MongoDB-Paritaet ist ein eigener Backend-Schnitt und sollte erst nach relationaler API-Klaerung geschnitten werden.<br>MongoDB parity is its own backend boundary and should be scoped after the relational API is clear. |

## Abgeschlossen oder nicht operativ / Completed or Non-Operative

| Lastenheft | Einordnung / Classification | Begruendung / Rationale |
|---|---|---|
| `Lastenheft_PostgreSQL_Implementation.001-pgsql-paritaet.md` | Abgeschlossen und branch-suffig archiviert / completed and archived with branch suffix | Der Lauf `001-pgsql-paritaet` ist umgesetzt und in `specs/001-pgsql-paritaet/` dokumentiert.<br>The `001-pgsql-paritaet` run is implemented and documented in `specs/001-pgsql-paritaet/`. |
| `Lastenheft_SQLite_ViewQuery_Bugfix.md` | Bereits im aktuellen Code geloest / already resolved in current code | Die betroffenen View-Abfragen verwenden im aktuellen `SqliteDbService` die View-Namen mit `ORDER BY Name`.<br>The affected view queries in the current `SqliteDbService` use the view names with `ORDER BY Name`. |
| `Lastenheft_Constitution_Change.md` | Durch aktuelle Governance ueberholt / superseded by current governance | Die Kernpunkte sind durch Constitution, Agent-Guidance und Spec-Kit-Preset-Governance aktueller abgedeckt.<br>The main points are covered more up to date by the constitution, agent guidance, and Spec Kit preset governance. |
| `Lastenheft_TerminalGui_Migration.md` | Uebersicht, kein einzelner Lauf / overview, not a single run | Die operative Arbeit ist in Entscheidung und drei konkrete TUI-Migrationslastenhefte aufgeteilt.<br>The operative work is split into the decision file and three concrete TUI migration requirements files. |

## Nutzungsregel / Usage Rule

- Vor einem spaeteren Spec-Kit-Lauf wird das erste aktive, noch nicht
  abgearbeitete Lastenheft aus der Tabelle verwendet.
- Ein Lauf soll nur dann mehrere Lastenhefte zusammenfassen, wenn die Kopplung
  fachlich begruendet und vor dem Start dokumentiert ist.
- Nach Abschluss eines dedizierten Feature-Branches wird das gelieferte
  Lastenheft gemaess Repository-Regel mit Branch-Suffix umbenannt:
  `Lastenheft_<Thema>.<feature-branch>.md`.
- Wenn sich Status oder Reihenfolge aendern, wird diese Datei vor dem naechsten
  Spec-Kit-Lauf aktualisiert.

- Before a later Spec Kit run, use the first active requirements file that has
  not yet been processed.
- A run should combine multiple requirements files only when the coupling is
  justified by the domain and documented before the start.
- After a dedicated feature branch is completed, rename the delivered
  requirements file according to the repository rule:
  `Lastenheft_<Topic>.<feature-branch>.md`.
- If status or order changes, update this file before the next Spec Kit run.

## Pflegepruefung / Maintenance Check

Jedes der aktuell bekannten vierzehn Lastenhefte steht in dieser Datei genau
einmal in der aktiven Reihenfolge oder in der Status-Tabelle. Neue Lastenhefte
werden ergaenzt, sobald sie als spaeterer Spec-Kit-Input vorgesehen sind.

Each of the fourteen currently known requirements files appears exactly once in
the active order or in the status table. Add new requirements files as soon as
they are intended as input for a later Spec Kit run.


<!-- secure-development-hardening-order:start -->
## Automatisch ermittelte Lastenheft-Reihenfolge / Automatically Detected Requirements Order

Diese Tabelle wird aus `Lastenheft*.md` im Repository-Root erzeugt. Sie ist eine Vorbereitung fuer spaetere Spec-Kit-Laeufe und startet selbst keinen Lauf. Manuelle Projektentscheidungen ausserhalb dieses markierten Abschnitts bleiben erhalten.

*This table is generated from `Lastenheft*.md` in the repository root. It prepares later Spec Kit runs and does not start a run. Manual project decisions outside this marked section remain preserved.*

| Rang | Lastenheft | Gruppe | Status |
|---:|---|---|---|
| 1 | `Lastenheft_Constitution_Change.md` | Governance/Baseline | aktiv / active |
| 2 | `Lastenheft_TerminalGui_Migration.md` | Migration/Tooling | aktiv / active |
| 3 | `Lastenheft_TG_Migration_CtrlWorkerServiceApp.md` | Migration/Tooling | aktiv / active |
| 4 | `Lastenheft_TG_Migration_CtrlWorkerServiceCmdlet.md` | Migration/Tooling | aktiv / active |
| 5 | `Lastenheft_TG_Migration_InventarViewerApp.md` | Migration/Tooling | aktiv / active |
| 6 | `Lastenheft_IDbService_Interface.md` | Kernlogik/Runtime | aktiv / active |
| 7 | `Lastenheft_MongoDB_Paritaet.md` | Kernlogik/Runtime | aktiv / active |
| 8 | `Lastenheft_PostgreSQL_Implementation.001-pgsql-paritaet.md` | Kernlogik/Runtime | archiviert oder abgeschlossen / archived or completed |
| 9 | `Lastenheft_SQLite_ViewQuery_Bugfix.md` | Kernlogik/Runtime | aktiv / active |
| 10 | `Lastenheft_A11Y_TUI_API.md` | UI/A11Y/Dokumentation | aktiv / active |
| 11 | `Lastenheft_Didactic-Inline-Code-Comment-Hardening.md` | UI/A11Y/Dokumentation | aktiv / active |
| 12 | `Lastenheft_Secure-Development-Hardening.md` | Secure-Development-Hardening | aktiv / active |
| 13 | `Lastenheft_Statistik_View_Lesemethoden.md` | Weitere Anforderungen | aktiv / active |
| 14 | `Lastenheft_TG_Elmish_Entscheidung.md` | Weitere Anforderungen | aktiv / active |
<!-- secure-development-hardening-order:end -->
