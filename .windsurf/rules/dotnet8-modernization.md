---
trigger: glob
globs: eShopModernizedDotNet8/**/*.{cs,cshtml,csproj,json,razor}, eShopModernizedDotNet8.Tests/**/*.{cs,csproj,json}
---

# .NET 8 modernization rules

- Scope: the new Mac-runnable .NET 8 ASP.NET Core slice under `eShopModernizedDotNet8/` and its optional `eShopModernizedDotNet8.Tests/` companion. For the legacy .NET Framework solutions, see `dotnet-framework-modernization.md`.
- Target framework is `net8.0`; use SDK-style `.csproj`, nullable enabled, implicit usings allowed.
- Use ASP.NET Core Razor Pages (not MVC controllers), dependency injection via `builder.Services`, and `appsettings.json` instead of `Web.config`.
- Do not bring forward .NET Framework-only dependencies (Autofac, log4net, EF6, System.Web, WCF). Prefer the built-in DI container, `Microsoft.Extensions.Logging`, and `Microsoft.EntityFrameworkCore` only if explicitly required.
- Seed catalog data from an in-memory service; do not introduce SQL Server, IIS, Windows Containers, Azure services, or real secrets.
- Mirror legacy behavior shape (field names, pagination metadata, route segments) when migrating, but do not copy `.cshtml` markup verbatim if a Razor Pages-idiomatic form is cleaner.
- Treat files under `eShopLegacy*`, `eShopModernizedMVCSolution`, `eShopModernizedWebFormsSolution`, and `eShopModernizedNTier` as read-only behavioral reference unless the prompt explicitly authorizes edits.
- Validate locally with `dotnet restore`, `dotnet build`, and (when relevant) `dotnet run --project eShopModernizedDotNet8/eShopModernizedDotNet8.csproj`; use `dotnet test` if a test project is added. Do not require Mono MSBuild, NuGet.exe, or Windows-only tooling.
