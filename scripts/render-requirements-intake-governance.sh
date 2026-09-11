#!/usr/bin/env bash
# Repository-native renderer for linked requirements-intake order views.

set -euo pipefail

SDH_PREPARE_RESULT=""
SDH_PREPARE_REASON=""
SDH_DETECTED_LANGUAGE=""

sdh_diagnostic_remediation() {
  case "$1" in
    LIE001) printf '%s' 'Datei als gueltiges UTF-8 ohne NUL speichern und erneut pruefen / Save the file as valid UTF-8 without NUL and check again.' ;;
    LIE002) printf '%s' 'Schema und Pflichtfelder in der kanonischen Quelle korrigieren / Correct the schema and required fields in the canonical source.' ;;
    LIE003) printf '%s' 'Repository-relativen Pfad ohne Traversal oder Optionskomponente verwenden / Use a repository-relative path without traversal or option components.' ;;
    LIE004) printf '%s' 'Kanonischen relativen Pfad und erwarteten Typ pruefen / Check the canonical relative path and expected type.' ;;
    LIE005) printf '%s' 'Symlink und physische Pfadauflosung innerhalb des Repositorys korrigieren / Correct the symlink and physical path resolution inside the repository.' ;;
    LIE006) printf '%s' 'Kanonische Identitaeten und Positionen eindeutig machen / Make canonical identities and positions unique.' ;;
    LIE007) printf '%s' 'From, To, Kind und Binding einzeln mit dem Manifest abgleichen / Compare from, to, kind, and binding individually with the manifest.' ;;
    LIE008) printf '%s' 'Genau einen expliziten vorhandenen Feature-Nachweis bereitstellen / Provide exactly one explicit existing feature proof.' ;;
    LIE009) printf '%s' 'Kanonische Quelle pruefen und den begrenzten Schreibmodus ausfuehren / Review the canonical source and run the bounded write mode.' ;;
    LIE010) printf '%s' 'Fehlerursache beheben und die vollstaendige Transaktion erneut ausfuehren / Fix the cause and run the complete transaction again.' ;;
    LIE011) printf '%s' 'Beide Ausgaben aus derselben typisierten Projektion regenerieren / Regenerate both outputs from the same typed projection.' ;;
    LIE012) printf '%s' 'Gemeinsame Fixtures, Exitklasse, Diagnose und Ausgabebytes vergleichen / Compare shared fixtures, exit class, diagnostic, and output bytes.' ;;
    *) printf '%s' 'Sicheren relativen Eingabekontext pruefen und den Befehl erneut ausfuehren / Check the safe relative input context and run the command again.' ;;
  esac
}

sdh_redact_public_diagnostic() {
  local message="$1"
  # Diagnostics are a trust boundary too: one physical line prevents terminal
  # control injection, while credential-shaped values and private home roots
  # are removed without hiding an ordinary repository-relative subject.
  printf '%s' "$message" \
    | LC_ALL=C tr '\001-\037\177' '?' \
    | sed -E \
      -e 's#(/Users/|/home/)[^[:space:]:;,]+#[private-path]#g' \
      -e 's#([Tt][Oo][Kk][Ee][Nn]|[Pp][Aa][Ss][Ss][Ww][Oo][Rr][Dd]|[Ss][Ee][Cc][Rr][Ee][Tt]|[Aa][Uu][Tt][Hh][Oo][Rr][Ii][Zz][Aa][Tt][Ii][Oo][Nn]|[Aa][Pp][Ii][_-]?[Kk][Ee][Yy])[[:space:]]*[:=][[:space:]]*[^[:space:];,]+#\1=[redacted]#g'
}

sdh_log() {
  local message="$*"
  local code remediation
  case "$message" in
    LIE[0-9][0-9][0-9]:*)
      message="$(sdh_redact_public_diagnostic "$message")"
      code="${message%%:*}"
      remediation="$(sdh_diagnostic_remediation "$code")"
      message="${message}; Abhilfe / remediation: ${remediation}"
      ;;
  esac
  printf '%s\n' "$message"
}

sdh_jq() {
  # Native jq on Windows otherwise translates output to CRLF. Binary mode
  # keeps structured values byte-stable without masking embedded CR input.
  case "$(uname -s)" in
    MINGW*|MSYS*|CYGWIN*) command jq -b "$@" ;;
    *) command jq "$@" ;;
  esac
}

sdh_sha256_file() {
  local file="$1"
  if command -v sha256sum >/dev/null 2>&1; then
    command sha256sum -- "$file" | sdh_normalize_sha256_output
  elif command -v shasum >/dev/null 2>&1; then
    command shasum -a 256 -- "$file" | sdh_normalize_sha256_output
  else
    sdh_log 'LIE002: SHA-256-Werkzeug fehlt / SHA-256 tool is missing' >&2
    return 1
  fi
}

sdh_normalize_sha256_output() {
  # GNU coreutils prefixes a checksum line with "\" when an input filename
  # needs escaping, which is common for RUNNER_TEMP paths under Git Bash.
  # The marker describes the filename field and is not part of the digest.
  awk '{
    digest = $1
    sub(/^\\/, "", digest)
    if (length(digest) != 64 || digest ~ /[^0-9A-Fa-f]/) {
      exit 1
    }
    print tolower(digest)
  }'
}

sdh_sha256_stream() {
  if command -v sha256sum >/dev/null 2>&1; then
    command sha256sum | sdh_normalize_sha256_output
  elif command -v shasum >/dev/null 2>&1; then
    command shasum -a 256 | sdh_normalize_sha256_output
  else
    sdh_log 'LIE002: SHA-256-Werkzeug fehlt / SHA-256 tool is missing' >&2
    return 1
  fi
}

sdh_create_test_file_symlink() {
  local target="$1"
  local link="$2"
  local windows_target windows_link

  case "$(uname -s)" in
    MINGW*|MSYS*|CYGWIN*)
      command -v cygpath >/dev/null 2>&1 || return 1
      command -v pwsh >/dev/null 2>&1 || return 1
      windows_target="$(cygpath -w -- "$target")"
      windows_link="$(cygpath -w -- "$link")"
      SDH_TEST_SYMLINK_TARGET="$windows_target" \
        SDH_TEST_SYMLINK_LINK="$windows_link" \
        pwsh -NoProfile -Command '
        Set-StrictMode -Version Latest
        $ErrorActionPreference = "Stop"
        New-Item -ItemType SymbolicLink -Path $env:SDH_TEST_SYMLINK_LINK -Target $env:SDH_TEST_SYMLINK_TARGET | Out-Null
      '
      ;;
    *) ln -s "$target" "$link" ;;
  esac
}

sdh_find_intake_series_manifest() {
  local repo="$1"
  local explicit_manifest="${2:-}"
  local preferred="$repo/requirements/intakes/series/home-baseline-delivery/manifest.json"
  local matches

  if [ -n "$explicit_manifest" ]; then
    sdh_assert_safe_repository_path "$repo" "$explicit_manifest" file || return 1
    printf '%s\n' "$repo/$explicit_manifest"
    return 0
  fi

  if [ -f "$preferred" ]; then
    printf '%s\n' "$preferred"
    return 0
  fi

  matches="$(
    find "$repo/requirements/intakes/series" -mindepth 2 -maxdepth 2 -type f -name manifest.json -print 2>/dev/null || true
    find "$repo/specs/intake-series" -mindepth 2 -maxdepth 2 -type f -name manifest.json -print 2>/dev/null || true
  )"
  [ "$(printf '%s\n' "$matches" | sed '/^$/d' | wc -l | tr -d ' ')" = "1" ] || return 1
  printf '%s\n' "$matches"
}

