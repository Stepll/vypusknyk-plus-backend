using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Data.Configurations;

public class PromoCodeUsageConfiguration : IEntityTypeConfiguration<PromoCodeUsage>
{
    public void Configure(EntityTypeBuilder<PromoCodeUsage> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.DiscountAmount).HasPrecision(10, 2);

        builder.HasOne(u => u.PromoCode)
            .WithMany(p => p.Usages)
            .HasForeignKey(u => u.PromoCodeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.User)
            .WithMany()
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasOne(u => u.Order)
            .WithMany()
            .HasForeignKey(u => u.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(u => u.PromoCodeId);
        builder.HasIndex(u => u.UserId);
    }
}
