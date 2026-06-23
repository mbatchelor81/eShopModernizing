var builder = WebApplication.CreateBuilder(args);

// Add gRPC services
builder.Services.AddGrpc();

var app = builder.Build();

// gRPC service endpoints will be mapped here in T8
app.MapGet("/", () => "eShop gRPC Service is running.");

app.Run();
