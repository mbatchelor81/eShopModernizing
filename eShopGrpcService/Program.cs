using Microsoft.EntityFrameworkCore;
using eShopGrpcService.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogDb")));

var app = builder.Build();

app.MapGet("/", () => "eShop gRPC Service is running.");

app.Run();
