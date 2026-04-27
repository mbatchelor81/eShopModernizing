using eShopGrpcService.Data;
using Microsoft.EntityFrameworkCore;

namespace eShopGrpcService.Tests;

public static class TestDbContextFactory
{
    public static CatalogDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new CatalogDbContext(options);

        context.CatalogBrands.AddRange(PreconfiguredData.GetPreconfiguredCatalogBrands());
        context.CatalogTypes.AddRange(PreconfiguredData.GetPreconfiguredCatalogTypes());
        context.CatalogItems.AddRange(PreconfiguredData.GetPreconfiguredCatalogItems());
        context.CatalogItemsStocks.AddRange(PreconfiguredData.GetPreconfiguredCatalogItemsStock());
        context.DiscountItems.AddRange(PreconfiguredData.GetPreconfiguredDiscountItems());
        context.SaveChanges();

        return context;
    }
}
