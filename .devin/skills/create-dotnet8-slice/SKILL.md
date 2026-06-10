---
name: create-dotnet8-slice
description: Step-by-step guide for creating a .NET 8 ASP.NET Core Razor Pages migration slice from a legacy eShop component. Use after analyzing the legacy component — covers project setup, model porting, Razor Page creation, DI configuration, and validation.
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
mkdir -p eShopModernizedDotNet8
cd eShopModernizedDotNet8
dotnet new webapp -n eShop.<SliceName> --no-https
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

### 3. Create the in-memory data service

Seed catalog data from a plain C# in-memory service (do not use EF Core InMemory provider):

```csharp
public interface ICatalogService
{
    IReadOnlyList<CatalogItem> GetItems(int pageSize, int pageIndex);
    CatalogItem? GetById(int id);
    int Count { get; }
}

public class InMemoryCatalogService : ICatalogService
{
    private readonly List<CatalogItem> _items = SeedData.GetCatalogItems();

    public IReadOnlyList<CatalogItem> GetItems(int pageSize, int pageIndex) =>
        _items.OrderBy(i => i.Name)
              .Skip(pageSize * pageIndex)
              .Take(pageSize)
              .ToList();

    public CatalogItem? GetById(int id) =>
        _items.FirstOrDefault(i => i.Id == id);

    public int Count => _items.Count;
}
```

Register in `Program.cs`:
```csharp
builder.Services.AddSingleton<ICatalogService, InMemoryCatalogService>();
```

### 4. Create Razor Pages

Use ASP.NET Core Razor Pages (not MVC controllers) per project rules:

```csharp
// Pages/Catalog/Index.cshtml.cs
public class IndexModel : PageModel
{
    private readonly ICatalogService _catalog;

    public IndexModel(ICatalogService catalog) => _catalog = catalog;

    public IReadOnlyList<CatalogItem> Items { get; private set; } = [];
    [BindProperty(SupportsGet = true)] public int PageSize { get; set; } = 10;
    [BindProperty(SupportsGet = true)] public int PageIndex { get; set; } = 0;
    public int TotalItems { get; private set; }

    public void OnGet()
    {
        Items = _catalog.GetItems(PageSize, PageIndex);
        TotalItems = _catalog.Count;
    }
}
```

```html
@* Pages/Catalog/Index.cshtml *@
@page
@model IndexModel

<h1>Catalog</h1>
<table>
    @foreach (var item in Model.Items)
    {
        <tr>
            <td>@item.Name</td>
            <td>@item.Price.ToString("C")</td>
        </tr>
    }
</table>
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

### 7. Add seed data

Create a `SeedData` class that returns hardcoded catalog items:

```csharp
public static class SeedData
{
    public static List<CatalogItem> GetCatalogItems() =>
    [
        new() { Id = 1, Name = ".NET Bot Black Hoodie", Price = 19.5m, CatalogTypeId = 2 },
        new() { Id = 2, Name = ".NET Black & White Mug", Price = 8.5m, CatalogTypeId = 1 },
        // ... mirror items from the legacy CatalogServiceMock
    ];
}
```

### 8. Validate

```bash
dotnet restore
dotnet build
dotnet test   # if tests exist
dotnet run    # verify endpoints work
```

Verify in browser:
```bash
# Navigate to http://localhost:5000/Catalog
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
