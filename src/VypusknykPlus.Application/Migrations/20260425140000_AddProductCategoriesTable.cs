using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using VypusknykPlus.Application.Data;

#nullable disable

namespace VypusknykPlus.Application.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260425140000_AddProductCategoriesTable")]
    public partial class AddProductCategoriesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create ProductCategories table
            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                });

            // 2. Seed 4 default categories
            migrationBuilder.Sql(@"
                INSERT INTO ""ProductCategories"" (""Id"", ""Name"", ""Order"")
                OVERRIDING SYSTEM VALUE
                VALUES
                    (1, 'Стрічки',   1),
                    (2, 'Медалі',    2),
                    (3, 'Грамоти',   3),
                    (4, 'Аксесуари', 4);
            ");

            // 3. Create ProductSubcategories table
            migrationBuilder.CreateTable(
                name: "ProductSubcategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSubcategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSubcategories_ProductCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductSubcategories_CategoryId",
                table: "ProductSubcategories",
                column: "CategoryId");

            // 4. Add CategoryId (nullable first for data migration)
            migrationBuilder.AddColumn<long>(
                name: "CategoryId",
                table: "Products",
                type: "bigint",
                nullable: true);

            // 5. Add SubcategoryId
            migrationBuilder.AddColumn<long>(
                name: "SubcategoryId",
                table: "Products",
                type: "bigint",
                nullable: true);

            // 6. Migrate data: map old Category string to CategoryId
            migrationBuilder.Sql(@"
                UPDATE ""Products"" SET ""CategoryId"" = CASE ""Category""
                    WHEN 'Ribbon'      THEN 1
                    WHEN 'Medal'       THEN 2
                    WHEN 'Certificate' THEN 3
                    WHEN 'Accessory'   THEN 4
                    ELSE 1
                END;
            ");

            // 7. Make CategoryId NOT NULL
            migrationBuilder.AlterColumn<long>(
                name: "CategoryId",
                table: "Products",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            // 8. Drop old Category column and its index
            migrationBuilder.DropIndex(
                name: "IX_Products_Category",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Products");

            // 9. Add index and FK for CategoryId
            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductCategories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "ProductCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // 10. Add FK for SubcategoryId
            migrationBuilder.CreateIndex(
                name: "IX_Products_SubcategoryId",
                table: "Products",
                column: "SubcategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductSubcategories_SubcategoryId",
                table: "Products",
                column: "SubcategoryId",
                principalTable: "ProductSubcategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductSubcategories_SubcategoryId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductCategories_CategoryId",
                table: "Products");

            migrationBuilder.DropIndex(name: "IX_Products_SubcategoryId", table: "Products");
            migrationBuilder.DropIndex(name: "IX_Products_CategoryId", table: "Products");

            migrationBuilder.DropColumn(name: "SubcategoryId", table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Products",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Ribbon");

            migrationBuilder.Sql(@"
                UPDATE ""Products"" p
                SET ""Category"" = CASE pc.""Name""
                    WHEN 'Стрічки'   THEN 'Ribbon'
                    WHEN 'Медалі'    THEN 'Medal'
                    WHEN 'Грамоти'   THEN 'Certificate'
                    WHEN 'Аксесуари' THEN 'Accessory'
                    ELSE 'Ribbon'
                END
                FROM ""ProductCategories"" pc
                WHERE p.""CategoryId"" = pc.""Id"";
            ");

            migrationBuilder.DropColumn(name: "CategoryId", table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Category",
                table: "Products",
                column: "Category");

            migrationBuilder.DropTable(name: "ProductSubcategories");
            migrationBuilder.DropTable(name: "ProductCategories");
        }
    }
}
