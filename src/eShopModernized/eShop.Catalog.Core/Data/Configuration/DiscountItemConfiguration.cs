using eShop.Catalog.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShop.Catalog.Core.Data.Configuration;

public class DiscountItemConfiguration : IEntityTypeConfiguration<DiscountItem>
{
    public void Configure(EntityTypeBuilder<DiscountItem> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Start)
            .HasColumnType("date");

        builder.Property(d => d.End)
            .HasColumnType("date");
    }
}
