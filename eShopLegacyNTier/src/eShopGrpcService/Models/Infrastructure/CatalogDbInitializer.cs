using eShopGrpcService.Models;

namespace eShopGrpcService.Models.Infrastructure;

public static class CatalogDbInitializer
{
    public static void Seed(CatalogDbContext context)
    {
        if (!context.CatalogTypes.Any())
        {
            context.AddRange(PreconfiguredData.GetPreconfiguredCatalogTypes());
            context.SaveChanges();
        }

        if (!context.CatalogBrands.Any())
        {
            context.AddRange(PreconfiguredData.GetPreconfiguredCatalogBrands());
            context.SaveChanges();
        }

        if (!context.CatalogItems.Any())
        {
            context.AddRange(PreconfiguredData.GetPreconfiguredCatalogItems());
            context.SaveChanges();
        }

        if (!context.CatalogItemsStocks.Any())
        {
            context.AddRange(PreconfiguredData.GetPreconfiguredCatalogItemsStock());
            context.SaveChanges();
        }

        if (!context.DiscountItems.Any())
        {
            context.AddRange(PreconfiguredData.GetPreconfiguredDiscountItems());
            context.SaveChanges();
        }
    }
}
