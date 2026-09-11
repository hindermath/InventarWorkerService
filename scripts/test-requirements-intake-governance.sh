#!/usr/bin/env bash
# Contract tests for the linked requirements-intake order projection.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
LIB_FILE="$SCRIPT_DIR/render-requirements-intake-governance.sh"
FIXTURE_DIR="$SCRIPT_DIR/tests/linked-intake-evidence/positive"
NEGATIVE_FIXTURE="$SCRIPT_DIR/tests/linked-intake-evidence/negative/cases.json"
FEATURE_MATRIX="$SCRIPT_DIR/tests/linked-intake-evidence/feature-proof-matrix.json"
A11Y_LINEAR_DIR="$SCRIPT_DIR/tests/linked-intake-evidence/accessibility/linearized"
A11Y_DIAGNOSTICS="$SCRIPT_DIR/tests/linked-intake-evidence/accessibility/diagnostics/reference.json"

usage() {
  cat <<'EOF'
test-requirements-intake-governance.sh — Intake-Reihenfolge pruefen

Prueft die fuenf Felder, LIE001-LIE012, vollstaendige Intake-Dateinamen,
direkte Kanten, alle Feature-Proof-Arten, Check/Write, Source-/Containment-
Recheck, atomaren Multi-Output-Rollback, Idempotenz, SHA-256-Escaping und
LF-Ausgabe in isolierten Fixture-Repositories. Das Zielrepository bleibt
unveraendert.

Tests the five fields, LIE001-LIE012, complete intake filenames, direct edges,
all feature-proof kinds, check/write, source and containment rechecks, atomic
multi-output rollback, idempotence, SHA-256 escaping, and LF output in isolated
fixture repositories. The target repository remains unchanged.

Usage:
  bash scripts/test-requirements-intake-governance.sh
  bash scripts/test-requirements-intake-governance.sh --help
EOF
}

case "${1:-}" in
  -h|--help)
    usage
    exit 0
    ;;
  "")
    ;;
  *)
    printf 'Fehler: unbekannte Option: %s\nError: unknown option: %s\n' "$1" "$1" >&2
    exit 2
    ;;
esac

command -v jq >/dev/null 2>&1 || {
  printf '%s\n' 'Fehler: jq wird fuer die JSON-Fixtures benoetigt.' >&2
  printf '%s\n' 'Error: jq is required for the JSON fixtures.' >&2
  exit 1
}
[ -f "$LIB_FILE" ] || {
  printf 'Fehler: Bibliothek fehlt: %s\nError: library is missing: %s\n' "$LIB_FILE" "$LIB_FILE" >&2
  exit 1
}

# shellcheck source=/dev/null
. "$LIB_FILE"

set_fixture_manifest_hashes() {
  local repo="$1"
  local manifest="$2"
  local intake_path digest

  while IFS= read -r intake_path; do
    digest="$(sdh_sha256_file "$repo/$intake_path")"
    sdh_jq --arg intake_path "$intake_path" --arg digest "$digest" \
      '(.orderedTargets[] | select(.path == $intake_path).normalizedSha256) = $digest' \
      "$manifest" > "$manifest.tmp"
    mv "$manifest.tmp" "$manifest"
  done < <(sdh_jq -r '.orderedTargets[].path' "$manifest")
}

fixture_repo="$(mktemp -d)"
cleanup() {
  rm -rf -- "$fixture_repo"
}
trap cleanup EXIT HUP INT TERM

cases_file="$FIXTURE_DIR/cases.json"
expected_file="$FIXTURE_DIR/expected-root.md"
manifest_file="$fixture_repo/requirements/intakes/series/home-baseline-delivery/manifest.json"
actual_section="$fixture_repo/actual-section.md"
actual_table="$fixture_repo/actual-table.md"
actual_series_section="$fixture_repo/actual-series-section.md"
actual_series_table="$fixture_repo/actual-series-table.md"
mkdir -p -- "$(dirname "$manifest_file")"

sdh_jq '{
  schemaVersion,
  documentType: "IntakeSeriesManifest",
  seriesId: "linked-intake-test",
  status: "Active",
  orderedTargets: [.entries[] | {path: .intakePath, role: .role, status: .status}],
  roots: [.entries[] | select((.incomingDependencies | length) == 0) | .intakePath],
  dependencies: [.entries[].incomingDependencies[]]
}' "$cases_file" > "$manifest_file"

while IFS=$'\t' read -r intake_path display_position; do
  intake_file="$fixture_repo/$intake_path"
  mkdir -p -- "$(dirname "$intake_file")"
  printf '# Fixture\n\n**Reihenfolge:** sichtbare Position %s\n' "$display_position" > "$intake_file"
done < <(sdh_jq -r '.entries[] | [.intakePath, (.displayPosition | tostring)] | @tsv' "$cases_file")
set_fixture_manifest_hashes "$fixture_repo" "$manifest_file"

feature_dir="$fixture_repo/specs/032-linked-intake-evidence"
mkdir -p -- "$feature_dir"
cat > "$feature_dir/spec.md" <<'EOF'
# Fixture Feature

**Binding Input / Bindende Eingabe**: `Lastenheft_Verlinkte-Abarbeitungsreihenfolgen-und-Spec-Kit-Feature-Nachweise.md`
EOF

sdh_build_order_section "$fixture_repo" > "$actual_section"
sdh_build_order_section "$fixture_repo" 'requirements/intakes/series/home-baseline-delivery/order.md' > "$actual_series_section"
awk '
  /^\| Position \| Status \| Lastenheft\/Intake \|/ { in_table = 1 }
  in_table && /^\|/ { print; next }
  in_table { exit }
' "$actual_section" > "$actual_table"
awk '
  /^\| Position \| Status \| Lastenheft\/Intake \|/ { in_table = 1 }
  in_table && /^\|/ { print; next }
  in_table { exit }
' "$actual_series_section" > "$actual_series_table"

failures=0

escaped_digest='073383cddad492c767071dce2011cbc988ac02320368a4a2d5861b12a2b64b03'
normalized_digest="$(printf '\\%s  D:\\a\\_temp\\linked-intake-bash.log\n' "$escaped_digest" | sdh_normalize_sha256_output)"
if [ "$normalized_digest" != "$escaped_digest" ]; then
  printf '%s\n' 'FEHLER / FAIL: SHA-256-Dateinamen-Escape-Marker wurde nicht entfernt / SHA-256 filename escape marker was not removed' >&2
  failures=$((failures + 1))
