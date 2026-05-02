using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(20);
        builder.Property(o => o.Total).HasPrecision(10, 2);

        builder.HasOne(o => o.OrderStatus)
            .WithMany(s => s.Orders)
            .HasForeignKey(o => o.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.DeliveryMethod)
            .WithMany(dm => dm.Orders)
            .HasForeignKey(o => o.DeliveryMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.PaymentMethod)
            .WithMany(pm => pm.Orders)
            .HasForeignKey(o => o.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(o => o.Email).HasMaxLength(256);
        builder.Property(o => o.Comment).HasMaxLength(1000);

        builder.OwnsOne(o => o.Delivery, d =>
        {
            d.Property(x => x.City).HasMaxLength(200);
            d.Property(x => x.Warehouse).HasMaxLength(200);
            d.Property(x => x.PostalCode).HasMaxLength(10);
        });

        builder.OwnsOne(o => o.Recipient, r =>
        {
            r.Property(x => x.FullName).IsRequired().HasMaxLength(200);
            r.Property(x => x.Phone).IsRequired().HasMaxLength(20);
        });

        builder.Property(o => o.GuestToken).HasMaxLength(36);

        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasOne(o => o.Promotion)
            .WithMany(p => p.Orders)
            .HasForeignKey(o => o.PromotionId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasOne(o => o.PromoCode)
            .WithMany(p => p.Orders)
            .HasForeignKey(o => o.PromoCodeId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.Property(o => o.PromotionDiscount).HasPrecision(10, 2);
        builder.Property(o => o.PromoCodeDiscount).HasPrecision(10, 2);

        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.GuestToken);
        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.HasIndex(o => o.DeliveryMethodId);
        builder.HasIndex(o => o.PaymentMethodId);
    }
}