sdh_assert_safe_repository_path() {
  local repo="$1"
  local relative="$2"
  local expected_type="$3"
  local target resolved_repo resolved_target unsafe_class=""

  case "$relative" in
    "") unsafe_class='empty' ;;
    /*|\\*|[A-Za-z]:*) unsafe_class='absolute' ;;
    ..|../*|*/../*|*/..) unsafe_class='traversal' ;;
    -*|*/-*) unsafe_class='option-component' ;;
    *'\'*) unsafe_class='backslash' ;;
    *$'\n'*|*$'\r'*|*$'\t'*) unsafe_class='control-byte' ;;
  esac
  if [ -n "$unsafe_class" ]; then
      # Rejected path bytes may themselves be credentials or control text, so
      # the public diagnostic preserves the code and remediation, not the data.
    sdh_log "LIE003: unsicherer Repositorypfad / unsafe repository path: [redacted] (class: $unsafe_class)" >&2
    return 1
  fi

  target="$repo/$relative"
  case "$expected_type" in
    file) [ -f "$target" ] || { sdh_log "LIE004: Datei fehlt / file is missing: $relative" >&2; return 1; } ;;
    directory) [ -d "$target" ] || { sdh_log "LIE004: Verzeichnis fehlt / directory is missing: $relative" >&2; return 1; } ;;
    *) return 1 ;;
  esac

  # Git Bash exposes the same Windows path through both an MSYS namespace and
  # a native drive namespace. Resolve both containment operands with the same
  # tool so an in-repository file cannot become a false escape on Windows.
  resolved_repo="$(realpath "$repo")"
  resolved_target="$(realpath "$target")"
  case "$resolved_target" in
    "$resolved_repo"|"$resolved_repo"/*) ;;
    *)
      sdh_log "LIE005: Pfad verlaesst das Repository / path escapes repository: $relative" >&2
      return 1
      ;;
  esac
}

sdh_assert_safe_output_path() {
  local repo="$1"
  local relative="$2"
  local target parent resolved_repo resolved_parent

  case "$relative" in
    ""|/*|\\*|[A-Za-z]:*|..|../*|*/../*|*/..|-*|*/-*|*'\'*|*$'\n'*|*$'\r'*|*$'\t'*)
      sdh_log 'LIE003: unsicherer Ausgabepfad / unsafe output path: [redacted]' >&2
      return 1
      ;;
  esac
  target="$repo/$relative"
  parent="$(dirname "$target")"
  [ -d "$parent" ] || { sdh_log "LIE004: Ausgabe-Elternverzeichnis fehlt / output parent is missing: $(dirname "$relative")" >&2; return 1; }
  [ ! -e "$target" ] || [ -f "$target" ] || { sdh_log "LIE004: Ausgabe hat den falschen Typ / output has the wrong type: $relative" >&2; return 1; }
  [ ! -L "$target" ] || { sdh_log "LIE005: Ausgabe darf kein symbolischer Link sein / output must not be a symbolic link: $relative" >&2; return 1; }
  # Keep repository and parent in one canonical namespace on Git Bash while
  # retaining realpath's physical symlink resolution on every platform.
  resolved_repo="$(realpath "$repo")"
  resolved_parent="$(realpath "$parent")"
  case "$resolved_parent" in
    "$resolved_repo"|"$resolved_repo"/*) ;;
    *) sdh_log "LIE005: Ausgabepfad verlaesst das Repository / output path escapes repository: $relative" >&2; return 1 ;;
  esac
}

sdh_repository_relative_from_absolute_path() {
  local repo="$1"
  local absolute="$2"
  local resolved_repo resolved_absolute relative

  # Discovered paths may use a different Git-Bash namespace than their input.
  # Canonicalize both before stripping the repository prefix; the returned
  # value is safe to pass through the ordinary relative-path validator.
  resolved_repo="$(realpath "$repo")" || return 1
  resolved_absolute="$(realpath "$absolute")" || return 1
  case "$resolved_absolute" in
    "$resolved_repo"/*) relative="${resolved_absolute#"$resolved_repo"/}" ;;
    *)
      sdh_log 'LIE005: ermittelter Pfad verlaesst das Repository / discovered path escapes repository: [redacted]' >&2
      return 1
      ;;
  esac
  # A native Windows realpath may retain backslashes after the common physical
  # prefix has been removed. Repository-relative contracts always use '/'.
  relative="${relative//\\//}"
  printf '%s\n' "$relative"
}

sdh_resolve_linked_intake_path() {
  local repo="$1"
  local logical_path="$2"
  local directory base_name candidate candidate_name resolved stamp state_path is_active_logical=0
  local -a candidates=()

  case "$logical_path" in
    ""|/*|\\*|[A-Za-z]:*|..|../*|*/../*|*/..|-*|*/-*|*'\'*|*$'\n'*|*$'\r'*|*$'\t'*)
      sdh_log 'LIE003: unsicherer Repositorypfad / unsafe repository path: [redacted]' >&2
      return 1
      ;;
    requirements/intakes/active/*.md) is_active_logical=1 ;;
    *)
      if [ -f "$repo/$logical_path" ]; then
        sdh_assert_safe_repository_path "$repo" "$logical_path" file || return 1
        printf '%s\n' "$logical_path"
        return 0
      fi
      sdh_log "LIE004: Datei fehlt / file is missing: $logical_path" >&2
      return 1
      ;;
  esac

  directory="${logical_path%/*}"
  base_name="$(basename "${logical_path%.md}")"
  if [ ! -d "$repo/$directory" ]; then
    sdh_log "LIE004: Datei fehlt / file is missing: $logical_path" >&2
    return 1
  fi
  sdh_assert_safe_repository_path "$repo" "$directory" directory || return 1
  while IFS= read -r candidate; do
    candidate_name="$(basename "$candidate")"
    case "$candidate_name" in
      "$base_name".[0-9][0-9][0-9]-*.md)
        candidates+=("${candidate#"$repo/"}")
        ;;
    esac
  done < <(find "$repo/$directory" -maxdepth 1 -type f -print | LC_ALL=C sort)

  if [ -f "$repo/$logical_path" ]; then
    sdh_assert_safe_repository_path "$repo" "$logical_path" file || return 1
    if [ "$is_active_logical" -eq 1 ] && [ "${#candidates[@]}" -gt 0 ]; then
      sdh_log "LIE006: Original- und gestempelter Intake existieren gleichzeitig / original and stamped intake both exist: $logical_path" >&2
      return 1
    fi
    printf '%s\n' "$logical_path"
    return 0
  fi

  case "${#candidates[@]}" in
    0)
      sdh_log "LIE004: Datei fehlt / file is missing: $logical_path" >&2
      return 1
      ;;
    1) resolved="${candidates[0]}" ;;
    *)
      sdh_log "LIE006: logischer Intake ist mehrdeutig / logical intake is ambiguous: $logical_path" >&2
      return 1
      ;;
  esac
  sdh_assert_safe_repository_path "$repo" "$resolved" file || return 1
  stamp="$(basename "$resolved" | sed -nE 's/^.*\.([0-9]{3}-[^/]+)\.md$/\1/p')"
  state_path="specs/$stamp/autonomous-run-state.json"
  [ -n "$stamp" ] && [ -f "$repo/$state_path" ] || {
    sdh_log "LIE008: gestempelter Intake hat keinen Feature-Nachweis / stamped intake has no feature proof: $logical_path" >&2
    return 1
  }
  sdh_assert_safe_repository_path "$repo" "$state_path" file || return 1
  sdh_assert_strict_utf8_file "$repo/$state_path" "$state_path" || return 1
  sdh_assert_linked_intake_completion_proof "$repo" "$logical_path" "$resolved" "$state_path" || return 1
  printf '%s\n' "$resolved"
}

sdh_linked_intake_input_paths() {
  local repo="$1"
  local manifest_relative="$2"
  local intake_path resolved_path archive_stamp proof_path
  local spec_file state_path

  printf '%s\n' "$manifest_relative"
  while IFS= read -r intake_path; do
    printf '%s\n' "$intake_path"
    resolved_path="$(sdh_resolve_linked_intake_path "$repo" "$intake_path")" || return 1
    [ "$resolved_path" = "$intake_path" ] || printf '%s\n' "$resolved_path"
  done < <(sdh_jq -r '.orderedTargets[].path' "$repo/$manifest_relative")
  for spec_file in "$repo"/specs/[0-9][0-9][0-9]-*/spec.md; do
    [ -f "$spec_file" ] || continue
    printf '%s\n' "${spec_file#"$repo/"}"
  done
  while IFS= read -r intake_path; do
    resolved_path="$(sdh_resolve_linked_intake_path "$repo" "$intake_path")" || return 1
    archive_stamp="$(basename "$resolved_path" | sed -nE 's/^.*\.([0-9]{3}-[^/]+)\.md$/\1/p')"
    [ -n "$archive_stamp" ] || continue
    state_path="specs/$archive_stamp/autonomous-run-state.json"
    [ ! -f "$repo/$state_path" ] || printf '%s\n' "$state_path"
    for proof_path in "$repo"/specs/intake-authoring-archive/series/*/"$archive_stamp"/series-receipt.json; do
      [ -f "$proof_path" ] || continue
      printf '%s\n' "${proof_path#"$repo/"}"
    done
  done < <(sdh_jq -r '.orderedTargets[].path' "$repo/$manifest_relative")
  while IFS= read -r proof_path; do
    [ -z "$proof_path" ] || printf '%s\n' "$proof_path"
  done < <(sdh_jq -r '(.featureEvidence // [])[].featurePath' "$repo/$manifest_relative")
}

sdh_linked_intake_input_fingerprint() {
  local repo="$1"
  local manifest_relative="$2"
  local relative raw_hash

  while IFS= read -r relative; do
    if [ -f "$repo/$relative" ]; then
      raw_hash="$(sdh_sha256_file "$repo/$relative")"
      printf '%s\0file\0%s\n' "$relative" "$raw_hash"
    elif [ -d "$repo/$relative" ]; then
      printf '%s\0directory\n' "$relative"
    else
      printf '%s\0missing\n' "$relative"
    fi
  done < <(sdh_linked_intake_input_paths "$repo" "$manifest_relative" | LC_ALL=C sort -u) \
    | sdh_sha256_stream
}

sdh_assert_fixture_fault_scope() {
  local repo="$1"
  local resolved_repo resolved_tmp

  [ -f "$repo/.sdh-linked-intake-test-fixture" ] || return 1
  resolved_repo="$(cd "$repo" && pwd -P)"
  resolved_tmp="$(cd "${TMPDIR:-/tmp}" && pwd -P)"
  case "$resolved_repo" in
    "$resolved_tmp"/*) return 0 ;;
    *) return 1 ;;
  esac
}

sdh_assert_strict_utf8_file() {
  local file="$1"
  local subject="$2"

  if LC_ALL=C od -An -tx1 -v "$file" | tr -s ' ' '\n' | grep -Fqx '00'; then
    sdh_log "LIE001: NUL-Inhalt ist unzulaessig / NUL content is not allowed: $subject" >&2
    return 1
  fi
  if ! iconv -f UTF-8 -t UTF-8 "$file" >/dev/null 2>&1; then
    sdh_log "LIE001: ungueltiges UTF-8 / invalid UTF-8: $subject" >&2
    return 1
  fi
}

sdh_markdown_text() {
  # Entity-escape raw HTML in untrusted data. Renderer-owned <br> separators
  # are inserted only after this boundary and therefore remain functional.
  printf '%s' "$1" | sed 's/&/\&amp;/g; s/</\&lt;/g; s/>/\&gt;/g; s/\\/\\\\/g; s/|/\\|/g; s/\[/\\[/g; s/\]/\\]/g; s/(/\\(/g; s/)/\\)/g; s/`/\\`/g'
}

sdh_assert_linked_intake_completion_proof() {
  local repo="$1"
  local logical_path="$2"
  local resolved_path="$3"
  local state_path="$4"
  local resolved_hash stamp archive_root receipt="" receipt_candidate receipt_relative="" receipt_count=0

  resolved_hash="$(sdh_sha256_file "$repo/$resolved_path")" || return 1
  if sdh_jq -e \
    --arg logical "$logical_path" \
    --arg resolved "$resolved_path" \
    --arg resolved_hash "$resolved_hash" '
      .status == "Completed"
      and (.closeout | type == "object")
      and .closeout.mergeOrPublication == "Completed"
      and .closeout.defaultBranchSync == "Completed"
      and .closeout.finalValidation == "Completed"
      and (.acceptedArtifacts | type == "array")
      and ([.acceptedArtifacts[] |
        select(
          (.path == $logical or .path == $resolved)
          and (.sha256 | type == "string")
          and .sha256 == $resolved_hash
        )] | length) == 1
    ' "$repo/$state_path" >/dev/null 2>&1; then
    return 0
  fi

  # Older completed features may bind the constitutional closeout rename in
  # the archived series receipt instead of repeating it in acceptedArtifacts.
  # The proof remains fail-closed: one receipt, the exact feature stamp, both
  # rename endpoints, the current file hash, and the terminal run state.
  stamp="${state_path#specs/}"
  stamp="${stamp%/autonomous-run-state.json}"
  archive_root="$repo/specs/intake-authoring-archive/series"
  if [ -d "$archive_root" ]; then
    while IFS= read -r receipt_candidate; do
      receipt_count=$((receipt_count + 1))
      receipt="$receipt_candidate"
    done < <(find "$archive_root" -type f -path "*/$stamp/series-receipt.json" -print | LC_ALL=C sort)
  fi
  if [ "$receipt_count" -eq 1 ]; then
    receipt_relative="${receipt#"$repo/"}"
    sdh_assert_safe_repository_path "$repo" "$receipt_relative" file || return 1
    sdh_assert_strict_utf8_file "$receipt" "$receipt_relative" || return 1
    if sdh_jq -e \
      --arg logical "$logical_path" \
      --arg resolved "$resolved_path" \
      --arg resolved_hash "$resolved_hash" '
        .documentType == "IntakeSeriesReceipt"
        and .status == "Ready"
        and (.lineage.rename | type == "object")
        and (.lineage.rename.from | type == "string" and length > 0)
        and .lineage.rename.to == $resolved
        and .lineage.rename.normalizedSha256After == $resolved_hash
      ' "$receipt" >/dev/null 2>&1; then
      return 0
    fi
  fi

  sdh_log "LIE008: gestempelter Intake ist nicht durch einen hashgebundenen terminalen Feature-Abschluss belegt / stamped intake is not proven by a hash-bound terminal feature completion: $logical_path" >&2
  return 1
}

