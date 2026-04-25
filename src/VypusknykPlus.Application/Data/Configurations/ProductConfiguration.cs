using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    private static readonly DateTime SeedDate = new(2026, 4, 8, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Color).HasMaxLength(50);
        builder.Property(p => p.Price).HasPrecision(10, 2);
        builder.Property(p => p.Description).HasMaxLength(2000);

        builder.Property(p => p.ImageKey).HasMaxLength(500).IsRequired(false);

        builder.HasIndex(p => p.CategoryId);

        builder.HasData(GetSeedProducts());
    }

    private static Product[] GetSeedProducts() =>
    [
        new()
        {
            Id = 1,
            Name = "Атласна стрічка \"Випускник\"",
            CategoryId = 1,
            Color = "coral",
            Price = 45m,
            MinOrder = 10,
            Popular = true,
            IsNew = false,
            Description = "Класична атласна стрічка з індивідуальним написом. Матеріал — поліестер 100%, ширина 10 см.",
            Tags = ["випускний", "атлас", "іменна"],
            CreatedAt = SeedDate,
            UpdatedAt = SeedDate
        },
        new()
        {
            Id = 2,
            Name = "Стрічка синьо-жовта \"Патріот\"",
            CategoryId = 1,
            Color = "blue-yellow",
            Price = 50m,
            MinOrder = 10,
            Popular = true,
            IsNew = false,
            Description = "Двоколірна стрічка в кольорах прапору з можливістю нанесення будь-якого тексту сріблом або золотом.",
            Tags = ["випускний", "патріотична", "двоколірна"],
            CreatedAt = SeedDate,
            UpdatedAt = SeedDate
        },
        new()
        {
            Id = 3,
            Name = "Стрічка \"Золотий випускник\"",
            CategoryId = 1,
            Color = "gold",
            Price = 65m,
            MinOrder = 5,
            Popular = true,
            IsNew = false,
            Description = "Преміальна золота стрічка з блискучою вишивкою. Відмінний вибір для нагородження відмінників.",
            Tags = ["відмінник", "золото", "преміум"],
            CreatedAt = SeedDate,
            UpdatedAt = SeedDate
        },
        new()
        {
            Id = 4,
            Name = "Стрічка біла \"Класика\"",
            CategoryId = 1,
            Color = "white",
            Price = 40m,
            MinOrder = 10,
            Popular = false,
            IsNew = false,
            Description = "Елегантна біла стрічка — вічна класика для шкільних урочистостей. Чорний або кольоровий друк.",
            Tags = ["класика", "біла", "елегантна"],
            CreatedAt = SeedDate,
            UpdatedAt = SeedDate
        },
        new()
        {
            Id = 5,
            Name = "Стрічка червона \"Переможець\"",
            CategoryId = 1,
            Color = "red",
            Price = 48m,
            MinOrder = 10,
            Popular = false,
            IsNew = true,
            Description = "Яскраво-червона стрічка для переможців олімпіад та конкурсів.",
            Tags = ["олімпіада", "перемога", "конкурс"],
            CreatedAt = SeedDate,
            UpdatedAt = SeedDate
        },
        new()
        {
            Id = 6,
            Name = "Стрічка фіолетова \"Творчість\"",
            CategoryId = 1,
            Color = "purple",
            Price = 48m,
            MinOrder = 10,
            Popular = false,
            IsNew = true,
            Description = "Нестандартний вибір для творчих заходів, театральних фестивалів та мистецьких конкурсів.",
            Tags = ["творчість", "фестиваль", "мистецтво"],
            CreatedAt = SeedDate,
            UpdatedAt = SeedDate
        },
        new()
        {
            Id = 7,
            Name = "Медаль \"Випускник року\"",
            CategoryId = 2,
            Color = null,
            Price = 85m,
            MinOrder = 1,
            Popular = true,
            IsNew = false,
            Description = "Металева медаль на стрічці з гравіюванням імені та року. Діаметр 70 мм, колір на вибір.",
            Tags = ["медаль", "нагорода", "гравіювання"],
            CreatedAt = SeedDate,
            UpdatedAt = SeedDate
        },
        new()
        {
            Id = 8,
            Name = "Медаль \"За відмінне навчання\"",
            CategoryId = 2,
            Color = null,
            Price = 90m,
            MinOrder = 1,
            Popular = false,
            IsNew = false,
            Description = "Класична шкільна медаль із зображенням книги та факела. Золоте або срібне покриття.",
            Tags = ["медаль", "відмінник", "нагорода"],
            CreatedAt = SeedDate,
            UpdatedAt = SeedDate
        },
        new()
        {
            Id = 9,
            Name = "Медаль \"Переможець олімпіади\"",
            CategoryId = 2,
            Color = null,
            Price = 95m,
            MinOrder = 1,
            Popular = false,
            IsNew = true,
            Description = "Спортивна медаль для олімпіад і змагань. I, II, III місця. Індивідуальна гравіювання.",
            Tags = ["олімпіада", "змагання", "місце"],
            CreatedAt = SeedDate,
            UpdatedAt = SeedDate
        },
        new()
        {
            Id = 10,
            Name = "Грамота \"Випускник\"",
            CategoryId = 3,
            Color = null,
            Price = 25m,
            MinOrder = 5,
            Popular = false,
            IsNew = false,
            Description = "Святкова грамота А4 з іменем, класом та датою. Кольоровий друк на дизайнерському папері.",
            Tags = ["грамота", "диплом", "друк"],
            CreatedAt = SeedDate,
            UpdatedAt = SeedDate
        },
        new()
        {
            Id = 11,
            Name = "Диплом \"З відзнакою\"",
            CategoryId = 3,
            Color = null,
            Price = 35m,
            MinOrder = 1,
            Popular = false,
            IsNew = false,
            Description = "Преміальний диплом на картоні з тисненням і стрічкою. Формат A4.",
            Tags = ["диплом", "відзнака", "преміум"],
            CreatedAt = SeedDate,
            UpdatedAt = SeedDate
        },
        new()
        {
            Id = 12,
            Name = "Набір \"Випускний під ключ\"",
            CategoryId = 4,
            Color = null,
            Price = 320m,
            MinOrder = 1,
            Popular = true,
            IsNew = false,
            Description = "Комплект для класу: стрічки + медаль + грамота для кожного учня. Знижка 15% від окремих цін.",
            Tags = ["набір", "комплект", "знижка"],
            CreatedAt = SeedDate,
            UpdatedAt = SeedDate
        },
        new()
        {
            Id = 13,
            Name = "Підвіска \"Зірка випускника\"",
            CategoryId = 4,
            Color = null,
            Price = 55m,
            MinOrder = 1,
            Popular = false,
            IsNew = true,
            Description = "Металева зірка-підвіска із гравіюванням на замовлення. Можна прикріпити до стрічки або рюкзака.",
            Tags = ["підвіска", "зірка", "декор"],
            CreatedAt = SeedDate,
            UpdatedAt = SeedDate
        },
        new()
        {
            Id = 14,
            Name = "Стрічка зелена \"Еко\"",
            CategoryId = 1,
            Color = "green",
            Price = 45m,
            MinOrder = 10,
            Popular = false,
            IsNew = true,
            Description = "Зелена стрічка з еко-принтом. Підходить для шкільних екологічних заходів та фестивалів.",
            Tags = ["еко", "зелена", "фестиваль"],
            CreatedAt = SeedDate,
            UpdatedAt = SeedDate
        },
        new()
        {
            Id = 15,
            Name = "Стрічка чорна \"Преміум\"",
            CategoryId = 1,
            Color = "black",
            Price = 70m,
            MinOrder = 5,
            Popular = false,
            IsNew = false,
            Description = "Чорна оксамитова стрічка з золотим написом — для особливих урочистостей і нагороджень.",
            Tags = ["чорна", "оксамит", "золото"],
            CreatedAt = SeedDate,
            UpdatedAt = SeedDate
        }
    ];
}
