using eShop.Shared.Core;

var builder = WebApplication.CreateBuilder(args);

// EM-65: Serilog logging
builder.Host.UseEShopSerilog();

// EM-67: Dependency injection
builder.Services.AddCatalogServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

// EM-94: HSTS for production environments
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromSeconds(31536000);
    options.IncludeSubDomains = true;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

// EM-94: Security headers middleware (OWASP A05)
app.UseSecurityHeaders();

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
