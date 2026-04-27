using System.Net.Http.Json;
using eShop.Catalog.Core.Entities;

namespace eShop.Catalog.Web.Services;

public class CatalogApiClient : ICatalogApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CatalogApiClient> _logger;

    public CatalogApiClient(HttpClient httpClient, ILogger<CatalogApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PaginatedItemsViewModel> GetCatalogItemsAsync(int pageSize, int pageIndex)
    {
        _logger.LogInformation("Fetching catalog items page {PageIndex} size {PageSize}", pageIndex, pageSize);

        var response = await _httpClient.GetAsync($"/api/catalog/items?pageSize={pageSize}&pageIndex={pageIndex}");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ApiPaginatedResponse>();
        if (result is null)
            return new PaginatedItemsViewModel();

        return new PaginatedItemsViewModel
        {
            PageIndex = result.PageIndex,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            Data = result.Data ?? Enumerable.Empty<CatalogItem>()
        };
    }

    public async Task<CatalogItem?> FindCatalogItemAsync(int id)
    {
        _logger.LogInformation("Finding catalog item {Id}", id);

        var response = await _httpClient.GetAsync($"/api/catalog/items/{id}");
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<CatalogItem>();
    }

    public async Task<IEnumerable<CatalogBrand>> GetCatalogBrandsAsync()
    {
        var response = await _httpClient.GetAsync("/api/catalog/brands");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<IEnumerable<CatalogBrand>>()
            ?? Enumerable.Empty<CatalogBrand>();
    }

    public async Task<IEnumerable<CatalogType>> GetCatalogTypesAsync()
    {
        var response = await _httpClient.GetAsync("/api/catalog/types");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<IEnumerable<CatalogType>>()
            ?? Enumerable.Empty<CatalogType>();
    }

    public async Task CreateCatalogItemAsync(CatalogItem item)
    {
        _logger.LogInformation("Creating catalog item {Name}", item.Name);

        var request = new
        {
            item.Name,
            item.Description,
            item.Price,
            item.PictureFileName,
            item.CatalogTypeId,
            item.CatalogBrandId,
            item.AvailableStock,
            item.RestockThreshold,
            item.MaxStockThreshold,
            item.OnReorder
        };

        var response = await _httpClient.PostAsJsonAsync("/api/catalog/items", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateCatalogItemAsync(CatalogItem item)
    {
        _logger.LogInformation("Updating catalog item {Id}", item.Id);

        var request = new
        {
            item.Name,
            item.Description,
            item.Price,
            item.PictureFileName,
            item.CatalogTypeId,
            item.CatalogBrandId,
            item.AvailableStock,
            item.RestockThreshold,
            item.MaxStockThreshold,
            item.OnReorder
        };

        var response = await _httpClient.PutAsJsonAsync($"/api/catalog/items/{item.Id}", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveCatalogItemAsync(int id)
    {
        _logger.LogInformation("Removing catalog item {Id}", id);

        var response = await _httpClient.DeleteAsync($"/api/catalog/items/{id}");
        response.EnsureSuccessStatusCode();
    }

    private record ApiPaginatedResponse(
        int PageIndex,
        int PageSize,
        int TotalCount,
        IEnumerable<CatalogItem>? Data);
}
