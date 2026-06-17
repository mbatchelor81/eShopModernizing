using System.Net.Http.Json;
using System.Text.Json;

namespace eShop.Catalog.Client;

/// <summary>
/// Cross-platform console client that replaces the legacy WinForms/WCF client.
/// Demonstrates calling the eShop Catalog REST API via HttpClient.
/// </summary>
public class Program
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public static async Task Main(string[] args)
    {
        var baseUrl = args.Length > 0 ? args[0] : "http://localhost:5000";
        Console.WriteLine($"eShop Catalog Console Client — connecting to {baseUrl}");
        Console.WriteLine(new string('=', 60));

        using var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };

        await ListCatalogItemsAsync(httpClient);
        await GetCatalogItemAsync(httpClient, 1);
        await ListBrandsAsync(httpClient);
        await ListTypesAsync(httpClient);

        var newItemId = await CreateCatalogItemAsync(httpClient);
        if (newItemId > 0)
        {
            await UpdateCatalogItemAsync(httpClient, newItemId);
            await DeleteCatalogItemAsync(httpClient, newItemId);
        }

        Console.WriteLine();
        Console.WriteLine("All operations completed successfully.");
    }

    private static async Task ListCatalogItemsAsync(HttpClient client)
    {
        Console.WriteLine();
        Console.WriteLine("[LIST] Fetching catalog items (page 0, size 5)...");

        var response = await client.GetAsync("/api/catalog/items?pageSize=5&pageIndex=0");
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<PaginatedResponse>(JsonOptions);
        Console.WriteLine($"  Total items: {body?.TotalCount}");
        if (body?.Data != null)
        {
            foreach (var item in body.Data)
            {
                Console.WriteLine($"  [{item.Id}] {item.Name} - ${item.Price:F2} (Stock: {item.AvailableStock})");
            }
        }
    }

    private static async Task GetCatalogItemAsync(HttpClient client, int id)
    {
        Console.WriteLine();
        Console.WriteLine($"[GET] Fetching catalog item {id}...");

        var response = await client.GetAsync($"/api/catalog/items/{id}");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"  Item {id} not found.");
            return;
        }

        var item = await response.Content.ReadFromJsonAsync<CatalogItemResponse>(JsonOptions);
        Console.WriteLine($"  Name: {item?.Name}");
        Console.WriteLine($"  Brand: {item?.CatalogBrandName}");
        Console.WriteLine($"  Type: {item?.CatalogTypeName}");
        Console.WriteLine($"  Price: ${item?.Price:F2}");
    }

    private static async Task ListBrandsAsync(HttpClient client)
    {
        Console.WriteLine();
        Console.WriteLine("[BRANDS] Fetching catalog brands...");

        var brands = await client.GetFromJsonAsync<List<BrandResponse>>("/api/catalog/brands", JsonOptions);
        if (brands != null)
        {
            foreach (var b in brands)
                Console.WriteLine($"  [{b.Id}] {b.Brand}");
        }
    }

    private static async Task ListTypesAsync(HttpClient client)
    {
        Console.WriteLine();
        Console.WriteLine("[TYPES] Fetching catalog types...");

        var types = await client.GetFromJsonAsync<List<TypeResponse>>("/api/catalog/types", JsonOptions);
        if (types != null)
        {
            foreach (var t in types)
                Console.WriteLine($"  [{t.Id}] {t.Type}");
        }
    }

    private static async Task<int> CreateCatalogItemAsync(HttpClient client)
    {
        Console.WriteLine();
        Console.WriteLine("[CREATE] Creating a new catalog item...");

        var request = new
        {
            Name = "Console Client Test Item",
            Description = "Created by the cross-platform console client",
            Price = 42.99m,
            PictureFileName = "console-test.png",
            CatalogTypeId = 1,
            CatalogBrandId = 1,
            AvailableStock = 100,
            RestockThreshold = 10,
            MaxStockThreshold = 200,
            OnReorder = false
        };

        var response = await client.PostAsJsonAsync("/api/catalog/items", request);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"  Failed: {response.StatusCode}");
            return 0;
        }

        var created = await response.Content.ReadFromJsonAsync<CatalogItemResponse>(JsonOptions);
        Console.WriteLine($"  Created item ID: {created?.Id}, Name: {created?.Name}");
        return created?.Id ?? 0;
    }

    private static async Task UpdateCatalogItemAsync(HttpClient client, int id)
    {
        Console.WriteLine();
        Console.WriteLine($"[UPDATE] Updating catalog item {id}...");

        var request = new
        {
            Name = "Console Client Updated Item",
            Description = "Updated by the cross-platform console client",
            Price = 49.99m,
            PictureFileName = "console-test.png",
            CatalogTypeId = 1,
            CatalogBrandId = 1,
            AvailableStock = 200,
            RestockThreshold = 20,
            MaxStockThreshold = 400,
            OnReorder = false
        };

        var response = await client.PutAsJsonAsync($"/api/catalog/items/{id}", request);
        response.EnsureSuccessStatusCode();
        Console.WriteLine($"  Updated successfully.");
    }

    private static async Task DeleteCatalogItemAsync(HttpClient client, int id)
    {
        Console.WriteLine();
        Console.WriteLine($"[DELETE] Deleting catalog item {id}...");

        var response = await client.DeleteAsync($"/api/catalog/items/{id}");
        response.EnsureSuccessStatusCode();
        Console.WriteLine($"  Deleted successfully.");
    }

    private record PaginatedResponse(int PageIndex, int PageSize, int TotalCount, List<CatalogItemResponse>? Data);
    private record CatalogItemResponse(int Id, string Name, string Description, decimal Price,
        string PictureFileName, string? PictureUri, int CatalogTypeId, string CatalogTypeName,
        int CatalogBrandId, string CatalogBrandName, int AvailableStock, int RestockThreshold,
        int MaxStockThreshold, bool OnReorder);
    private record BrandResponse(int Id, string Brand);
    private record TypeResponse(int Id, string Type);
}
