# 1.Aktuell
## Core Principles

### I. Didaktische Klarheit (Pedagogical Clarity)

Code MUST prioritize readability and educational value over performance or cleverness.
All source code, comments, and documentation MUST be written in German (the project's target audience
is German-speaking Fachinformatiker trainees). Every compiler phase (Lexer, Parser, Symbol Table,
Code Generator, VM) MUST remain clearly separated and independently comprehensible.
Implementation shortcuts that obscure the learning path are forbidden.

**Rationale**: TinyPl0 is a teaching artefact. A trainee reading the code must be able to trace
the full compilation pipeline step-by-step without expert guidance.

# 2. Änderungen
Hinzufügen/Änderungen zu ### I. Didaktische Klarheit (Pedagogical Clarity)
- Texte zusätzlich in Englisch. Der deutsche Textblock zuerst und danach der englische Textblock.
- die deutsche und englischen Texte sollen dem Sprachniveau B2 (nach Gemeinsamer Europäischer Referenzrahmen für Sprachen – abgekürzt GER bzw. international CEFR (Common European Framework of Reference for Languages). ) entsprechen, da auch nicht-muttersprachliche Auszubildenden an diesem Projekt ausgebildet werden und sollen alles in dem Projekt verstehen können.
- I. Didaktische Klarheit (Pedagogical Clarity) ändern zu I. Didaktische und sprachliche Klarheit (Pedagogical and Linguistic Clarity)
- XML-Kommentare sollen für alle Methoden, Klassen, Variablen  etc. im Quellcode verwendet werden.
- An geeigneten Stellen kann der Quellcode darüber hinaus noch mit Block- oder Zeilen-Kommentaren versehen werden um auf wichtige Didaktische Aspekte hinzuweisen. Auch diese in Deutsch und Englisch.
- All documentation — code, API reference, guides, and examples — MUST serve as learning material for IT application-development specialists (Fachinformatiker Anwendungsentwicklung):
- Every public type, member, parameter, and return value MUST carry complete XML documentation (<summary>, <param>, <returns>, and <exception> where applicable; <remarks> and <example> where instructive).
Comments explain the why (decision, trade-off, constraint), not only the what.
- Missing XML documentation for public API members is treated as a build error (CS1591 MUST NOT be suppressed globally).
- When API signatures or XML comments change, the docfx output MUST be regenerated in the same commit/PR.
- Use CLAUDE.md, GEMINI.md, copilot-instructions.md and AGENTS.md for runtime agent-specific development guidance.
- Es sollte imer nochmal überprüft werden, ob die Dokumentationsrichtlinien eingehalten worden sind. Wenn nicht, sollen die Fehlende Dokumentation nachgeholt werden.
- Bei Änderungen an der Dokumentation soll ein docfx-Lauf mit dem externen Befehl docfx stattfinden. Die docfx.json befindet sich im Projecthauptverzeichnis.

# 3. Erklärung für das Sprachniveau B2
Ein Auszubildender sollte mindestens das Sprachniveau B1, besser B2, besitzen, um eine Ausbildung zum Fachinformatiker erfolgreich absolvieren zu können. Das ergibt sich nicht aus einer expliziten gesetzlichen Vorgabe, sondern aus den tatsächlichen Anforderungen der Ausbildungsordnung und des Berufsschulunterrichts.
## 3.1🧩 Warum B1 das Minimum ist
Die Ausbildungsordnung für Fachinformatiker verlangt u.a.:
- Sachverhalte präsentieren und kommunizieren, auch mit Kunden
  (z.B. „Kunden informieren sowie Sachverhalte präsentieren und dabei deutsche und englische Fachbegriffe anwenden“)
- Arbeitsaufträge verstehen, analysieren und dokumentieren
- Teamarbeit, Abstimmung, Problemlösung
- Berufsschulunterricht in Fächern wie Wirtschaft, Sozialkunde, Informatik, Englisch
  Diese Tätigkeiten setzen voraus, dass ein Auszubildender zusammenhängende Texte verstehen, sich mündlich verständigen und schriftlich klar ausdrücken kann – typische Merkmale des Niveaus B1.

## 3.2🧠 Warum B2 deutlich besser ist
In der Praxis zeigt sich, dass Auszubildende mit B2:
- komplexe technische Inhalte schneller erfassen
- Projektdokumentationen sicherer schreiben
- Prüfungsaufgaben (insbesondere die schriftlichen) besser bewältigen
- im Kundenkontakt souveräner auftreten
- weniger sprachbedingte Lernhürden haben
  Gerade die Abschlussprüfung enthält viele textlastige Aufgaben, die ein gutes Sprachverständnis erfordern.

