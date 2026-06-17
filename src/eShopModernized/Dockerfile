FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Directory.Build.props .
COPY eShop.Shared.Core/eShop.Shared.Core.csproj eShop.Shared.Core/
COPY eShop.Catalog.Core/eShop.Catalog.Core.csproj eShop.Catalog.Core/
COPY eShop.Catalog.Api/eShop.Catalog.Api.csproj eShop.Catalog.Api/
COPY eShop.Catalog.Tests/eShop.Catalog.Tests.csproj eShop.Catalog.Tests/
COPY eShopModernized.sln .
RUN dotnet restore

COPY . .
RUN dotnet publish eShop.Catalog.Api/eShop.Catalog.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "eShop.Catalog.Api.dll"]