fi
set +e
printf '%s\n' 'not-a-sha256  unsafe.log' | sdh_normalize_sha256_output >/dev/null
invalid_digest_exit=$?
set -e
if [ "$invalid_digest_exit" -eq 0 ]; then
  printf '%s\n' 'FEHLER / FAIL: ungueltige SHA-256-Ausgabe wurde akzeptiert / invalid SHA-256 output was accepted' >&2
  failures=$((failures + 1))
fi

assert_contains() {
  local description="$1"
  local expected="$2"
  if ! grep -Fq -- "$expected" "$actual_table"; then
    printf 'FEHLER / FAIL: %s\n  erwartet / expected: %s\n' "$description" "$expected" >&2
    failures=$((failures + 1))
  fi
}

assert_contains 'fuenf Spalten in Vertragsreihenfolge / five columns in contract order' \
  '| Position | Status | Lastenheft/Intake | Abhängigkeiten / Dependencies | Spec-Kit-Feature |'
assert_contains 'vollstaendiger Intake-Dateiname / complete intake filename' \
  '[Lastenheft_Verlinkte-Abarbeitungsreihenfolgen-und-Spec-Kit-Feature-Nachweise.md](Lastenheft_Verlinkte-Abarbeitungsreihenfolgen-und-Spec-Kit-Feature-Nachweise.md)'
assert_contains 'Root-Fallback / root fallback' '— (Root / keine direkte Abhängigkeit)'
assert_contains 'bindende direkte Kante / binding direct edge' '`HardCompletionGate`, binding: true'
assert_contains 'beratende direkte Kante / advisory direct edge' '`PreferredSerialOrder`, binding: false'
assert_contains 'eindeutig belegtes Feature / uniquely proven feature' \
  '[032-linked-intake-evidence](specs/032-linked-intake-evidence/)'
assert_contains 'exakter Feature-Fallback / exact feature fallback' \
  '— (kein Spec-Kit-Feature / no Spec Kit feature)'
assert_contains 'Metadatenposition 39 trotz Manifestplatz 1 / metadata position 39 despite manifest slot 1' \
  '| 39 | Completed |'

if ! cmp -s -- "$expected_file" "$actual_table"; then
  printf '%s\n' 'FEHLER / FAIL: gerenderte Root-Tabelle weicht vom positiven Vertrag ab.' >&2
  printf '%s\n' 'Rendered root table differs from the positive contract.' >&2
  diff -u -- "$expected_file" "$actual_table" >&2 || true
  failures=$((failures + 1))
fi

if ! cmp -s -- "$FIXTURE_DIR/expected-series.md" "$actual_series_table"; then
  printf '%s\n' 'FEHLER / FAIL: view-relative Series-Tabelle weicht vom positiven Vertrag ab.' >&2
  printf '%s\n' 'View-relative series table differs from the positive contract.' >&2
  diff -u -- "$FIXTURE_DIR/expected-series.md" "$actual_series_table" >&2 || true
  failures=$((failures + 1))
fi

expected_codes='LIE001 LIE002 LIE003 LIE004 LIE005 LIE006 LIE007 LIE008 LIE009 LIE010 LIE011 LIE012'
actual_codes="$(sdh_jq -r '.cases[].expectedDiagnostic' "$NEGATIVE_FIXTURE" | sort -u | tr '\n' ' ' | sed 's/ $//')"
if [ "$actual_codes" != "$expected_codes" ]; then
  printf 'FEHLER / FAIL: Diagnostikfamilien unvollstaendig / diagnostic families incomplete\n  erwartet / expected: %s\n  tatsaechlich / actual: %s\n' "$expected_codes" "$actual_codes" >&2
  failures=$((failures + 1))
fi

a11y_files=()
while IFS= read -r a11y_file; do
  a11y_files+=("$a11y_file")
done < <(find "$A11Y_LINEAR_DIR" -maxdepth 1 -type f -name '*.txt' -print | sort)
if [ "${#a11y_files[@]}" -ne 5 ]; then
  printf '%s\n' 'FEHLER / FAIL: linearisierte A11Y-Referenzen sind unvollstaendig / linearized accessibility references are incomplete' >&2
  failures=$((failures + 1))
fi
for a11y_file in "${a11y_files[@]+"${a11y_files[@]}"}"; do
  previous_line=0
  for label in 'Position:' 'Status:' 'Lastenheft/Intake:' 'Abhängigkeiten / Dependencies:' 'Spec-Kit-Feature:' 'Nächste Aktion / Next action:'; do
    current_line="$(grep -n -m1 -F -- "$label" "$a11y_file" | cut -d: -f1 || true)"
    if [ -z "$current_line" ] || [ "$current_line" -le "$previous_line" ]; then
      printf 'FEHLER / FAIL: A11Y-Source-Order fehlt / accessibility source order is missing: %s (%s)\n' "$a11y_file" "$label" >&2
      failures=$((failures + 1))
      break
    fi
    previous_line="$current_line"
  done
  if LC_ALL=C grep -q $'\033' "$a11y_file"; then
    printf 'FEHLER / FAIL: A11Y-Referenz enthaelt Farbsteuerung / accessibility reference contains colour control: %s\n' "$a11y_file" >&2
    failures=$((failures + 1))
  fi
done

if ! sdh_jq -e '
  .languageOrder == ["de","en"] and
  (.cases | length) == 12 and
  ([.cases[].code] | unique) == ["LIE001","LIE002","LIE003","LIE004","LIE005","LIE006","LIE007","LIE008","LIE009","LIE010","LIE011","LIE012"] and
  all(.cases[];
    (.de|length)>0 and (.en|length)>0 and (.remediationDe|length)>0 and (.remediationEn|length)>0 and
    ((.subject == "[redacted unsafe path]") or (.subject|startswith("/")|not)) and
    ([.subject,.de,.en,.remediationDe,.remediationEn] | join(" ") | test(("/Users/|token=|pass" + "word=|secret=|CategoryInfo|ScriptStackTrace");"i") | not))
