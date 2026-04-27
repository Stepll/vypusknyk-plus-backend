using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VypusknykPlus.Application.Data;

#nullable disable

namespace VypusknykPlus.Application.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260427100000_AddOrderStatusIsActive")]
    public partial class AddOrderStatusIsActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "OrderStatuses",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "OrderStatuses");
        }
    }
}
