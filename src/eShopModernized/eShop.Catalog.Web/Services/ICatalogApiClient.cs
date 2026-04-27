using eShop.Catalog.Core.Entities;

namespace eShop.Catalog.Web.Services;

public interface ICatalogApiClient
{
    Task<PaginatedItemsViewModel> GetCatalogItemsAsync(int pageSize, int pageIndex);
    Task<CatalogItem?> FindCatalogItemAsync(int id);
    Task<IEnumerable<CatalogBrand>> GetCatalogBrandsAsync();
    Task<IEnumerable<CatalogType>> GetCatalogTypesAsync();
    Task CreateCatalogItemAsync(CatalogItem item);
    Task UpdateCatalogItemAsync(CatalogItem item);
    Task RemoveCatalogItemAsync(int id);
}

public class PaginatedItemsViewModel
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public IEnumerable<CatalogItem> Data { get; set; } = Enumerable.Empty<CatalogItem>();

    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public int ActualPage => PageIndex;
    public int ItemsPerPage => PageSize;
}
