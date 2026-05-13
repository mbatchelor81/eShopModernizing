# Project Instructions

## Demo constraint
- This repository is prepared for a Windsurf 2.0 Agent Command Center demo using locally-executing agents only.
- Allowed agent runtimes: Windsurf local Cascade agents and Devin Local / Devin for Terminal (running on this machine).
- Do not use cloud Devin sessions, cloud agents, or any cloud-hosted agent execution path in demo prompts or workflows.
- Prefer Worktree mode for agent work so parallel sessions stay isolated until a human reviews and merges them.

## Project overview
- eShopModernizing demonstrates legacy .NET Framework modernization with Windows Containers and Azure.
- The repo contains legacy and modernized ASP.NET MVC, ASP.NET WebForms, WCF, and WinForms catalog-management samples.
- High-value demo areas:
  - `eShopModernizedMVCSolution/`: modernized ASP.NET MVC 5 catalog app.
  - `eShopModernizedWebFormsSolution/`: modernized ASP.NET WebForms catalog app.
  - `eShopModernizedNTier/`: WCF service plus WinForms client modernization path.
  - `Kubernetes/`, `ACI/`, `ServiceFabric/`, `VM/`: deployment modernization targets.

## Coding conventions
- Preserve existing .NET Framework and ASP.NET MVC/WebForms conventions.
- Keep changes minimal and scoped to the target solution.
- Use C# naming conventions already present in the touched file.
- Prefer constructor injection and existing Autofac registrations for new services.
- Do not move generated designer files unless the matching markup/code-behind requires it.
- Do not commit secrets, real connection strings, Azure keys, or local `.env` files.

## Build and validation commands
- Restore NuGet packages:
  - `nuget restore eShopModernizedMVCSolution/eShopModernizedMVC.sln`
  - `nuget restore eShopModernizedWebFormsSolution/eShopModernizedWebForms.sln`
  - `nuget restore eShopModernizedNTier/eShopModernizedNTier.sln`
- Linux/Mono build check:
  - `msbuild eShopModernizedMVCSolution/eShopModernizedMVC.sln /t:Build /p:Configuration=Debug /p:Disable_CopyWebApplication=true`
  - `msbuild eShopModernizedWebFormsSolution/eShopModernizedWebForms.sln /t:Build /p:Configuration=Debug /p:Disable_CopyWebApplication=true`
  - `msbuild eShopModernizedNTier/eShopModernizedNTier.sln /t:Build /p:Configuration=Debug /p:Disable_CopyWebApplication=true`
- There are no dedicated test projects in this repository; use focused build checks for validation.
- Windows container execution requires Windows Docker/Visual Studio tooling and is not expected to run in Linux worktrees.

## Branch and PR conventions
- Use short, descriptive feature branches.
- Keep each local-agent worktree focused on one concern.
- Merge worktree results only after reviewing the diff and running the relevant build command.
