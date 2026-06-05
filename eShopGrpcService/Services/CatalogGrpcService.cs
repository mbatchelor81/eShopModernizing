using eShopGrpcService.Data;
using eShopGrpcService.Mapping;
using eShopGrpcService.Models;
using eShopGrpcService.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace eShopGrpcService.Services;

public class CatalogGrpcService : Protos.CatalogService.CatalogServiceBase
{
    private readonly CatalogDbContext _db;

    public CatalogGrpcService(CatalogDbContext db)
    {
        _db = db;
    }

    public override async Task<CatalogItemResponse> FindCatalogItem(
        FindCatalogItemRequest request, ServerCallContext context)
    {
        var item = await _db.CatalogItems
            .Include(x => x.CatalogBrand)
            .Include(x => x.CatalogType)
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (item == null)
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Catalog item with id {request.Id} not found"));

        return new CatalogItemResponse { Item = item.ToMessage() };
    }

    public override async Task<CatalogBrandsResponse> GetCatalogBrands(
        Empty request, ServerCallContext context)
    {
        var brands = await _db.CatalogBrands.ToListAsync();
        var response = new CatalogBrandsResponse();
        response.Brands.AddRange(brands.Select(b => b.ToMessage()));
        return response;
    }

    public override async Task<CatalogItemsResponse> GetCatalogItems(
        GetCatalogItemsRequest request, ServerCallContext context)
    {
        IQueryable<CatalogItem> query = _db.CatalogItems;

        if (request.BrandIdFilter != null && request.BrandIdFilter.Value != 0)
            query = query.Where(x => x.CatalogBrandId == request.BrandIdFilter.Value);

        if (request.TypeIdFilter != null && request.TypeIdFilter.Value != 0)
            query = query.Where(x => x.CatalogTypeId == request.TypeIdFilter.Value);

        var items = await query.ToListAsync();
        var response = new CatalogItemsResponse();
        response.Items.AddRange(items.Select(i => i.ToMessage()));
        return response;
    }

    public override async Task<CatalogTypesResponse> GetCatalogTypes(
        Empty request, ServerCallContext context)
    {
        var types = await _db.CatalogTypes.ToListAsync();
        var response = new CatalogTypesResponse();
        response.Types_.AddRange(types.Select(t => t.ToMessage()));
        return response;
    }

    public override async Task<AvailableStockResponse> GetAvailableStock(
        GetAvailableStockRequest request, ServerCallContext context)
    {
        var date = request.Date?.ToDateTime().Date ?? DateTime.UtcNow.Date;

        var stock = await _db.CatalogItemsStocks
            .Where(x => x.CatalogItemId == request.CatalogItemId && x.Date == date)
            .FirstOrDefaultAsync();

        return new AvailableStockResponse
        {
            AvailableStock = stock?.AvailableStock ?? 0
        };
    }

    public override async Task<Empty> CreateAvailableStock(
        CatalogItemsStockMessage request, ServerCallContext context)
    {
        var date = request.Date?.ToDateTime().Date ?? DateTime.UtcNow.Date;

        var existing = await _db.CatalogItemsStocks
            .Where(x => x.CatalogItemId == request.CatalogItemId && x.Date == date)
            .FirstOrDefaultAsync();

        if (existing != null)
        {
            existing.AvailableStock = request.AvailableStock;
            _db.Entry(existing).State = EntityState.Modified;
        }
        else
        {
            var maxId = await _db.CatalogItemsStocks.AnyAsync()
                ? await _db.CatalogItemsStocks.MaxAsync(i => i.StockId)
                : 0;

            var entity = new CatalogItemsStock
            {
                StockId = maxId + 1,
                CatalogItemId = request.CatalogItemId,
                AvailableStock = request.AvailableStock,
                Date = date
            };
            _db.CatalogItemsStocks.Add(entity);
        }

        await _db.SaveChangesAsync();
        return new Empty();
    }

    public override async Task<Empty> CreateCatalogItem(
        CatalogItemMessage request, ServerCallContext context)
    {
        var entity = request.ToEntity();

        var maxId = await _db.CatalogItems.AnyAsync()
            ? await _db.CatalogItems.MaxAsync(i => i.Id)
            : 0;
        entity.Id = maxId + 1;

        _db.CatalogItems.Add(entity);
        await _db.SaveChangesAsync();
        return new Empty();
    }

    public override async Task<Empty> UpdateCatalogItem(
        CatalogItemMessage request, ServerCallContext context)
    {
        var entity = request.ToEntity();
        _db.Entry(entity).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return new Empty();
    }

    public override async Task<Empty> RemoveCatalogItem(
        CatalogItemMessage request, ServerCallContext context)
    {
        var item = await _db.CatalogItems.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (item == null)
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Catalog item with id {request.Id} not found"));

        _db.CatalogItems.Remove(item);
        await _db.SaveChangesAsync();
        return new Empty();
    }

    public override async Task<DiscountItemResponse> GetDiscount(
        GetDiscountRequest request, ServerCallContext context)
    {
        var day = request.Day?.ToDateTime().Date ?? DateTime.UtcNow.Date;

        var discount = await _db.DiscountItems
            .Where(y => y.Start.Date <= day && y.End.Date >= day)
            .FirstOrDefaultAsync();

        return new DiscountItemResponse
        {
            Discount = discount?.ToMessage()
        };
    }
}
