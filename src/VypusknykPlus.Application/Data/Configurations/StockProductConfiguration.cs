using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Data.Configurations;

public class StockProductConfiguration : IEntityTypeConfiguration<StockProduct>
{
    public void Configure(EntityTypeBuilder<StockProduct> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).HasMaxLength(500);

        builder.HasIndex(p => p.CategoryId);

        builder.HasMany(p => p.Variants)
            .WithOne(v => v.Product)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(GetSeedProducts());
    }

    private static StockProduct[] GetSeedProducts() =>
    [
        // Випускник 11 клас (CategoryId = 1)
        new() { Id = 1, CategoryId = 1, Name = "Випускник атлас об'ємний орнамент" },
        new() { Id = 2, CategoryId = 1, Name = "Випускниця атлас об'ємна орнамент" },
        new() { Id = 3, CategoryId = 1, Name = "Випускник атлас фольга" },
        new() { Id = 4, CategoryId = 1, Name = "Випускниця атлас фольга" },
        new() { Id = 5, CategoryId = 1, Name = "Випускник атлас кольоровий" },
        new() { Id = 6, CategoryId = 1, Name = "Випускник атлас голограма" },
        new() { Id = 7, CategoryId = 1, Name = "Випускник орнамент (шовк)" },
        new() { Id = 8, CategoryId = 1, Name = "Випускниця орнамент (шовк)" },

        // Випускник 9 клас (CategoryId = 2)
        new() { Id = 9, CategoryId = 2, Name = "Випускник атлас об'ємний 9 клас" },
        new() { Id = 10, CategoryId = 2, Name = "Випускник 21 атлас фольга" },
        new() { Id = 11, CategoryId = 2, Name = "Випускник 21 атлас фольга 9 клас" },

        // Вчителі та педагоги (CategoryId = 3)
        new() { Id = 12, CategoryId = 3, Name = "Класний керівник об'ємний" },
        new() { Id = 13, CategoryId = 3, Name = "Класний керівник атлас фольга" },
        new() { Id = 14, CategoryId = 3, Name = "Перший вчитель об'ємний" },
        new() { Id = 15, CategoryId = 3, Name = "Перший вчитель необ'ємний" },
        new() { Id = 16, CategoryId = 3, Name = "Директор об'ємний" },
        new() { Id = 17, CategoryId = 3, Name = "Директор необ'ємний" },
        new() { Id = 18, CategoryId = 3, Name = "Завуч об'ємний" },
        new() { Id = 19, CategoryId = 3, Name = "Завуч необ'ємний" },

        // Першокласник (CategoryId = 4)
        new() { Id = 20, CategoryId = 4, Name = "Першокласник об'ємний" },
        new() { Id = 21, CategoryId = 4, Name = "Першокласник необ'ємний" },

        // Молодша школа (CategoryId = 5)
        new() { Id = 22, CategoryId = 5, Name = "Випускник початкової школи об'ємний" },
        new() { Id = 23, CategoryId = 5, Name = "Випускник початкової школи необ'ємний" },

        // Дошкільні заклади (CategoryId = 6)
        new() { Id = 24, CategoryId = 6, Name = "Випускник дошкільного закладу об'ємний" },
        new() { Id = 25, CategoryId = 6, Name = "Випускник дошкільного закладу необ'ємний" },

        // Інші (CategoryId = 7)
        new() { Id = 26, CategoryId = 7, Name = "Гімназист" },
        new() { Id = 27, CategoryId = 7, Name = "Відмінник навчання" },
        new() { Id = 28, CategoryId = 7, Name = "Кращий учень" },
    ];
}
