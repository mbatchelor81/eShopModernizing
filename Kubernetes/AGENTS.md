# Kubernetes Instructions

## Scope
- Applies to AKS and Windows-node deployment manifests.

## Patterns
- Preserve Windows container node selectors unless the task explicitly targets Linux portability.
- Keep app environment variables aligned with `Web.config` and Docker Compose names.
- Do not commit real passwords, Azure storage keys, AAD tenant IDs, or instrumentation keys.
- Prefer mock-data or placeholder values for demo manifests.

## Validation
- Run `git diff --check` for manifest whitespace.
- If `kubectl` is available, use `kubectl apply --dry-run=client -f <manifest>` on edited YAML files.
- Old manifests may target older Kubernetes API versions; call that out rather than broad-upgrading unrelated files.
