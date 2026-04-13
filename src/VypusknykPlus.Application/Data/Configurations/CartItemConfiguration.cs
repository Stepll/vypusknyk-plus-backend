using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Data.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(ci => ci.Id);

        builder.OwnsOne(ci => ci.ProductSnapshot, ps =>
        {
            ps.ToJson();
            ps.Property(x => x.Name).HasMaxLength(200);
            ps.Property(x => x.Price).HasPrecision(10, 2);
            ps.Property(x => x.Category).HasMaxLength(50);
            ps.Property(x => x.Color).HasMaxLength(50);
        });

        builder.OwnsOne(ci => ci.NamesData, nd =>
        {
            nd.ToJson();
            nd.Property(x => x.School).HasMaxLength(500);
            nd.OwnsMany(x => x.Groups, g =>
            {
                g.Property(x => x.ClassName).HasMaxLength(100);
                g.Property(x => x.Names).HasMaxLength(5000);
            });
        });

        builder.OwnsOne(ci => ci.RibbonCustomization, rc =>
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

        builder.HasOne(ci => ci.Product)
            .WithMany()
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(ci => ci.User)
            .WithMany(u => u.CartItems)
            .HasForeignKey(ci => ci.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ci => ci.UserId);
    }
}
