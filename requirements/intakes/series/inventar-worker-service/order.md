# Lastenheft-Abarbeitungsreihenfolge / Requirements Processing Order

Diese Datei haelt die sichtbare Abarbeitungsreihenfolge der vorhandenen Lastenhefte fest. Sie ist eine Vorbereitung fuer spaetere Spec-Kit-Laeufe und startet selbst keinen Lauf.

*This file records the visible processing order of existing requirements documents. It prepares later Spec Kit runs and does not start a run by itself.*

<!-- secure-development-hardening-order:start -->
## Verlinkte Lastenheft-Reihenfolge / Linked Requirements Order

Diese Tabelle wird aus dem kanonischen Series-Manifest und ausdruecklicher Feature-Evidence erzeugt. Vollstaendige Dateinamen, direkte eingehende Kanten und sichtbare Positionen bleiben erhalten. Manuelle Abschnitte ausserhalb dieses Markers bleiben unberuehrt.

*This table is generated from the canonical series manifest and explicit feature evidence. Complete filenames, direct incoming edges, and visible positions are preserved. Manual sections outside this marker remain unchanged.*

| Position | Status | Lastenheft/Intake | Abhängigkeiten / Dependencies | Spec-Kit-Feature |
|---:|---|---|---|---|
| 1 | Completed | [Lastenheft_Secure-Development-Hardening.002-secure-development-hardening.md](../../../../Lastenheft_Secure-Development-Hardening.002-secure-development-hardening.md) | — (Root / keine direkte Abhängigkeit) | [002-secure-development-hardening](../../../../specs/002-secure-development-hardening/) |
| 2 | Eligible | [Lastenheft_Sandbox-gestuetzte-Secure-Development-Haertung.md](../../../../Lastenheft_Sandbox-gestuetzte-Secure-Development-Haertung.md) | [Lastenheft_Secure-Development-Hardening.002-secure-development-hardening.md](../../../../Lastenheft_Secure-Development-Hardening.002-secure-development-hardening.md) → current (`HardCompletionGate`, binding: true) | — (kein Spec-Kit-Feature / no Spec Kit feature) |
| 3 | Blocked | [Lastenheft_Didactic-Inline-Code-Comment-Hardening.md](../../../../Lastenheft_Didactic-Inline-Code-Comment-Hardening.md) | [Lastenheft_Sandbox-gestuetzte-Secure-Development-Haertung.md](../../../../Lastenheft_Sandbox-gestuetzte-Secure-Development-Haertung.md) → current (`HardCompletionGate`, binding: true) | — (kein Spec-Kit-Feature / no Spec Kit feature) |
| 4 | Blocked | [Lastenheft_TG_Elmish_Entscheidung.md](../../../../Lastenheft_TG_Elmish_Entscheidung.md) | [Lastenheft_Didactic-Inline-Code-Comment-Hardening.md](../../../../Lastenheft_Didactic-Inline-Code-Comment-Hardening.md) → current (`HardCompletionGate`, binding: true) | — (kein Spec-Kit-Feature / no Spec Kit feature) |
| 5 | Blocked | [Lastenheft_TG_Migration_InventarViewerApp.md](../../../../Lastenheft_TG_Migration_InventarViewerApp.md) | [Lastenheft_TG_Elmish_Entscheidung.md](../../../../Lastenheft_TG_Elmish_Entscheidung.md) → current (`HardCompletionGate`, binding: true) | — (kein Spec-Kit-Feature / no Spec Kit feature) |
| 6 | Blocked | [Lastenheft_TG_Migration_CtrlWorkerServiceCmdlet.md](../../../../Lastenheft_TG_Migration_CtrlWorkerServiceCmdlet.md) | [Lastenheft_TG_Migration_InventarViewerApp.md](../../../../Lastenheft_TG_Migration_InventarViewerApp.md) → current (`HardCompletionGate`, binding: true) | — (kein Spec-Kit-Feature / no Spec Kit feature) |
| 7 | Blocked | [Lastenheft_TG_Migration_CtrlWorkerServiceApp.md](../../../../Lastenheft_TG_Migration_CtrlWorkerServiceApp.md) | [Lastenheft_TG_Migration_CtrlWorkerServiceCmdlet.md](../../../../Lastenheft_TG_Migration_CtrlWorkerServiceCmdlet.md) → current (`HardCompletionGate`, binding: true) | — (kein Spec-Kit-Feature / no Spec Kit feature) |
| 8 | Blocked | [Lastenheft_A11Y_TUI_API.md](../../../../Lastenheft_A11Y_TUI_API.md) | [Lastenheft_TG_Migration_CtrlWorkerServiceApp.md](../../../../Lastenheft_TG_Migration_CtrlWorkerServiceApp.md) → current (`HardCompletionGate`, binding: true) | — (kein Spec-Kit-Feature / no Spec Kit feature) |
| 9 | Blocked | [Lastenheft_Statistik_View_Lesemethoden.md](../../../../Lastenheft_Statistik_View_Lesemethoden.md) | [Lastenheft_A11Y_TUI_API.md](../../../../Lastenheft_A11Y_TUI_API.md) → current (`HardCompletionGate`, binding: true) | — (kein Spec-Kit-Feature / no Spec Kit feature) |
| 10 | Blocked | [Lastenheft_IDbService_Interface.md](../../../../Lastenheft_IDbService_Interface.md) | [Lastenheft_Statistik_View_Lesemethoden.md](../../../../Lastenheft_Statistik_View_Lesemethoden.md) → current (`HardCompletionGate`, binding: true) | — (kein Spec-Kit-Feature / no Spec Kit feature) |
| 11 | Blocked | [Lastenheft_MongoDB_Paritaet.md](../../../../Lastenheft_MongoDB_Paritaet.md) | [Lastenheft_IDbService_Interface.md](../../../../Lastenheft_IDbService_Interface.md) → current (`HardCompletionGate`, binding: true) | — (kein Spec-Kit-Feature / no Spec Kit feature) |
| 12 | Blocked | [Lastenheft_RL-SE-Checklist-Selbstpruefung.md](../../../../Lastenheft_RL-SE-Checklist-Selbstpruefung.md) | [Lastenheft_MongoDB_Paritaet.md](../../../../Lastenheft_MongoDB_Paritaet.md) → current (`AssessmentBaseline`, binding: true) | — (kein Spec-Kit-Feature / no Spec Kit feature) |
| 13 | Blocked | [Lastenheft_GSDB-Spec-Kit-Intensivpruefung.md](../../../../Lastenheft_GSDB-Spec-Kit-Intensivpruefung.md) | [Lastenheft_RL-SE-Checklist-Selbstpruefung.md](../../../../Lastenheft_RL-SE-Checklist-Selbstpruefung.md) → current (`FinalAuditInput`, binding: true) | — (kein Spec-Kit-Feature / no Spec Kit feature) |
| 14 | Completed | [Lastenheft_Constitution_Change.md](../../../../Lastenheft_Constitution_Change.md) | — (Root / keine direkte Abhängigkeit) | — (kein Spec-Kit-Feature / no Spec Kit feature) |
| 15 | Completed | [Lastenheft_TerminalGui_Migration.md](../../../../Lastenheft_TerminalGui_Migration.md) | — (Root / keine direkte Abhängigkeit) | — (kein Spec-Kit-Feature / no Spec Kit feature) |
| 16 | Completed | [Lastenheft_SQLite_ViewQuery_Bugfix.md](../../../../Lastenheft_SQLite_ViewQuery_Bugfix.md) | — (Root / keine direkte Abhängigkeit) | — (kein Spec-Kit-Feature / no Spec Kit feature) |
| 17 | Completed | [Lastenheft_PostgreSQL_Implementation.001-pgsql-paritaet.md](../../../../Lastenheft_PostgreSQL_Implementation.001-pgsql-paritaet.md) | — (Root / keine direkte Abhängigkeit) | — (kein Spec-Kit-Feature / no Spec Kit feature) |
<!-- secure-development-hardening-order:end -->
