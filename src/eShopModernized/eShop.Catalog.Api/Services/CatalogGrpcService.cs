using eShop.Catalog.Core.Data;
using eShop.Catalog.Core.Entities;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace eShop.Catalog.Api.Services;

/// <summary>
/// gRPC service implementation for the Catalog domain.
/// Mirrors all 10 WCF operations as gRPC RPCs.
/// </summary>
public class CatalogGrpcService : CatalogGrpc.CatalogGrpcBase
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
            .Include(i => i.CatalogBrand)
            .Include(i => i.CatalogType)
            .FirstOrDefaultAsync(i => i.Id == request.Id, context.CancellationToken);

        if (item is null)
            throw new RpcException(new Status(StatusCode.NotFound, $"Catalog item {request.Id} not found"));

        return MapToResponse(item);
    }

    public override async Task<CatalogItemListResponse> GetCatalogItems(
        GetCatalogItemsRequest request, ServerCallContext context)
    {
        var query = _db.CatalogItems
            .Include(i => i.CatalogBrand)
            .Include(i => i.CatalogType)
            .AsQueryable();

        if (request.BrandId > 0)
            query = query.Where(i => i.CatalogBrandId == request.BrandId);
        if (request.TypeId > 0)
            query = query.Where(i => i.CatalogTypeId == request.TypeId);

        var totalCount = await query.CountAsync(context.CancellationToken);

        var pageSize = request.PageSize > 0 ? request.PageSize : 10;
        var pageIndex = request.PageIndex > 0 ? request.PageIndex : 0;

        var items = await query
            .OrderBy(i => i.Id)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync(context.CancellationToken);

        var response = new CatalogItemListResponse
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalCount = totalCount
        };
        response.Data.AddRange(items.Select(MapToResponse));
        return response;
    }

    public override async Task<CatalogBrandListResponse> GetCatalogBrands(
        Empty request, ServerCallContext context)
    {
        var brands = await _db.CatalogBrands
            .OrderBy(b => b.Id)
            .ToListAsync(context.CancellationToken);

        var response = new CatalogBrandListResponse();
        response.Brands.AddRange(brands.Select(b => new CatalogBrandResponse
        {
            Id = b.Id,
            Brand = b.Brand
        }));
        return response;
    }

    public override async Task<CatalogTypeListResponse> GetCatalogTypes(
        Empty request, ServerCallContext context)
    {
        var types = await _db.CatalogTypes
            .OrderBy(t => t.Id)
            .ToListAsync(context.CancellationToken);

        var response = new CatalogTypeListResponse();
        response.CatalogTypes.AddRange(types.Select(t => new CatalogTypeResponse
        {
            Id = t.Id,
            Type = t.Type
        }));
        return response;
    }

    public override async Task<Empty> CreateCatalogItem(
        CatalogItemRequest request, ServerCallContext context)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable, context.CancellationToken);

        var item = new CatalogItem
        {
            Name = request.Name,
            Description = request.Description,
            Price = (decimal)request.Price,
            PictureFileName = request.PictureFileName,
            CatalogTypeId = request.CatalogTypeId,
            CatalogBrandId = request.CatalogBrandId,
            AvailableStock = request.AvailableStock,
            RestockThreshold = request.RestockThreshold,
            MaxStockThreshold = request.MaxStockThreshold,
            OnReorder = request.OnReorder
        };

        if (await _db.CatalogItems.AnyAsync(context.CancellationToken))
        {
            item.Id = await _db.CatalogItems.MaxAsync(i => i.Id, context.CancellationToken) + 1;
        }
        else
        {
            item.Id = 1;
        }

        _db.CatalogItems.Add(item);
        await _db.SaveChangesAsync(context.CancellationToken);
        await transaction.CommitAsync(context.CancellationToken);
        return new Empty();
    }

    public override async Task<Empty> UpdateCatalogItem(
        CatalogItemRequest request, ServerCallContext context)
    {
        var existing = await _db.CatalogItems
            .FirstOrDefaultAsync(i => i.Id == request.Id, context.CancellationToken);

        if (existing is null)
            throw new RpcException(new Status(StatusCode.NotFound, $"Catalog item {request.Id} not found"));

        existing.Name = request.Name;
        existing.Description = request.Description;
        existing.Price = (decimal)request.Price;
        existing.PictureFileName = request.PictureFileName;
        existing.CatalogTypeId = request.CatalogTypeId;
        existing.CatalogBrandId = request.CatalogBrandId;
        existing.AvailableStock = request.AvailableStock;
        existing.RestockThreshold = request.RestockThreshold;
        existing.MaxStockThreshold = request.MaxStockThreshold;
        existing.OnReorder = request.OnReorder;

        await _db.SaveChangesAsync(context.CancellationToken);
        return new Empty();
    }

    public override async Task<Empty> RemoveCatalogItem(
        RemoveCatalogItemRequest request, ServerCallContext context)
    {
        var item = await _db.CatalogItems
            .FirstOrDefaultAsync(i => i.Id == request.Id, context.CancellationToken);

        if (item is null)
            throw new RpcException(new Status(StatusCode.NotFound, $"Catalog item {request.Id} not found"));

        _db.CatalogItems.Remove(item);
        await _db.SaveChangesAsync(context.CancellationToken);
        return new Empty();
    }

    public override async Task<StockResponse> GetAvailableStock(
        GetStockRequest request, ServerCallContext context)
    {
        var date = request.Date.ToDateTime().Date;
        var stock = await _db.CatalogItemsStocks
            .FirstOrDefaultAsync(
                s => s.CatalogItemId == request.CatalogItemId && s.Date.Date == date,
                context.CancellationToken);

        if (stock is null)
            return new StockResponse { AvailableStock = 0 };

        return new StockResponse
        {
            StockId = stock.StockId,
            Date = Timestamp.FromDateTime(DateTime.SpecifyKind(stock.Date, DateTimeKind.Utc)),
            CatalogItemId = stock.CatalogItemId,
            AvailableStock = stock.AvailableStock
        };
    }

    public override async Task<Empty> CreateAvailableStock(
        CreateStockRequest request, ServerCallContext context)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable, context.CancellationToken);

        var date = request.Date.ToDateTime().Date;
        var existing = await _db.CatalogItemsStocks
            .FirstOrDefaultAsync(
                s => s.CatalogItemId == request.CatalogItemId && s.Date.Date == date,
                context.CancellationToken);

        if (existing is not null)
        {
            existing.AvailableStock = request.AvailableStock;
        }
        else
        {
            var maxId = await _db.CatalogItemsStocks.AnyAsync(context.CancellationToken)
                ? await _db.CatalogItemsStocks.MaxAsync(s => s.StockId, context.CancellationToken)
                : 0;

            _db.CatalogItemsStocks.Add(new CatalogItemsStock
            {
                StockId = maxId + 1,
                Date = date,
                CatalogItemId = request.CatalogItemId,
                AvailableStock = request.AvailableStock
            });
        }

        await _db.SaveChangesAsync(context.CancellationToken);
        await transaction.CommitAsync(context.CancellationToken);
        return new Empty();
    }

    public override async Task<DiscountResponse> GetDiscount(
        GetDiscountRequest request, ServerCallContext context)
    {
        var date = request.Date.ToDateTime().Date;
        var discount = await _db.DiscountItems
            .FirstOrDefaultAsync(
                d => d.Start.Date <= date && d.End.Date >= date,
                context.CancellationToken);

        if (discount is null)
            throw new RpcException(new Status(StatusCode.NotFound, "No discount found for the specified date"));

        return new DiscountResponse
        {
            Id = discount.Id,
            Size = discount.Size,
            Start = Timestamp.FromDateTime(DateTime.SpecifyKind(discount.Start, DateTimeKind.Utc)),
            End = Timestamp.FromDateTime(DateTime.SpecifyKind(discount.End, DateTimeKind.Utc))
        };
    }

    private static CatalogItemResponse MapToResponse(CatalogItem item)
    {
        return new CatalogItemResponse
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Price = (double)item.Price,
            PictureFileName = item.PictureFileName,
            PictureUri = item.PictureUri ?? string.Empty,
            CatalogTypeId = item.CatalogTypeId,
            CatalogTypeName = item.CatalogType?.Type ?? string.Empty,
            CatalogBrandId = item.CatalogBrandId,
            CatalogBrandName = item.CatalogBrand?.Brand ?? string.Empty,
            AvailableStock = item.AvailableStock,
            RestockThreshold = item.RestockThreshold,
            MaxStockThreshold = item.MaxStockThreshold,
            OnReorder = item.OnReorder
        };
    }
}