' "$A11Y_DIAGNOSTICS" >/dev/null; then
  printf '%s\n' 'FEHLER / FAIL: bilinguale redigierte LIE001-LIE012-Referenzen sind unvollstaendig / bilingual redacted LIE001-LIE012 references are incomplete' >&2
  failures=$((failures + 1))
fi

create_transaction_fixture() {
  local case_id="$1"
  local intake_path display_position intake_file

  CASE_REPO="$fixture_repo/cases/$case_id"
  CASE_MANIFEST_REL='requirements/intakes/series/home-baseline-delivery/manifest.json'
  CASE_MANIFEST="$CASE_REPO/$CASE_MANIFEST_REL"
  CASE_ROOT_OUTPUT='Lastenheft_Abarbeitungsreihenfolge.md'
  CASE_SERIES_OUTPUT='requirements/intakes/series/home-baseline-delivery/order.md'
  mkdir -p -- "$(dirname "$CASE_MANIFEST")" "$CASE_REPO/.git"
  : > "$CASE_REPO/.sdh-linked-intake-test-fixture"
  sdh_jq '{
    schemaVersion,
    documentType: "IntakeSeriesManifest",
    seriesId: "linked-intake-test",
    status: "Active",
    orderedTargets: [.entries[] | {path: .intakePath, role: .role, status: .status}],
    roots: [.entries[] | select((.incomingDependencies | length) == 0) | .intakePath],
    dependencies: [.entries[].incomingDependencies[]]
  }' "$cases_file" > "$CASE_MANIFEST"
  while IFS=$'\t' read -r intake_path display_position; do
    intake_file="$CASE_REPO/$intake_path"
    mkdir -p -- "$(dirname "$intake_file")"
    printf '# Fixture\n\n**Reihenfolge:** sichtbare Position %s\n' "$display_position" > "$intake_file"
  done < <(sdh_jq -r '.entries[] | [.intakePath, (.displayPosition | tostring)] | @tsv' "$cases_file")
  set_fixture_manifest_hashes "$CASE_REPO" "$CASE_MANIFEST"
  mkdir -p -- "$CASE_REPO/specs/032-linked-intake-evidence" "$(dirname "$CASE_REPO/$CASE_SERIES_OUTPUT")"
  printf '# Fixture Feature\n\n**Binding Input / Bindende Eingabe**: `%s`\n' \
    'Lastenheft_Verlinkte-Abarbeitungsreihenfolgen-und-Spec-Kit-Feature-Nachweise.md' \
    > "$CASE_REPO/specs/032-linked-intake-evidence/spec.md"
}

run_projection() {
  local repo="$1"
  local manifest="$2"
  local mode="$3"
  local fault="$4"
  local vanish_path="$5"
  shift 5
  local log_file="$fixture_repo/projection.log"

  set +e
  SDH_TEST_FAULT="$fault" SDH_TEST_VANISH_PATH="$vanish_path" \
    sdh_render_linked_intake_views "$repo" "$manifest" "$mode" "$@" > "$log_file" 2>&1
  PROJECTION_EXIT=$?
  set -e
  PROJECTION_OUTPUT="$(sed "s#${repo}#<REPO>#g" "$log_file")"
}

assert_projection() {
  local case_id="$1"
  local expected_code="$2"
  local expected_exit_class="$3"
  local expected_writes="$4"
  local actual_exit_class='nonzero'
  [ "$PROJECTION_EXIT" -eq 0 ] && actual_exit_class='zero'
  if [ "$actual_exit_class" != "$expected_exit_class" ] \
    || [ "${SDH_RENDER_WRITE_COUNT:-unset}" != "$expected_writes" ] \
    || { [ -n "$expected_code" ] && ! grep -Fq -- "$expected_code" <<< "$PROJECTION_OUTPUT"; }; then
    printf 'FEHLER / FAIL: %s\n  erwartet / expected: code=%s exit=%s writes=%s\n  tatsaechlich / actual: exit=%s writes=%s output=%s\n' \
      "$case_id" "${expected_code:-none}" "$expected_exit_class" "$expected_writes" \
      "$PROJECTION_EXIT" "${SDH_RENDER_WRITE_COUNT:-unset}" "$PROJECTION_OUTPUT" >&2
    failures=$((failures + 1))
  fi
}

if ! declare -F sdh_render_linked_intake_views >/dev/null; then
  printf '%s\n' 'FEHLER / FAIL: oeffentliche Check-/Write-Transaktionsfunktion fehlt / public check/write transaction function is missing' >&2
  failures=$((failures + 1))
