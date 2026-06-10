---
name: create-dotnet8-slice
description: Step-by-step guide for creating a .NET 8 ASP.NET Core migration slice from a legacy eShop component. Use after analyzing the legacy component — covers project setup, model porting, controller creation, DI configuration, and validation.
---

# Create .NET 8 Migration Slice

This skill walks through building a Mac-runnable .NET 8 ASP.NET Core project that replaces functionality from a legacy eShop component.

## Prerequisites

- Completed legacy component analysis (use [analyze-legacy-component](../analyze-legacy-component/SKILL.md) first)
- .NET 8 SDK installed (`dotnet --version` should show 8.x)

## Constraints

- **Legacy projects are reference-only** — do not attempt to compile them on macOS
- All new code goes under `eShopModernizedDotNet8/`
- Prefer mock/in-memory data over real database connections for initial slices
- Use Worktree mode for parallel migration work

## Steps

### 1. Create the project

```bash
cd eShopModernizedDotNet8
dotnet new webapi -n eShop.<SliceName> --no-https
cd eShop.<SliceName>
```

### 2. Port the data models

Translate EF6 models to EF Core 8:

**Legacy (EF6):**
```csharp
[Table("CatalogItems")]
public class CatalogItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int CatalogTypeId { get; set; }
    public virtual CatalogType CatalogType { get; set; }
}
```

**Modern (EF Core 8):**
```csharp
public class CatalogItem
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public int CatalogTypeId { get; set; }
    public CatalogType? CatalogType { get; set; }
}
```

Key changes:
- Use `required` keyword instead of null-forgiving
- Replace `virtual` navigation properties with nullable references
- Use data annotations or Fluent API (prefer Fluent API for EF Core)

### 3. Set up the DbContext

For initial slices, use in-memory database:

```csharp
public class SliceDbContext : DbContext
{
    public SliceDbContext(DbContextOptions<SliceDbContext> options)
        : base(options) { }

    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
}
```

Register in `Program.cs`:
```csharp
builder.Services.AddDbContext<SliceDbContext>(options =>
    options.UseInMemoryDatabase("eShopSlice"));
```

### 4. Create controllers

Map legacy MVC controllers to ASP.NET Core Minimal APIs or Controllers:

```csharp
[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly SliceDbContext _db;

    public CatalogController(SliceDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetItems(
        [FromQuery] int pageSize = 10,
        [FromQuery] int pageIndex = 0)
    {
        var items = await _db.CatalogItems
            .OrderBy(i => i.Name)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

        return Ok(items);
    }
}
```

### 5. Configure DI

Replace Autofac/Unity with built-in DI:

```csharp
// Legacy (Autofac)
builder.RegisterType<CatalogService>().As<ICatalogService>();

// Modern (built-in DI)
builder.Services.AddScoped<ICatalogService, CatalogService>();
```

### 6. Port configuration

Replace `Web.config` / `ConfigurationManager` with `appsettings.json`:

```json
{
  "CatalogSettings": {
    "PageSize": 10,
    "ImageBaseUrl": "/images/"
  }
}
```

Bind via Options pattern:
```csharp
builder.Services.Configure<CatalogSettings>(
    builder.Configuration.GetSection("CatalogSettings"));
```

### 7. Add seed data (optional)

For demo/testing, seed mock data:

```csharp
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<SliceDbContext>();
    db.CatalogItems.AddRange(SeedData.GetCatalogItems());
    db.SaveChanges();
}
```

### 8. Validate

```bash
dotnet restore
dotnet build
dotnet test   # if tests exist
dotnet run    # verify endpoints work
```

Test the API:
```bash
curl http://localhost:5000/api/catalog
```

## Validation Checklist

- [ ] `dotnet restore` succeeds
- [ ] `dotnet build` succeeds with zero warnings
- [ ] `dotnet run` starts without errors
- [ ] API endpoints return expected data
- [ ] No `System.Web` references in new code
- [ ] No hardcoded connection strings or secrets
- [ ] All legacy behavior documented in the analysis is covered or explicitly deferred

## Additional Resources

- Use the `catalog-parity-inspector` agent to validate feature parity
- Use the `config-secrets-hardener` agent to audit configuration security
- Use the `review-captain` agent for merge readiness review
