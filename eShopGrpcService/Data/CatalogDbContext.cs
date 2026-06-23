using Microsoft.EntityFrameworkCore;
using eShopGrpcService.Models;
using eShopGrpcService.Models.Infrastructure;

namespace eShopGrpcService.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<CatalogBrand> CatalogBrands { get; set; }
    public DbSet<CatalogItem> CatalogItems { get; set; }
    public DbSet<CatalogItemsStock> CatalogItemsStocks { get; set; }
    public DbSet<CatalogType> CatalogTypes { get; set; }
    public DbSet<DiscountItem> DiscountItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Port Fluent API from legacy EF6 EntityModel
        modelBuilder.Entity<CatalogBrand>()
            .Property(e => e.Brand)
            .IsUnicode(false);

        modelBuilder.Entity<CatalogItem>()
            .Property(e => e.Price)
            .HasPrecision(19, 4);

        modelBuilder.Entity<CatalogType>()
            .Property(e => e.Type)
            .IsUnicode(false);

        // Seed data using HasData (replaces CatalogDBInitializer)
        modelBuilder.Entity<CatalogBrand>().HasData(
            PreconfiguredData.GetPreconfiguredCatalogBrands());

        modelBuilder.Entity<CatalogType>().HasData(
            PreconfiguredData.GetPreconfiguredCatalogTypes());

        modelBuilder.Entity<CatalogItem>().HasData(
            PreconfiguredData.GetPreconfiguredCatalogItems());

        modelBuilder.Entity<CatalogItemsStock>().HasData(
            PreconfiguredData.GetPreconfiguredCatalogItemsStock());

        modelBuilder.Entity<DiscountItem>().HasData(
            PreconfiguredData.GetPreconfiguredDiscountItems());
    }
}
