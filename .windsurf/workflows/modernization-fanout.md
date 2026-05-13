---
name: modernization-fanout
description: Plan local-only parallel Cascade or Devin Local work for the eShop modernization demo
---

# /modernization-fanout

Use this workflow from the main workspace before starting parallel local agents.

## Steps

1. Restate the local-only constraint: use locally-executing agents (Windsurf local Cascade or Devin Local / Devin for Terminal), Worktree mode, Spaces, rules, workflows, and hooks; do not use cloud Devin sessions or any cloud-hosted agent execution path.
2. Inspect the requested modernization theme and map it to one or more repo lanes:
   - MVC catalog experience: `eShopModernizedMVCSolution/`.
   - WebForms parity or validation: `eShopModernizedWebFormsSolution/`.
   - WCF/WinForms modernization: `eShopModernizedNTier/`.
   - Deployment readiness: `Kubernetes/`, `ACI/`, `ServiceFabric/`, `VM/`, Docker Compose files.
3. Propose 2-4 independent Worktree-mode local agent sessions (Cascade or Devin Local) with non-overlapping files.
4. For each session, provide a two-sentence-or-less prompt that can be pasted into a fresh Cascade or Devin Local session, and note which runtime is recommended for that lane.
5. Recommend a Space name that groups the sessions in Agent Command Center.
6. List the validation command each worktree should run before the human reviews and merges it.

## Output format

- Space:
- Local agents:
  1. Lane, files, prompt, validation.
  2. Lane, files, prompt, validation.
  3. Lane, files, prompt, validation.
- Merge order:
- Risks:
