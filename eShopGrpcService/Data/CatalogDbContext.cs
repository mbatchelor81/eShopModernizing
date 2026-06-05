using eShopGrpcService.Models;
using Microsoft.EntityFrameworkCore;

namespace eShopGrpcService.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<CatalogBrand> CatalogBrands => Set<CatalogBrand>();
    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
    public DbSet<CatalogItemsStock> CatalogItemsStocks => Set<CatalogItemsStock>();
    public DbSet<CatalogType> CatalogTypes => Set<CatalogType>();
    public DbSet<DiscountItem> DiscountItems => Set<DiscountItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatalogBrand>(entity =>
        {
            entity.Property(e => e.Brand)
                .IsUnicode(false)
                .HasMaxLength(50);
            entity.Property(e => e.Id)
                .ValueGeneratedNever();
        });

        modelBuilder.Entity<CatalogItem>(entity =>
        {
            entity.Property(e => e.Price)
                .HasPrecision(19, 4)
                .HasColumnType("money");
            entity.Property(e => e.Id)
                .ValueGeneratedNever();

            entity.HasOne(e => e.CatalogBrand)
                .WithMany()
                .HasForeignKey(e => e.CatalogBrandId);

            entity.HasOne(e => e.CatalogType)
                .WithMany()
                .HasForeignKey(e => e.CatalogTypeId);
        });

        modelBuilder.Entity<CatalogType>(entity =>
        {
            entity.Property(e => e.Type)
                .IsUnicode(false)
                .HasMaxLength(50);
            entity.Property(e => e.Id)
                .ValueGeneratedNever();
        });

        modelBuilder.Entity<CatalogItemsStock>(entity =>
        {
            entity.ToTable("CatalogItemsStock");
            entity.HasKey(e => e.StockId);
            entity.Property(e => e.StockId)
                .ValueGeneratedNever();
            entity.Property(e => e.Date)
                .HasColumnType("date");
        });

        modelBuilder.Entity<DiscountItem>(entity =>
        {
            entity.Property(e => e.Start)
                .HasColumnType("date");
            entity.Property(e => e.End)
                .HasColumnType("date");
        });
    }
}
