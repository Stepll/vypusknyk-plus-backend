using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Data.Configurations;

public class StockVariantConfiguration : IEntityTypeConfiguration<StockVariant>
{
    public void Configure(EntityTypeBuilder<StockVariant> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).ValueGeneratedOnAdd();
        builder.Property(v => v.Material).IsRequired().HasMaxLength(20);
        builder.Property(v => v.Color).IsRequired().HasMaxLength(50);

        builder.HasIndex(v => new { v.ProductId, v.Material, v.Color }).IsUnique();

        builder.HasMany(v => v.Transactions)
            .WithOne(t => t.Variant)
            .HasForeignKey(t => t.VariantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
