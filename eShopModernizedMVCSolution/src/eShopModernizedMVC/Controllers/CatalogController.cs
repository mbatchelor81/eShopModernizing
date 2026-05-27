using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using eShopModernizedMVC.Models;
using eShopModernizedMVC.Services;
using System.IO;
using log4net;

namespace eShopModernizedMVC.Controllers
{
    public class CatalogController : Controller
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ICatalogService _service;
        private readonly IImageService _imageService;

        public CatalogController(ICatalogService service, IImageService imageService)
        {
            _service = service;
            _imageService = imageService;
        }

        // GET /[?pageSize=3&pageIndex=10]
        public ActionResult Index(int pageSize = 10, int pageIndex = 0)
        {
            _log.Info($"Now loading... /Catalog/Index?pageSize={pageSize}&pageIndex={pageIndex}");
            var paginatedItems = _service.GetCatalogItemsPaginated(pageSize, pageIndex);
            ChangeUriPlaceholder(paginatedItems.Data);
            return View(paginatedItems);
        }

        // GET: Catalog/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _log.Info($"Now loading... /Catalog/Details?id={id}");
            CatalogItem catalogItem = _service.FindCatalogItem(id.Value);
            if (catalogItem == null)
            {
                return HttpNotFound();
            }
            AddUriPlaceHolder(catalogItem);

            return View(catalogItem);
        }

        // GET: Catalog/Create
        [Authorize]
        public ActionResult Create()
        {
            _log.Info($"Now loading... /Catalog/Create");
            ViewBag.CatalogBrandId = new SelectList(_service.GetCatalogBrands(), "Id", "Brand");
            ViewBag.CatalogTypeId = new SelectList(_service.GetCatalogTypes(), "Id", "Type");
            ViewBag.UseAzureStorage = CatalogConfiguration.UseAzureStorage;

            return View(new CatalogItem()
            {
                PictureUri = _imageService.UrlDefaultImage()
            });
        }

        // POST: Catalog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Price,PictureFileName,CatalogTypeId,CatalogBrandId,AvailableStock,RestockThreshold,MaxStockThreshold,OnReorder,TempImageName")] CatalogItem catalogItem)
        {
            _log.Info($"Now processing... /Catalog/Create?catalogItemName={catalogItem.Name}");
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(catalogItem.TempImageName))
                {
                    var fileName = Path.GetFileName(catalogItem.TempImageName);
                    catalogItem.PictureFileName = fileName;
                }

                _service.CreateCatalogItem(catalogItem);
                if (!string.IsNullOrEmpty(catalogItem.TempImageName))
                {
                    _imageService.UpdateImage(catalogItem);
                }
                return RedirectToAction("Index");
            }

            ViewBag.CatalogBrandId = new SelectList(_service.GetCatalogBrands(), "Id", "Brand", catalogItem.CatalogBrandId);
            ViewBag.CatalogTypeId = new SelectList(_service.GetCatalogTypes(), "Id", "Type", catalogItem.CatalogTypeId);
            ViewBag.UseAzureStorage = CatalogConfiguration.UseAzureStorage;
            return View(catalogItem);
        }

        // GET: Catalog/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            _log.Info($"Now loading... /Catalog/Edit?id={id}");
            CatalogItem catalogItem = _service.FindCatalogItem(id.Value);

            if (catalogItem == null)
            {
                return HttpNotFound();
            }
            AddUriPlaceHolder(catalogItem);
            ViewBag.CatalogBrandId = new SelectList(_service.GetCatalogBrands(), "Id", "Brand", catalogItem.CatalogBrandId);
            ViewBag.CatalogTypeId = new SelectList(_service.GetCatalogTypes(), "Id", "Type", catalogItem.CatalogTypeId);
            ViewBag.UseAzureStorage = CatalogConfiguration.UseAzureStorage;
            return View(catalogItem);
        }

        // POST: Catalog/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Price,PictureFileName,CatalogTypeId,CatalogBrandId,AvailableStock,RestockThreshold,MaxStockThreshold,OnReorder,TempImageName")] CatalogItem catalogItem)
        {
            _log.Info($"Now processing... /Catalog/Edit?id={catalogItem.Id}");
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(catalogItem.TempImageName))
                {
                    _imageService.UpdateImage(catalogItem);
                    var fileName = Path.GetFileName(catalogItem.TempImageName);
                    catalogItem.PictureFileName = fileName;
                }

                _service.UpdateCatalogItem(catalogItem);
                return RedirectToAction("Index");
            }
            ViewBag.CatalogBrandId = new SelectList(_service.GetCatalogBrands(), "Id", "Brand", catalogItem.CatalogBrandId);
            ViewBag.CatalogTypeId = new SelectList(_service.GetCatalogTypes(), "Id", "Type", catalogItem.CatalogTypeId);
            ViewBag.UseAzureStorage = CatalogConfiguration.UseAzureStorage;
            return View(catalogItem);
        }

        // GET: Catalog/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            _log.Info($"Now loading... /Catalog/Delete?id={id}");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CatalogItem catalogItem = _service.FindCatalogItem(id.Value);
            if (catalogItem == null)
            {
                return HttpNotFound();
            }
            AddUriPlaceHolder(catalogItem);

            return View(catalogItem);
        }

        // POST: Catalog/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            _log.Info($"Now processing... /Catalog/DeleteConfirmed?id={id}");
            CatalogItem catalogItem = _service.FindCatalogItem(id);
            _service.RemoveCatalogItem(catalogItem);
            return RedirectToAction("Index");
        }

        private static readonly Regex SqlMetaCharPattern = new Regex(@"[;'\""]|--|/\*|\*/", RegexOptions.Compiled);
        private const int MaxSearchTermLength = 200;

        // GET: Catalog/Search?q=term
        public ActionResult Search(string q)
        {
            _log.Info($"Now loading... /Catalog/Search?q={q}");

            if (string.IsNullOrWhiteSpace(q))
            {
                return RedirectToAction("Index");
            }

            if (q.Length > MaxSearchTermLength)
            {
                ModelState.AddModelError("q", $"Search term must not exceed {MaxSearchTermLength} characters.");
                var paginatedItems = _service.GetCatalogItemsPaginated(10, 0);
                ChangeUriPlaceholder(paginatedItems.Data);
                return View("Index", paginatedItems);
            }

            if (SqlMetaCharPattern.IsMatch(q))
            {
                ModelState.AddModelError("q", "Search term contains invalid characters.");
                var paginatedItems = _service.GetCatalogItemsPaginated(10, 0);
                ChangeUriPlaceholder(paginatedItems.Data);
                return View("Index", paginatedItems);
            }

            var results = _service.SearchCatalogItems(q).ToList();
            ChangeUriPlaceholder(results);
            ViewBag.SearchTerm = q;
            return View("SearchResults", results);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _service.Dispose();
            }
            base.Dispose(disposing);
        }

        private void ChangeUriPlaceholder(IEnumerable<CatalogItem> items)
        {
            foreach (var catalogItem in items)
            {
                AddUriPlaceHolder(catalogItem);
            }
        }

        private void AddUriPlaceHolder(CatalogItem item)
        {
            item.PictureUri = _imageService.BuildUrlImage(item);
        }
    }
}

