---
trigger: model_decision
description: Apply when editing Docker Compose, Kubernetes, ACI, Service Fabric, or Azure configuration for the eShop modernization samples.
---

# Container and cloud readiness

- Treat this as a Windows Containers sample; do not assume Linux container runtime compatibility.
- Keep environment-variable names aligned across `Web.config`, Docker Compose, and Kubernetes manifests.
- Prefer placeholders for cloud settings: storage connection strings, AAD client IDs, tenant names, App Insights keys, Key Vault names.
- Do not commit real credentials.
- Preserve mock-data options where they make local demos simpler and faster.
- For Kubernetes changes, call out old API versions separately from functional application changes.
