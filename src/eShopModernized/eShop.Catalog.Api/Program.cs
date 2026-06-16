using eShop.Catalog.Core.Data;
using eShop.Shared.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// EM-65: Serilog logging
builder.Host.UseEShopSerilog();

// EM-68/70: EF Core DbContext registration
builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogDb")));

// EM-67: Dependency injection
builder.Services.AddCatalogServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Health endpoint
app.MapHealthChecks("/health");

// Catalog API placeholder
app.MapGet("/api/catalog", async (ICatalogService catalogService) =>
{
    var items = await catalogService.GetCatalogItemsAsync();
    return Results.Ok(items);
})
.WithName("GetCatalogItems")
.WithOpenApi();

app.Run();
