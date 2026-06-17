# Multi-stage build for eShopLegacyMVC
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS build
WORKDIR /src

COPY ["eShopLegacyMVC.csproj", "packages.config", "./"]
RUN nuget restore "eShopLegacyMVC.csproj" -PackagesDirectory ../packages

COPY . .
RUN msbuild "eShopLegacyMVC.csproj" /p:Configuration=Release /p:DeployOnBuild=true /p:PublishUrl=/app

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8
WORKDIR /inetpub/wwwroot

EXPOSE 80

COPY --from=build /app .

HEALTHCHECK --interval=30s --timeout=5s --start-period=60s --retries=3 \
  CMD powershell -Command "try { $response = Invoke-WebRequest -Uri http://localhost/health -UseBasicParsing; if ($response.StatusCode -eq 200) { exit 0 } else { exit 1 } } catch { exit 1 }"