sdh_validate_linked_intake_manifest() {
  local repo="$1"
  local manifest_relative="$2"
  local manifest="$repo/$manifest_relative"
  local path resolved_path declared_hash actual_hash root duplicate_count position positions_file target_count index kind from to
  local from_position to_position

  sdh_assert_safe_repository_path "$repo" "$manifest_relative" file || return 1
  sdh_assert_strict_utf8_file "$manifest" "$manifest_relative" || return 1
  sdh_jq -e '
    .schemaVersion == "1.0"
    and .documentType == "IntakeSeriesManifest"
    and (.seriesId | type == "string" and length > 0)
    and (.status | type == "string" and length > 0)
    and (.orderedTargets | type == "array" and length > 0)
    and (.roots | type == "array")
    and (.dependencies | type == "array")
    and all(.orderedTargets[];
      (type == "object")
      and (.path | type == "string" and length > 0)
      and (.role | type == "string" and length > 0)
      and (.status | type == "string" and length > 0)
      and (.normalizedSha256 | type == "string" and test("^[0-9a-f]{64}$")))
    and all(.roots[]; type == "string" and length > 0)
    and ((.featureEvidence // []) | type == "array")
    and all((.featureEvidence // [])[];
      (type == "object")
      and (.intakePath | type == "string" and length > 0)
      and (.featurePath | type == "string" and length > 0)
      and (.proofKind == "ReviewedLegacyMapping")
      and (.reviewed == true))
  ' "$manifest" >/dev/null 2>&1 || {
    sdh_log "LIE002: ungueltiges Series-Manifest / invalid series manifest: $manifest_relative" >&2
    return 1
  }

  sdh_jq -e '
    all(.dependencies[];
      (type == "object")
      and (.from | type == "string" and length > 0)
      and (.to | type == "string" and length > 0)
      and (.kind | type == "string" and length > 0)
      and (.binding | type == "boolean"))
  ' "$manifest" >/dev/null 2>&1 || {
    sdh_log 'LIE007: ungueltiges Dependency-Tupel / invalid dependency tuple' >&2
    return 1
  }

  duplicate_count="$(sdh_jq '[.orderedTargets[].path] | length - (unique | length)' "$manifest")"
  [ "$duplicate_count" = "0" ] || { sdh_log 'LIE006: doppelte Intake-Identitaet / duplicate intake identity' >&2; return 1; }
  duplicate_count="$(sdh_jq '[.roots[]] | length - (unique | length)' "$manifest")"
  [ "$duplicate_count" = "0" ] || { sdh_log 'LIE006: doppelte Root-Identitaet / duplicate root identity' >&2; return 1; }
  duplicate_count="$(sdh_jq '[.dependencies[] | [.from,.to,.kind,.binding]] | length - (unique | length)' "$manifest")"
  [ "$duplicate_count" = "0" ] || { sdh_log 'LIE007: doppeltes Dependency-Tupel / duplicate dependency tuple' >&2; return 1; }

  while IFS=$'\t' read -r path declared_hash; do
    resolved_path="$(sdh_resolve_linked_intake_path "$repo" "$path")" || return 1
    sdh_assert_strict_utf8_file "$repo/$resolved_path" "$resolved_path" || return 1
    actual_hash="$(sdh_sha256_file "$repo/$resolved_path")" || return 1
    # A terminal stamped successor is bound by its completed run-state. An
    # unstamped source has no supersession proof and must match the manifest.
    if [ "$resolved_path" = "$path" ] && [ "$actual_hash" != "$declared_hash" ]; then
      sdh_log "LIE009: Intake-Hash weicht vom Series-Manifest ab / intake hash differs from series manifest: $path" >&2
      return 1
    fi
  done < <(sdh_jq -r '.orderedTargets[] | [.path,.normalizedSha256] | @tsv' "$manifest")

  while IFS= read -r root; do
    sdh_jq -e --arg endpoint "$root" 'any(.orderedTargets[]; .path == $endpoint)' "$manifest" >/dev/null \
      || { sdh_log "LIE007: unbekannter Root-Endpoint / unknown root endpoint: $root" >&2; return 1; }
  done < <(sdh_jq -r '.roots[]' "$manifest")

  while IFS=$'\t' read -r from to kind; do
    sdh_jq -e --arg endpoint "$from" 'any(.orderedTargets[]; .path == $endpoint)' "$manifest" >/dev/null \
      || { sdh_log "LIE007: unbekannter Dependency-Endpoint / unknown dependency endpoint: $from" >&2; return 1; }
    sdh_jq -e --arg endpoint "$to" 'any(.orderedTargets[]; .path == $endpoint)' "$manifest" >/dev/null \
      || { sdh_log "LIE007: unbekannter Dependency-Endpoint / unknown dependency endpoint: $to" >&2; return 1; }
    case "$kind" in *$'\n'*|*$'\r'*|*$'\t'*) sdh_log 'LIE007: ungueltiger Dependency-Kind / invalid dependency kind' >&2; return 1 ;; esac
  done < <(sdh_jq -r '.dependencies[] | [.from,.to,.kind] | @tsv' "$manifest")

  sdh_jq -e '
    def acyclic($nodes; $edges):
      if ($nodes | length) == 0 then true
      else
        ([$nodes[] as $node
          | select(([$edges[] | select(.to == $node)] | length) == 0)
          | $node]) as $zero
        | if ($zero | length) == 0 then false
          else acyclic(
            [$nodes[] as $node | select(($zero | index($node)) == null) | $node];
            [$edges[] as $edge | select(($zero | index($edge.from)) == null) | $edge]
          )
          end
      end;
    (.orderedTargets | map(.path)) as $paths
    | .dependencies as $dependencies
    | all($dependencies[];
        .from != .to
        and (
          if .kind == "PreferredSerialOrder" then .binding == false
          elif (.kind == "HardCompletionGate"
            or .kind == "RequirementsGovernanceGate"
            or .kind == "AssessmentBaseline"
            or .kind == "FinalAuditInput") then .binding == true
          else false
          end
        ))
    and acyclic($paths; $dependencies)
    and ((.roots | sort) ==
      ([$paths[] as $path
        | select(($dependencies | any(.to == $path)) | not)
        | $path] | sort))
  ' "$manifest" >/dev/null 2>&1 || {
    sdh_log 'LIE007: Dependency-Graph, Kantenart, Binding oder Root-Menge ist ungueltig / dependency graph, edge kind, binding, or root set is invalid' >&2
    return 1
  }

  while IFS=$'\t' read -r path to; do
    sdh_jq -e --arg endpoint "$path" 'any(.orderedTargets[]; .path == $endpoint)' "$manifest" >/dev/null \
      || { sdh_log "LIE008: Legacy-Proof referenziert unbekannten Intake / legacy proof references unknown intake: $path" >&2; return 1; }
    case "$to" in specs/[0-9][0-9][0-9]-*) ;; *) sdh_log "LIE008: ungueltiger Feature-Nachweis / invalid feature evidence: $path" >&2; return 1 ;; esac
    sdh_assert_safe_repository_path "$repo" "$to" directory >/dev/null 2>&1 \
      || { sdh_log "LIE008: Feature-Ziel fehlt oder ist unsicher / feature target is missing or unsafe: $path" >&2; return 1; }
  done < <(sdh_jq -r '(.featureEvidence // [])[] | [.intakePath,.featurePath] | @tsv' "$manifest")

  positions_file="$(mktemp)"
  target_count="$(sdh_jq '.orderedTargets | length' "$manifest")"
  for ((index = 1; index <= target_count; index++)); do
    path="$(sdh_jq -r --argjson index "$((index - 1))" '.orderedTargets[$index].path' "$manifest")"
    resolved_path="$(sdh_resolve_linked_intake_path "$repo" "$path")" || { rm -f "$positions_file"; return 1; }
    position="$(sdh_display_position "$repo/$resolved_path" "$index")"
    case "$position" in ''|*[!0-9]*|0) rm -f "$positions_file"; sdh_log "LIE006: ungueltige sichtbare Position / invalid display position: $path" >&2; return 1 ;; esac
    printf '%s\t%s\n' "$path" "$position" >> "$positions_file"
  done
  duplicate_count="$(cut -f2 "$positions_file" | sort | uniq -d | wc -l | tr -d ' ')"
  [ "$duplicate_count" = "0" ] || { rm -f "$positions_file"; sdh_log 'LIE006: doppelte sichtbare Position / duplicate display position' >&2; return 1; }
  while IFS=$'\t' read -r from to; do
    from_position="$(awk -F '\t' -v endpoint="$from" '$1 == endpoint { print $2; exit }' "$positions_file")"
    to_position="$(awk -F '\t' -v endpoint="$to" '$1 == endpoint { print $2; exit }' "$positions_file")"
    if [ -z "$from_position" ] || [ -z "$to_position" ] || [ "$from_position" -ge "$to_position" ]; then
      rm -f "$positions_file"
      sdh_log 'LIE007: Dependency-Kante laeuft gegen die sichtbare Reihenfolge / dependency edge runs backward against visible order' >&2
      return 1
    fi
  done < <(sdh_jq -r '.dependencies[] | [.from,.to] | @tsv' "$manifest")
  rm -f "$positions_file"
}

