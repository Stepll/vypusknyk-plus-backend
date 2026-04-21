using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Data.Configurations;

public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Number).IsRequired().HasMaxLength(30);
        builder.Property(d => d.Status).IsRequired().HasMaxLength(20);
        builder.Property(d => d.Note).HasMaxLength(1000);

        builder.HasIndex(d => d.Number).IsUnique();
        builder.HasIndex(d => d.ExpectedDate);

        builder.HasMany(d => d.Items)
            .WithOne(i => i.Delivery)
            .HasForeignKey(i => i.DeliveryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
