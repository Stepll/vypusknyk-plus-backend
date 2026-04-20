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

        builder.HasMany(c => c.Subcategories)
            .WithOne(s => s.Category)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new StockCategory { Id = 1, Name = "Стрічки", Order = 1 },
            new StockCategory { Id = 2, Name = "Дзвоники", Order = 2 },
            new StockCategory { Id = 3, Name = "Прапорці та прапори", Order = 3 },
            new StockCategory { Id = 4, Name = "Нагороди та церемонія", Order = 4 },
            new StockCategory { Id = 5, Name = "Інше", Order = 5 }
        );
    }
}