sdh_relative_repository_path() {
  local view_path="$1"
  local target_path="$2"
  local view_dir="."
  local common=0 index result=""
  local -a view_parts=() target_parts=()

  case "$view_path" in
    */*) view_dir="${view_path%/*}" ;;
  esac
  [ "$view_dir" = "." ] || IFS='/' read -r -a view_parts <<< "$view_dir"
  IFS='/' read -r -a target_parts <<< "$target_path"

  while [ "$common" -lt "${#view_parts[@]}" ] \
    && [ "$common" -lt "${#target_parts[@]}" ] \
    && [ "${view_parts[$common]}" = "${target_parts[$common]}" ]; do
    common=$((common + 1))
  done
  for ((index = common; index < ${#view_parts[@]}; index++)); do
    result="${result}../"
  done
  for ((index = common; index < ${#target_parts[@]}; index++)); do
    [ "$index" = "$common" ] || result="${result}/"
    result="${result}${target_parts[$index]}"
  done
  printf '%s\n' "$result"
}

sdh_url_encode_repository_path() {
  local path="$1"
  local suffix=""
  local component encoded result=""

  case "$path" in
    */) suffix="/"; path="${path%/}" ;;
  esac
  while IFS= read -r component; do
    if [ "$component" = ".." ] || [ "$component" = "." ]; then
      encoded="$component"
    else
      encoded="$(sdh_jq -nr --arg value "$component" '$value | @uri')"
    fi
    [ -z "$result" ] || result="$result/"
    result="$result$encoded"
  done < <(printf '%s\n' "$path" | tr '/' '\n')
  printf '%s%s\n' "$result" "$suffix"
}

