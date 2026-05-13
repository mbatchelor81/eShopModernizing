# Windsurf 2.0 local-agent demo: eShopModernizing

## Demo thesis

Agent Command Center turns local Cascade from one chat panel into a managed local agent fleet. This repository is now staged to prove that with local Cascade agents, git worktrees, Spaces, rules, workflows, and hooks only.

## Non-negotiable constraint

- Use Windsurf local Cascade agents only.
- Do not use Devin, Devin for Terminal, cloud Devin sessions, or any cloud execution during the live Windsurf demo.
- Use Worktree mode when starting each parallel Cascade session so every agent works in an isolated checkout.

## Why this repo works for the story

eShopModernizing has several independent modernization surfaces that are easy to split across local agents:

- ASP.NET MVC catalog experience in `eShopModernizedMVCSolution/`.
- ASP.NET WebForms parity and validation in `eShopModernizedWebFormsSolution/`.
- WCF and WinForms modernization in `eShopModernizedNTier/`.
- Deployment modernization through Docker Compose, Kubernetes, ACI, VM, and Service Fabric manifests.

The project naturally supports a fan-out pattern: one developer acts as lead, starts several local agents, watches them in Agent Command Center, and merges only the reviewed worktree outputs.

## Demo setup checklist

1. Open the repository in Windsurf 2.0.
2. Open `AGENTS.md` and the subdirectory `AGENTS.md` files to show directory-scoped instructions.
3. Open the Customizations panel and show workspace rules and workflows under `.windsurf/`.
4. Run `git worktree list` in the integrated terminal.
5. Open Agent Command Center and create a Space named `eShop Modernization Sprint`.
6. Start each Cascade session in Worktree mode from the beginning of the session.

## Recommended live demo flow

### Act 1: encode the team once

Show the root `AGENTS.md`, scoped `AGENTS.md` files, `.windsurf/rules/`, and `.windsurf/workflows/`.

Talk track:

> The team’s modernization playbook is encoded in the repo. Every local Cascade agent gets the same project rules without me repeating them.

### Act 2: plan the local fleet

In the main workspace, run:

```text
/modernization-fanout
Plan a local-only Agent Command Center demo for improving the modernized catalog app. Use three parallel Worktree-mode Cascade sessions and keep each lane on non-overlapping files.
```

Expected output: a lane plan with prompts, validation commands, merge order, and risks.

### Act 3: launch parallel local agents

Start three new Cascade sessions in Worktree mode and group them in the `eShop Modernization Sprint` Space.

#### Agent 1: MVC catalog validation

```text
/catalog-bugfix
In this local worktree, harden MVC catalog create/edit validation so impossible stock thresholds are rejected before save. Stay within eShopModernizedMVCSolution and run the MVC build command before marking ready for review.
```

Suggested files:

- `eShopModernizedMVCSolution/src/eShopModernizedMVC/Models/CatalogItem.cs`
- `eShopModernizedMVCSolution/src/eShopModernizedMVC/Controllers/CatalogController.cs`
- `eShopModernizedMVCSolution/src/eShopModernizedMVC/Views/Catalog/*.cshtml`

#### Agent 2: observability and trace cleanup

```text
/catalog-bugfix
In this local worktree, improve request tracing for the modernized MVC catalog without changing behavior. Focus on ActionTracerFilter and existing log4net/Application Insights patterns, then run the MVC build command.
```

Suggested files:

- `eShopModernizedMVCSolution/src/eShopModernizedMVC/Filters/ActionTracerFilter.cs`
- `eShopModernizedMVCSolution/src/eShopModernizedMVC/FilterConfig.cs`
- `eShopModernizedMVCSolution/src/eShopModernizedMVC/Web.config`

#### Agent 3: deployment configuration hardening

```text
/config-hardening
In this local worktree, review MVC Kubernetes and Docker Compose configuration for safe demo defaults and placeholder cloud settings. Do not deploy anything; run git diff --check and summarize any Windows-container assumptions.
```

Suggested files:

- `docker-compose.override.yml`
- `eShopModernizedMVCSolution/docker-compose*.yml`
- `Kubernetes/eShopModernizedMVC-K8s/**/*.yml`

### Act 4: manage work in Agent Command Center

Show the Kanban board with all three local agents. Point out:

- Each card is a local Cascade session.
- Each session has its own git worktree under `~/.windsurf/worktrees/<repo_name>/`.
- The main workspace stays clean while agents build and test in isolation.
- The Space carries context across the sprint.

Talk track:

> This is not cloud delegation. These are local agents working against local worktrees, and Agent Command Center gives me the team-lead view.

### Act 5: review and merge

From the main workspace, run:

```text
/local-review-merge
Review the completed local worktree agents in this Space, merge only the approved diffs, and run the narrowest validation command after each merge.
```

Merge order:

1. Deployment configuration hardening.
2. Observability cleanup.
3. MVC catalog validation.

Final validation:

```bash
nuget restore eShopModernizedMVCSolution/eShopModernizedMVC.sln
msbuild eShopModernizedMVCSolution/eShopModernizedMVC.sln /t:Build /p:Configuration=Debug /p:Disable_CopyWebApplication=true
git diff --check
```

## Repository features added for the demo

- Root and scoped `AGENTS.md` files for always-on and directory-scoped local-agent context.
- Workspace rules for local-only execution, .NET Framework modernization, and container/cloud readiness.
- Workflows for modernization fan-out, catalog bugfixes, config hardening, test-writing decisions, and local review/merge.
- Workspace hooks for worktree setup, local-only command guardrails, and text-file whitespace cleanup.

## Windsurf documentation anchors

- Agent Command Center: local and cloud agents appear in a Kanban-style view grouped by status.
- Spaces: group agent sessions, PRs, files, and context for a task or project; new sessions inherit Space context.
- AGENTS.md: root files are always-on; subdirectory files apply automatically to matching paths.
- Workflows: markdown playbooks invoked manually with `/workflow-name`.
- Worktrees: each Cascade session can run in its own git worktree and merge back after review.
- Hooks: workspace `.windsurf/hooks.json` can run scripts for setup, safety, logging, and quality checks.

## Backup demo prompts

Use these if the first set of agents completes too quickly.

```text
/config-hardening
In this local worktree, compare WebForms and MVC mock-data configuration and identify any drift that would surprise a local demo. Keep changes minimal and run git diff --check.
```

```text
/catalog-bugfix
In this local worktree, inspect the WebForms catalog create/edit pages for validation gaps compared with MVC. Make only focused changes and run the WebForms build command.
```

```text
/modernization-fanout
Plan a second local-only fan-out for WCF/WinForms modernization readiness. Keep the prompts under two sentences each and avoid any cloud execution.
```
