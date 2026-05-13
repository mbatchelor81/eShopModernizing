---
trigger: glob
globs: **/*.{cs,cshtml,aspx,ascx,config,sln,csproj}
---

# .NET Framework modernization rules

- Preserve .NET Framework 4.x compatibility; do not migrate projects to SDK-style or .NET unless explicitly asked.
- Use existing ASP.NET MVC, WebForms, WCF, Entity Framework 6, Autofac, and log4net patterns.
- Keep `Web.config` app settings compatible with environment-variable config builders.
- Do not add new package dependencies unless the existing solution already uses the package family or the task requires it.
- Validate changed solutions with NuGet restore and Mono MSBuild when possible.
