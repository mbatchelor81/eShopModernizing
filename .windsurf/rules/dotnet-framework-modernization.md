---
trigger: glob
globs: eShopLegacyMVCSolution/**/*.{cs,cshtml,aspx,ascx,config,sln,csproj}, eShopLegacyWebFormsSolution/**/*.{cs,cshtml,aspx,ascx,config,sln,csproj}, eShopLegacyNTier/**/*.{cs,cshtml,aspx,ascx,config,sln,csproj}, eShopModernizedMVCSolution/**/*.{cs,cshtml,aspx,ascx,config,sln,csproj}, eShopModernizedWebFormsSolution/**/*.{cs,cshtml,aspx,ascx,config,sln,csproj}, eShopModernizedNTier/**/*.{cs,cshtml,aspx,ascx,config,sln,csproj}
---

# .NET Framework modernization rules

- Scope: legacy and Windows-container-era solutions only (`eShopLegacy*`, `eShopModernizedMVCSolution`, `eShopModernizedWebFormsSolution`, `eShopModernizedNTier`). For the new `eShopModernizedDotNet8/` path, see `dotnet8-modernization.md`.
- Preserve .NET Framework 4.x compatibility; do not migrate these projects to SDK-style or .NET unless explicitly asked.
- Use existing ASP.NET MVC, WebForms, WCF, Entity Framework 6, Autofac, and log4net patterns.
- Keep `Web.config` app settings compatible with environment-variable config builders.
- Do not add new package dependencies unless the existing solution already uses the package family or the task requires it.
- Validate changed solutions with NuGet restore and Mono MSBuild when possible.
- Treat these solutions as read-only reference when a task is explicitly migrating behavior into `eShopModernizedDotNet8/`; only edit them if the prompt asks for it.
