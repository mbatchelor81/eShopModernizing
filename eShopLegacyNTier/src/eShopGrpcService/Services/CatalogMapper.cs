using eShopGrpcService.Models;
using Google.Protobuf.WellKnownTypes;

namespace eShopGrpcService.Services;

public static class CatalogMapper
{
    public static CatalogItemMessage ToMessage(CatalogItem item)
    {
        var msg = new CatalogItemMessage
        {
            Id = item.Id,
            Description = item.Description ?? string.Empty,
            Name = item.Name ?? string.Empty,
            Price = item.Price.ToString(),
            Picturefilename = item.Picturefilename ?? string.Empty,
            CatalogBrandId = item.CatalogBrandId,
            CatalogTypeId = item.CatalogTypeId,
        };

        if (item.CatalogBrand != null)
            msg.CatalogBrand = ToMessage(item.CatalogBrand);

        if (item.CatalogType != null)
            msg.CatalogType = ToMessage(item.CatalogType);

        return msg;
    }

    public static CatalogBrandMessage ToMessage(CatalogBrand brand)
    {
        return new CatalogBrandMessage
        {
            Id = brand.Id,
            Brand = brand.Brand ?? string.Empty
        };
    }

    public static CatalogTypeMessage ToMessage(CatalogType type)
    {
        return new CatalogTypeMessage
        {
            Id = type.Id,
            Type = type.Type ?? string.Empty
        };
    }

    public static CatalogItem ToEntity(CatalogItemMessage msg)
    {
        return new CatalogItem
        {
            Id = msg.Id,
            Description = msg.Description,
            Name = msg.Name,
            Price = decimal.Parse(msg.Price),
            Picturefilename = msg.Picturefilename,
            CatalogBrandId = msg.CatalogBrandId,
            CatalogTypeId = msg.CatalogTypeId,
        };
    }

    public static CatalogItemsStock ToEntity(CatalogItemsStockMessage msg)
    {
        return new CatalogItemsStock
        {
            StockId = msg.StockId,
            Date = msg.Date.ToDateTime(),
            CatalogItemId = msg.CatalogItemId,
            AvailableStock = msg.AvailableStock,
        };
    }

    public static DateTime ToUtcDateTime(Timestamp timestamp)
    {
        return timestamp.ToDateTime();
    }

    public static Timestamp ToTimestamp(DateTime dateTime)
    {
        return Timestamp.FromDateTime(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc));
    }
}
