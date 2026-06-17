using eShop.Catalog.Core.Data;
using eShop.Catalog.Core.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace eShop.Catalog.Tests;

public class CatalogDbContextTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<CatalogDbContext> _options;

    public CatalogDbContextTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new CatalogDbContext(_options);
        context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    private CatalogDbContext CreateContext() => new(_options);

    // --- Seed Data Tests ---

    [Fact]
    public void SeedData_CatalogBrands_HasFiveEntries()
    {
        using var context = CreateContext();
        var brands = context.CatalogBrands.ToList();
        Assert.Equal(5, brands.Count);
        Assert.Contains(brands, b => b.Brand == "Azure");
        Assert.Contains(brands, b => b.Brand == ".NET");
        Assert.Contains(brands, b => b.Brand == "Visual Studio");
        Assert.Contains(brands, b => b.Brand == "SQL Server");
        Assert.Contains(brands, b => b.Brand == "Other");
    }

    [Fact]
    public void SeedData_CatalogTypes_HasFourEntries()
    {
        using var context = CreateContext();
        var types = context.CatalogTypes.ToList();
        Assert.Equal(4, types.Count);
        Assert.Contains(types, t => t.Type == "Mug");
        Assert.Contains(types, t => t.Type == "T-Shirt");
        Assert.Contains(types, t => t.Type == "Sheet");
        Assert.Contains(types, t => t.Type == "USB Memory Stick");
    }

    [Fact]
    public void SeedData_CatalogItems_HasTwelveEntries()
    {
        using var context = CreateContext();
        var items = context.CatalogItems.ToList();
        Assert.Equal(12, items.Count);
    }

    [Fact]
    public void SeedData_DiscountItems_HasSixEntries()
    {
        using var context = CreateContext();
        var discounts = context.DiscountItems.ToList();
        Assert.Equal(6, discounts.Count);
    }

    [Fact]
    public void SeedData_CatalogItemsStock_HasThreeEntries()
    {
        using var context = CreateContext();
        var stocks = context.CatalogItemsStocks.ToList();
        Assert.Equal(3, stocks.Count);
    }

    // --- CRUD Tests: CatalogItem ---

    [Fact]
    public void CatalogItem_Create_And_Read()
    {
        using (var context = CreateContext())
        {
            var item = new CatalogItem
            {
                Id = 100,
                Name = "Test Item",
                Description = "Test Description",
                Price = 9.99m,
                PictureFileName = "test.png",
                CatalogTypeId = 1,
                CatalogBrandId = 1,
                AvailableStock = 50
            };
            context.CatalogItems.Add(item);
            context.SaveChanges();
        }

        using (var context = CreateContext())
        {
            var item = context.CatalogItems.Find(100);
            Assert.NotNull(item);
            Assert.Equal("Test Item", item!.Name);
            Assert.Equal(9.99m, item.Price);
        }
    }

    [Fact]
    public void CatalogItem_Update()
    {
        using (var context = CreateContext())
        {
            var item = context.CatalogItems.First();
            item.Name = "Updated Name";
            item.Price = 99.99m;
            context.SaveChanges();
        }

        using (var context = CreateContext())
        {
            var item = context.CatalogItems.First();
            Assert.Equal("Updated Name", item.Name);
            Assert.Equal(99.99m, item.Price);
        }
    }

    [Fact]
    public void CatalogItem_Delete()
    {
        int initialCount;
        using (var context = CreateContext())
        {
            initialCount = context.CatalogItems.Count();
            var item = context.CatalogItems.First();
            context.CatalogItems.Remove(item);
            context.SaveChanges();
        }

        using (var context = CreateContext())
        {
            Assert.Equal(initialCount - 1, context.CatalogItems.Count());
        }
    }

    // --- CRUD Tests: CatalogBrand ---

    [Fact]
    public void CatalogBrand_Create_And_Read()
    {
        using (var context = CreateContext())
        {
            context.CatalogBrands.Add(new CatalogBrand { Id = 100, Brand = "Test Brand" });
            context.SaveChanges();
        }

        using (var context = CreateContext())
        {
            var brand = context.CatalogBrands.Find(100);
            Assert.NotNull(brand);
            Assert.Equal("Test Brand", brand!.Brand);
        }
    }

    [Fact]
    public void CatalogBrand_Update()
    {
        using (var context = CreateContext())
        {
            var brand = context.CatalogBrands.First();
            brand.Brand = "Updated Brand";
            context.SaveChanges();
        }

        using (var context = CreateContext())
        {
            var brand = context.CatalogBrands.First();
            Assert.Equal("Updated Brand", brand.Brand);
        }
    }

    [Fact]
    public void CatalogBrand_Delete()
    {
        using (var context = CreateContext())
        {
            // Add a standalone brand (not referenced by items) to delete
            context.CatalogBrands.Add(new CatalogBrand { Id = 200, Brand = "ToDelete" });
            context.SaveChanges();
        }

        using (var context = CreateContext())
        {
            var brand = context.CatalogBrands.Find(200);
            Assert.NotNull(brand);
            context.CatalogBrands.Remove(brand!);
            context.SaveChanges();
        }

        using (var context = CreateContext())
        {
            Assert.Null(context.CatalogBrands.Find(200));
        }
    }

    // --- CRUD Tests: CatalogType ---

    [Fact]
    public void CatalogType_Create_And_Read()
    {
        using (var context = CreateContext())
        {
            context.CatalogTypes.Add(new CatalogType { Id = 100, Type = "Test Type" });
            context.SaveChanges();
        }

        using (var context = CreateContext())
        {
            var type = context.CatalogTypes.Find(100);
            Assert.NotNull(type);
            Assert.Equal("Test Type", type!.Type);
        }
    }

    // --- CRUD Tests: CatalogItemsStock ---

    [Fact]
    public void CatalogItemsStock_Create_And_Read()
    {
        using (var context = CreateContext())
        {
            context.CatalogItemsStocks.Add(new CatalogItemsStock
            {
                StockId = 100,
                CatalogItemId = 1,
                Date = new DateTime(2024, 1, 1),
                AvailableStock = 200
            });
            context.SaveChanges();
        }

        using (var context = CreateContext())
        {
            var stock = context.CatalogItemsStocks.Find(100);
            Assert.NotNull(stock);
            Assert.Equal(200, stock!.AvailableStock);
            Assert.Equal(new DateTime(2024, 1, 1), stock.Date);
        }
    }

    // --- CRUD Tests: DiscountItem ---

    [Fact]
    public void DiscountItem_Create_And_Read()
    {
        using (var context = CreateContext())
        {
            context.DiscountItems.Add(new DiscountItem
            {
                Id = 100,
                Size = 0.15,
                Start = new DateTime(2024, 6, 1),
                End = new DateTime(2024, 6, 30)
            });
            context.SaveChanges();
        }

        using (var context = CreateContext())
        {
            var discount = context.DiscountItems.Find(100);
            Assert.NotNull(discount);
            Assert.Equal(0.15, discount!.Size);
        }
    }

    // --- Navigation Properties & FK Tests ---

    [Fact]
    public void CatalogItem_NavigationProperties_LoadCorrectly()
    {
        using var context = CreateContext();
        var item = context.CatalogItems
            .Include(ci => ci.CatalogBrand)
            .Include(ci => ci.CatalogType)
            .First(ci => ci.Id == 1);

        Assert.NotNull(item.CatalogBrand);
        Assert.NotNull(item.CatalogType);
        Assert.Equal(".NET", item.CatalogBrand.Brand);
        Assert.Equal("T-Shirt", item.CatalogType.Type);
    }

    [Fact]
    public void CatalogItem_ForeignKey_CatalogBrandId_IsCorrect()
    {
        using var context = CreateContext();
        var item = context.CatalogItems.First(ci => ci.Id == 1);
        Assert.Equal(2, item.CatalogBrandId);
    }

    [Fact]
    public void CatalogItem_ForeignKey_CatalogTypeId_IsCorrect()
    {
        using var context = CreateContext();
        var item = context.CatalogItems.First(ci => ci.Id == 1);
        Assert.Equal(2, item.CatalogTypeId);
    }

    // --- Pagination Tests ---

    [Fact]
    public void CatalogItems_Pagination_Skip_Take()
    {
        using var context = CreateContext();
        var page1 = context.CatalogItems
            .OrderBy(ci => ci.Id)
            .Skip(0)
            .Take(5)
            .ToList();

        var page2 = context.CatalogItems
            .OrderBy(ci => ci.Id)
            .Skip(5)
            .Take(5)
            .ToList();

        var page3 = context.CatalogItems
            .OrderBy(ci => ci.Id)
            .Skip(10)
            .Take(5)
            .ToList();

        Assert.Equal(5, page1.Count);
        Assert.Equal(5, page2.Count);
        Assert.Equal(2, page3.Count);
        Assert.Equal(1, page1.First().Id);
        Assert.Equal(6, page2.First().Id);
        Assert.Equal(11, page3.First().Id);
    }

    [Fact]
    public void CatalogItems_Pagination_EmptyPage()
    {
        using var context = CreateContext();
        var emptyPage = context.CatalogItems
            .OrderBy(ci => ci.Id)
            .Skip(100)
            .Take(10)
            .ToList();

        Assert.Empty(emptyPage);
    }
}
