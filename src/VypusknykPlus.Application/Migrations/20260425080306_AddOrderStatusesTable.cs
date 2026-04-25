using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VypusknykPlus.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderStatusesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create OrderStatuses table
            migrationBuilder.CreateTable(
                name: "OrderStatuses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsFinal = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatuses", x => x.Id);
                });

            // 2. Seed 4 default statuses with explicit IDs
            migrationBuilder.Sql(@"
                INSERT INTO ""OrderStatuses"" (""Id"", ""Name"", ""Color"", ""SortOrder"", ""IsFinal"", ""CreatedAt"", ""UpdatedAt"", ""IsDeleted"")
                OVERRIDING SYSTEM VALUE
                VALUES
                    (1, 'Прийнято',    '#6366f1', 1, false, NOW(), NOW(), false),
                    (2, 'Виробництво', '#f59e0b', 2, false, NOW(), NOW(), false),
                    (3, 'Відправлено', '#3b82f6', 3, false, NOW(), NOW(), false),
                    (4, 'Доставлено',  '#10b981', 4, true,  NOW(), NOW(), false);
                SELECT setval(pg_get_serial_sequence('""OrderStatuses""', 'Id'), 4);
            ");

            // 3. Add StatusId as nullable to allow data migration
            migrationBuilder.AddColumn<long>(
                name: "StatusId",
                table: "Orders",
                type: "bigint",
                nullable: true);

            // 4. Map old Status string values to new StatusId
            migrationBuilder.Sql(@"
                UPDATE ""Orders"" SET ""StatusId"" = CASE ""Status""
                    WHEN 'Accepted'   THEN 1
                    WHEN 'Production' THEN 2
                    WHEN 'Shipped'    THEN 3
                    WHEN 'Delivered'  THEN 4
                    ELSE 1
                END;
            ");

            // 5. Make StatusId NOT NULL
            migrationBuilder.AlterColumn<long>(
                name: "StatusId",
                table: "Orders",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            // 6. Drop old Status column
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            // 7. Add index and FK
            migrationBuilder.CreateIndex(
                name: "IX_Orders_StatusId",
                table: "Orders",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderStatuses_StatusId",
                table: "Orders",
                column: "StatusId",
                principalTable: "OrderStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderStatuses_StatusId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_StatusId",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Orders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                UPDATE ""Orders"" SET ""Status"" = CASE ""StatusId""
                    WHEN 1 THEN 'Accepted'
                    WHEN 2 THEN 'Production'
                    WHEN 3 THEN 'Shipped'
                    WHEN 4 THEN 'Delivered'
                    ELSE 'Accepted'
                END;
            ");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "OrderStatuses");
        }
    }
}
