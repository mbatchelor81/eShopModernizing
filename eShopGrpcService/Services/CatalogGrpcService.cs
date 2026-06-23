using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using eShopGrpcService.Data;
using eShopGrpcService.Mapping;

namespace eShopGrpcService.Services;

public class CatalogGrpcService : CatalogService.CatalogServiceBase
{
    private readonly CatalogDbContext _db;

    public CatalogGrpcService(CatalogDbContext db)
    {
        _db = db;
    }

    public override async Task<CatalogItemMsg> FindCatalogItem(
        FindCatalogItemRequest request, ServerCallContext context)
    {
        var item = await _db.CatalogItems
            .Include(i => i.CatalogBrand)
            .Include(i => i.CatalogType)
            .FirstOrDefaultAsync(x => x.Id == request.Id);

        if (item == null)
            throw new RpcException(new Status(StatusCode.NotFound,
                $"Catalog item with id {request.Id} not found."));

        return item.ToProto();
    }

    public override async Task<CatalogBrandListResponse> GetCatalogBrands(
        Empty request, ServerCallContext context)
    {
        var brands = await _db.CatalogBrands.ToListAsync();
        var response = new CatalogBrandListResponse();
        response.Brands.AddRange(brands.Select(b => b.ToProto()));
        return response;
    }

    public override async Task<CatalogItemListResponse> GetCatalogItems(
        GetCatalogItemsRequest request, ServerCallContext context)
    {
        // Fix anti-pattern: push Where before ToList (server-side filtering)
        IQueryable<Models.CatalogItem> query = _db.CatalogItems;

        if (request.BrandIdFilter != 0)
            query = query.Where(x => x.CatalogBrandId == request.BrandIdFilter);

        if (request.TypeIdFilter != 0)
            query = query.Where(x => x.CatalogTypeId == request.TypeIdFilter);

        var items = await query.ToListAsync();
        var response = new CatalogItemListResponse();
        response.Items.AddRange(items.Select(i => i.ToProto()));
        return response;
    }

    public override async Task<CatalogTypeListResponse> GetCatalogTypes(
        Empty request, ServerCallContext context)
    {
        var types = await _db.CatalogTypes.ToListAsync();
        var response = new CatalogTypeListResponse();
        response.Types_.AddRange(types.Select(t => t.ToProto()));
        return response;
    }

    public override async Task<GetAvailableStockResponse> GetAvailableStock(
        GetAvailableStockRequest request, ServerCallContext context)
    {
        var date = request.Date.ToDateTime().Date;
        var stock = await _db.CatalogItemsStocks
            .Where(x => x.CatalogItemId == request.CatalogItemId && x.Date.Date == date)
            .FirstOrDefaultAsync();

        return new GetAvailableStockResponse
        {
            AvailableStock = stock?.AvailableStock ?? 0
        };
    }

    public override async Task<Empty> CreateAvailableStock(
        CreateAvailableStockRequest request, ServerCallContext context)
    {
        var input = request.CatalogItemsStock;
        var date = input.Date.ToDateTime().Date;

        // Upsert logic: check if stock entry exists for this item and date
        var existing = await _db.CatalogItemsStocks
            .Where(x => x.CatalogItemId == input.CatalogItemId && x.Date.Date == date)
            .FirstOrDefaultAsync();

        if (existing != null)
        {
            // Update existing stock entry
            existing.AvailableStock = input.AvailableStock;
            _db.Entry(existing).State = EntityState.Modified;
        }
        else
        {
            // Create new stock entry
            var maxId = await _db.CatalogItemsStocks.AnyAsync()
                ? await _db.CatalogItemsStocks.MaxAsync(i => i.StockId)
                : 0;
            var newStock = input.ToEntity();
            newStock.StockId = maxId + 1;
            _db.CatalogItemsStocks.Add(newStock);
        }

        await _db.SaveChangesAsync();
        return new Empty();
    }

    public override async Task<Empty> CreateCatalogItem(
        CreateCatalogItemRequest request, ServerCallContext context)
    {
        var maxId = await _db.CatalogItems.AnyAsync()
            ? await _db.CatalogItems.MaxAsync(i => i.Id)
            : 0;
        var catalogItem = request.CatalogItem.ToEntity();
        catalogItem.Id = maxId + 1;
        _db.CatalogItems.Add(catalogItem);
        await _db.SaveChangesAsync();
        return new Empty();
    }

    public override async Task<Empty> UpdateCatalogItem(
        UpdateCatalogItemRequest request, ServerCallContext context)
    {
        var catalogItem = request.CatalogItem.ToEntity();
        _db.Entry(catalogItem).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return new Empty();
    }

    public override async Task<Empty> RemoveCatalogItem(
        RemoveCatalogItemRequest request, ServerCallContext context)
    {
        var catalogItem = request.CatalogItem.ToEntity();
        _db.CatalogItems.Remove(catalogItem);
        await _db.SaveChangesAsync();
        return new Empty();
    }

    public override async Task<DiscountItemMsg> GetDiscount(
        GetDiscountRequest request, ServerCallContext context)
    {
        var day = request.Day.ToDateTime().Date;

        // Fix anti-pattern: push Where before materialization (server-side filtering)
        var discount = await _db.DiscountItems
            .Where(y => y.Start.Date <= day && y.End.Date >= day)
            .FirstOrDefaultAsync();

        if (discount == null)
            throw new RpcException(new Status(StatusCode.NotFound,
                $"No discount found for date {day:yyyy-MM-dd}."));

        return discount.ToProto();
    }
}
