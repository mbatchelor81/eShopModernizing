---
name: dotnet8-migration-slice
description: Build a Mac-runnable .NET 8 modernization slice from the legacy eShop catalog
---

# /dotnet8-migration-slice

Use this workflow inside a Worktree-mode local Cascade session when the demo goal is to create or extend a .NET 8 ASP.NET Core path that can run on macOS.

## Steps

1. Confirm the session is local-only and running in a git worktree.
2. Treat the existing .NET Framework MVC/WebForms/WCF projects as reference implementations; do not attempt to compile them on macOS unless explicitly asked.
3. Keep new code under `eShopModernizedDotNet8/` or a clearly named `.NET 8` test/docs path.
4. Prefer mock or in-memory catalog data first so SQL Server, Visual Studio, IIS, and Windows Containers are not required.
5. Port one thin catalog behavior at a time: model, read API, list UI, create/edit validation, or local smoke-test docs.
6. Use ASP.NET Core and .NET 8 idioms while preserving business names from the legacy catalog where useful.
7. Validate with `dotnet restore`, `dotnet build`, `dotnet test` when tests exist, and `dotnet run` for a local smoke test.
8. Summarize what legacy behavior was ported, what remains legacy-only, and exactly how to run the .NET 8 slice on macOS.

## Output format

- Legacy reference files inspected:
- New .NET 8 files changed:
- Local validation:
- How to run on macOS:
- Migration gaps:
