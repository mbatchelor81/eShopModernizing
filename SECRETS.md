# Secrets Management Guide

This document describes how to manage database credentials and other sensitive configuration for the eShopModernizing application.

## Overview

All plaintext credentials have been removed from committed configuration files. Secrets are now injected at runtime via environment variables, .NET User Secrets, or Azure Key Vault.

## Local Development (Docker Compose)

1. Copy the environment template:

   ```bash
   cp .env.example .env
   ```

2. Set a strong SA password in `.env`:

   ```
   SA_PASSWORD=YourStrong!Passw0rd
   ```

3. Run docker-compose as usual — it will read from `.env` automatically:

   ```bash
   docker-compose -f docker-compose.yml -f docker-compose.override.yml up
   ```

> **Important:** The `.env` file must never be committed to source control.

## Local Development (Plain .NET without Docker)

For running the MVC or WebForms projects directly on your machine:

1. Initialize User Secrets for the project:

   ```bash
   cd eShopModernizedMVCSolution/src/eShopModernizedMVC
   dotnet user-secrets init
   dotnet user-secrets set "CatalogDBContext" "Server=(localdb)\mssqllocaldb;Initial Catalog=Microsoft.eShopOnContainers.Services.CatalogDb;Trusted_Connection=True;MultipleActiveResultSets=true;"
   ```

2. The `Web.config` already references `configBuilders` (Secrets, Environment, AzureKeyVault) which override the placeholder connection strings at runtime.

## Kubernetes Deployments

Before deploying, create the database secret in your cluster:

```bash
kubectl create secret generic eshop-db-secrets \
  --from-literal=sa-password='<YourStrongPassword>'
```

See `Kubernetes/eshop-db-secrets.example.yml` for a manifest-based approach.

## Production (Azure Key Vault)

For production deployments, store connection strings in Azure Key Vault:

1. Create a Key Vault and add the connection string as a secret.
2. Set the `KeyVaultName` app setting to your vault name.
3. Ensure the application has Managed Identity access to the vault.

The `configBuilders` in `Web.config` will automatically resolve secrets from Key Vault when `UseAzureManagedIdentity` is enabled.

## Credential Rotation

Since credentials were previously committed to this repository, the `sa` password (`Pass@word`) should be considered compromised. Rotate all database passwords in every environment before deploying this change.

## What NOT to Do

- Do not add passwords to `appsettings.json`, `Web.config`, or any committed file.
- Do not commit `.env` files — only `.env.example` (with placeholders) is tracked.
- Do not embed credentials in Dockerfiles or Kubernetes manifests.
