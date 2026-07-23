<!-- intake-authoring:begin -->
# Lastenheft: Didactic Inline Code Comment Hardening fuer InventarWorkerService

**Dokument-Status:** Spec-Kit-Eingabedatei, bereit fuer `/speckit-specify`  
**Erstellt:** 2026-06-05  
**Betrifft:** Service-, API-, Datenbank-, TUI-, Cross-Platform- und Test-Helfer-Flows, soweit sie Lernwert oder Wartungsrisiko besitzen.

## 1. Ziel / Goal

Deutsch:
InventarWorkerService enthaelt mehrere Service-, API-, Datenbank- und TUI-Pfade. XML-Kommentare bleiben die primaere API- und DocFX-Erklaerung. Dieses Lastenheft ergaenzt kurze Code-nahe Kommentare dort, wo Lernende oder Maintainer sonst nicht erkennen, warum ein Service-, API-, Persistenz-, Plattform- oder Test-Proof-Pfad so gebaut wurde.

English:
InventarWorkerService contains several service, API, database, and TUI paths. XML comments remain the primary API and DocFX explanation. This requirements document adds short code-near comments where learners or maintainers would otherwise not see why a service, API, persistence, platform, or test-proof path was built in that way.

## 2. Scope

In Scope:
- REST-/Service-Grenzen und Fehlerpfade;
- Datenbank- und Persistenzentscheidungen fuer SQLite, MongoDB, PostgreSQL und CSV-Export, soweit betroffen;
- Cross-Platform-Hardware-/Software-Inventarisierung und Fallbacks;
- Terminal.Gui-/TUI-Pfade und Bediengrenzen;
- Unit-, Integration- und Playwright-Test-Helfer, wenn sie Proof-Grenzen erklaeren;
- vorhandene Kommentare, die im geprueften Bereich veraltet, trivial oder irrefuehrend sind.

Out of Scope:
- keine Runtime-Verhaltensaenderung;
- keine neue Datenbank-, API- oder TUI-Funktion;
- keine Dependency- oder Architektur-Migration;
- keine flaechenhafte Kommentierung jeder Methode;
- keine DocFX-Regeneration, solange nur `//`- oder `/* */`-Kommentare ohne XML-Kommentar- oder API-Aenderung betroffen sind.

## 3. Kommentar-Intensitaet

- 1 bis 3 Zeilen vor einem nicht-trivialen Block reichen im Regelfall.
- Mehrzeilig nur bei komplexen Service-/Persistenz-/Plattform-/TUI-Flows, Sicherheits-/A11Y-Randbedingungen oder Test-Proof-Pfaden.
- Kommentare erklaeren Warum, Trade-off, Randbedingung, Plattformgrenze oder Proof-Grenze.
- Keine Kommentare, die nur offensichtlichen Code nacherzaehlen.
- German-first/English-second und CEFR-B2 fuer didaktische Erklaerbloecke.

## 4. Review-Modell

- `CommentAdequate`: vorhandene Kommentare reichen.
- `CommentNeeded`: nicht-triviale Logik braucht eine kurze didaktische Erklaerung.
- `NoCommentNeeded`: Code ist selbsterklaerend; ein Kommentar waere Rauschen.
- `UpdateExistingComment`: vorhandener Kommentar ist veraltet oder zu ungenau.
- `FollowUpHardening`: beim Review wurde ein echtes Code-, Test- oder Architekturproblem sichtbar, das nicht in diesen Kommentar-Lauf gehoert.

## 5. Akzeptanzkriterien

- Feature-Evidence dokumentiert gepruefte Dateien oder Flow-Bereiche, Entscheidung, Kommentarbedarf, Aenderung und Follow-up-Grenzen.
- Neue oder geaenderte didaktische Kommentare bleiben kurz und fachlich nuetzlich.
- Veraltete Kommentare in geprueften Bereichen werden aktualisiert oder entfernt.
- Agent-Guidance haelt die Regel fuer kuenftige neue oder geaenderte nicht-triviale Logik fest.
- XML-Kommentar- oder API-Aenderungen ziehen den normalen DocFX-/A11Y-Nachweispfad nach sich.

## 6. Kopierbarer `/speckit-specify`-Prompt

