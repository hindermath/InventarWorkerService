# POSTPROCESS-DOCFX-SITE(1)

## NAME / NAME

postprocess-docfx-site.sh — DocFX-Ausgabe barrierearm nachbearbeiten / improve
accessibility of generated DocFX output.

## SYNOPSIS / AUFRUF

`bash .github/scripts/postprocess-docfx-site.sh [SITE_DIRECTORY]`

## BESCHREIBUNG / DESCRIPTION

Bearbeitet ausschliesslich den angegebenen erzeugten Seitenordner, Standard
`_site`. Fuegt fehlende deutsche Seitensprache hinzu und markiert das redundante
Logo als dekorativ. Theme-Schalter ohne Linkziel werden im HTML und in
`public/docfx.min.js` in native Buttons umgewandelt. `aria-expanded` bleibt
erhalten: Bootstrap darf den Zustand auch nach Tastaturbedienung aktualisieren.
Dekorative Schalter-Icons werden vor Hilfsmitteln verborgen, damit der
lokalisierte Titel den zugaenglichen Namen liefert, nicht eine Icon-Font-Glyphe.
Echte Navigationslinks bleiben unveraendert. Wiederholung ist idempotent.

Processes only the specified generated site directory, default `_site`.
Adds missing German document language, marks the redundant logo decorative,
and converts linkless theme toggles to native buttons in HTML and generated
JavaScript. Preserve `aria-expanded` for Bootstrap state updates and keyboard
interaction. Decorative toggle icons are hidden from assistive technology so
the localized title supplies the accessible name, not an icon-font glyph.
Real navigation links remain unchanged; reruns are idempotent.

## PRUEFUNG / VALIDATION

Die Docs-Pages-CI prueft aktuelle und aeltere Markup-Fixtures, echte Links und
Idempotenz. Danach prueft sie die echte DocFX-Seite mit Textbrowser, Tastatur
und Axe. Ein reiner Quelltext-Mustervergleich ersetzt diesen Browsertest nicht.

Docs Pages CI checks current/legacy fixtures, navigation preservation and
idempotency, then tests actual DocFX output with a text browser, keyboard and
Axe. Static pattern checks alone do not prove browser accessibility.

## DOKUMENTATIONSAUSWIRKUNG / DOCUMENTATION IMPACT

UpdateRequired; Owner InventarWorkerService Maintainer. Zielgruppe: Docs-
Maintainer; Leserpfad Docs-Pages-Workflow → Nachbearbeitung → diese Referenz.
Quelle: vorhandenes Nachbearbeitungsskript; Klasse Betriebsreferenz, DE/EN,
source-only, kein Home-Sync. Linux-CI prueft die erzeugte Seite; macOS bleibt
primaere Entwicklungsplattform, kein neuer nativer macOS-Nachweis behauptet.
Wiedervorlage bei DocFX-Template- oder Bootstrap-Aenderung.

UpdateRequired; documentation maintainer owns this bilingual source-only
operations reference. Entry path: Docs Pages workflow, postprocessor, this
reference. No Home sync. Linux CI validates output without claiming new native
macOS evidence. Re-evaluate when DocFX templates or Bootstrap change.
