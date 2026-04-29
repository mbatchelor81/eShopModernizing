#!/usr/bin/env bash
# ============================================================================
# Regression test: Scan for hardcoded credentials in committed config files.
# Returns exit code 1 if any plaintext passwords are found.
#
# Usage:
#   ./tests/scan-hardcoded-secrets.sh
#
# This script is intended to run in CI to prevent re-introduction of
# hardcoded credentials (CWE-798).
# ============================================================================
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"

FAIL=0

echo "=== Scanning for hardcoded credentials in config files ==="

# Pattern matches known default passwords and common credential patterns
# in YAML, JSON, and XML config files. Excludes .example files, SECRETS.md,
# this script itself, and ARM template parameter definitions.
MATCHES=$(grep -rn \
  --include="*.yml" \
  --include="*.yaml" \
  --include="*.json" \
  --include="*.config" \
  -E '(Password|SA_PASSWORD)\s*[=:]\s*[^$\{"\s]*Pass@word' \
  "$REPO_ROOT" \
  --exclude-dir=".git" \
  --exclude="*.example.*" \
  --exclude="scan-hardcoded-secrets.sh" \
  --exclude="SECRETS.md" \
  || true)

# Filter out ARM template parameter definitions (which describe parameters,
# not actual credentials)
FILTERED=$(echo "$MATCHES" | grep -v '"description":' | grep -v '"adminPassword"' | grep -v 'parameters.json' || true)

if [ -n "$FILTERED" ]; then
  echo ""
  echo "ERROR: Hardcoded credentials found in the following files:"
  echo "$FILTERED"
  echo ""
  FAIL=1
fi

# Also check for the specific known password string in config files
KNOWN_PW_MATCHES=$(grep -rn \
  --include="*.yml" \
  --include="*.yaml" \
  --include="*.json" \
  --include="*.config" \
  -F 'Pass@word' \
  "$REPO_ROOT" \
  --exclude-dir=".git" \
  --exclude="*.example.*" \
  --exclude="scan-hardcoded-secrets.sh" \
  --exclude="SECRETS.md" \
  || true)

# Filter out ARM template definitions
KNOWN_PW_FILTERED=$(echo "$KNOWN_PW_MATCHES" | grep -v '"description":' | grep -v '"adminPassword"' | grep -v 'parameters.json' || true)

if [ -n "$KNOWN_PW_FILTERED" ]; then
  echo ""
  echo "ERROR: Known default password 'Pass@word' found in:"
  echo "$KNOWN_PW_FILTERED"
  echo ""
  FAIL=1
fi

if [ "$FAIL" -eq 0 ]; then
  echo "PASS: No hardcoded credentials detected."
else
  echo ""
  echo "FAIL: Hardcoded credentials detected. See above for details."
  exit 1
fi
