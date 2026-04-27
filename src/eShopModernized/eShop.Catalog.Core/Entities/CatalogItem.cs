namespace eShop.Catalog.Core.Entities;

/// <summary>
/// Domain entity for catalog items. Will be backed by EF Core in a later wave.
/// </summary>
public class CatalogItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string PictureFileName { get; set; } = string.Empty;
    public int CatalogTypeId { get; set; }
    public int CatalogBrandId { get; set; }
    public int AvailableStock { get; set; }
    public int MaxStockThreshold { get; set; }
    public bool OnReorder { get; set; }
}
