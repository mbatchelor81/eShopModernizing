# Windsurf local-agent demo flow and prompts

## Demo goal

Show that Windsurf 2.0 can coordinate a full modernization sprint using only local Cascade agents, Agent Command Center, Spaces, rules, workflows, hooks, and git worktrees. The live story is: encode team rules once, plan a local agent fleet, launch parallel worktrees, watch the work in Agent Command Center, review diffs, merge one lane at a time, and validate locally.

## Non-negotiables for the live demo

- Use Windsurf local Cascade agents only.
- Start every implementation lane in Worktree mode.
- Group all sessions in one Space: `eShop Modernization Sprint`.
- Do not use cloud execution or external agent delegation.
- Keep every lane scoped to non-overlapping files.
- Merge only after human diff review.

## Capabilities to explicitly showcase

| Capability | What to show | Repo asset |
| --- | --- | --- |
| Rules | Local-only and .NET modernization constraints are automatic context | `.windsurf/rules/*.md` |
| AGENTS.md | Root and directory-scoped project instructions load by path | `AGENTS.md`, solution-level `AGENTS.md` |
| Workflows | Repeatable slash-command playbooks drive each lane | `.windsurf/workflows/*.md` |
| Hooks | Worktree setup, command guardrails, and post-write cleanup run locally | `.windsurf/hooks.json`, `.windsurf/scripts/*` |
| Worktrees | Each agent edits an isolated checkout | Windsurf Worktree mode |
| Spaces | All local sessions share sprint context | Agent Command Center Space |
| Agent Command Center | Parallel local work is visible as a board | Agent Command Center |
| Local review/merge | Human lead reviews and merges one lane at a time | `/local-review-merge` |

## Demo architecture visual

```text
                         Windsurf 2.0 on your machine
┌────────────────────────────────────────────────────────────────────────────┐
│                                                                            │
│  Main workspace                                                            │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │ Repo rules + workflows + hooks                                      │  │
│  │ AGENTS.md  .windsurf/rules  .windsurf/workflows  .windsurf/hooks    │  │
│  └───────────────────────────────┬──────────────────────────────────────┘  │
│                                  │                                         │
│                                  ▼                                         │
│  Agent Command Center Space: eShop Modernization Sprint                    │
│  ┌──────────────────┬──────────────────┬──────────────────┬────────────┐  │
│  │ Local agent 1    │ Local agent 2    │ Local agent 3    │ Local      │  │
│  │ MVC validation   │ WebForms parity  │ Config hardening │ validation │  │
│  │ Worktree A       │ Worktree B       │ Worktree C       │ Worktree D │  │
│  └────────┬─────────┴────────┬─────────┴────────┬─────────┴─────┬──────┘  │
│           │                  │                  │               │         │
│           └──────────────────┴──────────┬───────┴───────────────┘         │
│                                          ▼                                 │
│                            Human review + one-at-a-time merge              │
│                                                                            │
└────────────────────────────────────────────────────────────────────────────┘

No cloud execution. No Devin. No public deploy. All work happens in local
Cascade sessions and local git worktrees.
```

## Pre-demo setup

1. Open this repository in Windsurf 2.0.
2. Open Agent Command Center.
3. Create a Space named `eShop Modernization Sprint`.
4. Open these files in tabs so the audience can see the repo-encoded operating model:
   - `AGENTS.md`
   - `eShopModernizedMVCSolution/AGENTS.md`
   - `eShopModernizedWebFormsSolution/AGENTS.md`
   - `.windsurf/rules/local-agent-only.md`
   - `.windsurf/hooks.json`
   - `.windsurf/workflows/modernization-fanout.md`
5. In the terminal, run:

```bash
git worktree list
git status --short
```

Expected: clean main workspace and no surprise local changes.

## Talk track opener

> This repo is not just source code anymore. It contains the operating model for a local agent fleet: scoped instructions, always-on rules, reusable workflows, local hooks, and a review/merge path.

> The key point of this demo is that all execution stays local in Windsurf. Agent Command Center gives me the team-lead view, while worktrees give every local agent an isolated workspace.

## Act 1: show encoded team context

Show these files in this order:

1. `AGENTS.md`
2. `eShopModernizedMVCSolution/AGENTS.md`
3. `.windsurf/rules/local-agent-only.md`
4. `.windsurf/rules/dotnet-framework-modernization.md`
5. `.windsurf/rules/container-cloud-readiness.md`

What to say:

