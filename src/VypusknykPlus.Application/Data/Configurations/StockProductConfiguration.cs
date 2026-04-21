using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Data.Configurations;

public class StockProductConfiguration : IEntityTypeConfiguration<StockProduct>
{
    public void Configure(EntityTypeBuilder<StockProduct> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).HasMaxLength(500);

        builder.HasIndex(p => p.SubcategoryId);

        builder.HasMany(p => p.Variants)
            .WithOne(v => v.Product)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(GetSeedProducts());
    }

    private static StockProduct[] GetSeedProducts() =>
    [
        // Випускник 11 клас (SubcategoryId = 1)
        new() { Id = 1,  SubcategoryId = 1, Name = "Випускник атлас об'ємний орнамент",       HasColor = true, HasMaterial = true },
        new() { Id = 2,  SubcategoryId = 1, Name = "Випускниця атлас об'ємна орнамент",        HasColor = true, HasMaterial = true },
        new() { Id = 3,  SubcategoryId = 1, Name = "Випускник атлас фольга",                   HasColor = true, HasMaterial = true },
        new() { Id = 4,  SubcategoryId = 1, Name = "Випускниця атлас фольга",                  HasColor = true, HasMaterial = true },
        new() { Id = 5,  SubcategoryId = 1, Name = "Випускник атлас кольоровий",               HasColor = true, HasMaterial = true },
        new() { Id = 6,  SubcategoryId = 1, Name = "Випускник атлас голограма",                HasColor = true, HasMaterial = true },
        new() { Id = 7,  SubcategoryId = 1, Name = "Випускник орнамент (шовк)",                HasColor = true, HasMaterial = true },
        new() { Id = 8,  SubcategoryId = 1, Name = "Випускниця орнамент (шовк)",               HasColor = true, HasMaterial = true },

        // Випускник 9 клас (SubcategoryId = 2)
        new() { Id = 9,  SubcategoryId = 2, Name = "Випускник атлас об'ємний 9 клас",          HasColor = true, HasMaterial = true },
        new() { Id = 10, SubcategoryId = 2, Name = "Випускник 21 атлас фольга",                HasColor = true, HasMaterial = true },
        new() { Id = 11, SubcategoryId = 2, Name = "Випускник 21 атлас фольга 9 клас",         HasColor = true, HasMaterial = true },

        // Вчителі та педагоги (SubcategoryId = 3)
        new() { Id = 12, SubcategoryId = 3, Name = "Класний керівник об'ємний",                HasColor = true, HasMaterial = true },
        new() { Id = 13, SubcategoryId = 3, Name = "Класний керівник атлас фольга",            HasColor = true, HasMaterial = true },
        new() { Id = 14, SubcategoryId = 3, Name = "Перший вчитель об'ємний",                  HasColor = true, HasMaterial = true },
        new() { Id = 15, SubcategoryId = 3, Name = "Перший вчитель необ'ємний",                HasColor = true, HasMaterial = true },
        new() { Id = 16, SubcategoryId = 3, Name = "Директор об'ємний",                        HasColor = true, HasMaterial = true },
        new() { Id = 17, SubcategoryId = 3, Name = "Директор необ'ємний",                      HasColor = true, HasMaterial = true },
        new() { Id = 18, SubcategoryId = 3, Name = "Завуч об'ємний",                           HasColor = true, HasMaterial = true },
        new() { Id = 19, SubcategoryId = 3, Name = "Завуч необ'ємний",                         HasColor = true, HasMaterial = true },

        // Першокласник (SubcategoryId = 4)
        new() { Id = 20, SubcategoryId = 4, Name = "Першокласник об'ємний",                    HasColor = true, HasMaterial = true },
        new() { Id = 21, SubcategoryId = 4, Name = "Першокласник необ'ємний",                  HasColor = true, HasMaterial = true },

        // Молодша школа (SubcategoryId = 5)
        new() { Id = 22, SubcategoryId = 5, Name = "Випускник початкової школи об'ємний",      HasColor = true, HasMaterial = true },
        new() { Id = 23, SubcategoryId = 5, Name = "Випускник початкової школи необ'ємний",    HasColor = true, HasMaterial = true },

        // Дошкільні заклади (SubcategoryId = 6)
        new() { Id = 24, SubcategoryId = 6, Name = "Випускник дошкільного закладу об'ємний",   HasColor = true, HasMaterial = true },
        new() { Id = 25, SubcategoryId = 6, Name = "Випускник дошкільного закладу необ'ємний", HasColor = true, HasMaterial = true },

        // Інші стрічки (SubcategoryId = 7)
        new() { Id = 26, SubcategoryId = 7, Name = "Гімназист",                                HasColor = true, HasMaterial = true },
        new() { Id = 27, SubcategoryId = 7, Name = "Відмінник навчання",                       HasColor = true, HasMaterial = true },
        new() { Id = 28, SubcategoryId = 7, Name = "Кращий учень",                             HasColor = true, HasMaterial = true },
    ];
}
