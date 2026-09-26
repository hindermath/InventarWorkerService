#!/usr/bin/env bash

set -euo pipefail

site_root="${1:-_site}"

if [[ ! -d "$site_root" ]]; then
  echo "DocFX site directory '$site_root' does not exist." >&2
  exit 1
fi

while IFS= read -r -d '' html_file; do
  perl -0pi -e 's/<html(?![^>]*\blang=)/<html lang="de"/g' "$html_file"
  perl -0pi -e 's#<img id="logo" class="svg" src="([^"]+)" alt="InventarWorkerService">#<img id="logo" class="svg" src="$1" alt="" aria-hidden="true">#g' "$html_file"
  # DE: Ein Schalter ohne Linkziel braucht native Button-Semantik. ARIA bleibt
  # erhalten, damit Bootstrap den geoeffneten Zustand korrekt melden kann.
  # EN: A toggle without a link target needs native button semantics. Preserve
  # ARIA so Bootstrap can report the expanded state, including after a click.
  perl -0pi -e 's{<a\b((?=[^>]*\bclass=["\x27][^"\x27]*\bdropdown-toggle\b)(?![^>]*\bhref\s*=)[^>]*)>(.*?)</a>}{<button type="button"$1>$2</button>}gs' "$html_file"
  # DE/EN: Dekorative Icon-Font-Glyphen duerfen den Titel nicht ueberstimmen.
  # Decorative icon glyphs must not override the accessible title.
  perl -0pi -e 's{(<button\b(?=[^>]*\bclass=["\x27][^"\x27]*\bdropdown-toggle\b)[^>]*>\s*)<i\b(?![^>]*\baria-hidden=)}{$1<i aria-hidden="true"}gs' "$html_file"
done < <(find "$site_root" -type f -name '*.html' -print0)

if [[ -f "$site_root/public/docfx.min.js" ]]; then
  # DE/EN: Auch zur Laufzeit erzeugtes Markup korrigieren; echte Links erhalten.
  # Fix runtime-generated markup too, while preserving real navigation links.
  perl -0pi -e 's{<a\b((?=[^>]*\bclass=["\x27][^"\x27]*\bdropdown-toggle\b)(?![^>]*\bhref\s*=)[^>]*)>(.*?)</a>}{<button type="button"$1>$2</button>}gs' "$site_root/public/docfx.min.js"
  perl -0pi -e 's{(<button\b(?=[^>]*\bclass=["\x27][^"\x27]*\bdropdown-toggle\b)[^>]*>\s*)<i\b(?![^>]*\baria-hidden=)}{$1<i aria-hidden="true"}gs' "$site_root/public/docfx.min.js"
fi