> I do not need to repeat repo conventions in every prompt. The root instructions cover the whole project, and solution-level instructions narrow the agent's behavior when it edits MVC, WebForms, N-tier, or Kubernetes files.

> The rules make the demo safe and consistent: local-only execution, .NET Framework compatibility, Windows Container assumptions, no real secrets, and focused validation.

## Act 2: show hooks before launching agents

Open `.windsurf/hooks.json` and `.windsurf/scripts/`.

What to say:

> Hooks are how the workspace adds local automation around every agent. Worktree setup can restore packages or copy local env files, the pre-command guard blocks unsafe local demo commands, and post-write cleanup keeps text files tidy without changing line endings.

Optional read-only prompt to the main Cascade session:

```text
Inspect `.windsurf/hooks.json` and `.windsurf/scripts/` and summarize how the local setup hook, command guard, and post-write cleanup support this demo. Use read-only inspection plus syntax validation only; do not run destructive git commands.
```

Optional validation commands:

```bash
python3 -m py_compile .windsurf/scripts/guard_local_only.py .windsurf/scripts/post_write_check.py
python3 -m json.tool .windsurf/hooks.json
git diff --check
```

## Act 3: plan the local fleet from the main workspace

Run this in the main Cascade session, not in a worktree:

```text
/modernization-fanout
Plan a local-only Agent Command Center sprint for this repo using four Worktree-mode Cascade sessions: MVC catalog validation, WebForms validation parity, deployment configuration hardening, and focused modernization validation. Keep lanes non-overlapping, list exact files, validation commands, merge order, and risks.
```

Expected agent output:

- One Space name.
- Four lane cards.
- Exact files for each lane.
- Copy/paste prompts for each agent.
- Validation command per lane.
- Merge order and risk notes.

What to say:

> This is the planning agent. It does not implement anything; it turns the repo rules and workflows into a parallel local execution plan.

## Act 4: create four local worktree agents

In Agent Command Center:

1. Start four new local Cascade sessions.
2. Put each session in the `eShop Modernization Sprint` Space.
3. Start each session in Worktree mode.
4. Paste one prompt from the sections below into each session.

### Agent 1: MVC catalog validation

Use workflow: `/catalog-bugfix`

```text
/catalog-bugfix
In this local worktree, harden MVC catalog create/edit validation so impossible stock thresholds are rejected before save. Stay within `eShopModernizedMVCSolution`, use existing MVC patterns, and run the MVC restore/build command or document any existing package drift.
```

Suggested file scope:

- `eShopModernizedMVCSolution/src/eShopModernizedMVC/Models/CatalogItem.cs`
- `eShopModernizedMVCSolution/src/eShopModernizedMVC/Controllers/CatalogController.cs`
- `eShopModernizedMVCSolution/src/eShopModernizedMVC/Views/Catalog/*.cshtml`
- `eShopModernizedMVCSolution/AGENTS.md`

Validation to request:

```bash
nuget restore eShopModernizedMVCSolution/eShopModernizedMVC.sln
msbuild eShopModernizedMVCSolution/eShopModernizedMVC.sln /t:Build /p:Configuration=Debug /p:Disable_CopyWebApplication=true
git diff --check
```

What this showcases:

- Directory-scoped `AGENTS.md`
- .NET Framework modernization rules
- Worktree-isolated code edits
- Focused local validation

### Agent 2: WebForms validation parity

Use workflow: `/catalog-bugfix`

```text
/catalog-bugfix
In this local worktree, compare the WebForms catalog create/edit validation path against the MVC lane and close the smallest safe parity gap. Keep markup, code-behind, and designer files synchronized, then run the WebForms restore/build command or document any existing package drift.
```

Suggested file scope:

- `eShopModernizedWebFormsSolution/src/eShopModernizedWebForms/Catalog/*.aspx`
- `eShopModernizedWebFormsSolution/src/eShopModernizedWebForms/Catalog/*.aspx.cs`
- `eShopModernizedWebFormsSolution/src/eShopModernizedWebForms/Models/CatalogItem.cs`
- `eShopModernizedWebFormsSolution/AGENTS.md`

Validation to request:

```bash
nuget restore eShopModernizedWebFormsSolution/eShopModernizedWebForms.sln
msbuild eShopModernizedWebFormsSolution/eShopModernizedWebForms.sln /t:Build /p:Configuration=Debug /p:Disable_CopyWebApplication=true
git diff --check
```

What this showcases:

- Parallel work on a second legacy UI stack
- Path-scoped instructions for WebForms
- Human-controlled parity decisions instead of broad refactoring

### Agent 3: deployment configuration hardening

Use workflow: `/config-hardening`

```text
/config-hardening
In this local worktree, review MVC and WebForms Docker Compose and Kubernetes configuration for demo-safe placeholders, environment-variable alignment, and Windows Container assumptions. Do not deploy anything; make only focused config edits or produce a concise findings list, then run `git diff --check`.
```

Suggested file scope:

- `docker-compose.override.yml`
- `eShopModernizedMVCSolution/docker-compose*.yml`
- `eShopModernizedWebFormsSolution/docker-compose*.yml`
- `Kubernetes/eShopModernizedMVC-K8s/**/*.yml`
- `Kubernetes/eShopModernizedWebForms-K8s/**/*.yml`
- `Kubernetes/AGENTS.md`

Validation to request:

```bash
git diff --check
```

If `kubectl` is locally available:

```bash
kubectl apply --dry-run=client -f <edited-manifest>
```

What this showcases:

- Model-decision rules for container/cloud readiness
- No public deployment
- Local review of old Windows-container manifests
- Secret and placeholder hygiene

### Agent 4: focused validation strategy

Use workflow: `/write-modernization-tests`

```text
/write-modernization-tests
In this local worktree, inspect whether the MVC or WebForms validation lane has an existing test pattern that can support a small focused test. If no practical test project exists, do not invent a large harness; create a concise manual validation checklist and run the narrowest restore/build validation available.
```

Suggested file scope:

- Existing test project files if the agent finds a real pattern
- `Docs/` checklist only if no practical test pattern exists
- Do not touch production behavior just to make testing easier

Validation to request:

```bash
git diff --check
nuget restore eShopModernizedMVCSolution/eShopModernizedMVC.sln
nuget restore eShopModernizedWebFormsSolution/eShopModernizedWebForms.sln
```

What this showcases:

- Testing judgment rather than test theater
- The repo rule that no large harness should be invented when no pattern exists
- A local validation lane that can run in parallel with implementation

## Act 5: use Agent Command Center as the team-lead surface

While agents run, show:

1. The `eShop Modernization Sprint` Space.
2. The four local sessions as separate work cards.
3. Each card's status.
4. That each agent has a different worktree.
5. That the main workspace remains clean until review/merge.

Ask any running agent for a status update with:

```text
Summarize your current lane status in three bullets: files inspected, changes made or proposed, and validation status. Do not expand scope beyond your assigned lane.
```

What to say:

> This is the parallelization moment. I am not waiting for one chat to finish before starting another; I am supervising a local fleet with separate worktrees and a shared Space.

## Act 6: demonstrate hook guardrails

Do not run destructive commands. Instead, ask the main local agent to inspect the guard behavior:

```text
Read `.windsurf/scripts/guard_local_only.py` and explain which command patterns it blocks during the demo. Do not execute blocked commands; summarize the safety intent and the known demo-only limitations.
```

If you want a safe scripted proof, run the hook directly with a fake payload rather than running a real git command:

```bash
printf '%s' '{"agent_action_name":"pre_run_command","tool_info":{"command_line":"git push --force"}}' | python3 .windsurf/scripts/guard_local_only.py
```

Expected: non-zero exit and `Force-push is blocked during local demo work.`

What to say:

> The guard is not a security boundary; it is a local demo safety net. It prevents accidental unsafe commands while still letting the local agent continue normal development work.

## Act 7: review and merge completed worktrees

When at least two agents finish, return to the main workspace and run:

```text
/local-review-merge
Review the completed local worktree agents in the `eShop Modernization Sprint` Space, reject unrelated diffs or skipped validation, and merge approved worktrees one at a time. After each merge, run `git status --short` and the narrowest relevant validation command.
```

Recommended merge order:

1. Deployment configuration hardening.
2. Focused validation strategy.
3. WebForms validation parity.
4. MVC catalog validation.

Why this order works:

- Config/docs lanes are less likely to conflict.
- Validation strategy can inform the app lanes.
- MVC and WebForms changes remain separate until the end.

Manual review checklist for each lane:

- Did it stay local-only?
- Did it stay in its assigned files?
- Did it avoid real credentials and cloud deployment?
- Did it run or document validation?
- Is the diff small enough to explain live?

## Act 8: final local validation

Run:

```bash
git status --short
git diff --check
python3 -m py_compile .windsurf/scripts/guard_local_only.py .windsurf/scripts/post_write_check.py
python3 -m json.tool .windsurf/hooks.json
```

For app lanes, run the relevant restore/build command if the local environment supports it:

```bash
nuget restore eShopModernizedMVCSolution/eShopModernizedMVC.sln
msbuild eShopModernizedMVCSolution/eShopModernizedMVC.sln /t:Build /p:Configuration=Debug /p:Disable_CopyWebApplication=true
```

```bash
nuget restore eShopModernizedWebFormsSolution/eShopModernizedWebForms.sln
msbuild eShopModernizedWebFormsSolution/eShopModernizedWebForms.sln /t:Build /p:Configuration=Debug /p:Disable_CopyWebApplication=true
```

What to say if restore/build has existing package drift:

> This is a legacy .NET Framework modernization repo, so local Linux validation may expose existing package-reference drift. The important demo behavior is that agents run the right local checks, report the exact failure, and keep their changes scoped.

## Exact prompt sequence

Use this as the copy/paste script during the demo.

### Prompt 1: main planning agent

```text
/modernization-fanout
Plan a local-only Agent Command Center sprint for this repo using four Worktree-mode Cascade sessions: MVC catalog validation, WebForms validation parity, deployment configuration hardening, and focused modernization validation. Keep lanes non-overlapping, list exact files, validation commands, merge order, and risks.
```

### Prompt 2: hook explainer

```text
Inspect `.windsurf/hooks.json` and `.windsurf/scripts/` and summarize how the local setup hook, command guard, and post-write cleanup support this demo. Use read-only inspection plus syntax validation only; do not run destructive git commands.
```

### Prompt 3: MVC local worktree agent

```text
/catalog-bugfix
In this local worktree, harden MVC catalog create/edit validation so impossible stock thresholds are rejected before save. Stay within `eShopModernizedMVCSolution`, use existing MVC patterns, and run the MVC restore/build command or document any existing package drift.
```

### Prompt 4: WebForms local worktree agent

```text
/catalog-bugfix
In this local worktree, compare the WebForms catalog create/edit validation path against the MVC lane and close the smallest safe parity gap. Keep markup, code-behind, and designer files synchronized, then run the WebForms restore/build command or document any existing package drift.
```

### Prompt 5: config local worktree agent

```text
/config-hardening
In this local worktree, review MVC and WebForms Docker Compose and Kubernetes configuration for demo-safe placeholders, environment-variable alignment, and Windows Container assumptions. Do not deploy anything; make only focused config edits or produce a concise findings list, then run `git diff --check`.
```

### Prompt 6: validation local worktree agent

```text
/write-modernization-tests
In this local worktree, inspect whether the MVC or WebForms validation lane has an existing test pattern that can support a small focused test. If no practical test project exists, do not invent a large harness; create a concise manual validation checklist and run the narrowest restore/build validation available.
```

### Prompt 7: status update for any running agent

```text
Summarize your current lane status in three bullets: files inspected, changes made or proposed, and validation status. Do not expand scope beyond your assigned lane.
```

### Prompt 8: local review and merge

```text
/local-review-merge
Review the completed local worktree agents in the `eShop Modernization Sprint` Space, reject unrelated diffs or skipped validation, and merge approved worktrees one at a time. After each merge, run `git status --short` and the narrowest relevant validation command.
```

## Backup prompts if a lane finishes early

### Backup 1: N-tier config review

```text
/config-hardening
In this local worktree, review the WCF/WinForms N-tier configuration for local-demo readiness and Windows-only validation gaps. Preserve service contracts and client proxies unless a minimal config fix is clearly needed.
```

### Backup 2: mock-data mode audit

```text
/config-hardening
In this local worktree, compare MVC and WebForms mock-data settings across `Web.config`, Docker Compose, and Kubernetes manifests. Keep changes minimal, prefer placeholders, and run `git diff --check`.
```

### Backup 3: modernization explainer

```text
/modernization-fanout
Plan a second local-only fan-out focused on WCF service hardening, WinForms modernization review, and deployment manifest cleanup. Keep every prompt under two sentences and avoid cloud execution.
```

## Closing talk track

> The demo showed a full local agent operating model: persistent repo rules, reusable workflows, safety hooks, isolated worktrees, visible parallel execution in Agent Command Center, and a human-controlled merge path.

> The win is not just that one local agent can edit code. The win is that Windsurf can make local agents feel like a coordinated engineering team without handing execution to the cloud.