sdh_markdown_label() {
  printf '%s' "$1" | sed 's/&/\&amp;/g; s/</\&lt;/g; s/>/\&gt;/g; s/\\/\\\\/g; s/\[/\\[/g; s/\]/\\]/g; s/|/\\|/g'
}

sdh_markdown_link() {
  local repo="$1"
  local view_path="$2"
  local target_path="$3"
  local expected_type="$4"
  local label relative suffix=""

  sdh_assert_safe_repository_path "$repo" "$target_path" "$expected_type" || return 1
  label="$(sdh_markdown_label "$(basename "$target_path")")"
  relative="$(sdh_relative_repository_path "$view_path" "$target_path")"
  [ "$expected_type" != "directory" ] || suffix="/"
  relative="$(sdh_url_encode_repository_path "${relative}${suffix}")"
  printf '[%s](%s)' "$label" "$relative"
}

sdh_display_position() {
  local intake_file="$1"
  local manifest_index="$2"
  local explicit

  explicit="$(sed -nE '
    s/^\*\*Reihenfolge:\*\* *(sichtbare )?Position ([0-9]+).*/\2/p
    s/^\*\*Order:\*\* *(visible )?[Pp]osition ([0-9]+).*/\2/p
    s/^Dieser Intake .*Position ([0-9]+).*/\1/p
    s/^Position ([0-9]+) .*/\1/p
  ' "$intake_file" | head -n 1)"
  if [ -n "$explicit" ]; then
    printf '%s\n' "$explicit"
  else
    printf '%s\n' "$manifest_index"
  fi
}

sdh_feature_cell() {
  local repo="$1"
  local view_path="$2"
  local intake_path="$3"
  local status="$4"
  local manifest="${5:-}"
  local resolved_intake_path="${6:-$intake_path}"
  local spec_file feature_dir state_file archive_stamp mapped_path
  local -a candidates=()

  if [ "$status" = "Completed" ] && [ -d "$repo/specs" ]; then
    while IFS= read -r spec_file; do
      sdh_assert_strict_utf8_file "$spec_file" "${spec_file#"$repo/"}" || return 1
      if awk -v needle="\`$intake_path\`" '
        /^\*\*(Binding Input|Bindende Eingabe)( \/ (Binding Input|Bindende Eingabe))?\*\*:/ && index($0, needle) { found = 1 }
        END { exit(found ? 0 : 1) }
      ' "$spec_file"; then
        feature_dir="${spec_file%/spec.md}"
        mapped_path="$(sdh_repository_relative_from_absolute_path "$repo" "$feature_dir")" || return 1
        candidates+=("$mapped_path")
      fi
    done < <(find "$repo/specs" -mindepth 2 -maxdepth 2 -type f -name spec.md -print | sort)

    archive_stamp="$(basename "$resolved_intake_path" | sed -nE 's/^.*\.([0-9]{3}-[^/]+)\.md$/\1/p')"
    if [ -n "$archive_stamp" ] && [ -d "$repo/specs/$archive_stamp" ]; then
      state_file="$repo/specs/$archive_stamp/autonomous-run-state.json"
      if [ -f "$state_file" ]; then
        sdh_assert_strict_utf8_file "$state_file" "specs/$archive_stamp/autonomous-run-state.json" || return 1
        sdh_assert_linked_intake_completion_proof \
          "$repo" "$intake_path" "$resolved_intake_path" "specs/$archive_stamp/autonomous-run-state.json" || return 1
        candidates+=("specs/$archive_stamp")
      fi
    fi

    if [ -n "$manifest" ]; then
      while IFS= read -r mapped_path; do
        [ -d "$repo/$mapped_path" ] || { sdh_log "LIE008: Feature-Ziel fehlt / feature target is missing: $intake_path" >&2; return 1; }
        candidates+=("$mapped_path")
      done < <(sdh_jq -r --arg intake "$intake_path" '(.featureEvidence // [])[] | select(.intakePath == $intake) | .featurePath' "$manifest")
    fi
  fi

  case "${#candidates[@]}" in
    0)
      printf '%s' '— (kein Spec-Kit-Feature / no Spec Kit feature)'
      ;;
    1)
      sdh_markdown_link "$repo" "$view_path" "${candidates[0]}" directory
      ;;
    *)
      sdh_log "LIE008: mehrdeutiger Feature-Nachweis / ambiguous feature evidence: $intake_path" >&2
      return 1
      ;;
  esac
}

