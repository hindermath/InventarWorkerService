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
  perl -0pi -e 's#(<a\b[^>]*\bclass="[^"]*\bdropdown-toggle\b[^"]*"[^>]*?)\saria-expanded="false"([^>]*>)#$1$2#g' "$html_file"
done < <(find "$site_root" -type f -name '*.html' -print0)

if [[ -f "$site_root/public/docfx.min.js" ]]; then
  perl -0pi -e 's!<a title='\''\$\{L\("changeTheme"\)\}'\'' class='\''btn border-0 dropdown-toggle'\'' data-bs-toggle='\''dropdown'\'' aria-expanded='\''false'\''>!<a title='\''\$\{L\("changeTheme"\)\}'\'' class='\''btn border-0 dropdown-toggle'\'' data-bs-toggle='\''dropdown'\''>!g' "$site_root/public/docfx.min.js"
  perl -0pi -e 's!<a class='\''nav-link dropdown-toggle \$\{s\}'\'' href='\''#'\'' role='\''button'\'' data-bs-toggle='\''dropdown'\'' aria-expanded='\''false'\''>!<a class='\''nav-link dropdown-toggle \$\{s\}'\'' href='\''#'\'' role='\''button'\'' data-bs-toggle='\''dropdown'\''>!g' "$site_root/public/docfx.min.js"
fi
