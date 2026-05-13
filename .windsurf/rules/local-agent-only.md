---
trigger: always_on
---

# Local-agent-only demo rule

- This demo must showcase locally-executing agents and local git worktrees only.
- Allowed agent runtimes: Windsurf local Cascade agents and Devin Local / Devin for Terminal (running on this machine).
- Do not suggest cloud Devin sessions or any cloud-hosted agent execution path.
- Use Agent Command Center as an observability and coordination surface for local agent sessions (Cascade or Devin Local).
- Use Worktree mode at the start of each parallel local agent session.
- Keep each session scoped to one reviewable lane and merge only after human diff review.
