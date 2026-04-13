using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VypusknykPlus.Application.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Color", "CreatedAt", "Description", "IsDeleted", "IsNew", "MinOrder", "Name", "Popular", "Price", "Tags", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Ribbon", "coral", new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Класична атласна стрічка з індивідуальним написом. Матеріал — поліестер 100%, ширина 10 см.", false, false, 10, "Атласна стрічка \"Випускник\"", true, 45m, new[] { "випускний", "атлас", "іменна" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "Ribbon", "blue-yellow", new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Двоколірна стрічка в кольорах прапору з можливістю нанесення будь-якого тексту сріблом або золотом.", false, false, 10, "Стрічка синьо-жовта \"Патріот\"", true, 50m, new[] { "випускний", "патріотична", "двоколірна" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "Ribbon", "gold", new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Преміальна золота стрічка з блискучою вишивкою. Відмінний вибір для нагородження відмінників.", false, false, 5, "Стрічка \"Золотий випускник\"", true, 65m, new[] { "відмінник", "золото", "преміум" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, "Ribbon", "white", new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Елегантна біла стрічка — вічна класика для шкільних урочистостей. Чорний або кольоровий друк.", false, false, 10, "Стрічка біла \"Класика\"", false, 40m, new[] { "класика", "біла", "елегантна" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, "Ribbon", "red", new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Яскраво-червона стрічка для переможців олімпіад та конкурсів.", false, true, 10, "Стрічка червона \"Переможець\"", false, 48m, new[] { "олімпіада", "перемога", "конкурс" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, "Ribbon", "purple", new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Нестандартний вибір для творчих заходів, театральних фестивалів та мистецьких конкурсів.", false, true, 10, "Стрічка фіолетова \"Творчість\"", false, 48m, new[] { "творчість", "фестиваль", "мистецтво" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, "Medal", null, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Металева медаль на стрічці з гравіюванням імені та року. Діаметр 70 мм, колір на вибір.", false, false, 1, "Медаль \"Випускник року\"", true, 85m, new[] { "медаль", "нагорода", "гравіювання" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, "Medal", null, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Класична шкільна медаль із зображенням книги та факела. Золоте або срібне покриття.", false, false, 1, "Медаль \"За відмінне навчання\"", false, 90m, new[] { "медаль", "відмінник", "нагорода" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, "Medal", null, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Спортивна медаль для олімпіад і змагань. I, II, III місця. Індивідуальна гравіювання.", false, true, 1, "Медаль \"Переможець олімпіади\"", false, 95m, new[] { "олімпіада", "змагання", "місце" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, "Certificate", null, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Святкова грамота А4 з іменем, класом та датою. Кольоровий друк на дизайнерському папері.", false, false, 5, "Грамота \"Випускник\"", false, 25m, new[] { "грамота", "диплом", "друк" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, "Certificate", null, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Преміальний диплом на картоні з тисненням і стрічкою. Формат A4.", false, false, 1, "Диплом \"З відзнакою\"", false, 35m, new[] { "диплом", "відзнака", "преміум" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, "Accessory", null, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Комплект для класу: стрічки + медаль + грамота для кожного учня. Знижка 15% від окремих цін.", false, false, 1, "Набір \"Випускний під ключ\"", true, 320m, new[] { "набір", "комплект", "знижка" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, "Accessory", null, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Металева зірка-підвіска із гравіюванням на замовлення. Можна прикріпити до стрічки або рюкзака.", false, true, 1, "Підвіска \"Зірка випускника\"", false, 55m, new[] { "підвіска", "зірка", "декор" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, "Ribbon", "green", new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Зелена стрічка з еко-принтом. Підходить для шкільних екологічних заходів та фестивалів.", false, true, 10, "Стрічка зелена \"Еко\"", false, 45m, new[] { "еко", "зелена", "фестиваль" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, "Ribbon", "black", new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Чорна оксамитова стрічка з золотим написом — для особливих урочистостей і нагороджень.", false, false, 5, "Стрічка чорна \"Преміум\"", false, 70m, new[] { "чорна", "оксамит", "золото" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15);
        }
    }
}
