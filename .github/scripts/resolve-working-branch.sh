#!/usr/bin/env bash
set -euo pipefail

# Force a UTF-8 locale so multibyte accent transliteration in slugify_title is
# deterministic regardless of the host (CI Ubuntu vs local macOS). Without this,
# bracket expressions over accented characters behave differently under a C locale.
if locale -a 2>/dev/null | grep -qix 'C.UTF-8'; then
  export LC_ALL=C.UTF-8
elif locale -a 2>/dev/null | grep -qiE '^en_US\.utf-?8$'; then
  export LC_ALL=en_US.UTF-8
fi

issue_number=""
issue_title=""
provided_branch=""

while [[ $# -gt 0 ]]; do
  case "$1" in
    --issue-number)
      issue_number="${2:-}"
      shift 2
      ;;
    --issue-title)
      issue_title="${2:-}"
      shift 2
      ;;
    --working-branch)
      provided_branch="${2:-}"
      shift 2
      ;;
    *)
      echo "Unknown argument: $1" >&2
      exit 1
      ;;
  esac
done

trim() {
  local value="$1"
  value="${value#${value%%[![:space:]]*}}"
  value="${value%${value##*[![:space:]]}}"
  printf '%s' "$value"
}

normalize_provided_branch() {
  local value
  value="$(trim "$1")"

  # Normalize branch refs from manual input.
  value="${value#refs/heads/}"

  # Collapse duplicate prefixes such as sdlc/sdlc/42-foo.
  while [[ "$value" == sdlc/sdlc/* ]]; do
    value="sdlc/${value#sdlc/sdlc/}"
  done

  if [[ "$value" != sdlc/* ]]; then
    value="sdlc/$value"
  fi

  printf '%s' "$value"
}

slugify_title() {
  local raw="$1"

  # Transliterate common Latin-1 accented characters to ASCII deterministically.
  # This avoids `iconv //TRANSLIT`, whose output differs across platforms
  # (e.g. macOS emits "l-egislation" where glibc emits "legislation").
  raw="$(printf '%s' "$raw" | sed -E '
    s/[àâäáãåÀÂÄÁÃÅ]/a/g
    s/[çÇ]/c/g
    s/[èéêëÈÉÊË]/e/g
    s/[ìîïíÌÎÏÍ]/i/g
    s/[ñÑ]/n/g
    s/[òôöóõøÒÔÖÓÕØ]/o/g
    s/[ùûüúÙÛÜÚ]/u/g
    s/[ýÿÝ]/y/g
    s/[æÆ]/ae/g
    s/[œŒ]/oe/g
    s/ß/ss/g
  ')"

  raw="$(printf '%s' "$raw" | tr '[:upper:]' '[:lower:]')"
  raw="$(printf '%s' "$raw" | sed -E 's/[^a-z0-9]+/-/g')"
  raw="$(printf '%s' "$raw" | sed -E 's/^-+//; s/-+$//; s/-+/-/g')"

  if [[ -z "$raw" ]]; then
    raw="issue"
  fi

  printf '%s' "${raw:0:50}"
}

# Idempotent resolution: when a branch already exists for this issue, reuse it
# verbatim. The issue number is the stable key; the title slug is NOT, because
# different runs (or LLM agents) may slugify the same title differently. This is
# the single source of truth that prevents downstream checkout mismatches.
find_existing_branch() {
  local n="$1"
  git ls-remote --heads origin "sdlc/${n}-*" 2>/dev/null \
    | sed -E 's#^.*refs/heads/##' \
    | sort \
    | head -n1 \
    || true
}

if [[ -n "$(trim "$provided_branch")" ]]; then
  printf '%s\n' "$(normalize_provided_branch "$provided_branch")"
  exit 0
fi

if [[ -z "$(trim "$issue_number")" ]]; then
  echo "Missing required argument: --issue-number" >&2
  exit 1
fi

existing="$(find_existing_branch "$(trim "$issue_number")")"
if [[ -n "$existing" ]]; then
  printf '%s\n' "$existing"
  exit 0
fi

if [[ -z "$(trim "$issue_title")" ]]; then
  echo "Missing required argument: --issue-title (when --working-branch is not provided)" >&2
  exit 1
fi

slug="$(slugify_title "$issue_title")"
printf 'sdlc/%s-%s\n' "$issue_number" "$slug"
