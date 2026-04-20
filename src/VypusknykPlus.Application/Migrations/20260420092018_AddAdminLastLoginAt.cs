using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VypusknykPlus.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminLastLoginAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginAt",
                table: "Admins",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastLoginAt",
                table: "Admins");
        }
    }
}
