using eShop.Catalog.Web.Services;
using eShop.Shared.Core;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// EM-65: Serilog logging
builder.Host.UseEShopSerilog();

// EM-84: Cookie-based authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
    });

// EM-80: Typed HttpClient for the Catalog REST API
builder.Services.AddHttpClient<ICatalogApiClient, CatalogApiClient>(client =>
{
    var apiBaseUrl = builder.Configuration.GetValue<string>("CatalogApiBaseUrl")
                     ?? "http://localhost:5000";
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Catalog/Index");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// EM-84: Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Catalog}/{action=Index}/{id?}");

app.Run();

// EM-86: Expose Program class for WebApplicationFactory test compatibility
namespace eShop.Catalog.Web
{
    public partial class Program { }
}
