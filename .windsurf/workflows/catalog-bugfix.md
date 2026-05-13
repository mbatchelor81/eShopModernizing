---
name: catalog-bugfix
description: Investigate and fix a catalog-management bug in a local worktree
---

# /catalog-bugfix

Use this workflow inside a Worktree-mode local Cascade session.

## Steps

1. Confirm the session is local and running in a git worktree.
2. Read the bug description and identify whether it affects MVC, WebForms, WCF, or deployment configuration.
3. Inspect the matching `AGENTS.md` file before editing.
4. Trace the catalog flow through controller/page, model, service, and configuration layers.
5. Make the smallest safe fix.
6. Add or update validation where the repository has an existing pattern; otherwise document the manual scenario.
7. Run the relevant restore/build command from `AGENTS.md`.
8. Summarize changed files, validation result, and any follow-up risk before marking the Agent Command Center card ready for review.
