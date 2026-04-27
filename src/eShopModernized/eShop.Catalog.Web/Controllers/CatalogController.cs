using eShop.Catalog.Core.Entities;
using eShop.Catalog.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eShop.Catalog.Web.Controllers;

public class CatalogController : Controller
{
    private readonly ICatalogApiClient _catalogClient;
    private readonly ILogger<CatalogController> _logger;

    public CatalogController(ICatalogApiClient catalogClient, ILogger<CatalogController> logger)
    {
        _catalogClient = catalogClient;
        _logger = logger;
    }

    // GET /Catalog[?pageSize=10&pageIndex=0]
    public async Task<IActionResult> Index(int pageSize = 10, int pageIndex = 0)
    {
        _logger.LogInformation("Loading /Catalog/Index?pageSize={PageSize}&pageIndex={PageIndex}", pageSize, pageIndex);
        var paginatedItems = await _catalogClient.GetCatalogItemsAsync(pageSize, pageIndex);
        return View(paginatedItems);
    }

    // GET: Catalog/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        _logger.LogInformation("Loading /Catalog/Details?id={Id}", id);
        if (id == null)
            return BadRequest();

        var catalogItem = await _catalogClient.FindCatalogItemAsync(id.Value);
        if (catalogItem == null)
            return NotFound();

        return View(catalogItem);
    }

    // GET: Catalog/Create
    [Authorize]
    public async Task<IActionResult> Create()
    {
        _logger.LogInformation("Loading /Catalog/Create");
        await PopulateBrandsAndTypesAsync();
        return View(new CatalogItem());
    }

    // POST: Catalog/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(CatalogItem catalogItem)
    {
        _logger.LogInformation("Processing /Catalog/Create?name={Name}", catalogItem.Name);
        if (ModelState.IsValid)
        {
            await _catalogClient.CreateCatalogItemAsync(catalogItem);
            return RedirectToAction(nameof(Index));
        }

        await PopulateBrandsAndTypesAsync(catalogItem.CatalogBrandId, catalogItem.CatalogTypeId);
        return View(catalogItem);
    }

    // GET: Catalog/Edit/5
    [Authorize]
    public async Task<IActionResult> Edit(int? id)
    {
        _logger.LogInformation("Loading /Catalog/Edit?id={Id}", id);
        if (id == null)
            return BadRequest();

        var catalogItem = await _catalogClient.FindCatalogItemAsync(id.Value);
        if (catalogItem == null)
            return NotFound();

        await PopulateBrandsAndTypesAsync(catalogItem.CatalogBrandId, catalogItem.CatalogTypeId);
        return View(catalogItem);
    }

    // POST: Catalog/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(CatalogItem catalogItem)
    {
        _logger.LogInformation("Processing /Catalog/Edit?id={Id}", catalogItem.Id);
        if (ModelState.IsValid)
        {
            await _catalogClient.UpdateCatalogItemAsync(catalogItem);
            return RedirectToAction(nameof(Index));
        }

        await PopulateBrandsAndTypesAsync(catalogItem.CatalogBrandId, catalogItem.CatalogTypeId);
        return View(catalogItem);
    }

    // GET: Catalog/Delete/5
    [Authorize]
    public async Task<IActionResult> Delete(int? id)
    {
        _logger.LogInformation("Loading /Catalog/Delete?id={Id}", id);
        if (id == null)
            return BadRequest();

        var catalogItem = await _catalogClient.FindCatalogItemAsync(id.Value);
        if (catalogItem == null)
            return NotFound();

        return View(catalogItem);
    }

    // POST: Catalog/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        _logger.LogInformation("Processing /Catalog/DeleteConfirmed?id={Id}", id);
        await _catalogClient.RemoveCatalogItemAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateBrandsAndTypesAsync(int? selectedBrandId = null, int? selectedTypeId = null)
    {
        var brands = await _catalogClient.GetCatalogBrandsAsync();
        var types = await _catalogClient.GetCatalogTypesAsync();

        ViewBag.CatalogBrandId = new SelectList(brands, "Id", "Brand", selectedBrandId);
        ViewBag.CatalogTypeId = new SelectList(types, "Id", "Type", selectedTypeId);
    }
}
