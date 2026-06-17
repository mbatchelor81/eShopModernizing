using eShop.Catalog.Core.Data.Configuration;
using eShop.Catalog.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace eShop.Catalog.Core.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
    public DbSet<CatalogBrand> CatalogBrands => Set<CatalogBrand>();
    public DbSet<CatalogType> CatalogTypes => Set<CatalogType>();
    public DbSet<CatalogItemsStock> CatalogItemsStocks => Set<CatalogItemsStock>();
    public DbSet<DiscountItem> DiscountItems => Set<DiscountItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CatalogItemConfiguration());
        modelBuilder.ApplyConfiguration(new CatalogBrandConfiguration());
        modelBuilder.ApplyConfiguration(new CatalogTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CatalogItemsStockConfiguration());
        modelBuilder.ApplyConfiguration(new DiscountItemConfiguration());

        // EM-71: HiLo ID generation for CatalogItem (SQL Server only; SQLite lacks sequence support)
        if (Database.IsSqlServer())
        {
            modelBuilder.Entity<CatalogItem>()
                .Property(ci => ci.Id)
                .UseHiLo("catalog_hilo");
        }

        SeedData(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatalogBrand>().HasData(
            new CatalogBrand { Id = 1, Brand = "Azure" },
            new CatalogBrand { Id = 2, Brand = ".NET" },
            new CatalogBrand { Id = 3, Brand = "Visual Studio" },
            new CatalogBrand { Id = 4, Brand = "SQL Server" },
            new CatalogBrand { Id = 5, Brand = "Other" }
        );

        modelBuilder.Entity<CatalogType>().HasData(
            new CatalogType { Id = 1, Type = "Mug" },
            new CatalogType { Id = 2, Type = "T-Shirt" },
            new CatalogType { Id = 3, Type = "Sheet" },
            new CatalogType { Id = 4, Type = "USB Memory Stick" }
        );

        modelBuilder.Entity<CatalogItem>().HasData(
            new CatalogItem { Id = 1, CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Bot Black Hoodie", Name = ".NET Bot Black Hoodie", Price = 19.5m, PictureFileName = "1.png" },
            new CatalogItem { Id = 2, CatalogTypeId = 1, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Black & White Mug", Name = ".NET Black & White Mug", Price = 8.50m, PictureFileName = "2.png" },
            new CatalogItem { Id = 3, CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12m, PictureFileName = "3.png" },
            new CatalogItem { Id = 4, CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Foundation T-shirt", Name = ".NET Foundation T-shirt", Price = 12m, PictureFileName = "4.png" },
            new CatalogItem { Id = 5, CatalogTypeId = 3, CatalogBrandId = 5, AvailableStock = 100, Description = "Roslyn Red Sheet", Name = "Roslyn Red Sheet", Price = 8.5m, PictureFileName = "5.png" },
            new CatalogItem { Id = 6, CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Blue Hoodie", Name = ".NET Blue Hoodie", Price = 12m, PictureFileName = "6.png" },
            new CatalogItem { Id = 7, CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12m, PictureFileName = "7.png" },
            new CatalogItem { Id = 8, CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Kudu Purple Hoodie", Name = "Kudu Purple Hoodie", Price = 8.5m, PictureFileName = "8.png" },
            new CatalogItem { Id = 9, CatalogTypeId = 1, CatalogBrandId = 5, AvailableStock = 100, Description = "Cup<T> White Mug", Name = "Cup<T> White Mug", Price = 12m, PictureFileName = "9.png" },
            new CatalogItem { Id = 10, CatalogTypeId = 3, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Foundation Sheet", Name = ".NET Foundation Sheet", Price = 12m, PictureFileName = "10.png" },
            new CatalogItem { Id = 11, CatalogTypeId = 3, CatalogBrandId = 2, AvailableStock = 100, Description = "Cup<T> Sheet", Name = "Cup<T> Sheet", Price = 8.5m, PictureFileName = "11.png" },
            new CatalogItem { Id = 12, CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Prism White TShirt", Name = "Prism White TShirt", Price = 12m, PictureFileName = "12.png" }
        );

        modelBuilder.Entity<DiscountItem>().HasData(
            new DiscountItem { Id = 1, Start = new DateTime(2017, 9, 18), End = new DateTime(2017, 9, 21), Size = 0.3 },
            new DiscountItem { Id = 2, Start = new DateTime(2017, 9, 22), End = new DateTime(2017, 9, 26), Size = 0.25 },
            new DiscountItem { Id = 3, Start = new DateTime(2017, 9, 27), End = new DateTime(2017, 9, 30), Size = 0.1 },
            new DiscountItem { Id = 4, Start = new DateTime(2017, 10, 5), End = new DateTime(2017, 10, 20), Size = 0.5 },
            new DiscountItem { Id = 5, Start = new DateTime(2017, 11, 13), End = new DateTime(2017, 11, 25), Size = 0.3 },
            new DiscountItem { Id = 6, Start = new DateTime(2017, 12, 20), End = new DateTime(2017, 12, 25), Size = 0.25 }
        );

        modelBuilder.Entity<CatalogItemsStock>().HasData(
            new CatalogItemsStock { StockId = 1, CatalogItemId = 1, Date = new DateTime(2017, 9, 20), AvailableStock = 100 },
            new CatalogItemsStock { StockId = 2, CatalogItemId = 1, Date = new DateTime(2017, 9, 21), AvailableStock = 120 },
            new CatalogItemsStock { StockId = 3, CatalogItemId = 1, Date = new DateTime(2017, 9, 22), AvailableStock = 80 }
        );
    }
}
