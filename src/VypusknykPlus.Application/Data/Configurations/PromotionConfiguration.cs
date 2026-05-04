using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Data.Configurations;

public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).HasMaxLength(1000);
        builder.Property(p => p.DiscountValue).HasPrecision(10, 2);
        builder.Property(p => p.MinOrderAmount).HasPrecision(10, 2);
        builder.HasIndex(p => p.IsActive);
        builder.HasIndex(p => p.StartsAt);
        builder.HasIndex(p => p.EndsAt);
    }
}

public class PromotionTargetCategoryConfiguration : IEntityTypeConfiguration<PromotionTargetCategory>
{
    public void Configure(EntityTypeBuilder<PromotionTargetCategory> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasOne(t => t.Promotion).WithMany(p => p.TargetCategories)
            .HasForeignKey(t => t.PromotionId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(t => t.Category).WithMany()
            .HasForeignKey(t => t.CategoryId).OnDelete(DeleteBehavior.Cascade).IsRequired(false);
        builder.HasOne(t => t.Subcategory).WithMany()
            .HasForeignKey(t => t.SubcategoryId).OnDelete(DeleteBehavior.Cascade).IsRequired(false);
    }
}

public class PromotionVolumeTierConfiguration : IEntityTypeConfiguration<PromotionVolumeTier>
{
    public void Configure(EntityTypeBuilder<PromotionVolumeTier> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.DiscountValue).HasPrecision(10, 2);
        builder.HasOne(t => t.Promotion).WithMany(p => p.VolumeTiers)
            .HasForeignKey(t => t.PromotionId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class PromotionBundleItemConfiguration : IEntityTypeConfiguration<PromotionBundleItem>
{
    public void Configure(EntityTypeBuilder<PromotionBundleItem> builder)
    {
        builder.HasKey(b => b.Id);
        builder.HasOne(b => b.Promotion).WithMany(p => p.BundleItems)
            .HasForeignKey(b => b.PromotionId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(b => b.Subcategory).WithMany()
            .HasForeignKey(b => b.SubcategoryId).OnDelete(DeleteBehavior.Cascade);
    }
}