sdh_build_linked_intake_order_section() {
  local repo="$1"
  local manifest="$2"
  local view_path="${3:-Lastenheft_Abarbeitungsreihenfolge.md}"
  local target_count manifest_index path resolved_path role status intake_link display_position manifest_relative
  local dependency_count dependency_index from resolved_from kind binding dependency_link dependencies feature_cell

  command -v jq >/dev/null 2>&1 || { sdh_log 'LIE002: jq fehlt / jq is missing' >&2; return 1; }
  manifest_relative="$(sdh_repository_relative_from_absolute_path "$repo" "$manifest")" || return 1
  sdh_validate_linked_intake_manifest "$repo" "$manifest_relative" || return 1

  target_count="$(sdh_jq '.orderedTargets | length' "$manifest")"
  cat <<'EOF'
<!-- secure-development-hardening-order:start -->
## Verlinkte Lastenheft-Reihenfolge / Linked Requirements Order

Diese Tabelle wird aus dem kanonischen Series-Manifest und ausdruecklicher Feature-Evidence erzeugt. Vollstaendige Dateinamen, direkte eingehende Kanten und sichtbare Positionen bleiben erhalten. Manuelle Abschnitte ausserhalb dieses Markers bleiben unberuehrt.

*This table is generated from the canonical series manifest and explicit feature evidence. Complete filenames, direct incoming edges, and visible positions are preserved. Manual sections outside this marker remain unchanged.*

| Position | Status | Lastenheft/Intake | Abhängigkeiten / Dependencies | Spec-Kit-Feature |
|---:|---|---|---|---|
EOF

  for ((manifest_index = 1; manifest_index <= target_count; manifest_index++)); do
    path="$(sdh_jq -r --argjson index "$((manifest_index - 1))" '.orderedTargets[$index].path' "$manifest")"
    role="$(sdh_jq -r --argjson index "$((manifest_index - 1))" '.orderedTargets[$index].role' "$manifest")"
    status="$(sdh_jq -r --argjson index "$((manifest_index - 1))" '.orderedTargets[$index].status' "$manifest")"
    [ -n "$role" ] || { sdh_log "LIE002: Rolle fehlt / role is missing: $path" >&2; return 1; }
    resolved_path="$(sdh_resolve_linked_intake_path "$repo" "$path")" || return 1
    intake_link="$(sdh_markdown_link "$repo" "$view_path" "$resolved_path" file)" || return 1
    display_position="$(sdh_display_position "$repo/$resolved_path" "$manifest_index")"

    dependency_count="$(sdh_jq --arg target "$path" '[.dependencies[] | select(.to == $target)] | length' "$manifest")"
    dependencies=""
    for ((dependency_index = 0; dependency_index < dependency_count; dependency_index++)); do
      from="$(sdh_jq -r --arg target "$path" --argjson index "$dependency_index" '[.dependencies[] | select(.to == $target)][$index].from' "$manifest")"
      kind="$(sdh_jq -r --arg target "$path" --argjson index "$dependency_index" '[.dependencies[] | select(.to == $target)][$index].kind' "$manifest")"
      binding="$(sdh_jq -r --arg target "$path" --argjson index "$dependency_index" '[.dependencies[] | select(.to == $target)][$index].binding' "$manifest")"
      sdh_jq -e --arg endpoint "$from" 'any(.orderedTargets[]; .path == $endpoint)' "$manifest" >/dev/null \
        || { sdh_log "LIE007: unbekannter Dependency-Endpoint / unknown dependency endpoint: $from" >&2; return 1; }
      resolved_from="$(sdh_resolve_linked_intake_path "$repo" "$from")" || return 1
      dependency_link="$(sdh_markdown_link "$repo" "$view_path" "$resolved_from" file)" || return 1
      [ -z "$dependencies" ] || dependencies="${dependencies}<br>"
      dependencies="${dependencies}${dependency_link} → current (\`$(sdh_markdown_text "$kind")\`, binding: ${binding})"
    done
    [ -n "$dependencies" ] || dependencies='— (Root / keine direkte Abhängigkeit)'
    feature_cell="$(sdh_feature_cell "$repo" "$view_path" "$path" "$status" "$manifest" "$resolved_path")" || return 1
    printf '| %s | %s | %s | %s | %s |\n' "$display_position" "$(sdh_markdown_text "$status")" "$intake_link" "$dependencies" "$feature_cell"
  done

  cat <<'EOF'
<!-- secure-development-hardening-order:end -->
EOF
}

sdh_build_order_section() {
  local repo="$1"
  local view_path="${2:-Lastenheft_Abarbeitungsreihenfolge.md}"
  local manifest

  manifest="$(sdh_find_intake_series_manifest "$repo" 2>/dev/null || true)"
  if [ -n "$manifest" ]; then
    sdh_build_linked_intake_order_section "$repo" "$manifest" "$view_path"
  else
    sdh_build_legacy_order_section "$repo"
  fi
}

sdh_build_order_file_candidate() {
  local repo="$1"
  local manifest="$2"
  local output_relative="$3"
  local destination="$4"
  local output="$repo/$output_relative"
  local section_file current_file

  section_file="$(mktemp)"
  current_file="$(mktemp)"
  sdh_build_linked_intake_order_section "$repo" "$manifest" "$output_relative" > "$section_file" || { rm -f "$section_file" "$current_file"; return 1; }

  if [ -f "$output" ]; then
    sdh_jq -Rrsj 'gsub("\r\n|\r"; "\n")' "$output" > "$current_file"
    if grep -q '<!-- secure-development-hardening-order:start -->' "$current_file" \
      && grep -q '<!-- secure-development-hardening-order:end -->' "$current_file"; then
      awk -v section_file="$section_file" '
        BEGIN { while ((getline line < section_file) > 0) section = section line "\n"; in_generated = 0 }
        /<!-- secure-development-hardening-order:start -->/ { printf "%s", section; in_generated = 1; next }
        /<!-- secure-development-hardening-order:end -->/ { in_generated = 0; next }
        in_generated == 0 { print }
      ' "$current_file" > "$destination"
    else
      { cat "$current_file"; printf '\n\n'; cat "$section_file"; } > "$destination"
    fi
  else
    {
      cat <<'EOF'
# Lastenheft-Abarbeitungsreihenfolge / Requirements Processing Order

Diese Datei haelt die sichtbare Abarbeitungsreihenfolge der vorhandenen Lastenhefte fest. Sie ist eine Vorbereitung fuer spaetere Spec-Kit-Laeufe und startet selbst keinen Lauf.

*This file records the visible processing order of existing requirements documents. It prepares later Spec Kit runs and does not start a run by itself.*

EOF
      cat "$section_file"
    } > "$destination"
  fi

  awk '{ lines[NR] = $0 } END { last = NR; while (last > 0 && lines[last] ~ /^[[:space:]]*$/) last--; for (i = 1; i <= last; i++) print lines[i] }' "$destination" > "$destination.normalized"
  mv "$destination.normalized" "$destination"
  rm -f "$section_file" "$current_file"
}

sdh_view_semantics() {
  local file="$1"
  awk '
    /^\| Position \| Status \| Lastenheft\/Intake \|/ { in_table = 1; next }
    in_table && /^\|---/ { next }
    in_table && /^\|/ { print; next }
    in_table { exit }
  ' "$file" | sed -E 's/\]\([^)]*\)/\]/g'
}