else
  create_transaction_fixture transaction
  run_projection "$CASE_REPO" "$CASE_MANIFEST_REL" write '' '' "$CASE_ROOT_OUTPUT" "$CASE_SERIES_OUTPUT"
  assert_projection 'write-update' '' zero 2
  root_hash_before="$(sdh_sha256_file "$CASE_REPO/$CASE_ROOT_OUTPUT")"
  series_hash_before="$(sdh_sha256_file "$CASE_REPO/$CASE_SERIES_OUTPUT")"
  cp "$CASE_REPO/$CASE_ROOT_OUTPUT" "$fixture_repo/bash-root-before-second-write.md"
  cp "$CASE_REPO/$CASE_SERIES_OUTPUT" "$fixture_repo/bash-series-before-second-write.md"
  run_projection "$CASE_REPO" "$CASE_MANIFEST_REL" write '' '' "$CASE_ROOT_OUTPUT" "$CASE_SERIES_OUTPUT"
  assert_projection 'second-unchanged-write' '' zero 0
  if ! git diff --no-index --exit-code -- "$fixture_repo/bash-root-before-second-write.md" "$CASE_REPO/$CASE_ROOT_OUTPUT" >/dev/null \
    || ! git diff --no-index --exit-code -- "$fixture_repo/bash-series-before-second-write.md" "$CASE_REPO/$CASE_SERIES_OUTPUT" >/dev/null; then
    printf '%s\n' 'FEHLER / FAIL: zweiter Bash-Write ist nicht byteidentisch / second Bash write is not byte-identical' >&2
    failures=$((failures + 1))
  fi
  run_projection "$CASE_REPO" "$CASE_MANIFEST_REL" check '' '' "$CASE_ROOT_OUTPUT" "$CASE_SERIES_OUTPUT"
  assert_projection 'check-current' '' zero 0

  sed 's/| 39 | Completed |/| 39 | Broken |/' "$CASE_REPO/$CASE_ROOT_OUTPUT" > "$CASE_REPO/$CASE_ROOT_OUTPUT.tmp"
  mv "$CASE_REPO/$CASE_ROOT_OUTPUT.tmp" "$CASE_REPO/$CASE_ROOT_OUTPUT"
  run_projection "$CASE_REPO" "$CASE_MANIFEST_REL" check '' '' "$CASE_ROOT_OUTPUT"
  assert_projection 'stale-check-output' 'LIE009' nonzero 0
  run_projection "$CASE_REPO" "$CASE_MANIFEST_REL" write '' '' "$CASE_ROOT_OUTPUT"
  assert_projection 'write-repair' '' zero 1

  sed 's/| 39 | Completed |/| 39 | Broken |/' "$CASE_REPO/$CASE_ROOT_OUTPUT" > "$CASE_REPO/$CASE_ROOT_OUTPUT.tmp"
  mv "$CASE_REPO/$CASE_ROOT_OUTPUT.tmp" "$CASE_REPO/$CASE_ROOT_OUTPUT"
  run_projection "$CASE_REPO" "$CASE_MANIFEST_REL" write source-drift '' "$CASE_ROOT_OUTPUT"
  assert_projection 'source-hash-recheck' 'LIE010' nonzero 0

  create_transaction_fixture input-drift
  run_projection "$CASE_REPO" "$CASE_MANIFEST_REL" write input-drift \
    'requirements/intakes/active/Lastenheft_Einzelkante.md' "$CASE_ROOT_OUTPUT"
  assert_projection 'complete-input-hash-recheck' 'LIE010' nonzero 0

  create_transaction_fixture input-overlap
  manifest_hash_before="$(sdh_sha256_file "$CASE_MANIFEST")"
  run_projection "$CASE_REPO" "$CASE_MANIFEST_REL" write '' '' "$CASE_MANIFEST_REL"
  assert_projection 'canonical-input-output-overlap' 'LIE006' nonzero 0
  manifest_hash_after="$(sdh_sha256_file "$CASE_MANIFEST")"
  if [ "$manifest_hash_before" != "$manifest_hash_after" ]; then
    printf '%s\n' 'FEHLER / FAIL: Overlap-Ablehnung veraenderte kanonische Eingabe / overlap rejection changed canonical input' >&2
    failures=$((failures + 1))
  fi

  create_transaction_fixture missing-logical-overlap
  logical_overlap_path="$(sdh_jq -r '.orderedTargets[1].path' "$CASE_MANIFEST")"
  stamped_overlap_path="${logical_overlap_path%.md}.123-output-overlap.md"
  mv "$CASE_REPO/$logical_overlap_path" "$CASE_REPO/$stamped_overlap_path"
  stamped_overlap_hash="$(sdh_sha256_file "$CASE_REPO/$stamped_overlap_path")"
  mkdir -p -- "$CASE_REPO/specs/123-output-overlap"
  sdh_jq -n \
    --arg accepted_path "$stamped_overlap_path" \
    --arg accepted_hash "$stamped_overlap_hash" \
    '{status:"Completed",closeout:{mergeOrPublication:"Completed",defaultBranchSync:"Completed",finalValidation:"Completed"},acceptedArtifacts:[{path:$accepted_path,sha256:$accepted_hash}]}' \
    > "$CASE_REPO/specs/123-output-overlap/autonomous-run-state.json"
  run_projection "$CASE_REPO" "$CASE_MANIFEST_REL" write '' '' "$logical_overlap_path"
  assert_projection 'missing-logical-input-output-overlap' 'LIE006' nonzero 0
  if [ -e "$CASE_REPO/$logical_overlap_path" ]; then
    printf '%s\n' 'FEHLER / FAIL: Overlap-Ablehnung erzeugte den fehlenden logischen Intake / overlap rejection created the missing logical intake' >&2
    failures=$((failures + 1))
  fi

  create_transaction_fixture vanished
  run_projection "$CASE_REPO" "$CASE_MANIFEST_REL" write vanish-target \
    'requirements/intakes/active/Lastenheft_Einzelkante.md' "$CASE_ROOT_OUTPUT"
  assert_projection 'vanished-target-recheck' 'LIE004' nonzero 0

  create_transaction_fixture containment
  run_projection "$CASE_REPO" "$CASE_MANIFEST_REL" write containment-drift \
    'requirements/intakes/active/Lastenheft_Einzelkante.md' "$CASE_ROOT_OUTPUT"
  assert_projection 'target-containment-recheck' 'LIE005' nonzero 0

  create_transaction_fixture rollback
  run_projection "$CASE_REPO" "$CASE_MANIFEST_REL" write '' '' "$CASE_ROOT_OUTPUT" "$CASE_SERIES_OUTPUT"
  assert_projection 'rollback-baseline-write' '' zero 2
  root_hash_before="$(sdh_sha256_file "$CASE_REPO/$CASE_ROOT_OUTPUT")"
  series_hash_before="$(sdh_sha256_file "$CASE_REPO/$CASE_SERIES_OUTPUT")"
  sdh_jq '.orderedTargets[0].status = "Eligible"' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp"
  mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
  run_projection "$CASE_REPO" "$CASE_MANIFEST_REL" write after-first-replace '' "$CASE_ROOT_OUTPUT" "$CASE_SERIES_OUTPUT"
  assert_projection 'simulated-publish-failure' 'LIE010' nonzero 0
  root_hash_after="$(sdh_sha256_file "$CASE_REPO/$CASE_ROOT_OUTPUT")"
  series_hash_after="$(sdh_sha256_file "$CASE_REPO/$CASE_SERIES_OUTPUT")"
  if [ "$root_hash_before" != "$root_hash_after" ] || [ "$series_hash_before" != "$series_hash_after" ]; then
    printf '%s\n' 'FEHLER / FAIL: atomarer Rollback stellte den vollstaendigen Altzustand nicht her / atomic rollback did not restore the complete prior state' >&2
    failures=$((failures + 1))
  fi
  if find "$CASE_REPO" -type f -name '.sdh-*-*.tmp' -print -quit | grep -q .; then
    printf '%s\n' 'FEHLER / FAIL: Rollback hinterliess renderer-owned temporaere Dateien / rollback left renderer-owned temporary files' >&2
    failures=$((failures + 1))
  fi

  create_transaction_fixture root-series-drift
  run_projection "$CASE_REPO" "$CASE_MANIFEST_REL" write '' '' "$CASE_ROOT_OUTPUT" "$CASE_SERIES_OUTPUT"
  assert_projection 'root-series-drift-baseline' '' zero 2
  sed 's/| 39 | Completed |/| 39 | Broken |/' "$CASE_REPO/$CASE_SERIES_OUTPUT" > "$CASE_REPO/$CASE_SERIES_OUTPUT.tmp"
  mv "$CASE_REPO/$CASE_SERIES_OUTPUT.tmp" "$CASE_REPO/$CASE_SERIES_OUTPUT"
  run_projection "$CASE_REPO" "$CASE_MANIFEST_REL" check '' '' "$CASE_ROOT_OUTPUT" "$CASE_SERIES_OUTPUT"
  assert_projection 'root-series-drift' 'LIE011' nonzero 0

  create_transaction_fixture escaping
  sdh_jq '.orderedTargets[0].status = "<status data-safe=\"yes\">Com|pleted & Ready</status> [x](y)\\z"' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp"
  mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
  run_projection "$CASE_REPO" "$CASE_MANIFEST_REL" write '' '' "$CASE_ROOT_OUTPUT" "$CASE_SERIES_OUTPUT"
  assert_projection 'markdown-escaping' '' zero 2
  if ! grep -Fq -- '&lt;status data-safe="yes"&gt;Com\|pleted &amp; Ready&lt;/status&gt; \[x\]\(y\)\\z' "$CASE_REPO/$CASE_ROOT_OUTPUT" \
    || ! grep -Fq -- '<br>' "$CASE_REPO/$CASE_ROOT_OUTPUT" \
    || grep -Fq -- '<status' "$CASE_REPO/$CASE_ROOT_OUTPUT" \
    || grep -Fq -- '<kind>' "$CASE_REPO/$CASE_ROOT_OUTPUT"; then
    printf '%s\n' 'FEHLER / FAIL: nicht vertrauenswuerdige Tabellenfelder sind nicht vollstaendig escaped / untrusted table fields are not fully escaped' >&2
    failures=$((failures + 1))
  fi

  if LC_ALL=C grep -q $'\r' "$CASE_REPO/$CASE_ROOT_OUTPUT" "$CASE_REPO/$CASE_SERIES_OUTPUT"; then
    printf '%s\n' 'FEHLER / FAIL: Ausgabe enthaelt CR statt stabiler LF-Zeilenenden / output contains CR instead of stable LF line endings' >&2
    failures=$((failures + 1))
  fi

  run_public_cli_case() {
    local case_name="$1"
    shift
    PUBLIC_STDOUT="$fixture_repo/${case_name}.stdout"
    PUBLIC_STDERR="$fixture_repo/${case_name}.stderr"
    set +e
    bash "$REPO_ROOT/scripts/render-requirements-intake-governance.sh" "$@" \
      >"$PUBLIC_STDOUT" 2>"$PUBLIC_STDERR"
    PUBLIC_EXIT=$?
    set -e
  }

  assert_public_diagnostic() {
    local case_name="$1"
    local expected_code="$2"
    local expected_safe_subject="$3"
    local forbidden_value="$4"
    if [ "$PUBLIC_EXIT" -eq 0 ] \
      || ! grep -Fq -- "$expected_code" "$PUBLIC_STDERR" \
      || ! grep -Fq -- "$expected_safe_subject" "$PUBLIC_STDERR" \
      || ! grep -Fq -- 'Abhilfe / remediation:' "$PUBLIC_STDERR" \
      || grep -Fq -- "$expected_code" "$PUBLIC_STDOUT" \
      || { [ -n "$forbidden_value" ] && grep -Fq -- "$forbidden_value" "$PUBLIC_STDOUT"; } \
      || { [ -n "$forbidden_value" ] && grep -Fq -- "$forbidden_value" "$PUBLIC_STDERR"; } \
      || grep -Eq -- '(/Users/|/home/|CategoryInfo|FullyQualifiedErrorId|ScriptStackTrace|at .*prepare-secure-development-hardening)' "$PUBLIC_STDOUT" "$PUBLIC_STDERR" \
      || LC_ALL=C grep -q $'\033' "$PUBLIC_STDOUT" "$PUBLIC_STDERR"; then
      printf 'FEHLER / FAIL: oeffentliche Bash-Diagnostik ist nicht sicher: %s / public Bash diagnostic is unsafe: %s\n' "$case_name" "$case_name" >&2
      failures=$((failures + 1))
    fi
  }

  create_transaction_fixture public-cli-redaction
  hostile_manifest="/Us""ers/synthetic-private/credentials/token=not-a-real-secret"$'\n'"manifest.json"
  run_public_cli_case public-lie003 \
    --repo "$CASE_REPO" --order-only --manifest "$hostile_manifest" \
    --order-output "$CASE_ROOT_OUTPUT" --dry-run
  assert_public_diagnostic public-lie003 LIE003 'repository-relative' 'token=not-a-real-secret'

  run_public_cli_case public-lie004-safe \
    --repo "$CASE_REPO" --order-only --manifest 'missing-manifest.md' \
    --order-output "$CASE_ROOT_OUTPUT" --dry-run
  assert_public_diagnostic public-lie004-safe LIE004 'missing-manifest.md' ''

  run_public_cli_case public-lie004-credential \
    --repo "$CASE_REPO" --order-only --manifest 'token=fixture-secret-value.md' \
    --order-output "$CASE_ROOT_OUTPUT" --dry-run
  assert_public_diagnostic public-lie004-credential LIE004 'token=[redacted]' 'fixture-secret-value'

  create_transaction_fixture public-lie007-safe
  sdh_jq '.dependencies[0].from = "requirements/intakes/active/missing.md"' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp"
  mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
  run_public_cli_case public-lie007-safe \
    --repo "$CASE_REPO" --order-only --manifest "$CASE_MANIFEST_REL" \
    --order-output "$CASE_ROOT_OUTPUT" --dry-run
  assert_public_diagnostic public-lie007-safe LIE007 'requirements/intakes/active/missing.md' ''

  create_transaction_fixture public-lie007-credential
  sdh_jq '.dependencies[0].from = "token=fixture-secret-value.md"' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp"
  mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
  run_public_cli_case public-lie007-credential \
    --repo "$CASE_REPO" --order-only --manifest "$CASE_MANIFEST_REL" \
    --order-output "$CASE_ROOT_OUTPUT" --dry-run
  assert_public_diagnostic public-lie007-credential LIE007 'token=[redacted]' 'fixture-secret-value'

  public_help="$(bash "$REPO_ROOT/scripts/render-requirements-intake-governance.sh" --help)"
  if ! grep -Fq -- 'Position, Status' <<< "$public_help" \
    || ! grep -Fq -- 'five-column' <<< "$public_help" \
    || grep -Fq -- '/Users/' <<< "$public_help"; then
    printf '%s\n' 'FEHLER / FAIL: oeffentliche Bash-Hilfe ist unvollstaendig oder enthaelt einen privaten Pfad / public Bash help is incomplete or contains a private path' >&2
    failures=$((failures + 1))
  fi

  create_transaction_fixture cross-parity-bash
  parity_bash_repo="$CASE_REPO"
  parity_manifest_rel="$CASE_MANIFEST_REL"
  parity_output="$CASE_ROOT_OUTPUT"
  create_transaction_fixture cross-parity-powershell
  parity_pwsh_repo="$CASE_REPO"
  run_public_cli_case cross-parity-bash-write \
    --repo "$parity_bash_repo" --order-only --manifest "$parity_manifest_rel" \
    --order-output "$parity_output" --allow-dirty
  parity_bash_exit="$PUBLIC_EXIT"
  parity_pwsh_stdout="$fixture_repo/cross-parity-powershell-write.stdout"
  parity_pwsh_stderr="$fixture_repo/cross-parity-powershell-write.stderr"
  set +e
  pwsh -NoProfile -File "$REPO_ROOT/scripts/render-requirements-intake-governance.ps1" \
    -Repo "$parity_pwsh_repo" -OrderOnly -Manifest "$parity_manifest_rel" \
    -OrderOutput "$parity_output" -AllowDirty \
    >"$parity_pwsh_stdout" 2>"$parity_pwsh_stderr"
  parity_pwsh_exit=$?
  set -e
  if [ "$parity_bash_exit" -ne 0 ] || [ "$parity_pwsh_exit" -ne 0 ] \
    || ! cmp -s -- "$parity_bash_repo/$parity_output" "$parity_pwsh_repo/$parity_output"; then
    printf '%s\n' 'FEHLER / FAIL: Bash-/PowerShell-Erfolgspfad ist nicht exit- und byteidentisch / Bash and PowerShell success path is not exit- and byte-equivalent' >&2
    failures=$((failures + 1))
  fi

  for parity_repo in "$parity_bash_repo" "$parity_pwsh_repo"; do
    parity_manifest="$parity_repo/$parity_manifest_rel"
    sdh_jq '.dependencies[0].kind = "UnknownGate"' "$parity_manifest" > "$parity_manifest.tmp"
    mv "$parity_manifest.tmp" "$parity_manifest"
  done
  run_public_cli_case cross-parity-bash-diagnostic \
    --repo "$parity_bash_repo" --order-only --manifest "$parity_manifest_rel" \
    --order-output "$parity_output" --dry-run
  parity_bash_exit="$PUBLIC_EXIT"
  parity_bash_stderr="$PUBLIC_STDERR"
  parity_pwsh_stdout="$fixture_repo/cross-parity-powershell-diagnostic.stdout"
  parity_pwsh_stderr="$fixture_repo/cross-parity-powershell-diagnostic.stderr"
  set +e
  pwsh -NoProfile -File "$REPO_ROOT/scripts/render-requirements-intake-governance.ps1" \
    -Repo "$parity_pwsh_repo" -OrderOnly -Manifest "$parity_manifest_rel" \
    -OrderOutput "$parity_output" -WhatIf \
    >"$parity_pwsh_stdout" 2>"$parity_pwsh_stderr"
  parity_pwsh_exit=$?
  set -e
  if [ "$parity_bash_exit" -eq 0 ] || [ "$parity_pwsh_exit" -eq 0 ] \
    || ! grep -Fq -- 'LIE007' "$parity_bash_stderr" \
    || ! grep -Fq -- 'LIE007' "$parity_pwsh_stderr"; then
    printf '%s\n' 'FEHLER / FAIL: Bash-/PowerShell-Negativpfad liefert keine gleiche Exitklasse und Diagnose / Bash and PowerShell negative path lacks matching exit class and diagnostic' >&2
    failures=$((failures + 1))
  fi

  create_transaction_fixture manual-content
  run_projection "$CASE_REPO" "$CASE_MANIFEST_REL" write '' '' "$CASE_ROOT_OUTPUT"
  assert_projection 'manual-content-baseline' '' zero 1
  {
    printf '# Manueller Prefix\rPrefix bleibt\r'
    cat "$CASE_REPO/$CASE_ROOT_OUTPUT"
    printf '\r## Manueller Suffix\rSuffix bleibt\r'
  } > "$CASE_REPO/$CASE_ROOT_OUTPUT.manual"
  mv "$CASE_REPO/$CASE_ROOT_OUTPUT.manual" "$CASE_REPO/$CASE_ROOT_OUTPUT"
  run_projection "$CASE_REPO" "$CASE_MANIFEST_REL" write '' '' "$CASE_ROOT_OUTPUT"
  assert_projection 'manual-content-normalization' '' zero 1
  if ! grep -Fxq '# Manueller Prefix' "$CASE_REPO/$CASE_ROOT_OUTPUT" \
    || ! grep -Fxq 'Prefix bleibt' "$CASE_REPO/$CASE_ROOT_OUTPUT" \
    || ! grep -Fxq '## Manueller Suffix' "$CASE_REPO/$CASE_ROOT_OUTPUT" \
    || ! grep -Fxq 'Suffix bleibt' "$CASE_REPO/$CASE_ROOT_OUTPUT" \
    || LC_ALL=C grep -q $'\r' "$CASE_REPO/$CASE_ROOT_OUTPUT"; then
    printf '%s\n' 'FEHLER / FAIL: manueller Prefix/Suffix wurde bei CR-Normalisierung nicht bytegetreu als Text bewahrt / manual prefix/suffix was not preserved as text during CR normalization' >&2
    failures=$((failures + 1))
  fi

  while IFS=$'\t' read -r case_id expected_code; do
    case "$case_id" in
      invalid-utf8)
        create_transaction_fixture "$case_id"
        printf '\303\050' > "$CASE_MANIFEST"
        ;;
      embedded-nul)
        create_transaction_fixture "$case_id"
        printf '{\000"x":1}' > "$CASE_MANIFEST"
        ;;
      missing-required-field)
        create_transaction_fixture "$case_id"
        sdh_jq 'del(.schemaVersion)' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp" && mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
        ;;
      wrong-type)
        create_transaction_fixture "$case_id"
        sdh_jq '.orderedTargets = {}' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp" && mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
        ;;
      absolute-posix|absolute-drive|absolute-unc|parent-traversal|option-path|missing-target|wrong-filesystem-type|symlink-escape)
        create_transaction_fixture "$case_id"
        bad_path="$(sdh_jq -r --arg id "$case_id" '.cases[] | select(.id == $id).path' "$NEGATIVE_FIXTURE")"
        if [ "$case_id" = 'symlink-escape' ]; then
          outside_file="$fixture_repo/outside.md"
          printf '# outside\n' > "$outside_file"
          mkdir -p "$CASE_REPO/fixtures"
          sdh_create_test_file_symlink "$outside_file" "$CASE_REPO/$bad_path"
        fi
        sdh_jq --arg path "$bad_path" '.orderedTargets[0].path = $path' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp" && mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
        ;;
      logical-and-stamped-both-present)
        create_transaction_fixture "$case_id"
        logical_path="$(sdh_jq -r '.orderedTargets[1].path' "$CASE_MANIFEST")"
        stamped_path="${logical_path%.md}.123-shadow.md"
        printf '# Stamped shadow\n' > "$CASE_REPO/$stamped_path"
        ;;
      duplicate-identity)
        create_transaction_fixture "$case_id"
        sdh_jq '.orderedTargets += [.orderedTargets[0]]' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp" && mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
        ;;
      duplicate-position)
        create_transaction_fixture "$case_id"
        printf '# Fixture\n\n**Reihenfolge:** sichtbare Position 39\n' > "$CASE_REPO/requirements/intakes/active/Lastenheft_Überblick.md"
        set_fixture_manifest_hashes "$CASE_REPO" "$CASE_MANIFEST"
        ;;
      unknown-endpoint)
        create_transaction_fixture "$case_id"
        sdh_jq '.dependencies += [{from:"requirements/intakes/active/missing.md",to:.orderedTargets[0].path,kind:"HardCompletionGate",binding:true}]' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp" && mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
        ;;
      missing-normalized-hash)
        create_transaction_fixture "$case_id"
        sdh_jq 'del(.orderedTargets[0].normalizedSha256)' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp" && mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
        ;;
      target-hash-mismatch)
        create_transaction_fixture "$case_id"
        sdh_jq '.orderedTargets[0].normalizedSha256 = ("0" * 64)' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp" && mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
        ;;
      non-string-target-field)
        create_transaction_fixture "$case_id"
        sdh_jq '.orderedTargets[0].status = 7' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp" && mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
        ;;
      non-string-edge-field)
        create_transaction_fixture "$case_id"
        sdh_jq '.dependencies[0].kind = 7' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp" && mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
        ;;
      unknown-edge-kind)
        create_transaction_fixture "$case_id"
        sdh_jq '.dependencies[0].kind = "UnknownGate"' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp" && mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
        ;;
      binding-mismatch)
        create_transaction_fixture "$case_id"
        sdh_jq '(.dependencies[] | select(.kind == "PreferredSerialOrder").binding) = true' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp" && mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
        ;;
      backward-edge)
        create_transaction_fixture "$case_id"
        sdh_jq '
          .dependencies[-1].from = .orderedTargets[3].path
          | .dependencies[-1].to = .orderedTargets[1].path
          | .roots = [.orderedTargets[2].path, .orderedTargets[3].path]
        ' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp" && mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
        ;;
      self-edge)
        create_transaction_fixture "$case_id"
        sdh_jq '.dependencies += [{from:.orderedTargets[1].path,to:.orderedTargets[1].path,kind:"HardCompletionGate",binding:true}]' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp" && mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
        ;;
      dependency-cycle)
        create_transaction_fixture "$case_id"
        sdh_jq '.dependencies += [{from:.orderedTargets[0].path,to:.orderedTargets[1].path,kind:"HardCompletionGate",binding:true}]' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp" && mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
        ;;
      incorrect-root-set)
        create_transaction_fixture "$case_id"
        sdh_jq '.roots += [.orderedTargets[0].path]' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp" && mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
        ;;
      missing-nested-directory)
        create_transaction_fixture "$case_id"
        sdh_jq '.orderedTargets[0].path = "requirements/intakes/missing/deep/Lastenheft.md"' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp" && mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
        ;;
      nonterminal-archive-proof|archive-hash-mismatch)
        create_transaction_fixture "$case_id"
        logical_path="$(sdh_jq -r '.orderedTargets[1].path' "$CASE_MANIFEST")"
        stamped_path="${logical_path%.md}.123-archive-proof.md"
        mv "$CASE_REPO/$logical_path" "$CASE_REPO/$stamped_path"
        mkdir -p "$CASE_REPO/specs/123-archive-proof"
        stamped_hash="$(sdh_sha256_file "$CASE_REPO/$stamped_path")"
        proof_status='Completed'
        proof_hash="$stamped_hash"
        [ "$case_id" != 'nonterminal-archive-proof' ] || proof_status='Active'
        [ "$case_id" != 'archive-hash-mismatch' ] || proof_hash="$(printf '0%.0s' {1..64})"
        printf '{"status":"%s","closeout":{"mergeOrPublication":"Completed","defaultBranchSync":"Completed","finalValidation":"Completed"},"acceptedArtifacts":[{"path":"%s","sha256":"%s"}]}\n' \
          "$proof_status" "$stamped_path" "$proof_hash" > "$CASE_REPO/specs/123-archive-proof/autonomous-run-state.json"
        ;;
      multiple-feature-candidates)
        create_transaction_fixture "$case_id"
        mkdir -p "$CASE_REPO/specs/033-second-proof"
        printf '# Second\n\n**Binding Input**: `%s`\n' 'Lastenheft_Verlinkte-Abarbeitungsreihenfolgen-und-Spec-Kit-Feature-Nachweise.md' > "$CASE_REPO/specs/033-second-proof/spec.md"
        ;;
      stale-check-output|simulated-publish-failure|root-series-drift|cross-implementation-parity)
        continue
        ;;
      *)
        printf 'FEHLER / FAIL: unbekannte Negativ-Fixture / unknown negative fixture: %s\n' "$case_id" >&2
        failures=$((failures + 1))
        continue
        ;;
    esac
    run_projection "$CASE_REPO" "$CASE_MANIFEST_REL" check '' '' "$CASE_ROOT_OUTPUT"
    assert_projection "$case_id" "$expected_code" nonzero 0
  done < <(sdh_jq -r '.cases[] | [.id, .expectedDiagnostic] | @tsv' "$NEGATIVE_FIXTURE")

  feature_case_count=0
  while IFS= read -r feature_case; do
    feature_case_count=$((feature_case_count + 1))
    feature_id="$(sdh_jq -r '.id' <<< "$feature_case")"
    create_transaction_fixture "feature-$feature_id"
    intake='Lastenheft_Verlinkte-Abarbeitungsreihenfolgen-und-Spec-Kit-Feature-Nachweise.md'
    case "$feature_id" in
      feature-binding) ;;
      archived-closeout-binding)
        rm -rf "$CASE_REPO/specs/032-linked-intake-evidence"
        intake='requirements/intakes/active/Lastenheft_Überblick.md'
        archived="${intake%.md}.123-archived-proof.md"
        rm -f "$CASE_REPO/$intake"
        mkdir -p "$CASE_REPO/specs/123-archived-proof"
        printf '# Fixture\n\n**Reihenfolge:** sichtbare Position 1\n' > "$CASE_REPO/$archived"
        archived_hash="$(sdh_sha256_file "$CASE_REPO/$archived")"
        sdh_jq --arg intake "$intake" '(.orderedTargets[] | select(.path == $intake).status) = "Completed"' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp" && mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
        printf '{"status":"Completed","closeout":{"mergeOrPublication":"Completed","defaultBranchSync":"Completed","finalValidation":"Completed"},"acceptedArtifacts":[{"path":"%s","sha256":"%s"}]}\n' \
          "$archived" "$archived_hash" > "$CASE_REPO/specs/123-archived-proof/autonomous-run-state.json"
        ;;
      reviewed-legacy-mapping)
        rm -rf "$CASE_REPO/specs/032-linked-intake-evidence"
        mkdir -p "$CASE_REPO/specs/124-reviewed-legacy"
        sdh_jq --arg intake "$intake" '.featureEvidence=[{intakePath:$intake,featurePath:"specs/124-reviewed-legacy",proofKind:"ReviewedLegacyMapping",reviewed:true}]' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp" && mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
        ;;
      no-evidence)
        rm -rf "$CASE_REPO/specs"
        ;;
      ambiguous)
        mkdir -p "$CASE_REPO/specs/033-second-proof"
        printf '# Second\n\n**Binding Input**: `%s`\n' "$intake" > "$CASE_REPO/specs/033-second-proof/spec.md"
        ;;
      invalid)
        rm -rf "$CASE_REPO/specs/032-linked-intake-evidence"
        sdh_jq --arg intake "$intake" '.featureEvidence=[{intakePath:$intake,featurePath:"specs/125-missing",proofKind:"ReviewedLegacyMapping",reviewed:true}]' "$CASE_MANIFEST" > "$CASE_MANIFEST.tmp" && mv "$CASE_MANIFEST.tmp" "$CASE_MANIFEST"
        ;;
      similarity-is-not-proof)
        rm -rf "$CASE_REPO/specs/032-linked-intake-evidence"
        mkdir -p "$CASE_REPO/specs/126-similar-name"
        ;;
    esac
    run_projection "$CASE_REPO" "$CASE_MANIFEST_REL" write '' '' "$CASE_ROOT_OUTPUT"
    expected_state="$(sdh_jq -r '.expectedState' <<< "$feature_case")"
    if [ "$expected_state" = 'Ambiguous' ] || [ "$expected_state" = 'Invalid' ]; then
      assert_projection "feature-$feature_id" 'LIE008' nonzero 0
    else
      assert_projection "feature-$feature_id" '' zero 1
      case "$feature_id" in
        feature-binding) expected_feature='[032-linked-intake-evidence](specs/032-linked-intake-evidence/)' ;;
        archived-closeout-binding) expected_feature='[123-archived-proof](specs/123-archived-proof/)' ;;
        reviewed-legacy-mapping) expected_feature='[124-reviewed-legacy](specs/124-reviewed-legacy/)' ;;
        no-evidence|similarity-is-not-proof) expected_feature='— (kein Spec-Kit-Feature / no Spec Kit feature)' ;;
      esac
      if ! grep -Fq -- "$expected_feature" "$CASE_REPO/$CASE_ROOT_OUTPUT"; then
        printf 'FEHLER / FAIL: feature-%s erwartete Zelle fehlt / expected cell is missing: %s\n' "$feature_id" "$expected_feature" >&2
        failures=$((failures + 1))
      fi
    fi
  done < <(sdh_jq -c '.cases[]' "$FEATURE_MATRIX")
  [ "$feature_case_count" -eq 7 ] || { printf '%s\n' 'FEHLER / FAIL: Feature-Proof-Matrix ist unvollstaendig / feature proof matrix is incomplete' >&2; failures=$((failures + 1)); }
fi

if [ "$failures" -ne 0 ]; then
  printf 'Ergebnis / Result: %s fehlgeschlagene Assertion(en) / failed assertion(s)\n' "$failures" >&2
  exit 1
fi

printf '%s\n' 'PASS: Bash-Vertrag fuer verlinkte Intake-Evidence ist erfuellt.'
printf '%s\n' 'PASS: Bash contract for linked intake evidence is satisfied.'
