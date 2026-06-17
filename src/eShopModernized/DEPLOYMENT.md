# eShop Modernized — Deployment Guide

## Architecture Overview

The modernized eShop application consists of three services:

| Service | Description | Internal Port | External Port (dev) |
|---------|-------------|---------------|---------------------|
| **catalog-api** | REST API + gRPC service | 8080 (HTTP), 8081 (gRPC) | 5000, 5001 |
| **catalog-web** | MVC web UI | 8080 | 5002 |
| **sqlserver** | SQL Server 2022 database | 1433 | 1433 (dev only) |

## Prerequisites

- Docker Engine 24+ with Compose V2
- 4 GB RAM minimum (SQL Server requires 2 GB)
- Ports 5000, 5001, 5002 available (development)
- Ports 80, 5000, 5001 available (production)

## Local Development

### Quick Start

```bash
cd src/eShopModernized

# Build and start all services
docker compose up -d --build

# Verify health
curl http://localhost:5000/health   # API
curl http://localhost:5002/health   # Web UI

# View logs
docker compose logs -f

# Tear down (preserves data volume)
docker compose down

# Tear down and remove data
docker compose down -v
```

### Access Points

- **REST API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **gRPC**: http://localhost:5001
- **Web UI**: http://localhost:5002
- **SQL Server**: localhost:1433 (sa / Your_password123)

### Development Overrides

The `docker-compose.override.yml` is automatically applied and provides:
- Debug-level logging
- Volume mounts for appsettings files (live config reloading)
- SQL Server port exposed for direct access

## Production Deployment

### Environment Variables

Create a `.env` file or set these variables:

```env
# Required
CATALOG_DB_CONNECTION_STRING=Server=sqlserver;Database=CatalogDb;User Id=sa;Password=<STRONG_PASSWORD>;TrustServerCertificate=true
MSSQL_SA_PASSWORD=<STRONG_PASSWORD>

# Optional
IMAGE_TAG=latest
GITHUB_REPOSITORY_OWNER=mbatchelor81
```

### Deploy with Docker Compose

```bash
cd src/eShopModernized

# Use production overrides (skips dev override file)
docker compose -f docker-compose.yml -f docker-compose.production.yml up -d

# Verify
docker compose -f docker-compose.yml -f docker-compose.production.yml ps
```

### Production Configuration

The `docker-compose.production.yml` provides:

- **Resource limits**: CPU and memory constraints per service
- **Restart policies**: `unless-stopped` for all services
- **Log rotation**: JSON file driver with 10 MB max size, 3-5 files retained
- **Network isolation**: SQL Server port is not exposed externally
- **Pre-built images**: Pulls from GitHub Container Registry (ghcr.io)

### Resource Requirements

| Service | CPU Limit | Memory Limit | CPU Reserved | Memory Reserved |
|---------|-----------|-------------|--------------|-----------------|
| catalog-api | 1.0 | 512 MB | 0.25 | 128 MB |
| catalog-web | 0.5 | 256 MB | 0.25 | 64 MB |
| sqlserver | 2.0 | 2 GB | 0.5 | 1 GB |

## CI/CD Pipeline

The GitHub Actions workflow (`.github/workflows/dotnet-modernized.yml`) provides:

1. **Build & Test**: Restores, builds, and runs all unit/integration tests
2. **Docker Build**: Builds both service images in parallel (matrix strategy)
3. **Integration Test**: Runs `docker compose up`, waits for health checks, runs smoke tests
4. **Push to GHCR**: On merge to `main`, pushes tagged images to GitHub Container Registry

### Image Tags

- `ghcr.io/<owner>/eshop-catalog-api:<commit-sha>` — pinned to specific commit
- `ghcr.io/<owner>/eshop-catalog-api:latest` — latest main build
- Same pattern for `eshop-catalog-web`

## Building Individual Images

```bash
cd src/eShopModernized

# Build API image
docker build -f Dockerfile.api -t eshop-catalog-api:local .

# Build Web image
docker build -f Dockerfile.web -t eshop-catalog-web:local .

# Run API standalone (requires external SQL Server)
docker run -p 5000:8080 -p 5001:8081 \
  -e "ConnectionStrings__CatalogDb=Server=host.docker.internal;Database=CatalogDb;..." \
  eshop-catalog-api:local
```

## Health Checks

Both services expose a `/health` endpoint:

```bash
# API health
curl http://localhost:5000/health

# Web health
curl http://localhost:5002/health
```

Docker health checks are configured in each Dockerfile with:
- 30-second interval
- 5-second timeout
- 10-second start period
- 3 retries before marking unhealthy

## Troubleshooting

### SQL Server Won't Start
- Ensure 2 GB+ RAM is available
- Check `ACCEPT_EULA=Y` is set
- View logs: `docker compose logs sqlserver`

### API Can't Connect to Database
- Wait for SQL Server health check to pass before starting API
- Verify connection string matches the SA password
- Check network: `docker compose exec catalog-api ping sqlserver`

### Web UI Shows Connection Errors
- Verify `CatalogApiBaseUrl` points to `http://catalog-api:8080` (internal Docker network)
- Check API is healthy: `docker compose exec catalog-web curl http://catalog-api:8080/health`
