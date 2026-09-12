# Barrierearme verlinkte Intake-Evidence / Accessible Linked Intake Evidence

## Ergebnis und Prüfgrenze / Result and review boundary

Die beiden Inventar-Reihenfolgeansichten wurden am 12. September 2026 als
Quelltext und linearer Text geprüft. Jede Datenzeile enthält in stabiler
Reihenfolge Position, ausgeschriebenen Status, vollständigen Intake-Dateinamen,
jede direkte Abhängigkeit mit Art und Bindungswert sowie einen verlinkten oder
ausdrücklich fehlenden Featurezustand. Keine Information hängt nur von Farbe,
Symbolform oder räumlichem Tabellenverständnis ab.

*The two Inventar order views were reviewed as source and linear text on 12
September 2026. Every row provides, in stable order, the position, written
status, complete intake filename, each direct dependency with kind and binding
value, and a linked or explicitly absent feature state. No information depends
solely on colour, symbol shape, or spatial table layout.*

Dies ist eine macOS-Quelltext- und Linearisierungsprüfung. Ein konkreter
Screenreader oder eine Braille-Zeile wurde nicht bedient; native Linux- und
Windows-Evidence folgt im Delivery-Checkpoint.

## WCAG-2.2-AA-Disposition

| Kriterium | Anwendung | Nachweis / Grenze |
|---|---|---|
| 1.3.1 Information und Beziehungen | Eindeutige Überschrift, Einleitung und fünf benannte Felder. | Semantischer Paarvergleich und Fixture-Zeilen; keine HTML-Behauptung. |
| 1.4.1 Farbe | Status, Root, Kantenart, Bindungswert und Fallback stehen als Text. | Keine Farbe oder ANSI-Steuerung erforderlich. |
| 2.1.1 Tastatur | Markdown-Links und statischer Text verlangen keine Zeigeraktion. | Web- oder IDE-Navigation ist nicht Teil dieses Slices. |
| 2.4.4 Linkzweck | Vollständige Intake- und Feature-Namen bilden den Linktext. | Root- und Series-Ansicht prüfen sichere relative Ziele. |
| 2.4.6 Überschriften und Beschriftungen | Deutsch steht vor Englisch; Zweck und Datenquelle sind benannt. | Beide Ansichten verwenden dieselbe Dokumentvorlage. |
| 3.1.2 Sprache von Teilen | Deutscher Haupttext und direkt zugeordneter englischer Partnertext. | Technische Literale und Dateinamen bleiben unverändert. |
| 3.3.1 Fehlererkennung | `LIE001`–`LIE012` unterscheiden stabile Fehlerfamilien. | Negativkatalog und ausführbare Regressionstests. |

Mehrere direkte Kanten werden erst nach sicherer Linkerzeugung mit `<br>`
getrennt. Dateinamen und Statuswerte werden als Daten escaped; die vollständige
Bedeutung bleibt nach Linearisierung erhalten.

## Agent-Parity-Disposition

Die Featurefunktion führt keine neue gemeinsame Agenten-, Routing-, Delivery-
oder Bedienregel ein. `AGENTS.md`, `CLAUDE.md`, `GEMINI.md`,
`.github/copilot-instructions.md` und
`.github/agents/copilot-instructions.md` bleiben daher bewusst unverändert
(`NoUpdateRequired`). Die beiden Constitution-Spiegel wurden gemeinsam nur auf
die bereits installierte autonome Presetversion 0.4.1 berichtigt. Eine neue
gemeinsame Regel, Promptsemantik oder Routingänderung würde die fünf Agenten-
flächen erneut auslösen.

*The feature introduces no shared agent, routing, delivery, or operating rule,
so all five agent-guidance surfaces remain deliberately unchanged
(`NoUpdateRequired`). Both constitution mirrors were corrected together only
to the already installed autonomous preset version 0.4.1. A new shared rule,
prompt semantic, or routing change would trigger all five surfaces again.*

Owner ist der InventarWorkerService Repository Owner; Reviewer ist die
Feature-032 A11Y-/Agent-Parity-Rolle. Restrisiko sind Unterschiede realer
assistiver Konfigurationen. Re-Evaluation bei Spalten-, Link-, Sprach-,
Diagnose-, HTML-, Medien- oder Interaktionsänderung.
