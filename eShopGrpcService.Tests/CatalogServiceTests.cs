using Grpc.Net.Client;
using Google.Protobuf.WellKnownTypes;
using eShopGrpcService;

namespace eShopGrpcService.Tests;

public class CatalogServiceTests : IClassFixture<GrpcTestFixture>, IDisposable
{
    private readonly GrpcTestFixture _factory;
    private readonly HttpClient _httpClient;
    private readonly GrpcChannel _channel;
    private readonly CatalogService.CatalogServiceClient _client;

    public CatalogServiceTests(GrpcTestFixture factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateDefaultClient(new ResponseVersionHandler());
        _channel = GrpcChannel.ForAddress(_httpClient.BaseAddress!, new GrpcChannelOptions
        {
            HttpClient = _httpClient
        });
        _client = new CatalogService.CatalogServiceClient(_channel);
    }

    public void Dispose()
    {
        _channel.Dispose();
        _httpClient.Dispose();
    }

    [Fact]
    public async Task FindCatalogItem_ReturnsItem_WithBrandAndType()
    {
        var response = await _client.FindCatalogItemAsync(
            new FindCatalogItemRequest { Id = 1 });

        Assert.Equal(1, response.Id);
        Assert.Equal(".NET Bot Black Hoodie", response.Name);
        Assert.NotNull(response.CatalogBrand);
        Assert.NotNull(response.CatalogType);
        Assert.Equal(".NET", response.CatalogBrand.Brand);
        Assert.Equal("T-Shirt", response.CatalogType.Type);
    }

    [Fact]
    public async Task FindCatalogItem_NotFound_ThrowsRpcException()
    {
        var ex = await Assert.ThrowsAsync<Grpc.Core.RpcException>(
            () => _client.FindCatalogItemAsync(
                new FindCatalogItemRequest { Id = 999 }).ResponseAsync);

        Assert.Equal(Grpc.Core.StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task GetCatalogBrands_ReturnsAllBrands()
    {
        var response = await _client.GetCatalogBrandsAsync(new Empty());

        Assert.Equal(5, response.Brands.Count);
        Assert.Contains(response.Brands, b => b.Brand == "Azure");
        Assert.Contains(response.Brands, b => b.Brand == ".NET");
    }

    [Fact]
    public async Task GetCatalogItems_NoFilter_ReturnsAll()
    {
        var response = await _client.GetCatalogItemsAsync(
            new GetCatalogItemsRequest { BrandIdFilter = 0, TypeIdFilter = 0 });

        Assert.Equal(12, response.Items.Count);
    }

    [Fact]
    public async Task GetCatalogItems_FilterByBrand_ReturnsFiltered()
    {
        var response = await _client.GetCatalogItemsAsync(
            new GetCatalogItemsRequest { BrandIdFilter = 2, TypeIdFilter = 0 });

        Assert.All(response.Items, item => Assert.Equal(2, item.CatalogBrandId));
        Assert.True(response.Items.Count > 0);
    }

    [Fact]
    public async Task GetCatalogItems_FilterByType_ReturnsFiltered()
    {
        var response = await _client.GetCatalogItemsAsync(
            new GetCatalogItemsRequest { BrandIdFilter = 0, TypeIdFilter = 1 });

        Assert.All(response.Items, item => Assert.Equal(1, item.CatalogTypeId));
        Assert.True(response.Items.Count > 0);
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
    public async Task GetAvailableStock_ReturnsStock()
    {
        var date = Timestamp.FromDateTime(
            DateTime.SpecifyKind(new DateTime(2017, 9, 20), DateTimeKind.Utc));

        var response = await _client.GetAvailableStockAsync(
            new GetAvailableStockRequest { Date = date, CatalogItemId = 1 });

        Assert.Equal(100, response.AvailableStock);
    }

    [Fact]
    public async Task GetAvailableStock_NoStock_ReturnsZero()
    {
        var date = Timestamp.FromDateTime(
            DateTime.SpecifyKind(new DateTime(2020, 1, 1), DateTimeKind.Utc));

        var response = await _client.GetAvailableStockAsync(
            new GetAvailableStockRequest { Date = date, CatalogItemId = 1 });

        Assert.Equal(0, response.AvailableStock);
    }

    [Fact]
    public async Task GetDiscount_ReturnsDiscount()
    {
        var date = Timestamp.FromDateTime(
            DateTime.SpecifyKind(new DateTime(2017, 9, 19), DateTimeKind.Utc));

        var response = await _client.GetDiscountAsync(
            new GetDiscountRequest { Day = date });

        Assert.Equal(0.3, response.Size, 2);
    }

    // HTTP/2 version handler for test client
    private class ResponseVersionHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Version = new Version(2, 0);
            var response = await base.SendAsync(request, cancellationToken);
            response.Version = request.Version;
            return response;
        }
    }
}
