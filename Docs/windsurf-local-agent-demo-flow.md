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
- Fully migrate one small, demonstrable path: the MVC catalog browse/read slice.
- Use mock/in-memory data first so SQL Server and Windows Containers are not required.
- Keep legacy MVC/WebForms/WCF code read-only unless an agent is explicitly extracting behavior.
- Validate with `dotnet restore`, `dotnet build`, and `dotnet run` on macOS.

## Actual modernization target

Migrate the read-only catalog browsing path from the modernized MVC 5 app to a new .NET 8 ASP.NET Core app. This is intentionally specific enough to finish in one live session and visual enough to demo on a Mac.

### Legacy source path

```text
eShopModernizedMVCSolution/src/eShopModernizedMVC/
├── Controllers/CatalogController.cs
│   ├── Index(pageSize, pageIndex)
│   └── Details(id)
├── Services/ICatalogService.cs
├── Services/CatalogServiceMock.cs
├── Models/CatalogItem.cs
├── Models/CatalogBrand.cs
├── Models/CatalogType.cs
├── ViewModel/PaginatedItemsViewModel.cs
├── Views/Catalog/Index.cshtml
├── Views/Catalog/CatalogTable.cshtml
├── Views/Catalog/Details.cshtml
└── Setup/CatalogItems.csv
```

### New .NET 8 target path

```text
eShopModernizedDotNet8/
├── eShopModernizedDotNet8.csproj
├── Program.cs
├── Domain/
│   ├── CatalogItem.cs
│   ├── CatalogBrand.cs
│   ├── CatalogType.cs
│   └── PaginatedItems.cs
├── Services/
│   ├── ICatalogService.cs
│   └── InMemoryCatalogService.cs
├── Pages/
│   └── Catalog/
│       ├── Index.cshtml
│       ├── Index.cshtml.cs
│       ├── Details.cshtml
│       └── Details.cshtml.cs
├── wwwroot/
├── README.md
└── Tests or smoke-test docs
```

### Target acceptance criteria

- `dotnet build eShopModernizedDotNet8/eShopModernizedDotNet8.csproj` succeeds on macOS.
- `dotnet run --project eShopModernizedDotNet8/eShopModernizedDotNet8.csproj` starts a local web app.
- `/catalog` lists seeded catalog items from in-memory data.
- `/catalog/details/{id}` shows name, description, brand, type, price, picture file name, and stock fields.
- Pagination preserves the legacy behavior shape: `pageSize`, `pageIndex`, total items, current page, total pages, previous/next links.
- The legacy .NET Framework app is not modified except as read-only reference.
- No SQL Server, IIS, Windows Containers, Azure services, or secrets are required.

### Why this is the right slice

```text
Too broad for one session                         Right-sized for one session
┌──────────────────────────────┐                 ┌─────────────────────────────┐
│ Whole MVC/WebForms/WCF repo  │                 │ MVC catalog browse/read     │
│ Auth + CRUD + EF + images    │      ───▶       │ Index + Details + mock data │
│ SQL + Windows containers     │                 │ Runs with dotnet on macOS   │
└──────────────────────────────┘                 └─────────────────────────────┘
```

This gives you a real migration story without overcommitting: one legacy controller path, one service contract, one model set, two views, and a visible Mac-runnable outcome.

## Parallel worktree execution model for this target

Use a short foundation step, then fan out. This avoids creating four worktrees that all fight over the first project scaffold files.

```text
Main workspace
    │
    ├─ Prompt 1: plan target and lanes with /modernization-fanout
    │
    ├─ Worktree A: create .NET 8 foundation
    │      └─ merge first after dotnet build succeeds
    │
    └─ Parallel work after foundation lands
           ├─ Worktree B: migrate catalog domain + in-memory service
           ├─ Worktree C: migrate Razor catalog list/details UI
           └─ Worktree D: add tests, README, and migration map
```

Target lane ownership:

| Lane | Owns | Avoids |
| --- | --- | --- |
| Foundation | `.csproj`, `Program.cs`, base Razor Pages app, README shell | Deep catalog behavior |
| Domain/service | `Domain/`, `Services/`, seed data in code or JSON | Razor markup except compile fixes |
| UI | `Pages/Catalog/`, `wwwroot/` catalog styling | Service internals |
| Validation/docs | test project, README runbook, migration map | Production behavior changes |

If you need to keep the demo shorter, run only Foundation + Domain/service + UI. The app can still build and run; tests/docs become the backup lane.

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
| Devin CLI subagents | Reusable expert tools for the modernization toolbox | `.devin/agents/*/AGENT.md` |

## Devin CLI subagent callout pack

