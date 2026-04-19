using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Name).IsRequired().HasMaxLength(200);
        builder.Property(oi => oi.Price).HasPrecision(10, 2);

        builder.OwnsOne(oi => oi.NamesData, nd =>
        {
            nd.ToJson();
            nd.Property(x => x.School).HasMaxLength(500);
            nd.OwnsMany(x => x.Groups, g =>
            {
                g.Property(x => x.ClassName).HasMaxLength(100);
                g.Property(x => x.Names).HasMaxLength(5000);
            });
        });

        builder.OwnsOne(oi => oi.RibbonCustomization, rc =>
        {
            rc.ToJson();
            rc.Property(x => x.MainText).HasMaxLength(500);
            rc.Property(x => x.School).HasMaxLength(500);
            rc.Property(x => x.Comment).HasMaxLength(1000);
            rc.Property(x => x.PrintType).HasMaxLength(50);
            rc.Property(x => x.Color).HasMaxLength(50);
            rc.Property(x => x.Material).HasMaxLength(50);
            rc.Property(x => x.TextColor).HasMaxLength(50);
            rc.Property(x => x.ExtraTextColor).HasMaxLength(50);
            rc.Property(x => x.Font).HasMaxLength(50);
            rc.Property(x => x.DesignName).HasMaxLength(200);
        });

        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
