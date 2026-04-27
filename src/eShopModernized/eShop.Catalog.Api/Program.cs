using eShop.Catalog.Api.DTOs;
using eShop.Catalog.Api.Services;
using eShop.Catalog.Core.Data;
using eShop.Catalog.Core.Entities;
using eShop.Shared.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// EM-65: Serilog logging
builder.Host.UseEShopSerilog();

// EM-68/70: EF Core DbContext registration
builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogDb")));

// EM-67: Dependency injection
builder.Services.AddCatalogServices(builder.Configuration);

// EM-74: OpenAPI / Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "eShop Catalog API",
        Version = "v1",
        Description = "REST API for the eShop Catalog service — migrated from legacy WCF"
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

// EM-76: gRPC
builder.Services.AddGrpc();

builder.Services.AddHealthChecks();

// EM-76: Configure Kestrel for dual HTTP/2 (gRPC) + HTTP/1.1 (REST)
var httpPort = builder.Configuration.GetValue("Kestrel:HttpPort", 5000);
var grpcPort = builder.Configuration.GetValue("Kestrel:GrpcPort", 5001);
builder.WebHost.ConfigureKestrel(options =>
{
    // HTTP/1.1 + HTTP/2 for REST + Swagger
    options.ListenAnyIP(httpPort, o => o.Protocols = HttpProtocols.Http1AndHttp2);
    // HTTP/2 only for gRPC
    options.ListenAnyIP(grpcPort, o => o.Protocols = HttpProtocols.Http2);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "eShop Catalog API v1");
    });
}

app.UseHttpsRedirection();

// Health endpoint
app.MapHealthChecks("/health");

// EM-76: gRPC service mapping
app.MapGrpcService<CatalogGrpcService>();

// ---------------------------------------------------------------------------
// EM-75 + EM-78: REST API Endpoints with XML documentation
// ---------------------------------------------------------------------------

// --- Catalog Items ---

/// <summary>
/// Retrieves a single catalog item by its ID, including brand and type details.
/// </summary>
app.MapGet("/api/catalog/items/{id:int}", async (int id, CatalogDbContext db) =>
{
    var item = await db.CatalogItems
        .Include(i => i.CatalogBrand)
        .Include(i => i.CatalogType)
        .FirstOrDefaultAsync(i => i.Id == id);

    if (item is null)
        return Results.NotFound(new { message = $"Catalog item {id} not found" });

    return Results.Ok(MapToDto(item));
})
.WithName("FindCatalogItem")
.WithTags("Catalog Items")
.WithDescription("Retrieves a single catalog item by ID with brand and type details.")
.Produces<CatalogItemDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithOpenApi();

/// <summary>
/// Retrieves a paginated list of catalog items, optionally filtered by brand and/or type.
/// </summary>
app.MapGet("/api/catalog/items", async (
    CatalogDbContext db,
    int? brand,
    int? type,
    int pageSize = 10,
    int pageIndex = 0) =>
{
    var query = db.CatalogItems
        .Include(i => i.CatalogBrand)
        .Include(i => i.CatalogType)
        .AsQueryable();

    if (brand.HasValue && brand.Value > 0)
        query = query.Where(i => i.CatalogBrandId == brand.Value);
    if (type.HasValue && type.Value > 0)
        query = query.Where(i => i.CatalogTypeId == type.Value);

    var totalCount = await query.CountAsync();

    var items = await query
        .OrderBy(i => i.Id)
        .Skip(pageIndex * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return Results.Ok(new PaginatedResponse<CatalogItemDto>(
        pageIndex, pageSize, totalCount, items.Select(MapToDto)));
})
.WithName("GetCatalogItems")
.WithTags("Catalog Items")
.WithDescription("Retrieves a paginated list of catalog items. Filter by brand (0 = all) and type (0 = all).")
.Produces<PaginatedResponse<CatalogItemDto>>(StatusCodes.Status200OK)
.WithOpenApi();

/// <summary>
/// Creates a new catalog item.
/// </summary>
app.MapPost("/api/catalog/items", async (CreateCatalogItemRequest request, CatalogDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest(new { message = "Name is required" });

    await using var transaction = await db.Database.BeginTransactionAsync(
        System.Data.IsolationLevel.Serializable);

    var newId = await db.CatalogItems.AnyAsync()
        ? await db.CatalogItems.MaxAsync(i => i.Id) + 1
        : 1;

    var item = new CatalogItem
    {
        Id = newId,
        Name = request.Name,
        Description = request.Description,
        Price = request.Price,
        PictureFileName = request.PictureFileName,
        CatalogTypeId = request.CatalogTypeId,
        CatalogBrandId = request.CatalogBrandId,
        AvailableStock = request.AvailableStock,
        RestockThreshold = request.RestockThreshold,
        MaxStockThreshold = request.MaxStockThreshold,
        OnReorder = request.OnReorder
    };

    db.CatalogItems.Add(item);
    await db.SaveChangesAsync();
    await transaction.CommitAsync();

    // Reload with navigation properties
    var created = await db.CatalogItems
        .Include(i => i.CatalogBrand)
        .Include(i => i.CatalogType)
        .FirstAsync(i => i.Id == newId);

    return Results.Created($"/api/catalog/items/{created.Id}", MapToDto(created));
})
.WithName("CreateCatalogItem")
.WithTags("Catalog Items")
.WithDescription("Creates a new catalog item. ID is auto-generated.")
.Accepts<CreateCatalogItemRequest>("application/json")
.Produces<CatalogItemDto>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest)
.WithOpenApi();