After the main Windsurf demo flow is complete, explicitly switch context to a separate optional Devin CLI follow-up. The Windsurf demo remains local Cascade agents only; `.devin/agents/*/AGENT.md` is a reusable Devin CLI toolbox for follow-up local analysis, validation, and review outside the Windsurf flow. For the short copy/paste prompt script, use `Docs/devin-cli-subagent-demo-prompts.md`.

```text
Main demo: local Windsurf agents create the .NET 8 catalog slice
After-demo CLI callout: repo-specific subagents become reusable expert tools

┌─────────────────────────────┐
│ eShop modernization toolbox │
├──────────────┬──────────────┤
│ Archaeology  │ Architecture │
│ Parity       │ Hardening    │
│ Review       │              │
└──────────────┴──────────────┘
```

Recommended callout order:

1. `legacy-archaeologist` — maps legacy MVC/WebForms/WCF/WinForms code into migration seams.
2. `dotnet8-slice-architect` — designs the Mac-runnable .NET 8 target shape and worktree ownership.
3. `catalog-parity-inspector` — compares legacy catalog behavior to the new .NET 8 slice.
4. `config-secrets-hardener` — audits config/secrets surfaces and maps them to local-safe .NET 8 settings.
5. `review-captain` — reviews worktree diffs for scope, validation, local-only compliance, and merge order.

CLI-style prompts to show after the main flow:

```text
Use the legacy-archaeologist subagent to identify the next smallest .NET Framework feature path that could migrate after the catalog browse/read slice.
```

```text
Use the catalog-parity-inspector subagent to compare the new `eShopModernizedDotNet8/Pages/Catalog` implementation against the legacy MVC catalog Index, CatalogTable, and Details views.
```

```text
Use the review-captain subagent to review all completed worktree diffs and produce a merge-ready summary with validation evidence.
```

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
│  │ .NET 8 base     │ Catalog service │ Razor UI slice   │ validation │  │
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
   - `.devin/agents/`
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
Plan a local-only Agent Command Center sprint to migrate the MVC catalog browse/read path into a Mac-runnable .NET 8 ASP.NET Core app. Use one foundation worktree first, then three parallel Worktree-mode sessions for catalog domain/service, Razor list/details UI, and validation/docs; keep lanes non-overlapping and list exact files, validation commands, merge order, and risks.
```

Expected agent output:

- One Space name.
- One foundation lane plus three parallel lane cards for the .NET 8 catalog browse/read migration.
- Exact files for each lane.
- Copy/paste prompts for each agent.
- Validation command per lane.
- Merge order and risk notes.

What to say:

> This is the planning agent. It does not implement anything; it turns the repo rules and workflows into a parallel local execution plan.

## Act 4: create four local worktree agents

In Agent Command Center:

1. Start one foundation Cascade session in Worktree mode.
2. Put each session in the `eShop Modernization Sprint` Space.
3. Merge the foundation worktree after it builds.
4. Start the remaining three sessions in Worktree mode and paste one prompt from the sections below into each session.

### Agent 1: .NET 8 app foundation

Use workflow: `/dotnet8-migration-slice`

```text
/dotnet8-migration-slice
In this local worktree, create only the foundation for a Mac-runnable .NET 8 ASP.NET Core Razor Pages app under `eShopModernizedDotNet8/`. Add the project file, `Program.cs`, base layout/static assets, and a README stub; do not migrate catalog behavior yet, and validate with `dotnet restore` and `dotnet build`.
```

Suggested file scope:

- `eShopModernizedDotNet8/`
- `eShopModernizedDotNet8/eShopModernizedDotNet8.csproj`
- `eShopModernizedDotNet8/Program.cs`
- `eShopModernizedDotNet8/Pages/Shared/`
- `eShopModernizedDotNet8/wwwroot/`
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

### Agent 2: catalog domain and service migration

Use workflow: `/dotnet8-migration-slice`

```text
/dotnet8-migration-slice
In this local worktree, migrate the legacy MVC catalog browse/read model into .NET 8 domain and service classes. Use `CatalogItem`, `CatalogBrand`, `CatalogType`, `PaginatedItems`, and an in-memory catalog service seeded from the legacy CSV/mock data; keep legacy files read-only and validate with `dotnet build`.
```

Suggested file scope:

- Read-only reference: `eShopModernizedMVCSolution/src/eShopModernizedMVC/Models/CatalogItem.cs`
- Read-only reference: `eShopModernizedMVCSolution/src/eShopModernizedMVC/Models/CatalogBrand.cs`
- Read-only reference: `eShopModernizedMVCSolution/src/eShopModernizedMVC/Models/CatalogType.cs`
- Read-only reference: `eShopModernizedMVCSolution/src/eShopModernizedMVC/Services/CatalogServiceMock.cs`
- Read-only reference: `eShopModernizedMVCSolution/src/eShopModernizedMVC/Setup/CatalogItems.csv`
- Read-only reference: `eShopModernizedMVCSolution/src/eShopModernizedMVC/Controllers/CatalogController.cs`
- `eShopModernizedDotNet8/Domain/`
- `eShopModernizedDotNet8/Services/`
- `eShopModernizedDotNet8/Program.cs`

Validation to request:

```bash
dotnet build eShopModernizedDotNet8/eShopModernizedDotNet8.csproj
git diff --check
```

What this showcases:

- Local agent reads legacy code and ports only the behavior needed for a vertical slice
- The old project stays stable while new .NET 8 code moves quickly
- Service modernization can happen in parallel with UI work after the foundation lands

### Agent 3: Razor UI slice

Use workflow: `/dotnet8-migration-slice`

```text
/dotnet8-migration-slice
In this local worktree, migrate the MVC catalog Index, CatalogTable, and Details views into Razor Pages under `eShopModernizedDotNet8/Pages/Catalog`. Preserve the visible fields and pagination shape, consume the catalog service interface, avoid database dependencies, and validate with `dotnet build` plus `dotnet run` if possible.
```

Suggested file scope:

- `eShopModernizedDotNet8/Pages/`
- Read-only reference: `eShopModernizedMVCSolution/src/eShopModernizedMVC/Views/Catalog/Index.cshtml`
- Read-only reference: `eShopModernizedMVCSolution/src/eShopModernizedMVC/Views/Catalog/CatalogTable.cshtml`
- Read-only reference: `eShopModernizedMVCSolution/src/eShopModernizedMVC/Views/Catalog/Details.cshtml`
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
In this local worktree, add focused validation and documentation for the .NET 8 catalog browse/read slice. Add small unit tests for pagination and details lookup if practical, document the legacy-to-.NET 8 file mapping and macOS run commands, and validate with `dotnet build` plus `dotnet test` if a test project is added.
```

