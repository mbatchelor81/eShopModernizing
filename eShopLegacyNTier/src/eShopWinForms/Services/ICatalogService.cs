using eShopWinForms.Models;
using System;
using System.Collections.Generic;

namespace eShopWinForms.Services
{
    public interface ICatalogService : IDisposable
    {
        CatalogItem FindCatalogItem(int id);
        List<CatalogBrand> GetCatalogBrands();
        List<CatalogItem> GetCatalogItems(int brandIdFilter, int typeIdFilter);
        List<CatalogType> GetCatalogTypes();
        int GetAvailableStock(DateTime date, int catalogItemId);
        void CreateAvailableStock(CatalogItemsStock catalogItemsStock);
        void CreateCatalogItem(CatalogItem catalogItem);
        void UpdateCatalogItem(CatalogItem catalogItem);
        void RemoveCatalogItem(CatalogItem catalogItem);
        DiscountItem GetDiscount(DateTime day);
    }
}
