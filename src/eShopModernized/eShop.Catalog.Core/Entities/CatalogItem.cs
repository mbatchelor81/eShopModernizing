namespace eShop.Catalog.Core.Entities;

public class CatalogItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string PictureFileName { get; set; } = string.Empty;
    public string? PictureUri { get; set; }
    public int CatalogTypeId { get; set; }
    public CatalogType CatalogType { get; set; } = null!;
    public int CatalogBrandId { get; set; }
    public CatalogBrand CatalogBrand { get; set; } = null!;
    public int AvailableStock { get; set; }
    public int RestockThreshold { get; set; }
    public int MaxStockThreshold { get; set; }
    public bool OnReorder { get; set; }
}
