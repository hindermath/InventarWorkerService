# Agent-Paritätscheckliste / Agent Parity Checklist

## Umfang / Scope

- Änderung / Change: Feature 002 prüft den aktuellen Zwölfer-Flottenvertrag,
  ohne die historische Standard-Achtermatrix umzudeuten. / Feature 002 verifies
  the current managed twelve-preset contract without rewriting the historical
  standard eight-preset matrix.
- Reviewer: Agent-Parity-Reviewer / Agent parity reviewer
- Datum / Date: 2026-08-30
- Referenz / Reference: lokaler Feature-Head
  `dc15bc4812245e71c1a5976b8241c7aeb518d4a9` / local feature head

## Gepflegte Agentenflächen / Maintained Agent Surfaces

| Fläche / Surface | Ergebnis / Result | Evidenz / Evidence |
|---|---|---|
| `AGENTS.md` | AlreadySatisfied | Gemeinsame Sicherheits-, Architektur-, A11Y-, Statistik- und Zwölfer-Flottenregel vorhanden. / Shared security, architecture, A11Y, statistics, and managed-twelve rule present. |
| `CLAUDE.md` | AlreadySatisfied | Paritätstest grün; keine finding-bedingte Änderung. / Parity test green; no finding-conditioned edit. |
| `GEMINI.md` | AlreadySatisfied | Paritätstest grün; keine finding-bedingte Änderung. / Parity test green; no finding-conditioned edit. |
| `.github/copilot-instructions.md` | AlreadySatisfied | Paritätstest grün; keine finding-bedingte Änderung. / Parity test green; no finding-conditioned edit. |
| `.github/agents/copilot-instructions.md` | AlreadySatisfied | Paritätstest grün; keine finding-bedingte Änderung. / Parity test green; no finding-conditioned edit. |

## Constitution und Templates / Constitution and Templates

| Ort / Location | Ergebnis / Result | Begründung / Rationale |
|---|---|---|
| `constitution.md` | AlreadySatisfied | Aktive Regeln und historische Einführung bleiben unterscheidbar. / Active rules and historical introduction remain distinguishable. |
| `.specify/memory/constitution.md` | AlreadySatisfied | Automatischer Paritätstest ist grün. / Automated parity test is green. |
| Agent-, Spec-, Plan- und Task-Templates | AlreadySatisfied | Keine aktuelle semantische Abweichung belegt; T087 ist bedingt und erzeugt keine künstliche Änderung. / No current semantic deviation was found; T087 is conditional and creates no artificial edit. |
| `scripts/config/spec-kit-governance-presets.json` | AlreadySatisfied | Kanonische Standard-Achtermatrix; historisch und weiterhin gültig. / Canonical standard eight-preset matrix; historical and still valid. |
| `scripts/config/spec-kit-model-routing-governance-presets.json` | AlreadySatisfied | Exaktes verwaltetes Zwölferprofil für dieses Repository. / Exact managed twelve-preset profile for this repository. |
| `.specify/presets/.registry` | AlreadySatisfied | Exakt zwölf aktive Presets mit Version und Priorität. / Exactly twelve enabled presets with version and priority. |

## Absichtliche Abgrenzungen / Intentional Boundaries

DE: Aussagen über sechs oder acht Presets beschreiben historische
Katalog-/Standardprofile. Sie widersprechen nicht dem aktuell installierten
Zwölfer-Flottenprofil. Der Default-Aufruf des Installationsprüfers verwendet
die Achtermatrix und schlägt deshalb bei diesem verwalteten Repository bewusst
fehl. Der gültige Check nennt
`-PresetConfig scripts/config/spec-kit-model-routing-governance-presets.json`
und bestätigt zwölf Presets. Konkrete Provider-Modellnamen fehlen absichtlich;
Modelle werden lokal erkannt und nicht in Governance-Dateien festgeschrieben.

EN: Statements about six or eight presets describe historical catalog or
standard profiles. They do not contradict the currently installed managed
twelve-preset profile. The install checker's default call uses the eight-preset
matrix and therefore intentionally fails for this managed repository. The
valid check supplies
`-PresetConfig scripts/config/spec-kit-model-routing-governance-presets.json`
and confirms twelve presets. Concrete provider model names are intentionally
absent; models are discovered locally and are not pinned in governance files.

## Verifikation / Verification

- [x] Fünf Agentenflächen und beide Constitutions seitengleich verglichen. / Five agent surfaces and both constitutions compared side by side.
- [x] `python3 -m unittest scripts/tests/test_spec_kit_agent_surface_parity.py` — drei Tests grün. / three tests green.
- [x] Zwölferprofil mit `install-spec-kit-governance-presets.ps1 -CheckOnly` und expliziter Profilmatrix grün. / managed twelve profile green with explicit profile matrix.
- [x] `specify preset list` zeigt exakt zwölf aktive Presets. / shows exactly twelve enabled presets.
- [x] Deutsch zuerst, Englisch danach und Pfade geprüft. / German first, English second, and paths verified.

## Folgepflichten / Follow-Up

DE: Keine lokale Governance-Reparatur ist nötig. Neu geprüft wird bei einer
Preset-Version, Priorität, Agentenfläche, Constitution- oder Templateänderung.

EN: No local governance repair is required. Re-check after a preset version,
priority, agent surface, constitution, or template change.
