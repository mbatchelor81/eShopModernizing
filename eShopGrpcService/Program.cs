using Microsoft.EntityFrameworkCore;
using eShopGrpcService.Data;
using eShopGrpcService.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel for HTTP/2 (required for gRPC)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5001, o => o.Protocols =
        Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2);
});

// Add services
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

// Register EF Core DbContext
builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogDb")));

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<CatalogDbContext>();

var app = builder.Build();

// Map gRPC service
app.MapGrpcService<CatalogGrpcService>();

// gRPC reflection for dev tooling (grpcurl, grpcui, etc.)
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

// Health check endpoint
app.MapHealthChecks("/health");

app.MapGet("/", () => "eShop gRPC Catalog Service. Use a gRPC client to communicate.");

app.Run();