/// <summary>
/// Updates an existing catalog item.
/// </summary>
app.MapPut("/api/catalog/items/{id:int}", async (int id, UpdateCatalogItemRequest request, CatalogDbContext db) =>
{
    var existing = await db.CatalogItems.FirstOrDefaultAsync(i => i.Id == id);
    if (existing is null)
        return Results.NotFound(new { message = $"Catalog item {id} not found" });

    existing.Name = request.Name;
    existing.Description = request.Description;
    existing.Price = request.Price;
    existing.PictureFileName = request.PictureFileName;
    existing.CatalogTypeId = request.CatalogTypeId;
    existing.CatalogBrandId = request.CatalogBrandId;
    existing.AvailableStock = request.AvailableStock;
    existing.RestockThreshold = request.RestockThreshold;
    existing.MaxStockThreshold = request.MaxStockThreshold;
    existing.OnReorder = request.OnReorder;

    await db.SaveChangesAsync();

    var updated = await db.CatalogItems
        .Include(i => i.CatalogBrand)
        .Include(i => i.CatalogType)
        .FirstAsync(i => i.Id == id);

    return Results.Ok(MapToDto(updated));
})
.WithName("UpdateCatalogItem")
.WithTags("Catalog Items")
.WithDescription("Updates an existing catalog item by ID.")
.Accepts<UpdateCatalogItemRequest>("application/json")
.Produces<CatalogItemDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithOpenApi();

