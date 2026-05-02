using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Data.Configurations;

public class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
{
    public void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Code).IsRequired().HasMaxLength(50);
        builder.Property(p => p.DisplayName).IsRequired().HasMaxLength(200);
        builder.Property(p => p.CardColor).IsRequired().HasMaxLength(20);
        builder.Property(p => p.Description).HasMaxLength(1000);
        builder.Property(p => p.DiscountValue).HasPrecision(10, 2);
        builder.Property(p => p.MinOrderAmount).HasPrecision(10, 2);

        builder.HasIndex(p => p.Code).IsUnique();
        builder.HasIndex(p => p.IsActive);
    }
}
