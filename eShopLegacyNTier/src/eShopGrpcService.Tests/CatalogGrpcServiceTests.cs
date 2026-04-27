using eShopGrpcService.Data;
using eShopGrpcService.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace eShopGrpcService.Tests;

public class CatalogGrpcServiceTests : IDisposable
{
    private readonly CatalogDbContext _db;
    private readonly CatalogGrpcService _service;
    private readonly ServerCallContext _callContext;

    public CatalogGrpcServiceTests()
    {
        _db = TestDbContextFactory.CreateInMemoryContext();
        _service = new CatalogGrpcService(_db);
        _callContext = TestServerCallContext.Create(
            method: nameof(CatalogGrpcService),
            host: "localhost",
            deadline: DateTime.UtcNow.AddMinutes(1),
            requestHeaders: new Metadata(),
            cancellationToken: CancellationToken.None,
            peer: "127.0.0.1",
            authContext: null,
            contextPropagationToken: null,
            writeHeadersFunc: (metadata) => Task.CompletedTask,
            writeOptionsGetter: () => new WriteOptions(),
            writeOptionsSetter: (writeOptions) => { });
    }

    public void Dispose()
    {
        _db.Dispose();
    }

    [Fact]
    public async Task FindCatalogItem_ExistingId_ReturnsItemWithFoundTrue()
    {
        var reply = await _service.FindCatalogItem(
            new FindCatalogItemRequest { Id = 1 }, _callContext);

        Assert.True(reply.Found);
        Assert.Equal(1, reply.Item.Id);
        Assert.Equal(".NET Bot Black Hoodie", reply.Item.Name);
    }

    [Fact]
    public async Task FindCatalogItem_MissingId_ReturnsFoundFalse()
    {
        var reply = await _service.FindCatalogItem(
            new FindCatalogItemRequest { Id = 999 }, _callContext);

        Assert.False(reply.Found);
    }

    [Fact]
    public async Task GetCatalogBrands_Returns5Brands()
    {
        var reply = await _service.GetCatalogBrands(new Empty(), _callContext);

        Assert.Equal(5, reply.Brands.Count);
    }

    [Fact]
    public async Task GetCatalogItems_NoFilter_Returns12Items()
    {
        var reply = await _service.GetCatalogItems(
            new GetCatalogItemsRequest { BrandIdFilter = 0, TypeIdFilter = 0 }, _callContext);

        Assert.Equal(12, reply.Items.Count);
    }

    [Fact]
    public async Task GetCatalogItems_BrandFilter_ReturnsFilteredItems()
    {
        var reply = await _service.GetCatalogItems(
            new GetCatalogItemsRequest { BrandIdFilter = 2, TypeIdFilter = 0 }, _callContext);

        Assert.All(reply.Items, item => Assert.Equal(2, item.CatalogBrandId));
        Assert.True(reply.Items.Count > 0);
    }

    [Fact]
    public async Task GetCatalogItems_TypeFilter_ReturnsFilteredItems()
    {
        var reply = await _service.GetCatalogItems(
            new GetCatalogItemsRequest { BrandIdFilter = 0, TypeIdFilter = 1 }, _callContext);

        Assert.All(reply.Items, item => Assert.Equal(1, item.CatalogTypeId));
        Assert.True(reply.Items.Count > 0);
    }

    [Fact]
    public async Task GetCatalogItems_CombinedFilter_ReturnsFilteredItems()
    {
        var reply = await _service.GetCatalogItems(
            new GetCatalogItemsRequest { BrandIdFilter = 2, TypeIdFilter = 2 }, _callContext);

        Assert.All(reply.Items, item =>
        {
            Assert.Equal(2, item.CatalogBrandId);
            Assert.Equal(2, item.CatalogTypeId);
        });
    }

    [Fact]
    public async Task GetCatalogTypes_Returns4Types()
    {
        var reply = await _service.GetCatalogTypes(new Empty(), _callContext);

        Assert.Equal(4, reply.CatalogTypes.Count);
    }

