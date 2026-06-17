using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using eShop.Catalog.Api.DTOs;
using eShop.Catalog.Core.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace eShop.Catalog.Tests;

public class CatalogApiIntegrationTests : IClassFixture<CatalogApiIntegrationTests.CatalogApiFactory>, IDisposable
{
    private readonly HttpClient _client;
    private readonly CatalogApiFactory _factory;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public CatalogApiIntegrationTests(CatalogApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    // --- GET /api/catalog/items (paginated, filtered) ---

    [Fact]
    public async Task GetCatalogItems_ReturnsSeededItems()
    {
        var response = await _client.GetAsync("/api/catalog/items");
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<PaginatedResponse<CatalogItemDto>>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(10, body!.PageSize);
        Assert.Equal(0, body.PageIndex);
        Assert.Equal(12, body.TotalCount);
        Assert.Equal(10, body.Data.Count());
    }

    [Fact]
    public async Task GetCatalogItems_WithPagination_ReturnsCorrectPage()
    {
        var response = await _client.GetAsync("/api/catalog/items?pageSize=5&pageIndex=1");
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<PaginatedResponse<CatalogItemDto>>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(5, body!.PageSize);
        Assert.Equal(1, body.PageIndex);
        Assert.Equal(12, body.TotalCount);
        Assert.Equal(5, body.Data.Count());
    }

    [Fact]
    public async Task GetCatalogItems_FilterByBrand_ReturnsFiltered()
    {
        // Brand 2 = ".NET" — should have multiple items
        var response = await _client.GetAsync("/api/catalog/items?brand=2&pageSize=50");
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<PaginatedResponse<CatalogItemDto>>(JsonOptions);
        Assert.NotNull(body);
        Assert.All(body!.Data, item => Assert.Equal(2, item.CatalogBrandId));
    }

    [Fact]
    public async Task GetCatalogItems_FilterByType_ReturnsFiltered()
    {
        // Type 1 = "Mug"
        var response = await _client.GetAsync("/api/catalog/items?type=1&pageSize=50");
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<PaginatedResponse<CatalogItemDto>>(JsonOptions);
        Assert.NotNull(body);
        Assert.All(body!.Data, item => Assert.Equal(1, item.CatalogTypeId));
    }

    // --- GET /api/catalog/items/{id} ---

    [Fact]
    public async Task FindCatalogItem_ExistingId_ReturnsItem()
    {
        var response = await _client.GetAsync("/api/catalog/items/1");
        response.EnsureSuccessStatusCode();

        var item = await response.Content.ReadFromJsonAsync<CatalogItemDto>(JsonOptions);
        Assert.NotNull(item);
        Assert.Equal(1, item!.Id);
        Assert.Equal(".NET Bot Black Hoodie", item.Name);
        Assert.Equal(".NET", item.CatalogBrandName);
        Assert.Equal("T-Shirt", item.CatalogTypeName);
    }

    [Fact]
    public async Task FindCatalogItem_NonExistingId_Returns404()
    {
        var response = await _client.GetAsync("/api/catalog/items/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // --- POST /api/catalog/items ---

    [Fact]
    public async Task CreateCatalogItem_ValidRequest_Returns201()
    {
        var request = new CreateCatalogItemRequest(
            Name: "Integration Test Item",
            Description: "Created by integration test",
            Price: 25.99m,
            PictureFileName: "test.png",
            CatalogTypeId: 1,
            CatalogBrandId: 1,
            AvailableStock: 50);

        var response = await _client.PostAsJsonAsync("/api/catalog/items", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<CatalogItemDto>(JsonOptions);
        Assert.NotNull(created);
        Assert.Equal("Integration Test Item", created!.Name);
        Assert.True(created.Id > 0);
    }

    [Fact]
    public async Task CreateCatalogItem_EmptyName_Returns400()
    {
        var request = new CreateCatalogItemRequest(
            Name: "",
            Description: "No name",
            Price: 10m,
            PictureFileName: "x.png",
            CatalogTypeId: 1,
            CatalogBrandId: 1,
            AvailableStock: 0);

        var response = await _client.PostAsJsonAsync("/api/catalog/items", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // --- PUT /api/catalog/items/{id} ---

    [Fact]
    public async Task UpdateCatalogItem_ExistingId_ReturnsUpdated()
    {
        var request = new UpdateCatalogItemRequest(
            Name: "Updated Hoodie",
            Description: "Updated description",
            Price: 99.99m,
            PictureFileName: "updated.png",
            CatalogTypeId: 2,
            CatalogBrandId: 2,
            AvailableStock: 200);

        var response = await _client.PutAsJsonAsync("/api/catalog/items/1", request);
        response.EnsureSuccessStatusCode();

        var updated = await response.Content.ReadFromJsonAsync<CatalogItemDto>(JsonOptions);
        Assert.NotNull(updated);
        Assert.Equal("Updated Hoodie", updated!.Name);
        Assert.Equal(99.99m, updated.Price);
    }

    [Fact]
    public async Task UpdateCatalogItem_NonExistingId_Returns404()
    {
        var request = new UpdateCatalogItemRequest(
            Name: "Ghost",
            Description: "Doesn't exist",
            Price: 1m,
            PictureFileName: "ghost.png",
            CatalogTypeId: 1,
            CatalogBrandId: 1,
            AvailableStock: 0);

        var response = await _client.PutAsJsonAsync("/api/catalog/items/9999", request);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // --- DELETE /api/catalog/items/{id} ---

    [Fact]
    public async Task RemoveCatalogItem_ExistingId_Returns204()
    {
        // Create an item first so we don't affect other tests' seed data
        var createReq = new CreateCatalogItemRequest(
            Name: "To Be Deleted",
            Description: "Will be removed",
            Price: 5m,
            PictureFileName: "del.png",
            CatalogTypeId: 1,
            CatalogBrandId: 1,
            AvailableStock: 1);
        var createResp = await _client.PostAsJsonAsync("/api/catalog/items", createReq);
        var created = await createResp.Content.ReadFromJsonAsync<CatalogItemDto>(JsonOptions);

        var response = await _client.DeleteAsync($"/api/catalog/items/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Confirm deletion
        var getResponse = await _client.GetAsync($"/api/catalog/items/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task RemoveCatalogItem_NonExistingId_Returns404()
    {
        var response = await _client.DeleteAsync("/api/catalog/items/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // --- GET /api/catalog/brands ---

    [Fact]
    public async Task GetCatalogBrands_ReturnsAllBrands()
    {
        var response = await _client.GetAsync("/api/catalog/brands");
        response.EnsureSuccessStatusCode();

        var brands = await response.Content.ReadFromJsonAsync<List<CatalogBrandDto>>(JsonOptions);
        Assert.NotNull(brands);
        Assert.Equal(5, brands!.Count);
        Assert.Contains(brands, b => b.Brand == "Azure");
        Assert.Contains(brands, b => b.Brand == ".NET");
    }

    // --- GET /api/catalog/types ---

    [Fact]
    public async Task GetCatalogTypes_ReturnsAllTypes()
    {
        var response = await _client.GetAsync("/api/catalog/types");
        response.EnsureSuccessStatusCode();

        var types = await response.Content.ReadFromJsonAsync<List<CatalogTypeDto>>(JsonOptions);
        Assert.NotNull(types);
        Assert.Equal(4, types!.Count);
        Assert.Contains(types, t => t.Type == "Mug");
        Assert.Contains(types, t => t.Type == "T-Shirt");
    }

    // --- GET /api/catalog/stock ---

    [Fact]
    public async Task GetAvailableStock_ExistingRecord_ReturnsStock()
    {
        // Seed has stock for item 1 on 2017-09-20
        var response = await _client.GetAsync("/api/catalog/stock?date=2017-09-20&itemId=1");
        response.EnsureSuccessStatusCode();

        var stock = await response.Content.ReadFromJsonAsync<CatalogItemStockDto>(JsonOptions);
        Assert.NotNull(stock);
        Assert.Equal(100, stock!.AvailableStock);
        Assert.Equal(1, stock.CatalogItemId);
    }

    [Fact]
    public async Task GetAvailableStock_NoRecord_ReturnsZeroStock()
    {
        var response = await _client.GetAsync("/api/catalog/stock?date=2099-01-01&itemId=1");
        response.EnsureSuccessStatusCode();

        var stock = await response.Content.ReadFromJsonAsync<CatalogItemStockDto>(JsonOptions);
        Assert.NotNull(stock);
        Assert.Equal(0, stock!.AvailableStock);
    }

    // --- POST /api/catalog/stock ---

    [Fact]
    public async Task CreateAvailableStock_NewRecord_Returns201()
    {
        var request = new CreateStockRequest(new DateTime(2025, 1, 1), 2, 500);
        var response = await _client.PostAsJsonAsync("/api/catalog/stock", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var stock = await response.Content.ReadFromJsonAsync<CatalogItemStockDto>(JsonOptions);
        Assert.NotNull(stock);
        Assert.Equal(500, stock!.AvailableStock);
        Assert.Equal(2, stock.CatalogItemId);
    }

    [Fact]
    public async Task CreateAvailableStock_ExistingRecord_UpdatesAndReturns200()
    {
        // Seed has stock for item 1 on 2017-09-21 with stock=120
        var request = new CreateStockRequest(new DateTime(2017, 9, 21), 1, 999);
        var response = await _client.PostAsJsonAsync("/api/catalog/stock", request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var stock = await response.Content.ReadFromJsonAsync<CatalogItemStockDto>(JsonOptions);
        Assert.NotNull(stock);
        Assert.Equal(999, stock!.AvailableStock);
    }

    // --- GET /api/catalog/discounts ---

    [Fact]
    public async Task GetDiscount_ExistingDate_ReturnsDiscount()
    {
        // Seed: discount 1 = 2017-09-18 to 2017-09-21
        var response = await _client.GetAsync("/api/catalog/discounts?date=2017-09-19");
        response.EnsureSuccessStatusCode();

        var discount = await response.Content.ReadFromJsonAsync<DiscountItemDto>(JsonOptions);
        Assert.NotNull(discount);
        Assert.Equal(0.3, discount!.Size);
    }

    [Fact]
    public async Task GetDiscount_NoMatchingDate_Returns404()
    {
        var response = await _client.GetAsync("/api/catalog/discounts?date=2099-01-01");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // --- Health check ---

    [Fact]
    public async Task HealthCheck_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/health");
        response.EnsureSuccessStatusCode();
    }

    // -----------------------------------------------------------------------
    // Test Factory — replaces SQL Server with SQLite in-memory
    // -----------------------------------------------------------------------
    public class CatalogApiFactory : WebApplicationFactory<Program>
    {
        private SqliteConnection? _connection;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<CatalogDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Remove the DbContext itself if registered
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(CatalogDbContext));
                if (dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                // Use a shared in-memory SQLite connection
                _connection = new SqliteConnection("DataSource=:memory:");
                _connection.Open();

                services.AddDbContext<CatalogDbContext>(options =>
                    options.UseSqlite(_connection));

                // Build service provider and initialize DB
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
                db.Database.EnsureCreated();
            });

            // Remove Kestrel port configuration for testing
            builder.UseKestrel(options => { });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _connection?.Dispose();
        }
    }
}
