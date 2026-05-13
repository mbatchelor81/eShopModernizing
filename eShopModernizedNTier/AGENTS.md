# eShopModernized N-Tier Instructions

## Scope
- Applies to the modernized WCF service and WinForms client sample.
- Primary code lives under `src/eShopWCFService/` and `src/eShopWinForms/`.

## Patterns
- Keep service contracts and client proxies compatible.
- WCF service changes should preserve `basicHttpBinding` behavior unless the task explicitly targets transport modernization.
- WinForms changes may require Windows-only validation; document when Linux/Mono can build only the service layer.

## Demo-ready local-agent lanes
- WCF service hardening: `src/eShopWCFService/CatalogService.svc.cs`, `Web.config`, model classes.
- Client modernization review: `src/eShopWinForms/` controllers, modules, and UI forms.
- Deployment readiness: `docker-compose*.yml` and Kubernetes WCF manifests.

## Validation
- Restore with `nuget restore eShopModernizedNTier/eShopModernizedNTier.sln`.
- Build with `msbuild eShopModernizedNTier/eShopModernizedNTier.sln /t:Build /p:Configuration=Debug /p:Disable_CopyWebApplication=true`.