    [Fact]
    public async Task GetAvailableStock_KnownDateAndItem_ReturnsCorrectStock()
    {
        var date = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2017, 9, 20), DateTimeKind.Utc));

        var reply = await _service.GetAvailableStock(
            new GetAvailableStockRequest { Date = date, CatalogItemId = 1 }, _callContext);

        Assert.Equal(100, reply.AvailableStock);
    }

    [Fact]
    public async Task GetAvailableStock_UnknownDateAndItem_ReturnsZero()
    {
        var date = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2020, 1, 1), DateTimeKind.Utc));

        var reply = await _service.GetAvailableStock(
            new GetAvailableStockRequest { Date = date, CatalogItemId = 999 }, _callContext);

        Assert.Equal(0, reply.AvailableStock);
    }

    [Fact]
    public async Task CreateAvailableStock_NewEntry_CreatesSuccessfully()
    {
        var date = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2024, 1, 1), DateTimeKind.Utc));

        await _service.CreateAvailableStock(
            new CatalogItemsStockMessage
            {
                Date = date,
                CatalogItemId = 3,
                AvailableStock = 50,
            }, _callContext);

        var reply = await _service.GetAvailableStock(
            new GetAvailableStockRequest { Date = date, CatalogItemId = 3 }, _callContext);

        Assert.Equal(50, reply.AvailableStock);
    }

    [Fact]
    public async Task CreateAvailableStock_ExistingEntry_UpsertsSuccessfully()
    {
        var date = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2017, 9, 20), DateTimeKind.Utc));

        await _service.CreateAvailableStock(
            new CatalogItemsStockMessage
            {
                Date = date,
                CatalogItemId = 1,
                AvailableStock = 200,
            }, _callContext);

        var reply = await _service.GetAvailableStock(
            new GetAvailableStockRequest { Date = date, CatalogItemId = 1 }, _callContext);

        Assert.Equal(200, reply.AvailableStock);
    }

    [Fact]
    public async Task CreateCatalogItem_CreatesAndVerifiable()
    {
        await _service.CreateCatalogItem(
            new CatalogItemMessage
            {
                Description = "Test Item",
                Name = "Test Item",
                Price = "9.99",
                Picturefilename = "test.png",
                CatalogBrandId = 1,
                CatalogTypeId = 1,
            }, _callContext);

        var reply = await _service.FindCatalogItem(
            new FindCatalogItemRequest { Id = 13 }, _callContext);

        Assert.True(reply.Found);
        Assert.Equal("Test Item", reply.Item.Name);
    }

    [Fact]
    public async Task UpdateCatalogItem_UpdatesPriceSuccessfully()
    {
        await _service.UpdateCatalogItem(
            new CatalogItemMessage
            {
                Id = 1,
                Description = ".NET Bot Black Hoodie",
                Name = ".NET Bot Black Hoodie",
                Price = "25.00",
                Picturefilename = "2.png",
                CatalogBrandId = 2,
                CatalogTypeId = 2,
            }, _callContext);

        var reply = await _service.FindCatalogItem(
            new FindCatalogItemRequest { Id = 1 }, _callContext);

        Assert.True(reply.Found);
        Assert.Equal("25.00", reply.Item.Price);
    }

    [Fact]
    public async Task RemoveCatalogItem_RemovesAndVerifiable()
    {
        await _service.RemoveCatalogItem(
            new CatalogItemMessage
            {
                Id = 12,
                Description = "Cup<T> TShirt",
                Name = "Cup<T> TShirt",
                Price = "12",
                Picturefilename = "4.png",
                CatalogBrandId = 5,
                CatalogTypeId = 2,
            }, _callContext);

        var reply = await _service.FindCatalogItem(
            new FindCatalogItemRequest { Id = 12 }, _callContext);

        Assert.False(reply.Found);
    }

    [Fact]
    public async Task GetDiscount_DateInRange_ReturnsDiscount()
    {
        var day = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2017, 9, 19), DateTimeKind.Utc));

        var reply = await _service.GetDiscount(
            new GetDiscountRequest { Day = day }, _callContext);

        Assert.True(reply.Found);
        Assert.Equal(0.3, reply.Size, 2);
    }

    [Fact]
    public async Task GetDiscount_DateOutsideRange_ReturnsFoundFalse()
    {
        var day = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2020, 1, 1), DateTimeKind.Utc));

        var reply = await _service.GetDiscount(
            new GetDiscountRequest { Day = day }, _callContext);

        Assert.False(reply.Found);
    }
}