```text
Ersetzter Alt-Prompt: speckit-specify Nutze Lastenheft_Didactic-Inline-Code-Comment-Hardening.md als verbindliche Eingabedatei. Erstelle die Feature-Spezifikation fuer einen didaktischen Inline-Code-Kommentar-Hardening-Lauf in InventarWorkerService.

Ziel: Zentrale Service-, API-, Persistenz-, Cross-Platform-, TUI- und Test-Helfer-Flows muessen fuer Auszubildende und Maintainer besser nachvollziehbar werden. XML-Kommentare bleiben die primaere API-/DocFX-Erklaerung; dieser Lauf ergaenzt nur Code-nahe didaktische Kommentare bei nicht-trivialer Logik.

Wichtig:
- Keine Runtime-Verhaltensaenderung, keine neue Datenbank-/API-/TUI-Funktion, keine Dependency- oder Architektur-Migration und kein globales "jede Methode kommentieren".
- Kommentarintensitaet moderat halten: 1 bis 3 Zeilen vor nicht-trivialen Blocks; mehrzeilig nur bei komplexen Service-/Persistenz-/Plattform-/TUI-Flows, Sicherheits-/A11Y-Randbedingungen oder Test-Proof-Pfaden.
- Kommentare muessen Warum, Trade-off, Randbedingung, Plattformgrenze oder Proof-Grenze erklaeren, nicht triviales Was.
- Review-Modell aufnehmen: `CommentAdequate`, `CommentNeeded`, `NoCommentNeeded`, `UpdateExistingComment`, `FollowUpHardening`.
- Mindestens pruefen: REST-/Service-Grenzen, Datenbank- und Persistenzentscheidungen, Cross-Platform-Inventarisierung, Terminal.Gui-/TUI-Pfade sowie Unit-/Integration-/Playwright-Test-Helfer.
- Wenn XML-Kommentare oder API-Signaturen beruehrt werden, gilt der normale DocFX-/A11Y-Nachweispfad; reine `//`- oder `/* */`-Kommentarhaertung loest keinen DocFX-Zwang aus.
```

---

## Spec-Kit-Intake-Reife / Spec Kit Intake Readiness

Dieses Lastenheft enthaelt bereits einen kopierbaren `/speckit-specify`-Prompt. Vor dem Start muss der aktuelle Repository-Stand trotzdem geprueft werden. Bereits erledigte oder branch-suffig archivierte Punkte werden nicht erneut umgesetzt; offene Punkte werden als `Applicable`, `AlreadySatisfied`, `N/A`, `Open` oder `FollowUp` klassifiziert.

*This requirements document already contains a copyable `/speckit-specify` prompt. Before starting, still check the current repository state. Completed or branch-suffixed archived items are not implemented again; open items are classified as `Applicable`, `AlreadySatisfied`, `N/A`, `Open`, or `FollowUp`.*
<!-- intake-authoring:prompts -->
## Kopierbare Spec-Kit-Prompts / Copy-Ready Spec Kit Prompts

Die folgenden Alternativen starten keinen Lauf automatisch. Der autonome
Prompt ist auf `LocalImplementation` begrenzt und erteilt keine Remote-,
PR-, Merge-, Bypass-, Secret- oder Provider-Berechtigung.

*The alternatives below do not start a run automatically. The autonomous
prompt is limited to `LocalImplementation` and grants no remote,
pull-request, merge, bypass, secret, or provider authority.*

### Specify

<!-- spec-kit-command-id: speckit.specify -->
```text
$speckit-specify Use Lastenheft_Didactic-Inline-Code-Comment-Hardening.md as the binding intake. Preserve its scope, non-goals, ordering, governance, evidence, and acceptance criteria. Create or update only the matching feature specification. Do not implement, commit, push, create a pull request, merge, or start another feature.
```

### Autonomous

<!-- spec-kit-command-id: speckit.autonomous -->
```text
$speckit-autonomous Execute one complete autonomous Spec Kit run using Lastenheft_Didactic-Inline-Code-Comment-Hardening.md as the binding intake. Delivery mode: LocalImplementation. Preserve all scope, ordering, security, accessibility, evidence, and acceptance boundaries. Do not push, create or merge a pull request, use bypass authority, expose secrets, or start a follow-up feature.
```
<!-- intake-authoring:end -->
