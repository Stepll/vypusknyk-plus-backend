using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Data.Configurations;

public class StockCategoryConfiguration : IEntityTypeConfiguration<StockCategory>
{
    public void Configure(EntityTypeBuilder<StockCategory> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);

        builder.HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new StockCategory { Id = 1, Name = "Випускник 11 клас", Order = 1 },
            new StockCategory { Id = 2, Name = "Випускник 9 клас", Order = 2 },
            new StockCategory { Id = 3, Name = "Вчителі та педагоги", Order = 3 },
            new StockCategory { Id = 4, Name = "Першокласник", Order = 4 },
            new StockCategory { Id = 5, Name = "Молодша школа", Order = 5 },
            new StockCategory { Id = 6, Name = "Дошкільні заклади", Order = 6 },
            new StockCategory { Id = 7, Name = "Інші", Order = 7 }
        );
    }
}
