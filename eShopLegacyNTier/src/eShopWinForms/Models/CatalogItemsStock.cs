using System;

namespace eShopWinForms.Models
{
    public class CatalogItemsStock
    {
        public DateTime Date { get; set; }
        public int CatalogItemId { get; set; }
        public int AvailableStock { get; set; }
        public int StockId { get; set; }
    }
}
