using Microsoft.EntityFrameworkCore;
using eShopGrpcService.Models;

namespace eShopGrpcService.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

    public DbSet<CatalogBrand> CatalogBrands => Set<CatalogBrand>();
    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
    public DbSet<CatalogItemsStock> CatalogItemsStocks => Set<CatalogItemsStock>();
    public DbSet<CatalogType> CatalogTypes => Set<CatalogType>();
    public DbSet<DiscountItem> DiscountItems => Set<DiscountItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatalogBrand>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Brand).HasMaxLength(50).IsUnicode(false);
        });

        modelBuilder.Entity<CatalogItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Price).HasPrecision(19, 4).HasColumnType("money");
            e.HasOne(x => x.CatalogBrand).WithMany().HasForeignKey(x => x.CatalogBrandId);
            e.HasOne(x => x.CatalogType).WithMany().HasForeignKey(x => x.CatalogTypeId);
        });

        modelBuilder.Entity<CatalogItemsStock>(e =>
        {
            e.ToTable("CatalogItemsStock");
            e.HasKey(x => x.StockId);
            e.Property(x => x.StockId).ValueGeneratedNever();
            e.Property(x => x.Date).HasColumnType("date");
            e.HasIndex(x => new { x.CatalogItemId, x.Date }).IsUnique();
        });

        modelBuilder.Entity<CatalogType>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Type).HasMaxLength(50).IsUnicode(false);
        });

        modelBuilder.Entity<DiscountItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Start).HasColumnType("date");
            e.Property(x => x.End).HasColumnType("date");
        });

        modelBuilder.Entity<CatalogBrand>().HasData(PreconfiguredData.GetPreconfiguredCatalogBrands());
        modelBuilder.Entity<CatalogType>().HasData(PreconfiguredData.GetPreconfiguredCatalogTypes());
        modelBuilder.Entity<CatalogItem>().HasData(
            PreconfiguredData.GetPreconfiguredCatalogItems()
                .Select(i => new { i.Id, i.CatalogTypeId, i.CatalogBrandId, i.Description, i.Name, i.Price, i.Picturefilename }));
        modelBuilder.Entity<CatalogItemsStock>().HasData(PreconfiguredData.GetPreconfiguredCatalogItemsStock());
        modelBuilder.Entity<DiscountItem>().HasData(PreconfiguredData.GetPreconfiguredDiscountItems());
    }
}
