using eShopGrpcService.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;

namespace eShopGrpcService.Tests;

public class CatalogGrpcServiceTests : IClassFixture<GrpcTestFixture>, IDisposable
{
    private readonly GrpcTestFixture _factory;
    private readonly HttpClient _httpClient;
    private readonly GrpcChannel _channel;
    private readonly CatalogService.CatalogServiceClient _client;

    public CatalogGrpcServiceTests(GrpcTestFixture factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateDefaultClient();
        _channel = GrpcChannel.ForAddress(_httpClient.BaseAddress!, new GrpcChannelOptions
        {
            HttpClient = _httpClient
        });
        _client = new CatalogService.CatalogServiceClient(_channel);
    }

    [Fact]
    public async Task FindCatalogItem_ExistingId_ReturnsItem()
    {
        var response = await _client.FindCatalogItemAsync(
            new FindCatalogItemRequest { Id = 1 });

        Assert.NotNull(response.Item);
        Assert.Equal(1, response.Item.Id);
        Assert.Equal(".NET Bot Black Hoodie", response.Item.Name);
        Assert.NotNull(response.Item.CatalogBrand);
        Assert.NotNull(response.Item.CatalogType);
    }

    [Fact]
    public async Task FindCatalogItem_NonExistingId_ThrowsNotFound()
    {
        var ex = await Assert.ThrowsAsync<RpcException>(async () =>
            await _client.FindCatalogItemAsync(
                new FindCatalogItemRequest { Id = 999 }));

        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task GetCatalogBrands_ReturnsAllBrands()
    {
        var response = await _client.GetCatalogBrandsAsync(new Empty());

        Assert.Equal(5, response.Brands.Count);
        Assert.Contains(response.Brands, b => b.Brand == ".NET");
        Assert.Contains(response.Brands, b => b.Brand == "Azure");
    }

    [Fact]
    public async Task GetCatalogTypes_ReturnsAllTypes()
    {
        var response = await _client.GetCatalogTypesAsync(new Empty());

        Assert.Equal(4, response.Types_.Count);
        Assert.Contains(response.Types_, t => t.Type == "Mug");
        Assert.Contains(response.Types_, t => t.Type == "T-Shirt");
    }

    [Fact]
    public async Task GetCatalogItems_NoFilter_ReturnsAll()
    {
        var response = await _client.GetCatalogItemsAsync(
            new GetCatalogItemsRequest());

        Assert.Equal(12, response.Items.Count);
    }

    [Fact]
    public async Task GetCatalogItems_BrandFilter_ReturnsFiltered()
    {
        var response = await _client.GetCatalogItemsAsync(
            new GetCatalogItemsRequest { BrandIdFilter = 2 });

        Assert.True(response.Items.Count > 0);
        Assert.All(response.Items, item => Assert.Equal(2, item.CatalogBrandId));
    }

    [Fact]
    public async Task GetCatalogItems_TypeFilter_ReturnsFiltered()
    {
        var response = await _client.GetCatalogItemsAsync(
            new GetCatalogItemsRequest { TypeIdFilter = 1 });

        Assert.True(response.Items.Count > 0);
        Assert.All(response.Items, item => Assert.Equal(1, item.CatalogTypeId));
    }

    [Fact]
    public async Task GetAvailableStock_ExistingEntry_ReturnsStock()
    {
        var date = new DateTime(2017, 9, 20, 0, 0, 0, DateTimeKind.Utc);
        var response = await _client.GetAvailableStockAsync(
            new GetAvailableStockRequest
            {
                Date = date.ToTimestamp(),
                CatalogItemId = 1
            });

        Assert.Equal(100, response.AvailableStock);
    }

    [Fact]
    public async Task GetAvailableStock_NoEntry_ReturnsZero()
    {
        var date = new DateTime(2099, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var response = await _client.GetAvailableStockAsync(
            new GetAvailableStockRequest
            {
                Date = date.ToTimestamp(),
                CatalogItemId = 1
            });

        Assert.Equal(0, response.AvailableStock);
    }

    [Fact]
    public async Task CreateAndGetAvailableStock_RoundTrip()
    {
        var date = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        await _client.CreateAvailableStockAsync(
            new CatalogItemsStockMessage
            {
                CatalogItemId = 3,
                AvailableStock = 50,
                Date = date.ToTimestamp()
            });

        var response = await _client.GetAvailableStockAsync(
            new GetAvailableStockRequest
            {
                Date = date.ToTimestamp(),
                CatalogItemId = 3
            });

        Assert.Equal(50, response.AvailableStock);
    }

    [Fact]
    public async Task CreateCatalogItem_AddsNewItem()
    {
        var newItem = new CatalogItemMessage
        {
            Description = "Test Item",
            Name = "Test Item",
            Price = "25.99",
            Picturefilename = "test.png",
            CatalogBrandId = 1,
            CatalogTypeId = 1
        };

        await _client.CreateCatalogItemAsync(newItem);

        var allItems = await _client.GetCatalogItemsAsync(new GetCatalogItemsRequest());
        Assert.Contains(allItems.Items, i => i.Name == "Test Item");
    }

    [Fact]
    public async Task UpdateCatalogItem_ModifiesExistingItem()
    {
        var original = await _client.FindCatalogItemAsync(
            new FindCatalogItemRequest { Id = 2 });

        var updated = new CatalogItemMessage
        {
            Id = 2,
            Description = "Updated Description",
            Name = original.Item.Name,
            Price = "99.99",
            Picturefilename = original.Item.Picturefilename,
            CatalogBrandId = original.Item.CatalogBrandId,
            CatalogTypeId = original.Item.CatalogTypeId
        };

        await _client.UpdateCatalogItemAsync(updated);

        var result = await _client.FindCatalogItemAsync(
            new FindCatalogItemRequest { Id = 2 });
        Assert.Equal("Updated Description", result.Item.Description);
    }

    [Fact]
    public async Task RemoveCatalogItem_DeletesItem()
    {
        // First create an item to remove
        var newItem = new CatalogItemMessage
        {
            Description = "To Be Removed",
            Name = "To Be Removed",
            Price = "5.00",
            Picturefilename = "remove.png",
            CatalogBrandId = 1,
            CatalogTypeId = 1
        };
        await _client.CreateCatalogItemAsync(newItem);

        var allItems = await _client.GetCatalogItemsAsync(new GetCatalogItemsRequest());
        var itemToRemove = allItems.Items.First(i => i.Name == "To Be Removed");

        await _client.RemoveCatalogItemAsync(new CatalogItemMessage { Id = itemToRemove.Id });

        var ex = await Assert.ThrowsAsync<RpcException>(async () =>
            await _client.FindCatalogItemAsync(
                new FindCatalogItemRequest { Id = itemToRemove.Id }));
        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task GetDiscount_WithActiveDiscount_ReturnsDiscount()
    {
        // Use a date within the preconfigured discount range
        var date = new DateTime(2017, 9, 19, 0, 0, 0, DateTimeKind.Utc);
        var response = await _client.GetDiscountAsync(
            new GetDiscountRequest { Day = date.ToTimestamp() });

        Assert.NotNull(response.Discount);
        Assert.True(response.Discount.Size > 0);
    }

    [Fact]
    public async Task GetDiscount_NoActiveDiscount_ReturnsNull()
    {
        var date = new DateTime(2099, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var response = await _client.GetDiscountAsync(
            new GetDiscountRequest { Day = date.ToTimestamp() });

        Assert.Null(response.Discount);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _httpClient?.Dispose();
    }
}