/// <summary>
/// Deletes a catalog item by ID.
/// </summary>
app.MapDelete("/api/catalog/items/{id:int}", async (int id, CatalogDbContext db) =>
{
    var item = await db.CatalogItems.FirstOrDefaultAsync(i => i.Id == id);
    if (item is null)
        return Results.NotFound(new { message = $"Catalog item {id} not found" });

    db.CatalogItems.Remove(item);
    await db.SaveChangesAsync();

    return Results.NoContent();
})
.WithName("RemoveCatalogItem")
.WithTags("Catalog Items")
.WithDescription("Removes a catalog item by ID.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.WithOpenApi();

// --- Catalog Brands ---

/// <summary>
/// Retrieves all catalog brands.
/// </summary>
app.MapGet("/api/catalog/brands", async (CatalogDbContext db) =>
{
    var brands = await db.CatalogBrands
        .OrderBy(b => b.Id)
        .Select(b => new CatalogBrandDto(b.Id, b.Brand))
        .ToListAsync();
    return Results.Ok(brands);
})
.WithName("GetCatalogBrands")
.WithTags("Catalog Brands")
.WithDescription("Retrieves all available catalog brands.")
.Produces<List<CatalogBrandDto>>(StatusCodes.Status200OK)
.WithOpenApi();

// --- Catalog Types ---

/// <summary>
/// Retrieves all catalog types.
/// </summary>
app.MapGet("/api/catalog/types", async (CatalogDbContext db) =>
{
    var types = await db.CatalogTypes
        .OrderBy(t => t.Id)
        .Select(t => new CatalogTypeDto(t.Id, t.Type))
        .ToListAsync();
    return Results.Ok(types);
})
.WithName("GetCatalogTypes")
.WithTags("Catalog Types")
.WithDescription("Retrieves all available catalog types.")
.Produces<List<CatalogTypeDto>>(StatusCodes.Status200OK)
.WithOpenApi();

// --- Stock ---

/// <summary>
/// Retrieves available stock for a catalog item on a specific date.
/// </summary>
app.MapGet("/api/catalog/stock", async (CatalogDbContext db, DateTime date, int itemId) =>
{
    var stock = await db.CatalogItemsStocks
        .FirstOrDefaultAsync(s => s.CatalogItemId == itemId && s.Date.Date == date.Date);

    if (stock is null)
        return Results.Ok(new CatalogItemStockDto(0, date, itemId, 0));

    return Results.Ok(new CatalogItemStockDto(stock.StockId, stock.Date, stock.CatalogItemId, stock.AvailableStock));
})
.WithName("GetAvailableStock")
.WithTags("Stock")
.WithDescription("Retrieves available stock for a catalog item on a given date. Returns 0 stock if no record found.")
.Produces<CatalogItemStockDto>(StatusCodes.Status200OK)
.WithOpenApi();

/// <summary>
/// Creates or updates stock availability for a catalog item on a specific date (upsert).
/// </summary>
app.MapPost("/api/catalog/stock", async (CreateStockRequest request, CatalogDbContext db) =>
{
    await using var transaction = await db.Database.BeginTransactionAsync(
        System.Data.IsolationLevel.Serializable);

    var existing = await db.CatalogItemsStocks
        .FirstOrDefaultAsync(s => s.CatalogItemId == request.CatalogItemId && s.Date.Date == request.Date.Date);

    if (existing is not null)
    {
        existing.AvailableStock = request.AvailableStock;
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return Results.Ok(new CatalogItemStockDto(
            existing.StockId, existing.Date, existing.CatalogItemId, existing.AvailableStock));
    }

    var maxId = await db.CatalogItemsStocks.AnyAsync()
        ? await db.CatalogItemsStocks.MaxAsync(s => s.StockId)
        : 0;

    var stock = new CatalogItemsStock
    {
        StockId = maxId + 1,
        Date = request.Date.Date,
        CatalogItemId = request.CatalogItemId,
        AvailableStock = request.AvailableStock
    };

    db.CatalogItemsStocks.Add(stock);
    await db.SaveChangesAsync();
    await transaction.CommitAsync();

    return Results.Created($"/api/catalog/stock?date={stock.Date:yyyy-MM-dd}&itemId={stock.CatalogItemId}",
        new CatalogItemStockDto(stock.StockId, stock.Date, stock.CatalogItemId, stock.AvailableStock));
})
.WithName("CreateAvailableStock")
.WithTags("Stock")
.WithDescription("Creates or updates stock availability for a catalog item on a given date (upsert pattern).")
.Accepts<CreateStockRequest>("application/json")
.Produces<CatalogItemStockDto>(StatusCodes.Status200OK)
.Produces<CatalogItemStockDto>(StatusCodes.Status201Created)
.WithOpenApi();

// --- Discounts ---

/// <summary>
/// Retrieves a discount applicable on the specified date.
/// </summary>
app.MapGet("/api/catalog/discounts", async (CatalogDbContext db, DateTime date) =>
{
    var discount = await db.DiscountItems
        .FirstOrDefaultAsync(d => d.Start.Date <= date.Date && d.End.Date >= date.Date);

    if (discount is null)
        return Results.NotFound(new { message = $"No discount found for date {date:yyyy-MM-dd}" });

    return Results.Ok(new DiscountItemDto(discount.Id, discount.Size, discount.Start, discount.End));
})
.WithName("GetDiscount")
.WithTags("Discounts")
.WithDescription("Retrieves a discount item active on the specified date.")
.Produces<DiscountItemDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithOpenApi();

app.Run();

// ---------------------------------------------------------------------------
// Helper: map entity to DTO
// ---------------------------------------------------------------------------
static CatalogItemDto MapToDto(CatalogItem item) =>
    new(
        item.Id,
        item.Name,
        item.Description,
        item.Price,
        item.PictureFileName,
        item.PictureUri,
        item.CatalogTypeId,
        item.CatalogType?.Type ?? string.Empty,
        item.CatalogBrandId,
        item.CatalogBrand?.Brand ?? string.Empty,
        item.AvailableStock,
        item.RestockThreshold,
        item.MaxStockThreshold,
        item.OnReorder);

// EM-77: Expose Program class for WebApplicationFactory
public partial class Program { }
