using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Data.Configurations;

public class DeliveryItemConfiguration : IEntityTypeConfiguration<DeliveryItem>
{
    public void Configure(EntityTypeBuilder<DeliveryItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.HasIndex(i => i.DeliveryId);
        builder.HasIndex(i => i.ProductId);

        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId);

        builder.HasMany(i => i.Transactions)
            .WithOne(t => t.DeliveryItem)
            .HasForeignKey(t => t.DeliveryItemId)
            .IsRequired(false);
    }
}