## 3.3📘 Anforderungen der Berufsschule
Berufsschulen orientieren sich in der Regel an B1–B2, weil:
- Fachtexte gelesen und verstanden werden müssen
- Präsentationen und Projektberichte verlangt werden
- Englisch als Fachsprache genutzt wird
- Prüfungen sprachlich anspruchsvoll sind
## 3.4 Empfehlung für die Praxis
- B1 → Ausbildung ist möglich, aber der Azubi braucht oft zusätzliche Sprachförderung.
- B2 → Idealer Startpunkt für eine erfolgreiche Ausbildung ohne größere sprachliche Hürden.
- C1 → Vorteilhaft, aber nicht notwendig.

## 4. Unit Tests
- Unit Tests sollen möglichst TDD-konform von Rot nach Grün entwickelt werden. 
- Bei allen neuen Features sollen möglichst zuerst die Tests erstellt werden und kompiliert werden. Das ergibt rot. Danach soll gegen die Tests entwickelt werden, so dass dieses nach und ach alle grün werden. Das zeigt den Auszubildenden, wie Test Driven Defelopment durchgeführt werden soll.
-

---

## Spec-Kit-Intake-Reife / Spec Kit Intake Readiness

Dieses Lastenheft ist als Eingabedatei fuer einen spaeteren `/speckit-specify`-Lauf vorgesehen. Vor dem Start muss der aktuelle Repository-Stand geprueft werden, damit bereits erledigte oder ueberholte Punkte nicht erneut umgesetzt werden.

*This requirements document is intended as input for a later `/speckit-specify` run. Before starting, check the current repository state so already completed or superseded items are not implemented again.*

Der spaetere Lauf muss mindestens klassifizieren:

- `Applicable`: gilt fuer diesen Lauf und braucht Umsetzung oder Evidenz.
- `AlreadySatisfied`: ist im aktuellen Stand bereits nachweisbar erledigt.
- `N/A`: gilt fuer diesen Lauf nicht und braucht eine kurze Begruendung.
- `Open`: gilt, ist aber noch nicht ausreichend geklaert oder belegt.
- `FollowUp`: fachlich relevant, aber nicht Teil dieses Laufs.

## Kopierbarer `/speckit-specify`-Prompt / Copyable `/speckit-specify` Prompt

```text
/speckit-specify Nutze Lastenheft_Constitution_Change.md als verbindliche Eingabedatei. Erstelle die Feature-Spezifikation fuer einen Governance- und Constitution-Abgleich im Repository InventarWorkerService.

Ziel: Pruefe das Lastenheft gegen den aktuellen Repository-Stand und erstelle eine belastbare Spec-Kit-Spezifikation, die fuer Auszubildende, Entwickler*innen, Reviewer und KI-Agenten nachvollziehbar ist.

Pflichtpunkte:
- Lies dieses Lastenheft vollstaendig und uebernehme vorhandene Anforderungen, Scope-Grenzen, Reihenfolgehinweise und Akzeptanzkriterien.
- Pruefe, welche Punkte bereits umgesetzt, ueberholt oder noch offen sind.
- Klassifiziere Anforderungen als `Applicable`, `AlreadySatisfied`, `N/A`, `Open` oder `FollowUp`.
- Plane nur `Applicable`-Punkte fuer diesen Lauf.
- Dokumentiere fuer `N/A` und `FollowUp` jeweils eine kurze Begruendung.
- Beachte `constitution.md`, `.specify/memory/constitution.md`, AGENTS/CLAUDE/GEMINI/Copilot-Guidance, installierte Spec-Kit-Presets, Secure-Development-Basis, A11Y-Regeln, CEFR-B2-Verstaendlichkeit und didaktische Kommentar-Governance.
- Starte keinen weiteren Lastenheft-Lauf und kombiniere mehrere Lastenhefte nur, wenn die Kopplung fachlich begruendet und dokumentiert ist.

Erzeuge eine Spezifikation mit Scope, Nicht-Zielen, Anforderungen, Abhaengigkeiten, Akzeptanzkriterien, Risiken, Teststrategie, Evidenzpfaden und offenen Folgepunkten.
```
