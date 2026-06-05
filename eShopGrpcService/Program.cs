using eShopGrpcService.Data;
using eShopGrpcService.Infrastructure;
using eShopGrpcService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("ConnectionString")
    ?? builder.Configuration.GetConnectionString("CatalogDb");

builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<CatalogGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

public partial class Program { }
