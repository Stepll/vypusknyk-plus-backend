using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Data.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.Property(p => p.ImageKey).IsRequired().HasMaxLength(500);
        builder.HasIndex(p => p.ProductId);
        builder.HasOne(p => p.Product)
               .WithMany(p => p.Images)
               .HasForeignKey(p => p.ProductId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
