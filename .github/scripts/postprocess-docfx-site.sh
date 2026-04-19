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
