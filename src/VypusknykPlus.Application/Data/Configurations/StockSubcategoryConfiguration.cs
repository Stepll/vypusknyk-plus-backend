using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Data.Configurations;

public class StockSubcategoryConfiguration : IEntityTypeConfiguration<StockSubcategory>
{
    public void Configure(EntityTypeBuilder<StockSubcategory> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();
        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);

        builder.HasIndex(s => s.CategoryId);

        builder.HasMany(s => s.Products)
            .WithOne(p => p.Subcategory)
            .HasForeignKey(p => p.SubcategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            // Стрічки (CategoryId = 1) — was id 1..7
            new StockSubcategory { Id = 1, CategoryId = 1, Name = "Випускник 11 клас", Order = 1 },
            new StockSubcategory { Id = 2, CategoryId = 1, Name = "Випускник 9 клас", Order = 2 },
            new StockSubcategory { Id = 3, CategoryId = 1, Name = "Вчителі та педагоги", Order = 3 },
            new StockSubcategory { Id = 4, CategoryId = 1, Name = "Першокласник", Order = 4 },
            new StockSubcategory { Id = 5, CategoryId = 1, Name = "Молодша школа", Order = 5 },
            new StockSubcategory { Id = 6, CategoryId = 1, Name = "Дошкільні заклади", Order = 6 },
            new StockSubcategory { Id = 7, CategoryId = 1, Name = "Інші стрічки", Order = 7 },

            // Дзвоники (CategoryId = 2)
            new StockSubcategory { Id = 8, CategoryId = 2, Name = "Дзвоники прості", Order = 1 },
            new StockSubcategory { Id = 9, CategoryId = 2, Name = "Дзвоники з бантами", Order = 2 },
            new StockSubcategory { Id = 10, CategoryId = 2, Name = "Банти", Order = 3 },
            new StockSubcategory { Id = 11, CategoryId = 2, Name = "Великі дзвони", Order = 4 },

            // Прапорці та прапори (CategoryId = 3)
            new StockSubcategory { Id = 12, CategoryId = 3, Name = "Прапорці прості", Order = 1 },
            new StockSubcategory { Id = 13, CategoryId = 3, Name = "Прапори великі", Order = 2 },
            new StockSubcategory { Id = 14, CategoryId = 3, Name = "Присоски для прапорців", Order = 3 },

            // Нагороди та церемонія (CategoryId = 4)
            new StockSubcategory { Id = 15, CategoryId = 4, Name = "Грамоти", Order = 1 },
            new StockSubcategory { Id = 16, CategoryId = 4, Name = "Кубки", Order = 2 },
            new StockSubcategory { Id = 17, CategoryId = 4, Name = "Значки", Order = 3 },
            new StockSubcategory { Id = 18, CategoryId = 4, Name = "Запрошення", Order = 4 },
            new StockSubcategory { Id = 19, CategoryId = 4, Name = "Бутун'єрки", Order = 5 },

            // Інше (CategoryId = 5)
            new StockSubcategory { Id = 20, CategoryId = 5, Name = "Мотки тканинні", Order = 1 },
            new StockSubcategory { Id = 21, CategoryId = 5, Name = "Маски", Order = 2 }
        );
    }
}
