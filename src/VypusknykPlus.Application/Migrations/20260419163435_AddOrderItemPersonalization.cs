using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VypusknykPlus.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderItemPersonalization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NamesData",
                table: "OrderItems",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RibbonCustomization",
                table: "OrderItems",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NamesData",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "RibbonCustomization",
                table: "OrderItems");
        }
    }
}
