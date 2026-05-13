# eShopModernized MVC Instructions

## Scope
- Applies to the modernized ASP.NET MVC 5 catalog application.
- Primary code lives under `src/eShopModernizedMVC/`.

## Patterns
- Use MVC controllers, Razor views, Autofac modules, and existing service interfaces.
- Catalog data access flows through `ICatalogService`; image handling flows through `IImageService`.
- Runtime toggles are read from `CatalogConfiguration` and `Web.config` app settings.
- Mock-data mode is valid for local demos; avoid making a SQL Server dependency mandatory for simple demo tasks.

## Demo-ready local-agent lanes
- Catalog UX fixes: `Controllers/CatalogController.cs`, `Views/Catalog/*.cshtml`, `Models/CatalogItem.cs`.
- Observability cleanup: `Filters/ActionTracerFilter.cs`, log4net and Application Insights configuration.
- Cloud-readiness checks: `Web.config`, Docker Compose files, and Kubernetes manifests that supply `UseMockData`, storage, and auth settings.

## Validation
- Restore with `nuget restore eShopModernizedMVCSolution/eShopModernizedMVC.sln`.
- Build with `msbuild eShopModernizedMVCSolution/eShopModernizedMVC.sln /t:Build /p:Configuration=Debug /p:Disable_CopyWebApplication=true`.
