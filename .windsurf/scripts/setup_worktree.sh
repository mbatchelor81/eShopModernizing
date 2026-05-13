#!/usr/bin/env bash
set -euo pipefail

echo "Preparing eShopModernizing Windsurf worktree"

if [[ -n "${ROOT_WORKSPACE_PATH:-}" && -d "$ROOT_WORKSPACE_PATH" ]]; then
  for file in .env .env.local .env.development; do
    if [[ -f "$ROOT_WORKSPACE_PATH/$file" && ! -f "$file" ]]; then
      cp "$ROOT_WORKSPACE_PATH/$file" "$file"
      echo "Copied $file from root workspace"
    fi
  done
fi

if command -v git >/dev/null 2>&1; then
  git worktree list || true
fi

if command -v nuget >/dev/null 2>&1; then
  for solution in \
    eShopModernizedMVCSolution/eShopModernizedMVC.sln \
    eShopModernizedWebFormsSolution/eShopModernizedWebForms.sln \
    eShopModernizedNTier/eShopModernizedNTier.sln; do
    if [[ -f "$solution" ]]; then
      echo "Restoring $solution"
      nuget restore "$solution"
    fi
  done
else
  echo "nuget not found; skipping package restore"
fi

echo "Worktree ready for local Cascade agent work"
