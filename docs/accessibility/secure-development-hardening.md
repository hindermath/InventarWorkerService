# Barrierefreiheitsnachweis / Accessibility Evidence: Feature 002

## Umfang / Scope

DE: Geprüft werden die textbasierten Sicherheits-, Architektur-, Lieferketten-
und Governance-Nachweise für Lernende, Maintainer, Reviewer sowie Nutzende von
Screenreadern und Braillezeilen. Basis ist WCAG 2.2 Level AA, soweit auf
Markdown, DocFX-HTML, CLI/TUI und API-JSON anwendbar. Vorwissen: grundlegende
.NET- und Repository-Kenntnisse. Reviewer: Accessibility Reviewer; Datum:
30. August 2026.

EN: The review covers text-based security, architecture, supply-chain, and
governance evidence for learners, maintainers, reviewers, screen-reader users,
and Braille-display users. WCAG 2.2 Level AA is the baseline where applicable
to Markdown, DocFX HTML, CLI/TUI, and API JSON. Prerequisite: basic .NET and
repository knowledge. Reviewer: Accessibility Reviewer; date: 30 August 2026.

## Prüfmatrix / Review Matrix

- Wahrnehmbar / Perceivable: Status, Risiken und Entscheidungen stehen im Text;
  keine Information hängt von Farbe, Bildposition oder Tabellenlayout ab.
- Bedienbar / Operable: Die neuen Artefakte besitzen lineare Überschriften- und
  Linkpfade; die statische Dokumentation erfordert keine Maus.
- Verständlich / Understandable: Deutsch steht zuerst, Englisch direkt danach;
  Abkürzungen wie SBOM und VEX werden beim ersten fachlichen Gebrauch erklärt.
- Robust / Robust: Markdown bleibt ohne CSS verständlich. DocFX, Lynx und Axe
  prüfen die generierte HTML-Ausgabe.

- Perceivable: status, risks, and decisions are expressed in text; no meaning
  depends on colour, image position, or table layout.
- Operable: new artefacts provide linear heading and link paths; static
  documentation does not require a mouse.
- Understandable: German appears first and English directly afterwards;
  abbreviations such as SBOM and VEX are explained on first technical use.
- Robust: Markdown remains understandable without CSS. DocFX, Lynx, and Axe
  verify generated HTML.

## Plattform- und Ausgabegrenzen / Platform and Output Boundaries

DE: CLI-Ausgaben bleiben zeilenweise lesbar und verwenden Statuswörter statt
nur Farbe. API-Fehler sind kurze JSON-/Textmeldungen ohne interne Details. Die
Terminal-UI behält Tastaturbedienung; dieses Feature verändert keine visuellen
Widgets. Windows-, macOS- und Linux-Belege stammen aus der CI-Matrix. Ein
physischer Braille-Display-Lauf ist `FollowUp`; die text-first-Prüfung und Lynx
sind der lokale Ersatznachweis, Owner Accessibility Reviewer, Termin
2026-09-15, Trigger: Providerlauf oder UI-Änderung.

EN: CLI output remains line-readable and uses status words instead of colour
alone. API errors are short JSON/text messages without internal details. The
terminal UI keeps keyboard operation; this feature changes no visual widget.
Windows, macOS, and Linux evidence comes from the CI matrix. A physical Braille
display run is `FollowUp`; text-first review and Lynx are the local substitute,
owner Accessibility Reviewer, due 2026-09-15, triggered by a provider run or
UI change.

## Ausführung / Execution

DE: Die Ergebnisfelder für DocFX, Lynx, Axe und die manuelle Prüfung werden bei
T082/T083 mit Befehl, UTC-Zeit, Ergebnis und Kandidatenbezug ergänzt. Ein
Fehler bleibt `Open` und schließt `G-A11Y-001` nicht.

EN: T082/T083 add command, UTC time, result, and candidate binding for DocFX,
Lynx, Axe, and manual review. A failure remains `Open` and does not close
`G-A11Y-001`.

## Ergebnis 2026-08-30 / Result 2026-08-30

- DocFX 2.78.5: `Pass` mit 32 bereits sichtbaren Linkwarnungen und ohne Fehler;
  Ausgabe `_site/index.html` vorhanden. / `Pass` with 32 visible link warnings
  and no errors; `_site/index.html` exists.
- HTML-Postprocessing: `Pass`; `lang`, Logo-Alternativtext und
  `aria-expanded`-Verträge sind grün. / `Pass`; `lang`, logo alternative text,
  and `aria-expanded` contracts are green.
- Lynx: `Pass`; Navigation, DE/EN-Inhalt, API-Scope und Linkziele sind linear
  verständlich. / `Pass`; navigation, DE/EN content, API scope, and link
  targets are understandable in a linear reading order.
- Axe: `Open`, Owner Accessibility Reviewer, Priorität hoch, Termin
  2026-09-15. Chromium konnte im verwalteten macOS-Sandboxprofil wegen
  `MachPortRendezvousServer ... Permission denied` nicht starten. Der
  unveränderte Axe-Workflow bleibt als Pflicht-Providerbeweis bestehen. /
  `Open`, owner Accessibility Reviewer, high priority, due 2026-09-15.
  Chromium could not start in the managed macOS sandbox because of
  `MachPortRendezvousServer ... Permission denied`. The unchanged Axe workflow
  remains mandatory provider evidence.
- Manuelle text-first-Prüfung: `Pass` für Überschriftenfolge, aussagekräftige
  Links, Statuswörter, Tastatur-/Screenreader-Lesefolge und Layout-Unabhängigkeit.
  Physische Braille-Hardware bleibt `FollowUp`. / Manual text-first review:
  `Pass` for heading order, descriptive links, status words, keyboard and
  screen-reader reading order, and layout independence. Physical Braille
  hardware remains `FollowUp`.

DE: Frischebasis der manuellen text-first-Quellenprüfung ist der lokale
Feature-Head plus Arbeitsbaum nach T099 vom 30. August 2026. Die frühere
DocFX-/Lynx-Ausführung beweist ihren damaligen generierten Stand; spätere
Markdown-Änderungen werden deshalb nicht stillschweigend auf das HTML
übertragen. Der exakte Kandidat benötigt weiterhin den deklarierten
`Docs Pages / build-docs`-Providerlauf mit DocFX, Postprocessing, Lynx und Axe.
Änderungen an Markdown, DocFX-Template, Navigation oder Node/Browser öffnen
diesen Nachweis erneut.

EN: The manual text-first source review is fresh for the local feature head and
working tree after T099 on 30 August 2026. The earlier DocFX/Lynx execution
proves its generated state at that time; later Markdown changes are therefore
not silently transferred to the HTML claim. The exact candidate still requires
the declared `Docs Pages / build-docs` provider run with DocFX,
postprocessing, Lynx, and Axe. Changes to Markdown, the DocFX template,
navigation, or Node/browser reopen this evidence.
