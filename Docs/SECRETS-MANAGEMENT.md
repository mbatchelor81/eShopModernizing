# Secrets Management Guide

This document describes how database credentials and other sensitive configuration values are managed across the eShopModernizing solution.

## Overview

All hardcoded credentials have been removed from committed configuration files. Secrets are now injected at runtime through environment variables, .NET User Secrets, or Azure Key Vault — depending on the deployment target.

## Local Development

### Option 1: .NET User Secrets (recommended for local dev)

The MVC and WebForms projects are already configured with [Configuration Builders](https://docs.microsoft.com/en-us/aspnet/config-builder) that read from User Secrets. To set up:

```bash
# Navigate to the project directory
cd eShopModernizedMVCSolution/src/eShopModernizedMVC

# Set the connection string via User Secrets
dotnet user-secrets set "CatalogDBContext" "Server=(localdb)\mssqllocaldb;Initial Catalog=Microsoft.eShopOnContainers.Services.CatalogDb;User Id=sa;Password=YourStrongPassword"
```

The `userSecretsId` is already defined in the Web.config configBuilders section (`5702632e-73c6-478a-a875-2714c09d921b`).

### Option 2: Environment Variables

Set the following environment variables before running the application:

| Variable | Description |
|----------|-------------|
| `DB_USER` | SQL Server username (default: `sa`) |
| `DB_PASSWORD` | SQL Server password |
| `CatalogDBContext` | Full connection string (overrides individual user/password) |
| `ConnectionString` | Full connection string (used by WebForms and WCF) |

## Docker Compose

### Setup

1. Copy the environment template:
   ```bash
   cp .env.template .env
   ```

2. Edit `.env` and set a strong password:
   ```
   DB_PASSWORD=YourStrong!Passw0rd
   ```

3. Run docker-compose as normal:
   ```bash
   docker-compose up
   ```

Docker Compose automatically reads the `.env` file and substitutes `${DB_USER}` and `${DB_PASSWORD}` in the override files.

> **Important:** Never commit the `.env` file. It is listed in `.gitignore`.

## Kubernetes

Kubernetes deployments now reference a `Secret` object named `eshop-db-secrets`. Create it before deploying:

```bash
kubectl create secret generic eshop-db-secrets \
  --from-literal=sa-password='YourStrong!Passw0rd' \
  --from-literal=catalog-connection-string='Server=sql-data;Database=Microsoft.eShopOnContainers.Services.CatalogDb;User Id=sa;Password=YourStrong!Passw0rd' \
  --from-literal=wcf-connection-string='Server=sql-data-for-wcf;Database=eShopDatabase;User Id=sa;Password=YourStrong!Passw0rd'
```

For production, use a secrets management solution such as:
- **Azure Key Vault** with the [CSI Secrets Store Driver](https://learn.microsoft.com/en-us/azure/aks/csi-secrets-store-driver)
- **HashiCorp Vault** with the Vault Agent Injector
- **Sealed Secrets** for GitOps workflows

## Azure Deployment

The Web.config files include an `AzureKeyVault` configuration builder. To use it:

1. Create an Azure Key Vault and store the connection string as a secret named `CatalogDBContext`.
2. Set the `KeyVaultName` app setting (via environment variable or App Service configuration).
3. Enable Managed Identity on your App Service or Container Instance.

## Credential Rotation

Since credentials were previously committed to this repository, they should be considered compromised. Rotate all SQL Server passwords immediately in any environment that used the old defaults.

## Files Modified

The following files had hardcoded credentials removed:

### Web.config
- `eShopModernizedWebFormsSolution/src/eShopModernizedWebForms/Web.config`

### Docker Compose
- `docker-compose.override.yml` (root)
- `eShopModernizedMVCSolution/docker-compose.override.yml`
- `eShopModernizedMVCSolution/docker-compose.override.nosql.yml`
- `eShopModernizedWebFormsSolution/docker-compose.override.yml`
- `eShopModernizedNTier/docker-compose.override.yml`
- `eShopModernizedNTier/temp/docker-compose.override.yml`
- `eShopModernizedNTier/temp/docker-compose.ci.build.yml`
- `VM/eShopModernizedMVC/docker-compose.override.yml`
- `VM/eShopModernizedMVC/docker-compose.override.nosql.yml`
- `VM/eShopModernizedWebForms/docker-compose.override.yml`

### Kubernetes
- `Kubernetes/eShopModernizedSQL-K8s/eshop-modernized-sql-k8s-services-deployment.yml`
- `Kubernetes/eShopModernizedNTier-WCF-K8s/eshop-sql-container-deployment.yml`
- `Kubernetes/eShopModernizedNTier-WCF-K8s/eshop-wcf-container-deployment.yml`
- `Kubernetes/eShopModernizedWebForms-K8s/eshop-modernized-webforms-k8s-services-deployment.yml`
- `Kubernetes/eShopModernizedWebForms-K8s/eshop-modernized-webforms-k8s-services-CD-deployment.yml`
- `Kubernetes/eShopModernizedMVC-K8s/ServiceDeployments/eshop-modernized-mvc-k8s-services-deployment.yml`
- `Kubernetes/eShopModernizedMVC-K8s/ServiceDeployments/eshop-modernized-mvc-k8s-services-CD-deployment.yml`
- `Kubernetes-DockerForWindows/eShopModernizedMVC-K8s/eshop-modernized-mvc-k8s-services-deployment.yml`
- `Kubernetes-DockerForWindows/eShopModernizedSQL-K8s/eshop-modernized-sql-k8s-services-deployment.yml`
