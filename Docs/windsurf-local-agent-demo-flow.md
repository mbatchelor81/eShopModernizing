# Windsurf local-agent demo flow and prompts

## Demo goal

Show that Windsurf 2.0 can coordinate a full modernization sprint using only local Cascade agents, Agent Command Center, Spaces, rules, workflows, hooks, and git worktrees. The recommended live story for macOS is: use local agents to carve a legacy .NET Framework eShop sample into a new Mac-runnable .NET 8 path, launch parallel worktrees, watch the work in Agent Command Center, review diffs, merge one lane at a time, and validate with local `dotnet` commands.

## Non-negotiables for the live demo

- Use Windsurf local Cascade agents only.
- Start every implementation lane in Worktree mode.
- Group all sessions in one Space: `eShop Modernization Sprint`.
- Do not use cloud execution or external agent delegation.
- Keep every lane scoped to non-overlapping files.
- Merge only after human diff review.
- Do not try to compile the existing .NET Framework 4.x MVC/WebForms/WCF apps on macOS during the live demo.
- Make the primary modernization target a new .NET 8 ASP.NET Core path that can run locally on macOS.

## macOS demo positioning

The legacy source in this repository is intentionally .NET Framework-era code: ASP.NET MVC 5, WebForms, WCF, WinForms, Windows Containers, and older deployment manifests. That is exactly why the macOS demo should not focus on building the existing apps. Instead, use local agents to modernize toward a new .NET 8 Core slice that can run on a Mac while preserving the old code as the reference implementation.

Recommended modernization target:

- Create a new `eShopModernizedDotNet8/` path.
- Use ASP.NET Core on .NET 8.
- Start with a thin catalog vertical slice.
- Use mock/in-memory data first so SQL Server and Windows Containers are not required.
- Keep legacy MVC/WebForms/WCF code read-only unless an agent is explicitly extracting behavior.
- Validate with `dotnet restore`, `dotnet build`, and `dotnet run` on macOS.

The PowerShell hook entries in `.windsurf/hooks.json` are Windows fallbacks. On macOS, Windsurf should use the bash/python `command` entries:

```json
"command": "bash .windsurf/scripts/setup_worktree.sh"
"command": "python3 .windsurf/scripts/guard_local_only.py"
"command": "python3 .windsurf/scripts/post_write_check.py"
```

You do not need to remove the PowerShell entries for a Mac demo; they make the same repo usable on Windows machines too.

## Capabilities to explicitly showcase

| Capability | What to show | Repo asset |
| --- | --- | --- |
| Rules | Local-only and .NET modernization constraints are automatic context | `.windsurf/rules/*.md` |
| AGENTS.md | Root and directory-scoped project instructions load by path | `AGENTS.md`, solution-level `AGENTS.md` |
| Workflows | Repeatable slash-command playbooks drive each lane | `.windsurf/workflows/*.md`, especially `/dotnet8-migration-slice` |
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
│  │ .NET 8 skeleton │ Catalog API      │ Razor UI slice   │ migration  │  │
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

1. Install .NET 8 SDK on the Mac if it is not already installed:

```bash
dotnet --info
```

2. Open this repository in Windsurf 2.0.
3. Open Agent Command Center.
4. Create a Space named `eShop Modernization Sprint`.
5. Open these files in tabs so the audience can see the repo-encoded operating model:
   - `AGENTS.md`
   - `eShopModernizedMVCSolution/AGENTS.md`
   - `eShopModernizedWebFormsSolution/AGENTS.md`
   - `.windsurf/rules/local-agent-only.md`
   - `.windsurf/hooks.json`
   - `.windsurf/workflows/modernization-fanout.md`
   - `.windsurf/workflows/dotnet8-migration-slice.md`
6. In the terminal, run:

```bash
git worktree list
git status --short
dotnet --info
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
Plan a local-only Agent Command Center sprint to modernize this legacy .NET Framework eShop repo into a Mac-runnable .NET 8 ASP.NET Core catalog slice. Use four Worktree-mode Cascade sessions: .NET 8 app skeleton, catalog domain/API extraction, Razor UI slice, and migration validation/docs; keep lanes non-overlapping and list exact files, validation commands, merge order, and risks.
```

Expected agent output:

- One Space name.
- Four lane cards for a .NET 8 modernization path.
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

### Agent 1: .NET 8 app skeleton

