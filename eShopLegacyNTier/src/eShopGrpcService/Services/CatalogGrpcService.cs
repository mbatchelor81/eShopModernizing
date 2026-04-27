using System.Globalization;
using eShopGrpcService.Data;
using eShopGrpcService.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace eShopGrpcService.Services;

public class CatalogGrpcService : eShopGrpcService.CatalogService.CatalogServiceBase
{
    private readonly CatalogDbContext _db;

    public CatalogGrpcService(CatalogDbContext db)
    {
        _db = db;
    }

    public override async Task<CatalogItemReply> FindCatalogItem(
        FindCatalogItemRequest request, ServerCallContext context)
    {
        var item = await _db.CatalogItems
            .Include(x => x.CatalogBrand)
            .Include(x => x.CatalogType)
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (item == null)
            return new CatalogItemReply { Found = false };

        return new CatalogItemReply
        {
            Found = true,
            Item = CatalogMapper.ToMessage(item)
        };
    }

    public override async Task<CatalogBrandListReply> GetCatalogBrands(
        Empty request, ServerCallContext context)
    {
        var brands = await _db.CatalogBrands.ToListAsync();
        var reply = new CatalogBrandListReply();
        reply.Brands.AddRange(brands.Select(CatalogMapper.ToMessage));
        return reply;
    }

    public override async Task<CatalogItemListReply> GetCatalogItems(
        GetCatalogItemsRequest request, ServerCallContext context)
    {
        bool brandFilterIsNull = request.BrandIdFilter == 0;
        bool typeFilterIsNull = request.TypeIdFilter == 0;

        var query = _db.CatalogItems.AsQueryable();

        if (!brandFilterIsNull)
            query = query.Where(x => x.CatalogBrandId == request.BrandIdFilter);

        if (!typeFilterIsNull)
            query = query.Where(x => x.CatalogTypeId == request.TypeIdFilter);

        var items = await query.ToListAsync();

        var reply = new CatalogItemListReply();
        reply.Items.AddRange(items.Select(CatalogMapper.ToMessage));
        return reply;
    }

    public override async Task<CatalogTypeListReply> GetCatalogTypes(
        Empty request, ServerCallContext context)
    {
        var types = await _db.CatalogTypes.ToListAsync();
        var reply = new CatalogTypeListReply();
        reply.CatalogTypes.AddRange(types.Select(CatalogMapper.ToMessage));
        return reply;
    }

    public override async Task<AvailableStockReply> GetAvailableStock(
        GetAvailableStockRequest request, ServerCallContext context)
    {
        var date = CatalogMapper.ToUtcDateTime(request.Date).Date;

        var stock = await _db.CatalogItemsStocks
            .Where(x => x.CatalogItemId == request.CatalogItemId && x.Date.Date == date)
            .FirstOrDefaultAsync();

        return new AvailableStockReply
        {
            AvailableStock = stock?.AvailableStock ?? 0
        };
    }

    public override async Task<Empty> CreateAvailableStock(
        CatalogItemsStockMessage request, ServerCallContext context)
    {
        const int maxRetries = 3;
        var ct = context.CancellationToken;
        var date = CatalogMapper.ToUtcDateTime(request.Date).Date;

        var existing = await _db.CatalogItemsStocks
            .Where(x => x.CatalogItemId == request.CatalogItemId && x.Date.Date == date)
            .FirstOrDefaultAsync(ct);

        if (existing != null)
        {
            existing.AvailableStock = request.AvailableStock;
            _db.Entry(existing).State = EntityState.Modified;
            await _db.SaveChangesAsync(ct);
        }
        else
        {
            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                ct.ThrowIfCancellationRequested();
                var maxId = await _db.CatalogItemsStocks.AnyAsync(ct)
                    ? await _db.CatalogItemsStocks.MaxAsync(i => i.StockId, ct)
                    : 0;

                var stock = new CatalogItemsStock
                {
                    StockId = maxId + 1,
                    Date = date,
                    CatalogItemId = request.CatalogItemId,
                    AvailableStock = request.AvailableStock,
                };
                _db.CatalogItemsStocks.Add(stock);
                try
                {
                    await _db.SaveChangesAsync(ct);
                    return new Empty();
                }
                catch (DbUpdateException) when (attempt < maxRetries - 1)
                {
                    _db.Entry(stock).State = EntityState.Detached;
                }
            }
        }
        return new Empty();
    }

    public override async Task<Empty> CreateCatalogItem(
        CatalogItemMessage request, ServerCallContext context)
    {
        const int maxRetries = 3;
        var ct = context.CancellationToken;
        var item = CatalogMapper.ToEntity(request);

        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            ct.ThrowIfCancellationRequested();
            var maxId = await _db.CatalogItems.AnyAsync(ct)
                ? await _db.CatalogItems.MaxAsync(i => i.Id, ct)
                : 0;
            item.Id = maxId + 1;

            _db.CatalogItems.Add(item);
            try
            {
                await _db.SaveChangesAsync(ct);
                return new Empty();
            }
            catch (DbUpdateException) when (attempt < maxRetries - 1)
            {
                _db.Entry(item).State = EntityState.Detached;
            }
        }

        return new Empty();
    }

    public override async Task<Empty> UpdateCatalogItem(
        CatalogItemMessage request, ServerCallContext context)
    {
        var existing = await _db.CatalogItems.FindAsync(request.Id);
        if (existing != null)
        {
            existing.Description = request.Description;
            existing.Name = request.Name;
            existing.Price = decimal.Parse(request.Price, CultureInfo.InvariantCulture);
            existing.Picturefilename = request.Picturefilename;
            existing.CatalogBrandId = request.CatalogBrandId;
            existing.CatalogTypeId = request.CatalogTypeId;
            await _db.SaveChangesAsync();
        }
        return new Empty();
    }

    public override async Task<Empty> RemoveCatalogItem(
        CatalogItemMessage request, ServerCallContext context)
    {
        var existing = await _db.CatalogItems.FindAsync(request.Id);
        if (existing != null)
        {
            _db.CatalogItems.Remove(existing);
            await _db.SaveChangesAsync();
        }
        return new Empty();
    }

    public override async Task<DiscountItemReply> GetDiscount(
        GetDiscountRequest request, ServerCallContext context)
    {
        var day = CatalogMapper.ToUtcDateTime(request.Day).Date;

        var discount = await _db.DiscountItems
            .Where(y => y.Start.Date <= day && y.End.Date >= day)
            .FirstOrDefaultAsync();

        if (discount == null)
            return new DiscountItemReply { Found = false };

        return new DiscountItemReply
        {
            Found = true,
            Id = discount.Id,
            Size = discount.Size,
            Start = CatalogMapper.ToTimestamp(discount.Start),
            End = CatalogMapper.ToTimestamp(discount.End),
        };
    }
}
