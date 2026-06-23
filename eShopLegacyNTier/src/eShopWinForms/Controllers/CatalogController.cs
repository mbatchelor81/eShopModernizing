using System.Collections.Generic;
using System;
using eShopWinForms.Services;

namespace eShopWinForms.Controllers
{
    public class CatalogController
    {
        private GrpcCatalogService _service;
        private ICatalogView _view;

        public CatalogController(GrpcCatalogService service, ICatalogView view)
        {
            this._service = service;
            this._view = view;
            this._view.filterChanged += new ViewHandler<ICatalogView>(this.filterChanged);
            this._view.availabilityButtonClicked += new AvailabilityHandler<ICatalogView>(this.addAvailability);
            this._view.searchStockButtonClicked += new SearchStockHandler<ICatalogView>(this.searchStockAvailable);
        }

        private void filterChanged(ICatalogView view, FilterEventArgs e)
        {
            LoadCatalogItems(e.brandFilterValue, e.typeFilterValue);
        }

        private void addAvailability(ICatalogView view, AvailabilityEventArgs e)
        {
            _service.CreateAvailableStock(e.itemId, e.itemStock, e.shipDate);
            _view.NotifyAvailabilityUpdated();
        }

        private void searchStockAvailable(ICatalogView view, SearchStockEventArgs e)
        {
            int res = _service.GetAvailableStock(e.date, e.itemId);
            _view.ShowStockAvailability(e, res);
        }

        private void CheckForDiscounts()
        {
            double discountPercentage = 0;
            DiscountItemDto discount = _service.GetDiscount(DateTime.Now);
            if (discount != null)
            {
                discountPercentage = Math.Round(discount.Size * 100, 0);
                String bannerText = String.Format("{0}% sale ends on {1}!", discountPercentage.ToString(), discount.End.ToShortDateString());
                _view.SetDiscountBanner(bannerText);
            }
        }

        public void LoadCatalogItems(int brandIdFilter, int typeIdFilter)
        {
            _view.ClearGrid();
            IEnumerable<CatalogItemDto> items = _service.GetCatalogItems(brandIdFilter, typeIdFilter);
            double discountVal = 0;
            DiscountItemDto discount = _service.GetDiscount(DateTime.Now);
            if (discount != null)
                discountVal = discount.Size;

            _view.SetCatalogItems(items, discountVal);
        }

        private void SetShipmentView()
        {
            IEnumerable<CatalogItemDto> items = _service.GetCatalogItems(0, 0);
            _view.SetShipmentView(items);
        }

        private void LoadBrandFilters()
        {
            IEnumerable<CatalogBrandDto> brands = _service.GetCatalogBrands();
            Dictionary<int, string> brandDictionary = new Dictionary<int, string>();
            brandDictionary.Add(0, "All");

            foreach (var catalogBrand in brands)
            {
                brandDictionary.Add(catalogBrand.Id, catalogBrand.Brand);
            }

            _view.SetBrandFilter(brandDictionary);
        }

        private void LoadTypeFilters()
        {
            IEnumerable<CatalogTypeDto> types = _service.GetCatalogTypes();
            Dictionary<int, string> typeDictionary = new Dictionary<int, string>();
            typeDictionary.Add(0, "All");

            foreach (var catalogtype in types)
            {
                typeDictionary.Add(catalogtype.Id, catalogtype.Type);
            }

            _view.SetTypeFilter(typeDictionary);
        }

        public void LoadView()
        {
            _view.SetController(this);
            CheckForDiscounts();
            LoadCatalogItems(0, 0);
            LoadBrandFilters();
            LoadTypeFilters();
            SetShipmentView();
        }
    }
}