Use workflow: `/dotnet8-migration-slice`

```text
/dotnet8-migration-slice
In this local worktree, create the smallest Mac-runnable .NET 8 ASP.NET Core catalog app skeleton under `eShopModernizedDotNet8/`. Use mock/in-memory catalog data, avoid SQL Server and Windows Containers, and validate with `dotnet restore` and `dotnet build`.
```

Suggested file scope:

- `eShopModernizedDotNet8/`
- `eShopModernizedDotNet8/eShopModernizedDotNet8.csproj`
- `eShopModernizedDotNet8/Program.cs`
- `eShopModernizedDotNet8/appsettings.Development.json`
- `eShopModernizedDotNet8/README.md`

Validation to request:

```bash
dotnet restore eShopModernizedDotNet8/eShopModernizedDotNet8.csproj
dotnet build eShopModernizedDotNet8/eShopModernizedDotNet8.csproj
git diff --check
```

What this showcases:

- A concrete Mac-runnable modernization target
- Worktree-isolated greenfield scaffolding
- Local `dotnet` validation instead of legacy Windows build tooling

### Agent 2: catalog domain and API extraction

Use workflow: `/dotnet8-migration-slice`

```text
/dotnet8-migration-slice
In this local worktree, inspect the legacy MVC catalog model and service flow, then add a focused .NET 8 catalog domain model, in-memory repository, and minimal API endpoints under `eShopModernizedDotNet8/`. Keep the legacy project read-only and validate with `dotnet build`.
```

Suggested file scope:

- Read-only reference: `eShopModernizedMVCSolution/src/eShopModernizedMVC/Models/CatalogItem.cs`
- Read-only reference: `eShopModernizedMVCSolution/src/eShopModernizedMVC/Controllers/CatalogController.cs`
- `eShopModernizedDotNet8/Domain/`
- `eShopModernizedDotNet8/Data/`
- `eShopModernizedDotNet8/Program.cs`

Validation to request:

```bash
dotnet build eShopModernizedDotNet8/eShopModernizedDotNet8.csproj
git diff --check
```

What this showcases:

- Local agent reads legacy code and ports only the behavior needed for a vertical slice
- The old project stays stable while new .NET 8 code moves quickly
- API modernization can happen in parallel with UI work

### Agent 3: Razor UI slice

Use workflow: `/dotnet8-migration-slice`

```text
/dotnet8-migration-slice
In this local worktree, add a minimal Razor Pages or MVC UI slice to the .NET 8 app that lists catalog items from the new in-memory catalog service. Keep styling simple, avoid database dependencies, and validate with `dotnet build` plus a local `dotnet run` smoke test if possible.
```

Suggested file scope:

- `eShopModernizedDotNet8/Pages/`
- `eShopModernizedDotNet8/Views/`
- `eShopModernizedDotNet8/wwwroot/`
- `eShopModernizedDotNet8/Program.cs`
- `eShopModernizedDotNet8/README.md`

Validation to request:

```bash
dotnet build eShopModernizedDotNet8/eShopModernizedDotNet8.csproj
dotnet run --project eShopModernizedDotNet8/eShopModernizedDotNet8.csproj
git diff --check
```

What this showcases:

- A visible app running on the presenter's Mac
- Local-only implementation and smoke testing
- Parallel UI work against the new .NET 8 slice

### Agent 4: migration validation and docs

Use workflow: `/dotnet8-migration-slice`

```text
/dotnet8-migration-slice
In this local worktree, add focused validation for the .NET 8 catalog slice and document how to run it on macOS. Prefer small unit tests if the new project has testable domain logic; otherwise add a concise smoke-test checklist and validate with `dotnet build`.
```

Suggested file scope:

- `eShopModernizedDotNet8.Tests/` if the lane adds a small test project
- `eShopModernizedDotNet8/README.md`
- `Docs/windsurf-local-agent-demo-flow.md` only if prompt updates are needed
- Do not touch legacy production behavior just to make testing easier

Validation to request:

```bash
dotnet build eShopModernizedDotNet8/eShopModernizedDotNet8.csproj
dotnet test eShopModernizedDotNet8.Tests/eShopModernizedDotNet8.Tests.csproj
git diff --check
```

What this showcases:

- Testing judgment while modernizing
- A Mac-native validation loop
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

