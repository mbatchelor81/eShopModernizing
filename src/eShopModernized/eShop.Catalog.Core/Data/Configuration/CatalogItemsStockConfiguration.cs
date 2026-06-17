using eShop.Catalog.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShop.Catalog.Core.Data.Configuration;

public class CatalogItemsStockConfiguration : IEntityTypeConfiguration<CatalogItemsStock>
{
    public void Configure(EntityTypeBuilder<CatalogItemsStock> builder)
    {
        builder.ToTable("CatalogItemsStock");

        builder.HasKey(s => s.StockId);

        builder.Property(s => s.StockId)
            .ValueGeneratedNever();

        builder.Property(s => s.Date)
            .HasColumnType("date");
    }
}
