using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using eShop.Catalog.Api.DTOs;
using eShop.Catalog.Core.Data;
using eShop.Catalog.Core.Entities;
using eShop.Catalog.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace eShop.Catalog.Tests;

public class CatalogWebIntegrationTests : IClassFixture<CatalogWebIntegrationTests.CatalogWebFactory>, IDisposable
{
    private readonly HttpClient _client;
    private readonly CatalogWebFactory _factory;

    public CatalogWebIntegrationTests(CatalogWebFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    // --- Index ---

    [Fact]
    public async Task Index_ReturnsSuccessAndContainsCatalog()
    {
        var response = await _client.GetAsync("/Catalog");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Catalog", content);
    }

    [Fact]
    public async Task Index_ContainsPaginationControls()
    {
        var response = await _client.GetAsync("/Catalog?pageSize=5&pageIndex=0");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Next", content);
    }

    [Fact]
    public async Task Index_WithPagination_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/Catalog?pageSize=3&pageIndex=1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // --- Details ---

    [Fact]
    public async Task Details_ExistingItem_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/Catalog/Details/1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains(".NET Bot Black Hoodie", content);
    }

    [Fact]
    public async Task Details_NonExistingItem_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/Catalog/Details/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Details_NullId_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/Catalog/Details");
        // No id route → returns 400 or 404 depending on routing
        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.NotFound);
    }

    // --- Auth-protected actions redirect to login ---

    [Fact]
    public async Task Create_Get_UnauthenticatedRedirectsToLogin()
    {
        var response = await _client.GetAsync("/Catalog/Create");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/Login", response.Headers.Location?.ToString() ?? "");
    }

    [Fact]
    public async Task Edit_Get_UnauthenticatedRedirectsToLogin()
    {
        var response = await _client.GetAsync("/Catalog/Edit/1");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/Login", response.Headers.Location?.ToString() ?? "");
    }

    [Fact]
    public async Task Delete_Get_UnauthenticatedRedirectsToLogin()
    {
        var response = await _client.GetAsync("/Catalog/Delete/1");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/Login", response.Headers.Location?.ToString() ?? "");
    }

    // --- Login ---

    [Fact]
    public async Task Login_Get_ReturnsLoginPage()
    {
        var response = await _client.GetAsync("/Account/Login");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Login", content);
        Assert.Contains("username", content);
        Assert.Contains("password", content);
    }

    // --- Layout ---

    [Fact]
    public async Task Layout_ContainsNavigation()
    {
        var response = await _client.GetAsync("/Catalog");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("eShop Catalog", content);
        Assert.Contains("Catalog Manager", content);
        Assert.Contains("Login", content);
        Assert.Contains("footer", content, StringComparison.OrdinalIgnoreCase);
    }

    // --- WebApplicationFactory ---

    public class CatalogWebFactory : WebApplicationFactory<eShop.Catalog.Web.Program>
    {
        private SqliteConnection? _connection;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");

            builder.ConfigureTestServices(services =>
            {
                // Remove existing ICatalogApiClient registration
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ICatalogApiClient));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Replace with in-memory implementation backed by SQLite CatalogDbContext
                _connection = new SqliteConnection("DataSource=:memory:");
                _connection.Open();

                var dbDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CatalogDbContext>));
                if (dbDescriptor != null)
                    services.Remove(dbDescriptor);

                services.AddDbContext<CatalogDbContext>(options =>
                    options.UseSqlite(_connection));

                services.AddScoped<ICatalogApiClient, DbCatalogApiClient>();

                // Ensure DB is created and seeded
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
                db.Database.EnsureCreated();
                SeedTestData(db);
            });
        }

        private static void SeedTestData(CatalogDbContext db)
        {
            if (db.CatalogBrands.Any()) return;

            db.CatalogBrands.AddRange(
                new CatalogBrand { Id = 1, Brand = "Azure" },
                new CatalogBrand { Id = 2, Brand = ".NET" },
                new CatalogBrand { Id = 3, Brand = "Visual Studio" },
                new CatalogBrand { Id = 4, Brand = "SQL Server" },
                new CatalogBrand { Id = 5, Brand = "Other" }
            );

            db.CatalogTypes.AddRange(
                new CatalogType { Id = 1, Type = "Mug" },
                new CatalogType { Id = 2, Type = "T-Shirt" },
                new CatalogType { Id = 3, Type = "Sheet" },
                new CatalogType { Id = 4, Type = "USB Memory Stick" }
            );

            db.CatalogItems.AddRange(
                new CatalogItem { Id = 1, Name = ".NET Bot Black Hoodie", Description = "A warm hoodie", Price = 19.5m, PictureFileName = "1.png", CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, RestockThreshold = 0, MaxStockThreshold = 0 },
                new CatalogItem { Id = 2, Name = ".NET Black & White Mug", Description = "Classic mug", Price = 8.50m, PictureFileName = "2.png", CatalogTypeId = 1, CatalogBrandId = 2, AvailableStock = 89, RestockThreshold = 0, MaxStockThreshold = 0 },
                new CatalogItem { Id = 3, Name = "Prism White T-Shirt", Description = "Clean design", Price = 12.00m, PictureFileName = "3.png", CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 56, RestockThreshold = 0, MaxStockThreshold = 0 },
                new CatalogItem { Id = 4, Name = ".NET Foundation T-Shirt", Description = "Foundation tee", Price = 12.00m, PictureFileName = "4.png", CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 120, RestockThreshold = 0, MaxStockThreshold = 0 },
                new CatalogItem { Id = 5, Name = "Roslyn Red Sheet", Description = "Red sheet", Price = 8.50m, PictureFileName = "5.png", CatalogTypeId = 3, CatalogBrandId = 5, AvailableStock = 55, RestockThreshold = 0, MaxStockThreshold = 0 }
            );

            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _connection?.Dispose();
        }
    }
}

