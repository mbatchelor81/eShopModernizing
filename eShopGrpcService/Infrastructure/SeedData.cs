using eShopGrpcService.Data;
using eShopGrpcService.Models;
using Microsoft.EntityFrameworkCore;

namespace eShopGrpcService.Infrastructure;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new CatalogDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<CatalogDbContext>>());

        if (context.CatalogBrands.Any())
            return;

        context.CatalogTypes.AddRange(GetPreconfiguredCatalogTypes());
        context.SaveChanges();

        context.CatalogBrands.AddRange(GetPreconfiguredCatalogBrands());
        context.SaveChanges();

        context.CatalogItems.AddRange(GetPreconfiguredCatalogItems());
        context.SaveChanges();

        context.CatalogItemsStocks.AddRange(GetPreconfiguredCatalogItemsStock());
        context.SaveChanges();

        context.DiscountItems.AddRange(GetPreconfiguredDiscountItems());
        context.SaveChanges();
    }

    public static List<CatalogItem> GetPreconfiguredCatalogItems()
    {
        return new List<CatalogItem>
        {
            new() { Id = 1, CatalogTypeId = 2, CatalogBrandId = 2, Description = ".NET Bot Black Hoodie", Name = ".NET Bot Black Hoodie", Price = 19.5M, Picturefilename = "2.png" },
            new() { Id = 2, CatalogTypeId = 1, CatalogBrandId = 2, Description = ".NET Black & White Mug", Name = ".NET Black & White Mug", Price = 8.50M, Picturefilename = "11.png" },
            new() { Id = 3, CatalogTypeId = 2, CatalogBrandId = 5, Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, Picturefilename = "7.png" },
            new() { Id = 4, CatalogTypeId = 2, CatalogBrandId = 2, Description = ".NET Foundation T-shirt", Name = ".NET Foundation T-shirt", Price = 12, Picturefilename = "5.png" },
            new() { Id = 5, CatalogTypeId = 3, CatalogBrandId = 5, Description = "Roslyn Red Sheet", Name = "Roslyn Red Sheet", Price = 8.5M, Picturefilename = "9.png" },
            new() { Id = 6, CatalogTypeId = 2, CatalogBrandId = 2, Description = ".NET Blue Hoodie", Name = ".NET Blue Hoodie", Price = 12, Picturefilename = "1.png" },
            new() { Id = 7, CatalogTypeId = 2, CatalogBrandId = 5, Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, Picturefilename = "6.png" },
            new() { Id = 8, CatalogTypeId = 2, CatalogBrandId = 5, Description = "Kudu Purple Hoodie", Name = "Kudu Purple Hoodie", Price = 8.5M, Picturefilename = "3.png" },
            new() { Id = 9, CatalogTypeId = 1, CatalogBrandId = 5, Description = "Cup<T> White Mug", Name = "Cup<T> White Mug", Price = 12, Picturefilename = "12.png" },
            new() { Id = 10, CatalogTypeId = 3, CatalogBrandId = 2, Description = ".NET Foundation Sheet", Name = ".NET Foundation Sheet", Price = 12, Picturefilename = "8.png" },
            new() { Id = 11, CatalogTypeId = 3, CatalogBrandId = 2, Description = "Cup<T> Sheet", Name = "Cup<T> Sheet", Price = 8.5M, Picturefilename = "10.png" },
            new() { Id = 12, CatalogTypeId = 2, CatalogBrandId = 5, Description = "Cup<T> TShirt", Name = "Cup<T> TShirt", Price = 12, Picturefilename = "4.png" },
        };
    }

    public static List<CatalogBrand> GetPreconfiguredCatalogBrands()
    {
        return new List<CatalogBrand>
        {
            new() { Id = 1, Brand = "Azure" },
            new() { Id = 2, Brand = ".NET" },
            new() { Id = 3, Brand = "Visual Studio" },
            new() { Id = 4, Brand = "SQL Server" },
            new() { Id = 5, Brand = "Other" }
        };
    }

    public static List<DiscountItem> GetPreconfiguredDiscountItems()
    {
        return new List<DiscountItem>
        {
            new() { Id = 1, Start = new DateTime(2017, 9, 18), End = new DateTime(2017, 9, 21), Size = 0.3 },
            new() { Id = 2, Start = new DateTime(2017, 9, 22), End = new DateTime(2017, 9, 26), Size = 0.25 },
            new() { Id = 3, Start = new DateTime(2017, 9, 27), End = new DateTime(2017, 9, 30), Size = 0.1 },
            new() { Id = 4, Start = new DateTime(2017, 10, 5), End = new DateTime(2017, 10, 20), Size = 0.5 },
            new() { Id = 5, Start = new DateTime(2017, 11, 13), End = new DateTime(2017, 11, 25), Size = 0.3 },
            new() { Id = 6, Start = new DateTime(2017, 12, 20), End = new DateTime(2017, 12, 25), Size = 0.25 },
        };
    }

    public static List<CatalogType> GetPreconfiguredCatalogTypes()
    {
        return new List<CatalogType>
        {
            new() { Id = 1, Type = "Mug" },
            new() { Id = 2, Type = "T-Shirt" },
            new() { Id = 3, Type = "Sheet" },
            new() { Id = 4, Type = "USB Memory Stick" }
        };
    }

    public static List<CatalogItemsStock> GetPreconfiguredCatalogItemsStock()
    {
        return new List<CatalogItemsStock>
        {
            new() { StockId = 1, CatalogItemId = 1, Date = new DateTime(2017, 9, 20), AvailableStock = 100 },
            new() { StockId = 2, CatalogItemId = 1, Date = new DateTime(2017, 9, 21), AvailableStock = 120 },
            new() { StockId = 3, CatalogItemId = 1, Date = new DateTime(2017, 9, 22), AvailableStock = 80 },
            new() { StockId = 4, CatalogItemId = 2, Date = new DateTime(2017, 9, 20), AvailableStock = 45 },
            new() { StockId = 5, CatalogItemId = 4, Date = new DateTime(2017, 9, 25), AvailableStock = 65 },
            new() { StockId = 6, CatalogItemId = 5, Date = new DateTime(2017, 9, 28), AvailableStock = 22 },
        };
    }
}
