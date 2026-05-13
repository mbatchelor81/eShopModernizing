---
name: config-hardening
description: Review local, container, and Kubernetes configuration for modernization readiness
---

# /config-hardening

Use this workflow inside a Worktree-mode local Cascade session focused on deployment or configuration.

## Steps

1. Confirm the task stays local-only for execution; do not deploy or invoke cloud agents.
2. Identify the app path and deployment target: MVC, WebForms, WCF, Docker Compose, Kubernetes, ACI, VM, or Service Fabric.
3. Compare configuration names across `Web.config`, Docker Compose, and deployment manifests.
4. Check for hard-coded credentials or cloud-specific values that should become placeholders or environment variables.
5. Preserve Windows Container assumptions unless the task explicitly requests portability analysis.
6. Make focused config edits or produce a concise findings list if no code change is needed.
7. Run `git diff --check` and any relevant manifest or build validation available locally.
8. Summarize exactly what is safer, what remains environment-specific, and what the human should review before merging.
