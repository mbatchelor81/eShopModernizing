using Microsoft.EntityFrameworkCore;

namespace eShopGrpcService.Models;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    public DbSet<CatalogBrand> CatalogBrands { get; set; } = null!;
    public DbSet<CatalogItem> CatalogItems { get; set; } = null!;
    public DbSet<CatalogItemsStock> CatalogItemsStocks { get; set; } = null!;
    public DbSet<CatalogType> CatalogTypes { get; set; } = null!;
    public DbSet<DiscountItem> DiscountItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatalogBrand>(entity =>
        {
            entity.Property(e => e.Brand).IsUnicode(false);
        });

        modelBuilder.Entity<CatalogItem>(entity =>
        {
            entity.Property(e => e.Price).HasPrecision(19, 4);
        });

        modelBuilder.Entity<CatalogType>(entity =>
        {
            entity.Property(e => e.Type).IsUnicode(false);
        });
    }
}
