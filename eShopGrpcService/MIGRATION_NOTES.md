# WCF to gRPC Migration Notes

## Overview
This migration converts the eShop WCF CatalogService to a .NET 8 ASP.NET Core gRPC service.

## Architecture Changes
| Aspect | WCF (Before) | gRPC (After) |
|--------|-------------|-------------|
| Protocol | SOAP/HTTP | HTTP/2 + Protocol Buffers |
| Framework | .NET Framework 4.6.1 | .NET 8 |
| ORM | Entity Framework 6 | Entity Framework Core 8 |
| DI | Manual instantiation | ASP.NET Core built-in DI |
| Serialization | DataContract/DataMember | Protobuf messages |
| Service Contract | [ServiceContract]/[OperationContract] | proto3 service definition |
| Seeding | CreateDatabaseIfNotExists | ModelBuilder.HasData() |
| Async | Synchronous | Fully async (Task-based) |

## Bug Fixes During Migration
1. **GetCatalogItems**: Server-side filtering (was client-side `.ToList().Where()`)
2. **GetDiscount**: Server-side date filtering (was client-side)
3. **FindCatalogItem**: Uses `.Include()` for eager loading (was manual nav property loading)
4. **Null handling**: Returns `RpcException(NotFound)` instead of null

## Preserved Behaviors
- CreateAvailableStock upsert logic (check existing entry, update or insert)
- All seed data (12 items, 5 brands, 4 types, 6 stock entries, 6 discounts)
- Database schema compatibility (same table names and column types)

## Running the Service
```bash
cd eShopGrpcService
dotnet run
# Service starts on http://localhost:5001 (HTTP/2)
```

## Running Tests
```bash
cd eShopGrpcService.Tests
dotnet test
```

## gRPC Reflection
In Development mode, gRPC reflection is enabled for tooling:
```bash
grpcurl -plaintext localhost:5001 list
grpcurl -plaintext localhost:5001 catalog.CatalogService/GetCatalogBrands
```
