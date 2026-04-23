using Microsoft.EntityFrameworkCore;
using Grpc.Core;
using Grpc.Core.Testing;
using Google.Protobuf.WellKnownTypes;
using eShopGrpcService.Models;
using eShopGrpcService.Models.Infrastructure;
using eShopGrpcService.Services;

namespace eShopGrpcService.Tests;

public class CatalogServiceTests
{
    private CatalogDbContext CreateSeededContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var context = new CatalogDbContext(options);
        CatalogDbInitializer.Seed(context);
        return context;
    }

    private ServerCallContext CreateTestCallContext()
    {
        return TestServerCallContext.Create(
            method: "TestMethod",
            host: "localhost",
            deadline: DateTime.UtcNow.AddMinutes(1),
            requestHeaders: new Metadata(),
            cancellationToken: CancellationToken.None,
            peer: "127.0.0.1",
            authContext: null,
            contextPropagationToken: null,
            writeHeadersFunc: (metadata) => Task.CompletedTask,
            writeOptionsGetter: () => new WriteOptions(),
            writeOptionsSetter: (writeOptions) => { }
        );
    }

    [Fact]
    public async Task FindCatalogItem_ReturnsItem_WhenExists()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateSeededContext(dbName);
        var service = new CatalogServiceImpl(context);
        var callContext = CreateTestCallContext();

        var request = new FindCatalogItemRequest { Id = 1 };
        var response = await service.FindCatalogItem(request, callContext);

        Assert.True(response.Found);
        Assert.Equal(".NET Bot Black Hoodie", response.CatalogItem.Name);
    }

    [Fact]
    public async Task FindCatalogItem_ReturnsEmpty_WhenNotFound()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateSeededContext(dbName);
        var service = new CatalogServiceImpl(context);
        var callContext = CreateTestCallContext();

        var request = new FindCatalogItemRequest { Id = 999 };
        var response = await service.FindCatalogItem(request, callContext);

        Assert.False(response.Found);
    }

    [Fact]
    public async Task GetCatalogBrands_ReturnsAllBrands()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateSeededContext(dbName);
        var service = new CatalogServiceImpl(context);
        var callContext = CreateTestCallContext();

        var request = new Empty();
        var response = await service.GetCatalogBrands(request, callContext);

        Assert.Equal(5, response.CatalogBrands.Count);
    }

    [Fact]
    public async Task GetCatalogItems_ReturnsFiltered_ByBrand()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateSeededContext(dbName);
        var service = new CatalogServiceImpl(context);
        var callContext = CreateTestCallContext();

        var request = new GetCatalogItemsRequest { BrandIdFilter = 2, TypeIdFilter = 0 };
        var response = await service.GetCatalogItems(request, callContext);

        Assert.All(response.CatalogItems, item => Assert.Equal(2, item.CatalogBrandId));
        Assert.True(response.CatalogItems.Count > 0);
    }

    [Fact]
    public async Task GetCatalogItems_ReturnsAll_WhenNoFilter()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateSeededContext(dbName);
        var service = new CatalogServiceImpl(context);
        var callContext = CreateTestCallContext();

        var request = new GetCatalogItemsRequest { BrandIdFilter = 0, TypeIdFilter = 0 };
        var response = await service.GetCatalogItems(request, callContext);

        Assert.Equal(12, response.CatalogItems.Count);
    }

    [Fact]
    public async Task GetCatalogTypes_ReturnsAllTypes()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateSeededContext(dbName);
        var service = new CatalogServiceImpl(context);
        var callContext = CreateTestCallContext();

        var request = new Empty();
        var response = await service.GetCatalogTypes(request, callContext);

        Assert.Equal(4, response.CatalogTypes.Count);
    }

    [Fact]
    public async Task GetAvailableStock_ReturnsStock_WhenExists()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateSeededContext(dbName);
        var service = new CatalogServiceImpl(context);
        var callContext = CreateTestCallContext();

        var request = new GetAvailableStockRequest
        {
            Date = Timestamp.FromDateTime(new DateTime(2017, 9, 20, 0, 0, 0, DateTimeKind.Utc)),
            CatalogItemId = 1
        };
        var response = await service.GetAvailableStock(request, callContext);

        Assert.Equal(100, response.AvailableStock);
    }

    [Fact]
    public async Task CreateAvailableStock_AddsNewStock()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateSeededContext(dbName);
        var service = new CatalogServiceImpl(context);
        var callContext = CreateTestCallContext();

        var initialCount = context.CatalogItemsStocks.Count();

        var request = new CreateAvailableStockRequest
        {
            CatalogItemId = 3,
            Date = Timestamp.FromDateTime(new DateTime(2017, 10, 1, 0, 0, 0, DateTimeKind.Utc)),
            AvailableStock = 50,
            StockId = 100
        };
        await service.CreateAvailableStock(request, callContext);

        var newCount = context.CatalogItemsStocks.Count();
        Assert.Equal(initialCount + 1, newCount);
    }

    [Fact]
    public async Task CreateCatalogItem_AddsItem()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateSeededContext(dbName);
        var service = new CatalogServiceImpl(context);
        var callContext = CreateTestCallContext();

        var initialCount = context.CatalogItems.Count();

        var request = new CatalogItemMessage
        {
            Id = 100,
            CatalogTypeId = 1,
            CatalogBrandId = 1,
            Description = "Test Item",
            Name = "Test Item",
            Price = 9.99,
            PictureFilename = "test.png"
        };
        await service.CreateCatalogItem(request, callContext);

        var newCount = context.CatalogItems.Count();
        Assert.Equal(initialCount + 1, newCount);
    }

    [Fact]
    public async Task RemoveCatalogItem_DeletesItem()
    {
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateSeededContext(dbName);
        var service = new CatalogServiceImpl(context);
        var callContext = CreateTestCallContext();

        var initialCount = context.CatalogItems.Count();

        var request = new CatalogItemMessage { Id = 1 };
        await service.RemoveCatalogItem(request, callContext);

        var newCount = context.CatalogItems.Count();
        Assert.Equal(initialCount - 1, newCount);
    }
}
