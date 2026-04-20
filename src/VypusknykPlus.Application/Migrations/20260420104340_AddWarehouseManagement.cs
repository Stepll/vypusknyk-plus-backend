using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VypusknykPlus.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockProducts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockProducts_StockCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "StockCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockVariants",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    Material = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CurrentStock = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockVariants_StockProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "StockProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockTransactions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VariantId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTransactions_StockVariants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "StockVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "StockCategories",
                columns: new[] { "Id", "Name", "Order" },
                values: new object[,]
                {
                    { 1L, "Випускник 11 клас", 1 },
                    { 2L, "Випускник 9 клас", 2 },
                    { 3L, "Вчителі та педагоги", 3 },
                    { 4L, "Першокласник", 4 },
                    { 5L, "Молодша школа", 5 },
                    { 6L, "Дошкільні заклади", 6 },
                    { 7L, "Інші", 7 }
                });

            migrationBuilder.InsertData(
                table: "StockProducts",
                columns: new[] { "Id", "CategoryId", "Description", "Name" },
                values: new object[,]
                {
                    { 1L, 1L, null, "Випускник атлас об'ємний орнамент" },
                    { 2L, 1L, null, "Випускниця атлас об'ємна орнамент" },
                    { 3L, 1L, null, "Випускник атлас фольга" },
                    { 4L, 1L, null, "Випускниця атлас фольга" },
                    { 5L, 1L, null, "Випускник атлас кольоровий" },
                    { 6L, 1L, null, "Випускник атлас голограма" },
                    { 7L, 1L, null, "Випускник орнамент (шовк)" },
                    { 8L, 1L, null, "Випускниця орнамент (шовк)" },
                    { 9L, 2L, null, "Випускник атлас об'ємний 9 клас" },
                    { 10L, 2L, null, "Випускник 21 атлас фольга" },
                    { 11L, 2L, null, "Випускник 21 атлас фольга 9 клас" },
                    { 12L, 3L, null, "Класний керівник об'ємний" },
                    { 13L, 3L, null, "Класний керівник атлас фольга" },
                    { 14L, 3L, null, "Перший вчитель об'ємний" },
                    { 15L, 3L, null, "Перший вчитель необ'ємний" },
                    { 16L, 3L, null, "Директор об'ємний" },
                    { 17L, 3L, null, "Директор необ'ємний" },
                    { 18L, 3L, null, "Завуч об'ємний" },
                    { 19L, 3L, null, "Завуч необ'ємний" },
                    { 20L, 4L, null, "Першокласник об'ємний" },
                    { 21L, 4L, null, "Першокласник необ'ємний" },
                    { 22L, 5L, null, "Випускник початкової школи об'ємний" },
                    { 23L, 5L, null, "Випускник початкової школи необ'ємний" },
                    { 24L, 6L, null, "Випускник дошкільного закладу об'ємний" },
                    { 25L, 6L, null, "Випускник дошкільного закладу необ'ємний" },
                    { 26L, 7L, null, "Гімназист" },
                    { 27L, 7L, null, "Відмінник навчання" },
                    { 28L, 7L, null, "Кращий учень" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockProducts_CategoryId",
                table: "StockProducts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_Date",
                table: "StockTransactions",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_VariantId",
                table: "StockTransactions",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_StockVariants_ProductId_Material_Color",
                table: "StockVariants",
                columns: new[] { "ProductId", "Material", "Color" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockTransactions");

            migrationBuilder.DropTable(
                name: "StockVariants");

            migrationBuilder.DropTable(
                name: "StockProducts");

            migrationBuilder.DropTable(
                name: "StockCategories");
        }
    }
}
