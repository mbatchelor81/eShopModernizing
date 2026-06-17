using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using eShopGrpcService;
using eShopWinForms.eShopServiceReference;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;

namespace eShopWinForms.GrpcClient
{
    public class CatalogGrpcClientWrapper : ICatalogService
    {
        private readonly CatalogService.CatalogServiceClient _client;
        private readonly GrpcChannel _channel;

        public CatalogGrpcClientWrapper()
        {
            var endpoint = ConfigurationManager.AppSettings["GrpcEndpoint"] ?? "http://localhost:5001";
            _channel = GrpcChannel.ForAddress(endpoint);
            _client = new CatalogService.CatalogServiceClient(_channel);
        }

        public CatalogItem FindCatalogItem(int id)
        {
            var reply = _client.FindCatalogItem(new FindCatalogItemRequest { Id = id });
            if (!reply.Found || reply.Item == null)
                return null;
            return ToCatalogItem(reply.Item);
        }

        public List<CatalogBrand> GetCatalogBrands()
        {
            var reply = _client.GetCatalogBrands(new Empty());
            return reply.Brands.Select(ToCatalogBrand).ToList();
        }

        public List<CatalogItem> GetCatalogItems(int brandIdFilter, int typeIdFilter)
        {
            var reply = _client.GetCatalogItems(new GetCatalogItemsRequest
            {
                BrandIdFilter = brandIdFilter,
                TypeIdFilter = typeIdFilter
            });
            return reply.Items.Select(ToCatalogItem).ToList();
        }

        public List<CatalogType> GetCatalogTypes()
        {
            var reply = _client.GetCatalogTypes(new Empty());
            return reply.CatalogTypes.Select(ToCatalogType).ToList();
        }

        public int GetAvailableStock(DateTime date, int catalogItemId)
        {
            var reply = _client.GetAvailableStock(new GetAvailableStockRequest
            {
                Date = Timestamp.FromDateTime(DateTime.SpecifyKind(date.Date, DateTimeKind.Utc)),
                CatalogItemId = catalogItemId
            });
            return reply.AvailableStock;
        }

        public void CreateAvailableStock(CatalogItemsStock catalogItemsStock)
        {
            _client.CreateAvailableStock(new CatalogItemsStockMessage
            {
                StockId = catalogItemsStock.StockId,
                Date = Timestamp.FromDateTime(DateTime.SpecifyKind(catalogItemsStock.Date.Date, DateTimeKind.Utc)),
                CatalogItemId = catalogItemsStock.CatalogItemId,
                AvailableStock = catalogItemsStock.AvailableStock
            });
        }

        public void CreateCatalogItem(CatalogItem catalogItem)
        {
            _client.CreateCatalogItem(ToCatalogItemMessage(catalogItem));
        }

        public void UpdateCatalogItem(CatalogItem catalogItem)
        {
            _client.UpdateCatalogItem(ToCatalogItemMessage(catalogItem));
        }

        public void RemoveCatalogItem(CatalogItem catalogItem)
        {
            _client.RemoveCatalogItem(ToCatalogItemMessage(catalogItem));
        }

        public DiscountItem GetDiscount(DateTime day)
        {
            var reply = _client.GetDiscount(new GetDiscountRequest
            {
                Day = Timestamp.FromDateTime(DateTime.SpecifyKind(day.Date, DateTimeKind.Utc))
            });
            if (!reply.Found)
                return null;
            return new DiscountItem
            {
                Id = reply.Id,
                Size = reply.Size,
                Start = reply.Start.ToDateTime(),
                End = reply.End.ToDateTime()
            };
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }

        private static CatalogItem ToCatalogItem(CatalogItemMessage msg)
        {
            return new CatalogItem
            {
                Id = msg.Id,
                Description = msg.Description,
                Name = msg.Name,
                Price = decimal.Parse(msg.Price, CultureInfo.InvariantCulture),
                Picturefilename = msg.Picturefilename,
                CatalogBrandId = msg.CatalogBrandId,
                CatalogTypeId = msg.CatalogTypeId,
                CatalogBrand = msg.CatalogBrand != null ? ToCatalogBrand(msg.CatalogBrand) : null,
                CatalogType = msg.CatalogType != null ? ToCatalogType(msg.CatalogType) : null
            };
        }

        private static CatalogBrand ToCatalogBrand(CatalogBrandMessage msg)
        {
            return new CatalogBrand
            {
                Id = msg.Id,
                Brand = msg.Brand
            };
        }

        private static CatalogType ToCatalogType(CatalogTypeMessage msg)
        {
            return new CatalogType
            {
                Id = msg.Id,
                Type = msg.Type
            };
        }

        private static CatalogItemMessage ToCatalogItemMessage(CatalogItem item)
        {
            return new CatalogItemMessage
            {
                Id = item.Id,
                Description = item.Description ?? string.Empty,
                Name = item.Name ?? string.Empty,
                Price = item.Price.ToString(CultureInfo.InvariantCulture),
                Picturefilename = item.Picturefilename ?? string.Empty,
                CatalogBrandId = item.CatalogBrandId,
                CatalogTypeId = item.CatalogTypeId
            };
        }
    }
}
