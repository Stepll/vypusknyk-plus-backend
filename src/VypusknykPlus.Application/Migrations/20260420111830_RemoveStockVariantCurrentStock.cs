using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VypusknykPlus.Application.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStockVariantCurrentStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentStock",
                table: "StockVariants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentStock",
                table: "StockVariants",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
