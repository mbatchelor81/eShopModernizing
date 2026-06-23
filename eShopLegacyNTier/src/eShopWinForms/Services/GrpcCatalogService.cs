using System;
using System.Collections.Generic;
using System.Linq;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using eShopGrpcService;

namespace eShopWinForms.Services
{
    public class GrpcCatalogService : IDisposable
    {
        private readonly Channel _channel;
        private readonly CatalogService.CatalogServiceClient _client;

        public GrpcCatalogService(string grpcAddress = "localhost", int port = 5001)
        {
            _channel = new Channel(grpcAddress, port, ChannelCredentials.Insecure);
            _client = new CatalogService.CatalogServiceClient(_channel);
        }

        public CatalogItemDto FindCatalogItem(int id)
        {
            var response = _client.FindCatalogItem(
                new FindCatalogItemRequest { Id = id });
            return MapToCatalogItemDto(response);
        }

        public List<CatalogBrandDto> GetCatalogBrands()
        {
            var response = _client.GetCatalogBrands(new Empty());
            return response.Brands.Select(b => new CatalogBrandDto
            {
                Id = b.Id,
                Brand = b.Brand
            }).ToList();
        }

        public List<CatalogItemDto> GetCatalogItems(int brandIdFilter, int typeIdFilter)
        {
            var response = _client.GetCatalogItems(
                new GetCatalogItemsRequest
                {
                    BrandIdFilter = brandIdFilter,
                    TypeIdFilter = typeIdFilter
                });
            return response.Items.Select(MapToCatalogItemDto).ToList();
        }

        public List<CatalogTypeDto> GetCatalogTypes()
        {
            var response = _client.GetCatalogTypes(new Empty());
            return response.Types.Select(t => new CatalogTypeDto
            {
                Id = t.Id,
                Type = t.Type
            }).ToList();
        }

        public int GetAvailableStock(DateTime date, int catalogItemId)
        {
            var response = _client.GetAvailableStock(
                new GetAvailableStockRequest
                {
                    Date = Timestamp.FromDateTime(DateTime.SpecifyKind(date.Date, DateTimeKind.Utc)),
                    CatalogItemId = catalogItemId
                });
            return response.AvailableStock;
        }

        public void CreateAvailableStock(int catalogItemId, int availableStock, DateTime date)
        {
            _client.CreateAvailableStock(
                new CreateAvailableStockRequest
                {
                    CatalogItemsStock = new CatalogItemsStockMsg
                    {
                        CatalogItemId = catalogItemId,
                        AvailableStock = availableStock,
                        Date = Timestamp.FromDateTime(DateTime.SpecifyKind(date.Date, DateTimeKind.Utc))
                    }
                });
        }

        public void CreateCatalogItem(CatalogItemDto catalogItem)
        {
            _client.CreateCatalogItem(
                new CreateCatalogItemRequest
                {
                    CatalogItem = MapToProto(catalogItem)
                });
        }

        public void UpdateCatalogItem(CatalogItemDto catalogItem)
        {
            _client.UpdateCatalogItem(
                new UpdateCatalogItemRequest
                {
                    CatalogItem = MapToProto(catalogItem)
                });
        }

        public void RemoveCatalogItem(CatalogItemDto catalogItem)
        {
            _client.RemoveCatalogItem(
                new RemoveCatalogItemRequest
                {
                    CatalogItem = MapToProto(catalogItem)
                });
        }

        public DiscountItemDto GetDiscount(DateTime day)
        {
            try
            {
                var response = _client.GetDiscount(
                    new GetDiscountRequest
                    {
                        Day = Timestamp.FromDateTime(DateTime.SpecifyKind(day.Date, DateTimeKind.Utc))
                    });
                return new DiscountItemDto
                {
                    Id = response.Id,
                    Size = response.Size,
                    Start = response.Start.ToDateTime(),
                    End = response.End.ToDateTime()
                };
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                return null;
            }
        }

        private static CatalogItemDto MapToCatalogItemDto(CatalogItemMsg msg)
        {
            return new CatalogItemDto
            {
                Id = msg.Id,
                Description = msg.Description,
                Name = msg.Name,
                Price = decimal.TryParse(msg.Price, out var p) ? p : 0,
                Picturefilename = msg.Picturefilename,
                CatalogBrandId = msg.CatalogBrandId,
                CatalogTypeId = msg.CatalogTypeId,
                CatalogBrand = msg.CatalogBrand != null ? new CatalogBrandDto { Id = msg.CatalogBrand.Id, Brand = msg.CatalogBrand.Brand } : null,
                CatalogType = msg.CatalogType != null ? new CatalogTypeDto { Id = msg.CatalogType.Id, Type = msg.CatalogType.Type } : null
            };
        }

        private static CatalogItemMsg MapToProto(CatalogItemDto dto)
        {
            return new CatalogItemMsg
            {
                Id = dto.Id,
                Description = dto.Description ?? "",
                Name = dto.Name ?? "",
                Price = dto.Price.ToString(),
                Picturefilename = dto.Picturefilename ?? "",
                CatalogBrandId = dto.CatalogBrandId,
                CatalogTypeId = dto.CatalogTypeId
            };
        }

        public void Dispose()
        {
            _channel?.ShutdownAsync().Wait();
        }
    }

    // DTOs to replace the WCF-generated types
    public class CatalogItemDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Picturefilename { get; set; }
        public int CatalogBrandId { get; set; }
        public int CatalogTypeId { get; set; }
        public CatalogTypeDto CatalogType { get; set; }
        public CatalogBrandDto CatalogBrand { get; set; }
    }

    public class CatalogBrandDto
    {
        public int Id { get; set; }
        public string Brand { get; set; }
    }

    public class CatalogTypeDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }

    public class CatalogItemsStockDto
    {
        public int StockId { get; set; }
        public DateTime Date { get; set; }
        public int CatalogItemId { get; set; }
        public int AvailableStock { get; set; }
    }

    public class DiscountItemDto
    {
        public int Id { get; set; }
        public double Size { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
