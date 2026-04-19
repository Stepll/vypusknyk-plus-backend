using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VypusknykPlus.Application.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    MinOrder = table.Column<int>(type: "integer", nullable: false),
                    Popular = table.Column<bool>(type: "boolean", nullable: false),
                    IsNew = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Tags = table.Column<string[]>(type: "text[]", nullable: false),
                    ImageKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PasswordHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Qty = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    NamesData = table.Column<string>(type: "jsonb", nullable: true),
                    ProductSnapshot = table.Column<string>(type: "jsonb", nullable: true),
                    RibbonCustomization = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CartItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Delivery_Method = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Delivery_City = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Delivery_Warehouse = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Delivery_PostalCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Recipient_FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Recipient_Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Payment = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsAnonymous = table.Column<bool>(type: "boolean", nullable: false),
                    GuestToken = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedDesigns",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DesignName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SavedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    State = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedDesigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedDesigns_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Color", "CreatedAt", "Description", "ImageKey", "IsDeleted", "IsNew", "MinOrder", "Name", "Popular", "Price", "Tags", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, "Ribbon", "coral", new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Класична атласна стрічка з індивідуальним написом. Матеріал — поліестер 100%, ширина 10 см.", null, false, false, 10, "Атласна стрічка \"Випускник\"", true, 45m, new[] { "випускний", "атлас", "іменна" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2L, "Ribbon", "blue-yellow", new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Двоколірна стрічка в кольорах прапору з можливістю нанесення будь-якого тексту сріблом або золотом.", null, false, false, 10, "Стрічка синьо-жовта \"Патріот\"", true, 50m, new[] { "випускний", "патріотична", "двоколірна" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3L, "Ribbon", "gold", new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Преміальна золота стрічка з блискучою вишивкою. Відмінний вибір для нагородження відмінників.", null, false, false, 5, "Стрічка \"Золотий випускник\"", true, 65m, new[] { "відмінник", "золото", "преміум" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4L, "Ribbon", "white", new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Елегантна біла стрічка — вічна класика для шкільних урочистостей. Чорний або кольоровий друк.", null, false, false, 10, "Стрічка біла \"Класика\"", false, 40m, new[] { "класика", "біла", "елегантна" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5L, "Ribbon", "red", new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Яскраво-червона стрічка для переможців олімпіад та конкурсів.", null, false, true, 10, "Стрічка червона \"Переможець\"", false, 48m, new[] { "олімпіада", "перемога", "конкурс" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6L, "Ribbon", "purple", new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Нестандартний вибір для творчих заходів, театральних фестивалів та мистецьких конкурсів.", null, false, true, 10, "Стрічка фіолетова \"Творчість\"", false, 48m, new[] { "творчість", "фестиваль", "мистецтво" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7L, "Medal", null, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Металева медаль на стрічці з гравіюванням імені та року. Діаметр 70 мм, колір на вибір.", null, false, false, 1, "Медаль \"Випускник року\"", true, 85m, new[] { "медаль", "нагорода", "гравіювання" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8L, "Medal", null, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Класична шкільна медаль із зображенням книги та факела. Золоте або срібне покриття.", null, false, false, 1, "Медаль \"За відмінне навчання\"", false, 90m, new[] { "медаль", "відмінник", "нагорода" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9L, "Medal", null, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Спортивна медаль для олімпіад і змагань. I, II, III місця. Індивідуальна гравіювання.", null, false, true, 1, "Медаль \"Переможець олімпіади\"", false, 95m, new[] { "олімпіада", "змагання", "місце" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10L, "Certificate", null, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Святкова грамота А4 з іменем, класом та датою. Кольоровий друк на дизайнерському папері.", null, false, false, 5, "Грамота \"Випускник\"", false, 25m, new[] { "грамота", "диплом", "друк" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11L, "Certificate", null, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Преміальний диплом на картоні з тисненням і стрічкою. Формат A4.", null, false, false, 1, "Диплом \"З відзнакою\"", false, 35m, new[] { "диплом", "відзнака", "преміум" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12L, "Accessory", null, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Комплект для класу: стрічки + медаль + грамота для кожного учня. Знижка 15% від окремих цін.", null, false, false, 1, "Набір \"Випускний під ключ\"", true, 320m, new[] { "набір", "комплект", "знижка" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13L, "Accessory", null, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Металева зірка-підвіска із гравіюванням на замовлення. Можна прикріпити до стрічки або рюкзака.", null, false, true, 1, "Підвіска \"Зірка випускника\"", false, 55m, new[] { "підвіска", "зірка", "декор" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14L, "Ribbon", "green", new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Зелена стрічка з еко-принтом. Підходить для шкільних екологічних заходів та фестивалів.", null, false, true, 10, "Стрічка зелена \"Еко\"", false, 45m, new[] { "еко", "зелена", "фестиваль" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15L, "Ribbon", "black", new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Чорна оксамитова стрічка з золотим написом — для особливих урочистостей і нагороджень.", null, false, false, 5, "Стрічка чорна \"Преміум\"", false, 70m, new[] { "чорна", "оксамит", "золото" }, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId",
                table: "CartItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_GuestToken",
                table: "Orders",
                column: "GuestToken");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_Token",
                table: "PasswordResetTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_UserId",
                table: "PasswordResetTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Category",
                table: "Products",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedDesigns_UserId",
                table: "SavedDesigns",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "PasswordResetTokens");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "SavedDesigns");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
