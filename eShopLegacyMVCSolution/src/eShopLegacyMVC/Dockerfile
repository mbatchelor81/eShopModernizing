# Multi-stage build for eShopLegacyMVC
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2022 AS build
WORKDIR /src

# Copy solution and restore NuGet packages
COPY eShopLegacyMVC.sln ./
COPY src/eShopLegacyMVC/eShopLegacyMVC.csproj src/eShopLegacyMVC/
COPY src/eShopLegacyMVC/packages.config src/eShopLegacyMVC/
RUN nuget restore eShopLegacyMVC.sln

# Copy all source and build
COPY . .
RUN msbuild eShopLegacyMVC.sln /t:Build /p:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile=FolderProfile

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2022
WORKDIR /inetpub/wwwroot

EXPOSE 80

# Copy published output
COPY --from=build /src/src/eShopLegacyMVC/obj/Release/Package/PackageTmp/ .

# Health check for container orchestrators
HEALTHCHECK --interval=30s --timeout=5s --start-period=60s --retries=3 \
  CMD powershell -Command "try { $response = Invoke-WebRequest -Uri http://localhost/health -UseBasicParsing; if ($response.StatusCode -eq 200) { exit 0 } else { exit 1 } } catch { exit 1 }"
