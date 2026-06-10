---
name: analyze-legacy-component
description: Analyze a legacy .NET Framework component for modernization readiness — dependency mapping, API surface extraction, behavioral baseline, and migration risk assessment. Use before planning any migration slice.
---

# Analyze Legacy Component

This skill walks through analyzing a legacy .NET Framework component in the eShopModernizing codebase to assess modernization readiness.

## Context

The eShopModernizing solution contains legacy .NET Framework projects:
- **eShopLegacyMVC** — ASP.NET MVC 5 web frontend
- **eShopLegacyWebForms** — ASP.NET WebForms web frontend
- **eShopWCFService** — WCF service layer
- **eShopWinForms** — Windows Forms desktop client

These target .NET 8 ASP.NET Core under `eShopModernizedDotNet8/`.

## Steps

### 1. Identify the component boundary

Determine what the component does and where it starts/ends:

```
grep -r "class.*Controller" --include="*.cs" <project_dir>/
grep -r "public.*interface" --include="*.cs" <project_dir>/
grep -r "ServiceContract" --include="*.cs" <project_dir>/
```

Record:
- Entry points (controllers, service contracts, page code-behinds)
- Public API surface (methods, endpoints, routes)
- Data models used

### 2. Map dependencies

#### Internal dependencies
```
grep -r "using eShop" --include="*.cs" <project_dir>/
```

Build a dependency graph: which namespaces does this component import from?

#### External dependencies (NuGet)
```
cat <project_dir>/packages.config
# or
grep "PackageReference" <project_dir>/*.csproj
```

For each NuGet package, classify:
| Category | Example | .NET 8 Equivalent |
|----------|---------|-------------------|
| **Direct port** | Newtonsoft.Json | System.Text.Json or Newtonsoft.Json (still works) |
| **Replaced** | System.Web.Mvc | Microsoft.AspNetCore.Mvc |
| **Removed** | System.ServiceModel (WCF) | gRPC or REST |
| **Unchanged** | EntityFramework 6 | EF Core 8 |

### 3. Extract behavioral baseline

Document what the component actually does at runtime:

- **Routes/URLs**: `grep -r 'Route\|MapRoute\|HttpGet\|HttpPost' --include="*.cs"`
- **Database queries**: `grep -r 'DbContext\|SqlCommand\|ExecuteReader' --include="*.cs"`
- **Config dependencies**: `grep -r 'ConfigurationManager\|appSettings\|connectionString' --include="*.cs"`
- **External service calls**: `grep -r 'HttpClient\|WebClient\|ServiceReference' --include="*.cs"`

### 4. Assess migration risk

For each aspect, rate LOW / MEDIUM / HIGH:

| Aspect | Check | Risk |
|--------|-------|------|
| Framework coupling | `System.Web.*` usage count | HIGH if >20 references |
| Data access | EF6 DbContext complexity | MEDIUM if >5 entities |
| Config | `Web.config` transform count | LOW if <3 transforms |
| Auth | Forms auth, Windows auth, custom | HIGH if custom auth |
| State | Session state, ViewState usage | HIGH if session-dependent |
| External services | WCF client proxies | HIGH if >2 services |

### 5. Produce the analysis report

```markdown
# Legacy Component Analysis: [Component Name]

## Summary
- Component: [name]
- Framework: .NET Framework [version]
- Migration risk: [LOW/MEDIUM/HIGH]
- Estimated effort: [days]

## API Surface
| Method | Route | Parameters | Returns |
|--------|-------|------------|---------|

## Dependencies
### Internal
- [namespace] — [what it provides]

### External (NuGet)
| Package | Version | .NET 8 Equivalent | Migration Action |
|---------|---------|-------------------|-----------------|

## Behavioral Baseline
- Routes served: [list]
- DB entities: [list]
- Config keys: [list]
- External calls: [list]

## Migration Risks
| Risk | Severity | Mitigation |
|------|----------|------------|

## Recommended Migration Path
1. ...
```

## Important Notes

- Legacy projects (.NET Framework) cannot be compiled on macOS — treat them as read-only reference
- Focus on extracting behavior, not running the legacy code
- The `legacy-archaeologist` agent can assist with deep dependency mapping
- The `dotnet8-slice-architect` agent can design the target shape after analysis
