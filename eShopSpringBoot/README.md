# eShop Catalog - Spring Boot

A Java 21 / Spring Boot 3.5.x migration of the legacy eShopLegacyMVC ASP.NET MVC 5 application.

## Prerequisites

- Java 21+
- Maven 3.9+
- SQL Server (or run with `mock` profile for in-memory data)

## Quick Start

### Run with Mock Data (no database required)

```bash
cd eShopSpringBoot
./mvnw spring-boot:run -Dspring-boot.run.profiles=mock
```

The application starts at http://localhost:8080

### Run with SQL Server

1. Ensure SQL Server is running with database `Microsoft.eShopOnContainers.Services.CatalogDb`
2. Update `src/main/resources/application.yml` with your connection details
3. Run:

```bash
cd eShopSpringBoot
./mvnw spring-boot:run
```

Flyway will automatically create tables and sequences on first run.

## Build

```bash
./mvnw clean package
```

## Test

```bash
./mvnw test
```

## Docker

```bash
docker build -t eshop-catalog .
docker run -p 8080:8080 -e SPRING_PROFILES_ACTIVE=mock eshop-catalog
```

## API Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| GET | /catalog | Paginated catalog list (MVC) |
| GET | /catalog/{id} | Item details (MVC) |
| GET | /catalog/create | Create form (MVC) |
| GET | /catalog/{id}/edit | Edit form (MVC) |
| GET | /catalog/{id}/delete | Delete confirmation (MVC) |
| GET | /api/brands | List all brands (REST) |
| GET | /api/brands/{id} | Get brand by ID (REST) |
| DELETE | /api/brands/{id} | Delete brand - demo only (REST) |
| GET | /api/files | List brands as DTOs (REST) |
| GET | /items/{id}/pic | Get item picture |

## Actuator

Health and metrics available at:
- GET /actuator/health
- GET /actuator/info
- GET /actuator/metrics

## Architecture

```
com.eshop.catalog
├── config/          # Configuration classes
├── controller/      # MVC controllers
│   └── api/         # REST API controllers
├── dto/             # Data transfer objects
├── entity/          # JPA entities
├── repository/      # Spring Data repositories
├── service/         # Service interfaces
│   └── impl/        # Service implementations
└── viewmodel/       # View models
```

## Migration Notes

This project migrates from:
- ASP.NET MVC 5 → Spring MVC + Thymeleaf
- Entity Framework 6 → Spring Data JPA + Hibernate
- SQL Server HiLo sequences → JPA SequenceGenerator
- log4net → SLF4J + Logback
- Web.config → application.yml
- Autofac DI → Spring IoC
- BinaryFormatter serialization → JSON (Jackson)

## Configuration

| Property | Description | Default |
|----------|-------------|---------|
| catalog.use-mock-data | Use in-memory mock data | false |
| catalog.use-customization-data | Load from CSV files | false |
| spring.profiles.active | Active profile (mock for no DB) | - |