1. .NET 8 app skeleton.
2. Catalog domain/API extraction.
3. Razor UI slice.
4. Migration validation and docs.

Why this order works:

- The skeleton establishes the project shape.
- Domain/API work should land before UI consumes it.
- UI lands after the app can build.
- Validation/docs lands last so it reflects the final structure.

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
dotnet restore eShopModernizedDotNet8/eShopModernizedDotNet8.csproj
dotnet build eShopModernizedDotNet8/eShopModernizedDotNet8.csproj
dotnet run --project eShopModernizedDotNet8/eShopModernizedDotNet8.csproj
```

If tests are added:

```bash
dotnet test eShopModernizedDotNet8.Tests/eShopModernizedDotNet8.Tests.csproj
```

What to say about the legacy projects:

> The original MVC/WebForms/WCF apps are legacy .NET Framework and Windows-container oriented, so I am not trying to compile them on my Mac. The modernization path is a new .NET 8 slice that local agents can build and run here while using the old code as the behavioral reference.

## Exact prompt sequence

Use this as the copy/paste script during the demo.

### Prompt 1: main planning agent

```text
/dotnet8-migration-slice
Plan a local-only Agent Command Center sprint to modernize this legacy .NET Framework eShop repo into a Mac-runnable .NET 8 ASP.NET Core catalog slice. Use four Worktree-mode Cascade sessions: .NET 8 app skeleton, catalog domain/API extraction, Razor UI slice, and migration validation/docs; keep lanes non-overlapping and list exact files, validation commands, merge order, and risks.
```

### Prompt 2: hook explainer

```text
Inspect `.windsurf/hooks.json` and `.windsurf/scripts/` and summarize how the local setup hook, command guard, and post-write cleanup support this demo. Use read-only inspection plus syntax validation only; do not run destructive git commands.
```

### Prompt 3: .NET 8 skeleton local worktree agent

```text
/modernization-fanout
In this local worktree, create the smallest Mac-runnable .NET 8 ASP.NET Core catalog app skeleton under `eShopModernizedDotNet8/`. Use mock/in-memory catalog data, avoid SQL Server and Windows Containers, and validate with `dotnet restore` and `dotnet build`.
```

### Prompt 4: catalog API local worktree agent

```text
/dotnet8-migration-slice
In this local worktree, inspect the legacy MVC catalog model and service flow, then add a focused .NET 8 catalog domain model, in-memory repository, and minimal API endpoints under `eShopModernizedDotNet8/`. Keep the legacy project read-only and validate with `dotnet build`.
```

### Prompt 5: Razor UI local worktree agent

```text
/dotnet8-migration-slice
In this local worktree, add a minimal Razor Pages or MVC UI slice to the .NET 8 app that lists catalog items from the new in-memory catalog service. Keep styling simple, avoid database dependencies, and validate with `dotnet build` plus a local `dotnet run` smoke test if possible.
```

### Prompt 6: validation local worktree agent

```text
/dotnet8-migration-slice
In this local worktree, add focused validation for the .NET 8 catalog slice and document how to run it on macOS. Prefer small unit tests if the new project has testable domain logic; otherwise add a concise smoke-test checklist and validate with `dotnet build`.
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
In this local worktree, review the WCF/WinForms N-tier code as a modernization source map and identify what should become HTTP APIs, background services, or unsupported desktop-only behavior in a .NET 8 path. Do not edit production code unless a small documentation update is clearly useful.
```

### Backup 2: mock-data mode audit

```text
/config-hardening
In this local worktree, compare legacy MVC and WebForms mock-data settings and map them to the new .NET 8 appsettings model. Keep changes minimal, prefer placeholders, and validate with `dotnet build` if the .NET 8 project exists.
```

### Backup 3: modernization explainer

```text
/modernization-fanout
Plan a second local-only fan-out focused on extending the .NET 8 catalog slice with create/edit flows, Dockerfile portability, and migration documentation. Keep every prompt under two sentences and avoid cloud execution.
```

## Closing talk track

> The demo showed a full local agent operating model: persistent repo rules, reusable workflows, safety hooks, isolated worktrees, visible parallel execution in Agent Command Center, and a human-controlled merge path.

> The modernization outcome is also practical on my Mac: the old .NET Framework applications remain as reference implementations, while the local agent fleet creates a .NET 8 path that can build and run locally without Windows Containers or Visual Studio.
