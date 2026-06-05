using eShopGrpcService.Models;
using eShopGrpcService.Protos;
using Google.Protobuf.WellKnownTypes;

namespace eShopGrpcService.Mapping;

public static class ProtoMappingExtensions
{
    // CatalogBrand -> CatalogBrandMessage
    public static CatalogBrandMessage ToMessage(this CatalogBrand entity)
    {
        return new CatalogBrandMessage
        {
            Id = entity.Id,
            Brand = entity.Brand ?? string.Empty
        };
    }

    // CatalogType -> CatalogTypeMessage
    public static CatalogTypeMessage ToMessage(this CatalogType entity)
    {
        return new CatalogTypeMessage
        {
            Id = entity.Id,
            Type = entity.Type ?? string.Empty
        };
    }

    // CatalogItem -> CatalogItemMessage
    public static CatalogItemMessage ToMessage(this CatalogItem entity)
    {
        var msg = new CatalogItemMessage
        {
            Id = entity.Id,
            Description = entity.Description ?? string.Empty,
            Name = entity.Name ?? string.Empty,
            Price = entity.Price.ToString("G"),
            Picturefilename = entity.Picturefilename ?? string.Empty,
            CatalogBrandId = entity.CatalogBrandId,
            CatalogTypeId = entity.CatalogTypeId
        };

        if (entity.CatalogType != null)
            msg.CatalogType = entity.CatalogType.ToMessage();

        if (entity.CatalogBrand != null)
            msg.CatalogBrand = entity.CatalogBrand.ToMessage();

        return msg;
    }

    // CatalogItemMessage -> CatalogItem
    public static CatalogItem ToEntity(this CatalogItemMessage msg)
    {
        return new CatalogItem
        {
            Id = msg.Id,
            Description = msg.Description,
            Name = msg.Name,
            Price = decimal.TryParse(msg.Price, out var p) ? p : 0m,
            Picturefilename = msg.Picturefilename,
            CatalogBrandId = msg.CatalogBrandId,
            CatalogTypeId = msg.CatalogTypeId
        };
    }

    // CatalogItemsStock -> CatalogItemsStockMessage
    public static CatalogItemsStockMessage ToMessage(this CatalogItemsStock entity)
    {
        return new CatalogItemsStockMessage
        {
            StockId = entity.StockId,
            CatalogItemId = entity.CatalogItemId,
            AvailableStock = entity.AvailableStock,
            Date = entity.Date.ToUniversalTime().ToTimestamp()
        };
    }

    // CatalogItemsStockMessage -> CatalogItemsStock
    public static CatalogItemsStock ToEntity(this CatalogItemsStockMessage msg)
    {
        return new CatalogItemsStock
        {
            StockId = msg.StockId,
            CatalogItemId = msg.CatalogItemId,
            AvailableStock = msg.AvailableStock,
            Date = msg.Date?.ToDateTime() ?? DateTime.UtcNow
        };
    }

    // DiscountItem -> DiscountItemMessage
    public static DiscountItemMessage ToMessage(this DiscountItem entity)
    {
        return new DiscountItemMessage
        {
            Id = entity.Id,
            Size = entity.Size,
            Start = entity.Start.ToUniversalTime().ToTimestamp(),
            End = entity.End.ToUniversalTime().ToTimestamp()
        };
    }

    // DiscountItemMessage -> DiscountItem
    public static DiscountItem ToEntity(this DiscountItemMessage msg)
    {
        return new DiscountItem
        {
            Id = msg.Id,
            Size = msg.Size,
            Start = msg.Start?.ToDateTime() ?? DateTime.UtcNow,
            End = msg.End?.ToDateTime() ?? DateTime.UtcNow
        };
    }
}
