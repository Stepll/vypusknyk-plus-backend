using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VypusknykPlus.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddStockSubcategoryAndProductParams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockProducts_StockCategories_CategoryId",
                table: "StockProducts");

            migrationBuilder.DeleteData(
                table: "StockCategories",
                keyColumn: "Id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "StockCategories",
                keyColumn: "Id",
                keyValue: 7L);

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "StockProducts",
                newName: "SubcategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_StockProducts_CategoryId",
                table: "StockProducts",
                newName: "IX_StockProducts_SubcategoryId");

            migrationBuilder.AddColumn<bool>(
                name: "HasColor",
                table: "StockProducts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasMaterial",
                table: "StockProducts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "StockSubcategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockSubcategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockSubcategories_StockCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "StockCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "StockCategories",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Name",
                value: "Стрічки");

            migrationBuilder.UpdateData(
                table: "StockCategories",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Name",
                value: "Дзвоники");

            migrationBuilder.UpdateData(
                table: "StockCategories",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Name",
                value: "Прапорці та прапори");

            migrationBuilder.UpdateData(
                table: "StockCategories",
                keyColumn: "Id",
                keyValue: 4L,
                column: "Name",
                value: "Нагороди та церемонія");

            migrationBuilder.UpdateData(
                table: "StockCategories",
                keyColumn: "Id",
                keyValue: 5L,
                column: "Name",
                value: "Інше");

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 9L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 10L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 11L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 12L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 13L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 14L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 15L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 16L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 17L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 18L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 19L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 20L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 21L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 22L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 23L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 24L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 25L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 26L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 27L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.UpdateData(
                table: "StockProducts",
                keyColumn: "Id",
                keyValue: 28L,
                columns: new[] { "HasColor", "HasMaterial" },
                values: new object[] { true, true });

            migrationBuilder.InsertData(
                table: "StockSubcategories",
                columns: new[] { "Id", "CategoryId", "Name", "Order" },
                values: new object[,]
                {
                    { 1L, 1L, "Випускник 11 клас", 1 },
                    { 2L, 1L, "Випускник 9 клас", 2 },
                    { 3L, 1L, "Вчителі та педагоги", 3 },
                    { 4L, 1L, "Першокласник", 4 },
                    { 5L, 1L, "Молодша школа", 5 },
                    { 6L, 1L, "Дошкільні заклади", 6 },
                    { 7L, 1L, "Інші стрічки", 7 },
                    { 8L, 2L, "Дзвоники прості", 1 },
                    { 9L, 2L, "Дзвоники з бантами", 2 },
                    { 10L, 2L, "Банти", 3 },
                    { 11L, 2L, "Великі дзвони", 4 },
                    { 12L, 3L, "Прапорці прості", 1 },
                    { 13L, 3L, "Прапори великі", 2 },
                    { 14L, 3L, "Присоски для прапорців", 3 },
                    { 15L, 4L, "Грамоти", 1 },
                    { 16L, 4L, "Кубки", 2 },
                    { 17L, 4L, "Значки", 3 },
                    { 18L, 4L, "Запрошення", 4 },
                    { 19L, 4L, "Бутун'єрки", 5 },
                    { 20L, 5L, "Мотки тканинні", 1 },
                    { 21L, 5L, "Маски", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockSubcategories_CategoryId",
                table: "StockSubcategories",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockProducts_StockSubcategories_SubcategoryId",
                table: "StockProducts",
                column: "SubcategoryId",
                principalTable: "StockSubcategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockProducts_StockSubcategories_SubcategoryId",
                table: "StockProducts");

            migrationBuilder.DropTable(
                name: "StockSubcategories");

            migrationBuilder.DropColumn(
                name: "HasColor",
                table: "StockProducts");

            migrationBuilder.DropColumn(
                name: "HasMaterial",
                table: "StockProducts");

            migrationBuilder.RenameColumn(
                name: "SubcategoryId",
                table: "StockProducts",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_StockProducts_SubcategoryId",
                table: "StockProducts",
                newName: "IX_StockProducts_CategoryId");

            migrationBuilder.UpdateData(
                table: "StockCategories",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Name",
                value: "Випускник 11 клас");

            migrationBuilder.UpdateData(
                table: "StockCategories",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Name",
                value: "Випускник 9 клас");

            migrationBuilder.UpdateData(
                table: "StockCategories",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Name",
                value: "Вчителі та педагоги");

            migrationBuilder.UpdateData(
                table: "StockCategories",
                keyColumn: "Id",
                keyValue: 4L,
                column: "Name",
                value: "Першокласник");

            migrationBuilder.UpdateData(
                table: "StockCategories",
                keyColumn: "Id",
                keyValue: 5L,
                column: "Name",
                value: "Молодша школа");

            migrationBuilder.InsertData(
                table: "StockCategories",
                columns: new[] { "Id", "Name", "Order" },
                values: new object[,]
                {
                    { 6L, "Дошкільні заклади", 6 },
                    { 7L, "Інші", 7 }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_StockProducts_StockCategories_CategoryId",
                table: "StockProducts",
                column: "CategoryId",
                principalTable: "StockCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
