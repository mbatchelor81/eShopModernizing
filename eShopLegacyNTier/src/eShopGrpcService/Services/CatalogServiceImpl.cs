using eShopGrpcService.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace eShopGrpcService.Services;

public class CatalogServiceImpl : CatalogGrpc.CatalogGrpcBase
{
    private readonly CatalogDbContext _db;

    public CatalogServiceImpl(CatalogDbContext db)
    {
        _db = db;
    }

    public override async Task<FindCatalogItemResponse> FindCatalogItem(FindCatalogItemRequest request, ServerCallContext context)
    {
        var item = await _db.CatalogItems
            .Include(x => x.CatalogBrand)
            .Include(x => x.CatalogType)
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (item == null)
        {
            return new FindCatalogItemResponse { Found = false };
        }

        return new FindCatalogItemResponse
        {
            Found = true,
            CatalogItem = MapToMessage(item)
        };
    }

    public override async Task<CatalogBrandListResponse> GetCatalogBrands(Empty request, ServerCallContext context)
    {
        var brands = await _db.CatalogBrands.ToListAsync();
        var response = new CatalogBrandListResponse();
        foreach (var brand in brands)
        {
            response.CatalogBrands.Add(new CatalogBrandMessage
            {
                Id = brand.Id,
                Brand = brand.Brand ?? string.Empty
            });
        }
        return response;
    }

    public override async Task<CatalogItemListResponse> GetCatalogItems(GetCatalogItemsRequest request, ServerCallContext context)
    {
        var items = await _db.CatalogItems.ToListAsync();

        bool brandFilterIsNull = request.BrandIdFilter == 0;
        bool typeFilterIsNull = request.TypeIdFilter == 0;

        var filtered = items.Where(x =>
            (brandFilterIsNull || x.CatalogBrandId == request.BrandIdFilter) &&
            (typeFilterIsNull || x.CatalogTypeId == request.TypeIdFilter)).ToList();

        var response = new CatalogItemListResponse();
        foreach (var item in filtered)
        {
            response.CatalogItems.Add(MapToMessage(item));
        }
        return response;
    }

    public override async Task<CatalogTypeListResponse> GetCatalogTypes(Empty request, ServerCallContext context)
    {
        var types = await _db.CatalogTypes.ToListAsync();
        var response = new CatalogTypeListResponse();
        foreach (var type in types)
        {
            response.CatalogTypes.Add(new CatalogTypeMessage
            {
                Id = type.Id,
                Type = type.Type ?? string.Empty
            });
        }
        return response;
    }

    public override async Task<GetAvailableStockResponse> GetAvailableStock(GetAvailableStockRequest request, ServerCallContext context)
    {
        var date = request.Date.ToDateTime().Date;
        var stocks = await _db.CatalogItemsStocks
            .Where(x => x.CatalogItemId == request.CatalogItemId)
            .ToListAsync();

        var s = stocks.FirstOrDefault(y => y.Date.Date == date);

        return new GetAvailableStockResponse
        {
            AvailableStock = s?.AvailableStock ?? 0
        };
    }

    public override async Task<Empty> CreateAvailableStock(CreateAvailableStockRequest request, ServerCallContext context)
    {
        var date = request.Date.ToDateTime().Date;
        var stocks = await _db.CatalogItemsStocks
            .Where(x => x.CatalogItemId == request.CatalogItemId)
            .ToListAsync();

        var s = stocks.FirstOrDefault(y => y.Date.Date == date);

        if (s != null)
        {
            s.AvailableStock = request.AvailableStock;
            _db.Entry(s).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
        else
        {
            var maxId = await _db.CatalogItemsStocks.MaxAsync(i => i.StockId);
            var stock = new CatalogItemsStock
            {
                StockId = maxId + 1,
                CatalogItemId = request.CatalogItemId,
                Date = date,
                AvailableStock = request.AvailableStock
            };
            _db.CatalogItemsStocks.Add(stock);
            await _db.SaveChangesAsync();
        }

        return new Empty();
    }

    public override async Task<Empty> CreateCatalogItem(CatalogItemMessage request, ServerCallContext context)
    {
        var maxId = await _db.CatalogItems.MaxAsync(i => i.Id);
        var item = new CatalogItem
        {
            Id = maxId + 1,
            Name = request.Name,
            Description = request.Description,
            Price = (decimal)request.Price,
            Picturefilename = request.PictureFilename,
            CatalogBrandId = request.CatalogBrandId,
            CatalogTypeId = request.CatalogTypeId
        };
        _db.CatalogItems.Add(item);
        await _db.SaveChangesAsync();

        return new Empty();
    }

    public override async Task<Empty> UpdateCatalogItem(CatalogItemMessage request, ServerCallContext context)
    {
        var item = await _db.CatalogItems.FirstOrDefaultAsync(x => x.Id == request.Id);
        if (item == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"CatalogItem with id {request.Id} not found."));
        }

        item.Name = request.Name;
        item.Description = request.Description;
        item.Price = (decimal)request.Price;
        item.Picturefilename = request.PictureFilename;
        item.CatalogBrandId = request.CatalogBrandId;
        item.CatalogTypeId = request.CatalogTypeId;

        _db.Entry(item).State = EntityState.Modified;
        await _db.SaveChangesAsync();

        return new Empty();
    }

    public override async Task<Empty> RemoveCatalogItem(CatalogItemMessage request, ServerCallContext context)
    {
        var item = await _db.CatalogItems.FirstOrDefaultAsync(x => x.Id == request.Id);
        if (item == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"CatalogItem with id {request.Id} not found."));
        }

        _db.CatalogItems.Remove(item);
        await _db.SaveChangesAsync();

        return new Empty();
    }

    public override async Task<GetDiscountResponse> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var day = request.Day.ToDateTime().Date;
        var discounts = await _db.DiscountItems.ToListAsync();
        var discount = discounts
            .Where(y => y.Start.Date <= day && y.End.Date >= day)
            .FirstOrDefault();

        if (discount == null)
        {
            return new GetDiscountResponse { Found = false };
        }

        return new GetDiscountResponse
        {
            Found = true,
            DiscountItem = new DiscountItemMessage
            {
                Id = discount.Id,
                Size = discount.Size,
                Start = Timestamp.FromDateTime(DateTime.SpecifyKind(discount.Start, DateTimeKind.Utc)),
                End = Timestamp.FromDateTime(DateTime.SpecifyKind(discount.End, DateTimeKind.Utc))
            }
        };
    }

    private static CatalogItemMessage MapToMessage(CatalogItem item)
    {
        var message = new CatalogItemMessage
        {
            Id = item.Id,
            Name = item.Name ?? string.Empty,
            Description = item.Description ?? string.Empty,
            Price = (double)item.Price,
            PictureFilename = item.Picturefilename ?? string.Empty,
            CatalogBrandId = item.CatalogBrandId,
            CatalogTypeId = item.CatalogTypeId
        };

        if (item.CatalogBrand != null)
        {
            message.CatalogBrand = new CatalogBrandMessage
            {
                Id = item.CatalogBrand.Id,
                Brand = item.CatalogBrand.Brand ?? string.Empty
            };
        }

        if (item.CatalogType != null)
        {
            message.CatalogType = new CatalogTypeMessage
            {
                Id = item.CatalogType.Id,
                Type = item.CatalogType.Type ?? string.Empty
            };
        }

        return message;
    }
}