sdh_restore_linked_intake_outputs() {
  local repo="$1"
  local backup_dir="$2"
  shift 2
  local index=0 relative target restore_tmp

  for relative in "$@"; do
    target="$repo/$relative"
    restore_tmp="$(dirname "$target")/.sdh-restore-$$-$index.tmp"
    if [ -f "$backup_dir/$index.file" ]; then
      cp "$backup_dir/$index.file" "$restore_tmp" && mv "$restore_tmp" "$target"
    else
      rm -f -- "$target" "$restore_tmp"
    fi
    index=$((index + 1))
  done
}

sdh_render_linked_intake_views() {
  local repo="$1"
  local manifest_relative="$2"
  local mode="$3"
  shift 3
  local -a outputs=("$@") candidates=() rechecks=() publish_temps=()
  local work_dir backup_dir input_fingerprint_before input_fingerprint_after relative target candidate recheck outside_target
  local index=0 other_index stale_count=0 replaced=0 publish_tmp fault="${SDH_TEST_FAULT:-}" vanish_path="${SDH_TEST_VANISH_PATH:-}"

  SDH_RENDER_RESULT="Failed"
  SDH_RENDER_WRITE_COUNT=0
  SDH_RENDER_ATTEMPTED_WRITES=0
  case "$mode" in check|write) ;; *) sdh_log 'LIE002: Modus muss check oder write sein / mode must be check or write' >&2; return 2 ;; esac
  [ "${#outputs[@]}" -gt 0 ] || { sdh_log 'LIE002: mindestens eine Ausgabe ist erforderlich / at least one output is required' >&2; return 2; }
  [ -d "$repo/.git" ] || { sdh_log 'LIE004: explizites Ziel ist kein Git-Repository / explicit target is not a Git repository' >&2; return 4; }
  command -v jq >/dev/null 2>&1 || { sdh_log 'LIE002: jq fehlt / jq is missing' >&2; return 2; }

  sdh_validate_linked_intake_manifest "$repo" "$manifest_relative" || return $?
  for relative in "${outputs[@]}"; do
    sdh_assert_safe_output_path "$repo" "$relative" || return $?
    if sdh_linked_intake_input_paths "$repo" "$manifest_relative" | LC_ALL=C grep -Fx -- "$relative" >/dev/null; then
      sdh_log "LIE006: Ausgabe ueberlappt kanonische Eingabe / output overlaps canonical input: $relative" >&2
      return 6
    fi
  done
  for ((index = 0; index < ${#outputs[@]}; index++)); do
    for ((other_index = index + 1; other_index < ${#outputs[@]}; other_index++)); do
      [ "${outputs[$index]}" != "${outputs[$other_index]}" ] || { sdh_log 'LIE006: doppelter Ausgabepfad / duplicate output path' >&2; return 6; }
    done
  done

  work_dir="$(mktemp -d)"
  backup_dir="$work_dir/backups"
  mkdir -p "$backup_dir"
  input_fingerprint_before="$(sdh_linked_intake_input_fingerprint "$repo" "$manifest_relative")"
  for ((index = 0; index < ${#outputs[@]}; index++)); do
    candidate="$work_dir/candidate-$index.md"
    sdh_build_order_file_candidate "$repo" "$repo/$manifest_relative" "${outputs[$index]}" "$candidate" || { rm -rf -- "$work_dir"; return 1; }
    candidates+=("$candidate")
  done

  if [ "${#outputs[@]}" -gt 1 ]; then
    for ((index = 0; index < ${#outputs[@]}; index++)); do
      target="$repo/${outputs[$index]}"
      [ -f "$target" ] || continue
      for ((other_index = index + 1; other_index < ${#outputs[@]}; other_index++)); do
        [ -f "$repo/${outputs[$other_index]}" ] || continue
        if [ "$(sdh_view_semantics "$target")" != "$(sdh_view_semantics "$repo/${outputs[$other_index]}")" ]; then
          rm -rf -- "$work_dir"
          sdh_log 'LIE011: Root- und Series-Ansicht widersprechen sich / root and series views disagree' >&2
          return 11
        fi
      done
    done
  fi

  for ((index = 0; index < ${#outputs[@]}; index++)); do
    target="$repo/${outputs[$index]}"
    if [ ! -f "$target" ] || ! cmp -s "$target" "${candidates[$index]}"; then
      stale_count=$((stale_count + 1))
    fi
  done
  if [ "$stale_count" = "0" ]; then
    rm -rf -- "$work_dir"
    SDH_RENDER_RESULT="Current"
    return 0
  fi
  if [ "$mode" = "check" ]; then
    rm -rf -- "$work_dir"
    SDH_RENDER_RESULT="Stale"
    sdh_log 'LIE009: erzeugte Ausgabe ist veraltet; Write-Modus ausfuehren / generated output is stale; run write mode' >&2
    return 9
  fi

  if [ -n "$fault" ]; then
    sdh_assert_fixture_fault_scope "$repo" || { rm -rf -- "$work_dir"; sdh_log 'LIE010: Testfehlerinjektion ist nur in isolierten Temp-Fixtures erlaubt / test fault injection is limited to isolated temporary fixtures' >&2; return 10; }
    case "$fault" in
      source-drift) printf ' ' >> "$repo/$manifest_relative" ;;
      input-drift)
        [ -n "$vanish_path" ] || { rm -rf -- "$work_dir"; sdh_log 'LIE010: Testziel fehlt / test target is missing' >&2; return 10; }
        sdh_assert_safe_repository_path "$repo" "$vanish_path" file || { rm -rf -- "$work_dir"; return 4; }
        printf '\n' >> "$repo/$vanish_path"
        ;;
      vanish-target)
        [ -n "$vanish_path" ] || { rm -rf -- "$work_dir"; sdh_log 'LIE010: Testziel fehlt / test target is missing' >&2; return 10; }
        sdh_assert_safe_repository_path "$repo" "$vanish_path" file || { rm -rf -- "$work_dir"; return 4; }
        rm -f -- "$repo/$vanish_path"
        ;;
      containment-drift)
        [ -n "$vanish_path" ] || { rm -rf -- "$work_dir"; sdh_log 'LIE010: Testziel fehlt / test target is missing' >&2; return 10; }
        sdh_assert_safe_repository_path "$repo" "$vanish_path" file || { rm -rf -- "$work_dir"; return 4; }
        outside_target="$(dirname "$repo")/.sdh-outside-$$.md"
        printf '# outside\n' > "$outside_target"
        rm -f -- "$repo/$vanish_path"
        sdh_create_test_file_symlink "$outside_target" "$repo/$vanish_path"
        ;;
      after-first-replace) ;;
      *) rm -rf -- "$work_dir"; sdh_log 'LIE010: unbekannte Testfehlerinjektion / unknown test fault injection' >&2; return 10 ;;
    esac
  fi

  input_fingerprint_after="$(sdh_linked_intake_input_fingerprint "$repo" "$manifest_relative")"
  if [ "$input_fingerprint_before" != "$input_fingerprint_after" ]; then
    rm -rf -- "$work_dir"
    sdh_log 'LIE010: kanonische Eingabemenge hat sich vor Publication geaendert / canonical input set changed before publication' >&2
    return 10
  fi
  sdh_validate_linked_intake_manifest "$repo" "$manifest_relative" || { rm -rf -- "$work_dir"; return 1; }
  for ((index = 0; index < ${#outputs[@]}; index++)); do
    recheck="$work_dir/recheck-$index.md"
    sdh_assert_safe_output_path "$repo" "${outputs[$index]}" || { rm -rf -- "$work_dir"; return 1; }
    sdh_build_order_file_candidate "$repo" "$repo/$manifest_relative" "${outputs[$index]}" "$recheck" || { rm -rf -- "$work_dir"; return 1; }
    cmp -s "${candidates[$index]}" "$recheck" || { rm -rf -- "$work_dir"; sdh_log 'LIE010: Kandidat driftete vor Publication / candidate drifted before publication' >&2; return 10; }
    rechecks+=("$recheck")
  done

  for ((index = 0; index < ${#outputs[@]}; index++)); do
    target="$repo/${outputs[$index]}"
    [ ! -f "$target" ] || cp "$target" "$backup_dir/$index.file"
    publish_tmp="$(dirname "$target")/.sdh-publish-$$-$index.tmp"
    if ! cp "${candidates[$index]}" "$publish_tmp"; then
      for candidate in "${publish_temps[@]+"${publish_temps[@]}"}"; do rm -f -- "$candidate"; done
      rm -f -- "$publish_tmp"
      rm -rf -- "$work_dir"
      return 10
    fi
    publish_temps+=("$publish_tmp")
  done

  for ((index = 0; index < ${#outputs[@]}; index++)); do
    target="$repo/${outputs[$index]}"
    if [ -f "$target" ] && cmp -s "$target" "${candidates[$index]}"; then
      rm -f -- "${publish_temps[$index]}"
      continue
    fi
    if ! mv "${publish_temps[$index]}" "$target"; then
      sdh_restore_linked_intake_outputs "$repo" "$backup_dir" "${outputs[@]}"
      for candidate in "${publish_temps[@]}"; do rm -f -- "$candidate"; done
      rm -rf -- "$work_dir"
      sdh_log 'LIE010: atomare Publication fehlgeschlagen; Altzustand wiederhergestellt / atomic publication failed; prior state restored' >&2
      return 10
    fi
    replaced=$((replaced + 1))
    SDH_RENDER_ATTEMPTED_WRITES="$replaced"
    if [ "$fault" = "after-first-replace" ] && [ "$replaced" = "1" ]; then
      sdh_restore_linked_intake_outputs "$repo" "$backup_dir" "${outputs[@]}"
      for candidate in "${publish_temps[@]}"; do rm -f -- "$candidate"; done
      rm -rf -- "$work_dir"
      SDH_RENDER_WRITE_COUNT=0
      sdh_log 'LIE010: simulierte Publication fehlgeschlagen; vollstaendiger Rollback / simulated publication failed; complete rollback' >&2
      return 10
    fi
  done

  for ((index = 0; index < ${#outputs[@]}; index++)); do
    cmp -s "$repo/${outputs[$index]}" "${candidates[$index]}" || {
      sdh_restore_linked_intake_outputs "$repo" "$backup_dir" "${outputs[@]}"
      for candidate in "${publish_temps[@]}"; do rm -f -- "$candidate"; done
      rm -rf -- "$work_dir"
      SDH_RENDER_WRITE_COUNT=0
      sdh_log 'LIE010: Post-Write-Verifikation fehlgeschlagen; vollstaendiger Rollback / post-write verification failed; complete rollback' >&2
      return 10
    }
  done
  for candidate in "${publish_temps[@]}"; do rm -f -- "$candidate"; done
  rm -rf -- "$work_dir"
  SDH_RENDER_WRITE_COUNT="$replaced"
  SDH_RENDER_RESULT="Updated"
  return 0
}



rig_usage() {
  cat <<'EOF'
render-requirements-intake-governance.sh — verlinkte Intake-Ansichten rendern

Liest ein repositoryrelatives Series-Manifest, validiert UTF-8, Pfade,
Abhaengigkeiten und explizite Feature-Nachweise und erzeugt die beiden
textorientierten Reihenfolgeansichten atomar. Jede Zeile enthaelt Position, Status,
vollstaendigen Intake-Link, direkte Abhaengigkeiten und Feature-Nachweis.
Der Standardmodus prueft nur. --write veroeffentlicht geaenderte Marker;
--dry-run und --check-only schreiben nichts.

Reads a repository-relative series manifest, validates UTF-8, paths,
dependencies, and explicit feature evidence, and atomically renders both
text-first order views. Every row follows the common five-column contract.
The default mode is check-only. --write publishes changed markers; --dry-run
and --check-only perform zero writes.

Usage:
  bash scripts/render-requirements-intake-governance.sh [--check-only|--dry-run]
  bash scripts/render-requirements-intake-governance.sh --write
  bash scripts/render-requirements-intake-governance.sh --repo PATH --manifest RELATIVE_PATH [--output RELATIVE_PATH ...] --write
EOF
}

rig_main() {
  local script_dir repo manifest="" manifest_path mode="check"
  local -a outputs=()

  script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
  repo="$(cd "$script_dir/.." && pwd -P)"

  while [ "$#" -gt 0 ]; do
    case "$1" in
      --repo)
        [ "$#" -ge 2 ] || { sdh_log 'LIE002: --repo benoetigt einen Wert / --repo requires a value' >&2; return 2; }
        repo="$(cd "$2" 2>/dev/null && pwd -P)" || { sdh_log 'LIE004: Repository fehlt / repository is missing' >&2; return 4; }
        shift 2
        ;;
      --manifest)
        [ "$#" -ge 2 ] || { sdh_log 'LIE002: --manifest benoetigt einen Wert / --manifest requires a value' >&2; return 2; }
        manifest="$2"
        shift 2
        ;;
      --output|--order-output)
        [ "$#" -ge 2 ] || { sdh_log 'LIE002: Ausgabeoption benoetigt einen Wert / output option requires a value' >&2; return 2; }
        outputs+=("$2")
        shift 2
        ;;
      --write)
        mode="write"
        shift
        ;;
      --check-only|--dry-run|--what-if)
        mode="check"
        shift
        ;;
      --allow-dirty)
        mode="write"
        shift
        ;;
      --order-only)
        shift
        ;;
      -h|--help)
        rig_usage
        return 0
        ;;
      --)
        shift
        break
        ;;
      *)
        printf 'Fehler: unbekannte Option: %s\nError: unknown option: %s\n' "$1" "$1" >&2
        return 2
        ;;
    esac
  done

  [ "$#" -eq 0 ] || { sdh_log 'LIE002: unerwartete Positionsargumente / unexpected positional arguments' >&2; return 2; }
  [ -d "$repo/.git" ] || { sdh_log 'LIE004: explizites Ziel ist kein Git-Repository / explicit target is not a Git repository' >&2; return 4; }
  if [ -z "$manifest" ]; then
    manifest_path="$(sdh_find_intake_series_manifest "$repo" 2>/dev/null || true)"
    [ -n "$manifest_path" ] || { sdh_log 'LIE004: eindeutiges Series-Manifest fehlt / unique series manifest is missing' >&2; return 4; }
    manifest="$(sdh_repository_relative_from_absolute_path "$repo" "$manifest_path")"
  fi
  if [ "${#outputs[@]}" -eq 0 ]; then
    outputs=(
      "Lastenheft_Abarbeitungsreihenfolge.md"
      "$(dirname "$manifest")/order.md"
    )
  fi

  sdh_render_linked_intake_views "$repo" "$manifest" "$mode" "${outputs[@]}"
  printf 'Ergebnis / Result: %s; Schreibvorgaenge / writes: %s\n' "$SDH_RENDER_RESULT" "$SDH_RENDER_WRITE_COUNT"
}

if [ "${BASH_SOURCE[0]}" = "$0" ]; then
  rig_main "$@"
fi
