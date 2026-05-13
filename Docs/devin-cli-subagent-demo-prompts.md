# Devin CLI subagent demo prompts

Use these only after the main Windsurf local-agent demo is complete. This is a separate optional Devin CLI follow-up story; do not paste these prompts into Windsurf local Cascade sessions.

## 1. Show the available toolbox

```text
List the custom subagents available in this repo and explain when I should use each one in one sentence each.
```

## 2. Archaeology: find the next slice

```text
Use the legacy-archaeologist subagent to identify the next smallest .NET Framework feature path that could migrate after the catalog browse/read slice.
```

## 3. Architecture: design the target

```text
Use the dotnet8-slice-architect subagent to turn that recommended feature path into a Mac-runnable .NET 8 project shape with files, worktree lanes, validation commands, and merge order.
```

## 4. Parity: inspect the migrated catalog

```text
Use the catalog-parity-inspector subagent to compare `eShopModernizedDotNet8/Pages/Catalog` against the legacy MVC catalog Index, CatalogTable, Details views, and CatalogController behavior.
```

## 5. Hardening: scan config and secrets

```text
Use the config-secrets-hardener subagent to audit the eShop Web.config, Docker Compose, Kubernetes, ACI, and Service Fabric config surfaces and map risky settings to safe .NET 8 appsettings or environment variables.
```

## 6. Review: act like the team lead

```text
Use the review-captain subagent to review the current branch for local-only compliance, scoped changes, validation evidence, and merge readiness.
```

## 7. Flashy parallel version

```text
Run legacy-archaeologist, config-secrets-hardener, and catalog-parity-inspector as background subagents, then summarize their findings as a modernization command-center brief.
```

## 8. Closing prompt

```text
Based on the subagent findings, recommend the next two demo-worthy modernization sessions and which subagent should lead each one.
```
