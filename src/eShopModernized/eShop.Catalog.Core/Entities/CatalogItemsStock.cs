namespace eShop.Catalog.Core.Entities;

public class CatalogItemsStock
{
    public int StockId { get; set; }
    public DateTime Date { get; set; }
    public int CatalogItemId { get; set; }
    public int AvailableStock { get; set; }
}
