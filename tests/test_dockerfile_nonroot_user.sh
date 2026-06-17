#!/usr/bin/env bash
# Regression test: verify all Dockerfiles specify a non-root USER directive (CWE-250 / EM-97).
# Usage: bash tests/test_dockerfile_nonroot_user.sh
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "$0")/.." && pwd)"
FAILED=0
CHECKED=0

DOCKERFILES=(
  "eShopModernizedMVCSolution/src/eShopModernizedMVC/Dockerfile"
  "eShopModernizedWebFormsSolution/src/eShopModernizedWebForms/Dockerfile"
  "eShopModernizedNTier/src/eShopWCFService/Dockerfile"
)

for df in "${DOCKERFILES[@]}"; do
  CHECKED=$((CHECKED + 1))
  FILE="$REPO_ROOT/$df"

  if [ ! -f "$FILE" ]; then
    echo "FAIL: $df — file not found"
    FAILED=$((FAILED + 1))
    continue
  fi

  if ! grep -q '^USER appuser' "$FILE"; then
    echo "FAIL: $df — missing 'USER appuser' directive"
    FAILED=$((FAILED + 1))
  else
    echo "PASS: $df — contains 'USER appuser'"
  fi
done

echo ""
echo "Results: $((CHECKED - FAILED))/$CHECKED passed"

if [ "$FAILED" -gt 0 ]; then
  echo "ERROR: $FAILED Dockerfile(s) missing non-root user directive"
  exit 1
fi

echo "All Dockerfiles specify a non-root user."
exit 0
