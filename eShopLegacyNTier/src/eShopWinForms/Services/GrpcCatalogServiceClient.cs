using eShopWinForms.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace eShopWinForms.Services
{
    public class GrpcCatalogServiceClient : ICatalogService
    {
        private readonly GrpcChannel _channel;
        private readonly eShopGrpcService.Protos.CatalogService.CatalogServiceClient _client;

        public GrpcCatalogServiceClient()
        {
            var endpoint = ConfigurationManager.AppSettings["GrpcServiceUrl"]
                ?? "http://localhost:5001";
            _channel = GrpcChannel.ForAddress(endpoint);
            _client = new eShopGrpcService.Protos.CatalogService.CatalogServiceClient(_channel);
        }

        public GrpcCatalogServiceClient(string endpoint)
        {
            _channel = GrpcChannel.ForAddress(endpoint);
            _client = new eShopGrpcService.Protos.CatalogService.CatalogServiceClient(_channel);
        }

        public CatalogItem FindCatalogItem(int id)
        {
            var request = new eShopGrpcService.Protos.FindCatalogItemRequest { Id = id };
            var response = _client.FindCatalogItem(request);
            return MapCatalogItem(response.Item);
        }

        public List<CatalogBrand> GetCatalogBrands()
        {
            var response = _client.GetCatalogBrands(new Empty());
            return response.Brands.Select(MapCatalogBrand).ToList();
        }

        public List<CatalogItem> GetCatalogItems(int brandIdFilter, int typeIdFilter)
        {
            var request = new eShopGrpcService.Protos.GetCatalogItemsRequest();
            if (brandIdFilter != 0)
                request.BrandIdFilter = brandIdFilter;
            if (typeIdFilter != 0)
                request.TypeIdFilter = typeIdFilter;

            var response = _client.GetCatalogItems(request);
            return response.Items.Select(MapCatalogItem).ToList();
        }

        public List<CatalogType> GetCatalogTypes()
        {
            var response = _client.GetCatalogTypes(new Empty());
            return response.Types_.Select(MapCatalogType).ToList();
        }

        public int GetAvailableStock(DateTime date, int catalogItemId)
        {
            var request = new eShopGrpcService.Protos.GetAvailableStockRequest
            {
                Date = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc).ToTimestamp(),
                CatalogItemId = catalogItemId
            };
            var response = _client.GetAvailableStock(request);
            return response.AvailableStock;
        }

        public void CreateAvailableStock(CatalogItemsStock catalogItemsStock)
        {
            var request = new eShopGrpcService.Protos.CatalogItemsStockMessage
            {
                StockId = catalogItemsStock.StockId,
                CatalogItemId = catalogItemsStock.CatalogItemId,
                AvailableStock = catalogItemsStock.AvailableStock,
                Date = DateTime.SpecifyKind(catalogItemsStock.Date.Date, DateTimeKind.Utc).ToTimestamp()
            };
            _client.CreateAvailableStock(request);
        }

        public void CreateCatalogItem(CatalogItem catalogItem)
        {
            _client.CreateCatalogItem(MapToMessage(catalogItem));
        }

        public void UpdateCatalogItem(CatalogItem catalogItem)
        {
            _client.UpdateCatalogItem(MapToMessage(catalogItem));
        }

        public void RemoveCatalogItem(CatalogItem catalogItem)
        {
            _client.RemoveCatalogItem(MapToMessage(catalogItem));
        }

        public DiscountItem GetDiscount(DateTime day)
        {
            var request = new eShopGrpcService.Protos.GetDiscountRequest
            {
                Day = DateTime.SpecifyKind(day.Date, DateTimeKind.Utc).ToTimestamp()
            };
            var response = _client.GetDiscount(request);
            return response.Discount != null ? MapDiscountItem(response.Discount) : null;
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }

        private static CatalogItem MapCatalogItem(eShopGrpcService.Protos.CatalogItemMessage msg)
        {
            if (msg == null) return null;
            return new CatalogItem
            {
                Id = msg.Id,
                Description = msg.Description,
                Name = msg.Name,
                Price = decimal.TryParse(msg.Price, NumberStyles.Number, CultureInfo.InvariantCulture, out var p) ? p : 0m,
                Picturefilename = msg.Picturefilename,
                CatalogBrandId = msg.CatalogBrandId,
                CatalogTypeId = msg.CatalogTypeId,
                CatalogType = msg.CatalogType != null ? MapCatalogType(msg.CatalogType) : null,
                CatalogBrand = msg.CatalogBrand != null ? MapCatalogBrand(msg.CatalogBrand) : null
            };
        }

        private static CatalogBrand MapCatalogBrand(eShopGrpcService.Protos.CatalogBrandMessage msg)
        {
            return new CatalogBrand { Id = msg.Id, Brand = msg.Brand };
        }

        private static CatalogType MapCatalogType(eShopGrpcService.Protos.CatalogTypeMessage msg)
        {
            return new CatalogType { Id = msg.Id, Type = msg.Type };
        }

        private static DiscountItem MapDiscountItem(eShopGrpcService.Protos.DiscountItemMessage msg)
        {
            return new DiscountItem
            {
                Id = msg.Id,
                Size = msg.Size,
                Start = msg.Start?.ToDateTime() ?? DateTime.MinValue,
                End = msg.End?.ToDateTime() ?? DateTime.MinValue
            };
        }

        private static eShopGrpcService.Protos.CatalogItemMessage MapToMessage(CatalogItem item)
        {
            return new eShopGrpcService.Protos.CatalogItemMessage
            {
                Id = item.Id,
                Description = item.Description ?? string.Empty,
                Name = item.Name ?? string.Empty,
                Price = item.Price.ToString("G", CultureInfo.InvariantCulture),
                Picturefilename = item.Picturefilename ?? string.Empty,
                CatalogBrandId = item.CatalogBrandId,
                CatalogTypeId = item.CatalogTypeId
            };
        }
    }
}
