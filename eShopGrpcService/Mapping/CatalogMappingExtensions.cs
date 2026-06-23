using Google.Protobuf.WellKnownTypes;
using eShopGrpcService.Models;

namespace eShopGrpcService.Mapping;

public static class CatalogMappingExtensions
{
    public static CatalogItemMsg ToProto(this CatalogItem entity)
    {
        var msg = new CatalogItemMsg
        {
            Id = entity.Id,
            Description = entity.Description ?? "",
            Name = entity.Name ?? "",
            Price = entity.Price.ToString(),
            Picturefilename = entity.Picturefilename ?? "",
            CatalogBrandId = entity.CatalogBrandId,
            CatalogTypeId = entity.CatalogTypeId
        };
        if (entity.CatalogType != null)
            msg.CatalogType = entity.CatalogType.ToProto();
        if (entity.CatalogBrand != null)
            msg.CatalogBrand = entity.CatalogBrand.ToProto();
        return msg;
    }

    public static CatalogItem ToEntity(this CatalogItemMsg msg)
    {
        return new CatalogItem
        {
            Id = msg.Id,
            Description = msg.Description,
            Name = msg.Name,
            Price = decimal.TryParse(msg.Price, out var p) ? p : 0,
            Picturefilename = msg.Picturefilename,
            CatalogBrandId = msg.CatalogBrandId,
            CatalogTypeId = msg.CatalogTypeId
        };
    }

    public static CatalogBrandMsg ToProto(this CatalogBrand entity)
    {
        return new CatalogBrandMsg { Id = entity.Id, Brand = entity.Brand ?? "" };
    }

    public static CatalogBrand ToEntity(this CatalogBrandMsg msg)
    {
        return new CatalogBrand { Id = msg.Id, Brand = msg.Brand };
    }

    public static CatalogTypeMsg ToProto(this CatalogType entity)
    {
        return new CatalogTypeMsg { Id = entity.Id, Type = entity.Type ?? "" };
    }

    public static CatalogType ToEntity(this CatalogTypeMsg msg)
    {
        return new CatalogType { Id = msg.Id, Type = msg.Type };
    }

    public static CatalogItemsStockMsg ToProto(this CatalogItemsStock entity)
    {
        return new CatalogItemsStockMsg
        {
            StockId = entity.StockId,
            Date = Timestamp.FromDateTime(DateTime.SpecifyKind(entity.Date, DateTimeKind.Utc)),
            CatalogItemId = entity.CatalogItemId,
            AvailableStock = entity.AvailableStock
        };
    }

    public static CatalogItemsStock ToEntity(this CatalogItemsStockMsg msg)
    {
        return new CatalogItemsStock
        {
            StockId = msg.StockId,
            Date = msg.Date.ToDateTime().Date,
            CatalogItemId = msg.CatalogItemId,
            AvailableStock = msg.AvailableStock
        };
    }

    public static DiscountItemMsg ToProto(this DiscountItem entity)
    {
        return new DiscountItemMsg
        {
            Id = entity.Id,
            Size = entity.Size,
            Start = Timestamp.FromDateTime(DateTime.SpecifyKind(entity.Start, DateTimeKind.Utc)),
            End = Timestamp.FromDateTime(DateTime.SpecifyKind(entity.End, DateTimeKind.Utc))
        };
    }

    public static DiscountItem ToEntity(this DiscountItemMsg msg)
    {
        return new DiscountItem
        {
            Id = msg.Id,
            Size = msg.Size,
            Start = msg.Start.ToDateTime().Date,
            End = msg.End.ToDateTime().Date
        };
    }
}