Suggested file scope:

- `eShopModernizedDotNet8.Tests/` if the lane adds a small test project
- `eShopModernizedDotNet8/README.md`
- `Docs/windsurf-dotnet8-catalog-migration-map.md`
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

1. .NET 8 app foundation.
2. Catalog domain/service migration.
3. Razor UI slice.
4. Migration validation and docs.

Why this order works:

- The foundation establishes the project shape.
- Domain/service work should land before UI consumes it.
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
/modernization-fanout
Plan a local-only Agent Command Center sprint to migrate the MVC catalog browse/read path into a Mac-runnable .NET 8 ASP.NET Core app. Use one foundation worktree first, then three parallel Worktree-mode sessions for catalog domain/service, Razor list/details UI, and validation/docs; keep lanes non-overlapping and list exact files, validation commands, merge order, and risks.
```

### Prompt 2: hook explainer

```text
Inspect `.windsurf/hooks.json` and `.windsurf/scripts/` and summarize how the local setup hook, command guard, and post-write cleanup support this demo. Use read-only inspection plus syntax validation only; do not run destructive git commands.
```

### Prompt 3: .NET 8 foundation local worktree agent

```text
/dotnet8-migration-slice
In this local worktree, create only the foundation for a Mac-runnable .NET 8 ASP.NET Core Razor Pages app under `eShopModernizedDotNet8/`. Add the project file, `Program.cs`, base layout/static assets, and a README stub; do not migrate catalog behavior yet, and validate with `dotnet restore` and `dotnet build`.
```

### Prompt 4: catalog domain/service local worktree agent

```text
/dotnet8-migration-slice
In this local worktree, migrate the legacy MVC catalog browse/read model into .NET 8 domain and service classes. Use `CatalogItem`, `CatalogBrand`, `CatalogType`, `PaginatedItems`, and an in-memory catalog service seeded from the legacy CSV/mock data; keep legacy files read-only and validate with `dotnet build`.
```

### Prompt 5: Razor UI local worktree agent

```text
/dotnet8-migration-slice
In this local worktree, migrate the MVC catalog Index, CatalogTable, and Details views into Razor Pages under `eShopModernizedDotNet8/Pages/Catalog`. Preserve the visible fields and pagination shape, consume the catalog service interface, avoid database dependencies, and validate with `dotnet build` plus `dotnet run` if possible.
```

### Prompt 6: validation local worktree agent

```text
/dotnet8-migration-slice
In this local worktree, add focused validation and documentation for the .NET 8 catalog browse/read slice. Add small unit tests for pagination and details lookup if practical, document the legacy-to-.NET 8 file mapping and macOS run commands, and validate with `dotnet build` plus `dotnet test` if a test project is added.
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
