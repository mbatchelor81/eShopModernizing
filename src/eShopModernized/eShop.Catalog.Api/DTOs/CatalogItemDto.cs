namespace eShop.Catalog.Api.DTOs;

/// <summary>
/// Represents a catalog item returned by the API.
/// </summary>
public record CatalogItemDto(
    int Id,
    string Name,
    string Description,
    decimal Price,
    string PictureFileName,
    string? PictureUri,
    int CatalogTypeId,
    string CatalogTypeName,
    int CatalogBrandId,
    string CatalogBrandName,
    int AvailableStock,
    int RestockThreshold,
    int MaxStockThreshold,
    bool OnReorder);

/// <summary>
/// Request body for creating a new catalog item.
/// </summary>
public record CreateCatalogItemRequest(
    string Name,
    string Description,
    decimal Price,
    string PictureFileName,
    int CatalogTypeId,
    int CatalogBrandId,
    int AvailableStock,
    int RestockThreshold = 0,
    int MaxStockThreshold = 0,
    bool OnReorder = false);

/// <summary>
/// Request body for updating an existing catalog item.
/// </summary>
public record UpdateCatalogItemRequest(
    string Name,
    string Description,
    decimal Price,
    string PictureFileName,
    int CatalogTypeId,
    int CatalogBrandId,
    int AvailableStock,
    int RestockThreshold = 0,
    int MaxStockThreshold = 0,
    bool OnReorder = false);

/// <summary>
/// Represents a catalog brand.
/// </summary>
public record CatalogBrandDto(int Id, string Brand);

/// <summary>
/// Represents a catalog type.
/// </summary>
public record CatalogTypeDto(int Id, string Type);

/// <summary>
/// Represents stock availability for a catalog item on a given date.
/// </summary>
public record CatalogItemStockDto(int StockId, DateTime Date, int CatalogItemId, int AvailableStock);

/// <summary>
/// Request body for creating or updating stock availability.
/// </summary>
public record CreateStockRequest(DateTime Date, int CatalogItemId, int AvailableStock);

/// <summary>
/// Represents a discount item.
/// </summary>
public record DiscountItemDto(int Id, double Size, DateTime Start, DateTime End);

/// <summary>
/// Paginated response wrapper.
/// </summary>
public record PaginatedResponse<T>(int PageIndex, int PageSize, int TotalCount, IEnumerable<T> Data);
