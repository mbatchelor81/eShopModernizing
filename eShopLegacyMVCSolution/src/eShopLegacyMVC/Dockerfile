# Multi-stage build for eShopLegacyMVC
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2022 AS build
WORKDIR /src

# Copy solution and project files for NuGet restore
COPY eShopLegacyMVC.sln ./
COPY src/eShopLegacyMVC/eShopLegacyMVC.csproj src/eShopLegacyMVC/
COPY src/eShopLegacyMVC/packages.config src/eShopLegacyMVC/
COPY eShopLegacy.Utilities/eShopLegacy.Utilities.csproj eShopLegacy.Utilities/
COPY eShopPorted/eShopPorted.csproj eShopPorted/
RUN nuget restore eShopLegacyMVC.sln

# Copy all source and build only the MVC project
COPY . .
RUN msbuild src/eShopLegacyMVC/eShopLegacyMVC.csproj /t:Build /p:Configuration=Release /p:OutputPath=C:\output

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2022
WORKDIR /inetpub/wwwroot

EXPOSE 80

# Copy build output
COPY --from=build C:\output\_PublishedWebsites\eShopLegacyMVC\ .

# Copy entrypoint script for environment variable substitution
COPY src/eShopLegacyMVC/docker-entrypoint.ps1 /docker-entrypoint.ps1

# Health check for container orchestrators
HEALTHCHECK --interval=30s --timeout=5s --start-period=60s --retries=3 \
  CMD powershell -Command "try { $response = Invoke-WebRequest -Uri http://localhost/health -UseBasicParsing; if ($response.StatusCode -eq 200) { exit 0 } else { exit 1 } } catch { exit 1 }"

ENTRYPOINT ["powershell", "-File", "C:\\docker-entrypoint.ps1"]
