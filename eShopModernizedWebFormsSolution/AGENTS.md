# eShopModernized WebForms Instructions

## Scope
- Applies to the modernized ASP.NET WebForms catalog application.
- Primary code lives under `src/eShopModernizedWebForms/`.

## Patterns
- Keep markup, code-behind, and designer files synchronized.
- Catalog data access flows through `ICatalogService`; image handling flows through `IImageService`.
- Autofac registrations live in `Modules/ApplicationModule.cs`.
- Preserve existing WebForms page lifecycle patterns.

## Demo-ready local-agent lanes
- Form validation and catalog CRUD improvements: `Catalog/*.aspx`, `Catalog/*.aspx.cs`, `Models/CatalogItem.cs`.
- Dependency-injection and mock-data toggles: `Modules/ApplicationModule.cs`, `Global.asax.cs`, `App_Start/CatalogConfiguration.cs`.
- Configuration hardening: `Web.config`, Docker Compose files, and Kubernetes manifests.

## Validation
- Restore with `nuget restore eShopModernizedWebFormsSolution/eShopModernizedWebForms.sln`.
- Build with `msbuild eShopModernizedWebFormsSolution/eShopModernizedWebForms.sln /t:Build /p:Configuration=Debug /p:Disable_CopyWebApplication=true`.
