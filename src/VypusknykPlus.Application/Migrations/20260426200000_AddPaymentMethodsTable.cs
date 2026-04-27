using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using VypusknykPlus.Application.Data;

#nullable disable

namespace VypusknykPlus.Application.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260426200000_AddPaymentMethodsTable")]
    public partial class AddPaymentMethodsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create PaymentMethods table
            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_Slug",
                table: "PaymentMethods",
                column: "Slug",
                unique: true);

            // 2. Seed Накладний платіж (cod) and Онлайн оплата (online)
            migrationBuilder.Sql(
                @"INSERT INTO ""PaymentMethods"" (""Name"", ""Slug"", ""IsEnabled"", ""CreatedAt"", ""UpdatedAt"", ""IsDeleted"")
                VALUES
                    ('Накладний платіж', 'cod',    true,  NOW(), NOW(), false),
                    ('Онлайн оплата',   'online', false, NOW(), NOW(), false);");

            // 3. Add PaymentMethodId nullable to Orders
            migrationBuilder.AddColumn<long>(
                name: "PaymentMethodId",
                table: "Orders",
                type: "bigint",
                nullable: true);

            // 4. Map old Payment string (enum name) to new FK
            migrationBuilder.Sql(
                @"UPDATE ""Orders"" SET ""PaymentMethodId"" = CASE ""Payment""
                    WHEN 'Cod'    THEN (SELECT ""Id"" FROM ""PaymentMethods"" WHERE ""Slug"" = 'cod')
                    WHEN 'Online' THEN (SELECT ""Id"" FROM ""PaymentMethods"" WHERE ""Slug"" = 'online')
                    ELSE (SELECT ""Id"" FROM ""PaymentMethods"" WHERE ""Slug"" = 'cod')
                END;");

            // 5. Make NOT NULL
            migrationBuilder.AlterColumn<long>(
                name: "PaymentMethodId",
                table: "Orders",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            // 6. Drop old Payment column
            migrationBuilder.DropColumn(
                name: "Payment",
                table: "Orders");

            // 7. Add index + FK
            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentMethodId",
                table: "Orders",
                column: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_PaymentMethods_PaymentMethodId",
                table: "Orders",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_PaymentMethods_PaymentMethodId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PaymentMethodId",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "Payment",
                table: "Orders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                @"UPDATE ""Orders"" SET ""Payment"" = CASE ""PaymentMethodId""
                    WHEN (SELECT ""Id"" FROM ""PaymentMethods"" WHERE ""Slug"" = 'cod')    THEN 'Cod'
                    WHEN (SELECT ""Id"" FROM ""PaymentMethods"" WHERE ""Slug"" = 'online') THEN 'Online'
                    ELSE 'Cod'
                END;");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_PaymentMethods_Slug",
                table: "PaymentMethods");

            migrationBuilder.DropTable(name: "PaymentMethods");
        }
    }
}
