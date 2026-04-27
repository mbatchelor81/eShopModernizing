using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Shared.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCatalogServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var useMockData = configuration.GetValue<bool>("UseMockData");

        if (useMockData)
        {
            // Register mock catalog services for development/demo
            services.AddSingleton<ICatalogService, CatalogServiceMock>();
        }
        else
        {
            // Register real catalog services backed by EF Core
            services.AddScoped<ICatalogService, CatalogService>();
        }

        return services;
    }
}

/// <summary>
/// Catalog service abstraction. Implementations will be fleshed out
/// when the domain entities and DbContext are ported in later waves.
/// </summary>
public interface ICatalogService
{
    Task<IEnumerable<object>> GetCatalogItemsAsync();
}

public class CatalogServiceMock : ICatalogService
{
    public Task<IEnumerable<object>> GetCatalogItemsAsync()
    {
        var items = new List<object>
        {
            new { Id = 1, Name = "Mock Catalog Item 1", Price = 19.99m },
            new { Id = 2, Name = "Mock Catalog Item 2", Price = 29.99m }
        };
        return Task.FromResult<IEnumerable<object>>(items);
    }
}

public class CatalogService : ICatalogService
{
    public Task<IEnumerable<object>> GetCatalogItemsAsync()
    {
        // Placeholder: will be wired to EF Core DbContext in a later wave
        return Task.FromResult<IEnumerable<object>>(Array.Empty<object>());
    }
}