/// <summary>
/// In-memory catalog API client backed by DbContext for testing the MVC web project.
/// </summary>
internal class DbCatalogApiClient : ICatalogApiClient
{
    private readonly CatalogDbContext _db;

    public DbCatalogApiClient(CatalogDbContext db)
    {
        _db = db;
    }

    public async Task<PaginatedItemsViewModel> GetCatalogItemsAsync(int pageSize, int pageIndex)
    {
        var query = _db.CatalogItems
            .Include(i => i.CatalogBrand)
            .Include(i => i.CatalogType);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(i => i.Id)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedItemsViewModel
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalCount = totalCount,
            Data = items
        };
    }

    public async Task<CatalogItem?> FindCatalogItemAsync(int id)
    {
        return await _db.CatalogItems
            .Include(i => i.CatalogBrand)
            .Include(i => i.CatalogType)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<CatalogBrand>> GetCatalogBrandsAsync()
    {
        return await _db.CatalogBrands.OrderBy(b => b.Id).ToListAsync();
    }

    public async Task<IEnumerable<CatalogType>> GetCatalogTypesAsync()
    {
        return await _db.CatalogTypes.OrderBy(t => t.Id).ToListAsync();
    }

    public async Task CreateCatalogItemAsync(CatalogItem item)
    {
        var maxId = await _db.CatalogItems.AnyAsync()
            ? await _db.CatalogItems.MaxAsync(i => i.Id) + 1
            : 1;
        item.Id = maxId;
        _db.CatalogItems.Add(item);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateCatalogItemAsync(CatalogItem item)
    {
        var existing = await _db.CatalogItems.FirstOrDefaultAsync(i => i.Id == item.Id);
        if (existing == null) return;

        existing.Name = item.Name;
        existing.Description = item.Description;
        existing.Price = item.Price;
        existing.PictureFileName = item.PictureFileName;
        existing.CatalogTypeId = item.CatalogTypeId;
        existing.CatalogBrandId = item.CatalogBrandId;
        existing.AvailableStock = item.AvailableStock;
        existing.RestockThreshold = item.RestockThreshold;
        existing.MaxStockThreshold = item.MaxStockThreshold;
        existing.OnReorder = item.OnReorder;

        await _db.SaveChangesAsync();
    }

    public async Task RemoveCatalogItemAsync(int id)
    {
        var item = await _db.CatalogItems.FirstOrDefaultAsync(i => i.Id == id);
        if (item != null)
        {
            _db.CatalogItems.Remove(item);
            await _db.SaveChangesAsync();
        }
    }
}
