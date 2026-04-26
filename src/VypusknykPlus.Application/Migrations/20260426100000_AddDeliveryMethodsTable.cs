using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VypusknykPlus.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryMethodsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create DeliveryMethods table
            migrationBuilder.CreateTable(
                name: "DeliveryMethods",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    Settings = table.Column<string>(type: "text", nullable: false, defaultValue: "{}"),
                    CheckoutFields = table.Column<string>(type: "text", nullable: false, defaultValue: "[]"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryMethods", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryMethods_Slug",
                table: "DeliveryMethods",
                column: "Slug",
                unique: true);

            // 2. Seed Nova Poshta (id=1) and Ukrposhta (id=2)
            migrationBuilder.Sql(
                @"INSERT INTO ""DeliveryMethods"" (""Id"", ""Name"", ""Slug"", ""IsEnabled"", ""Settings"", ""CheckoutFields"", ""CreatedAt"", ""UpdatedAt"", ""IsDeleted"")
                OVERRIDING SYSTEM VALUE
                VALUES
                    (1, 'Нова Пошта', 'nova-poshta', true, '{}', '[{""key"":""city"",""label"":""Місто"",""type"":""input"",""required"":true,""isEnabled"":true,""optionsJson"":""""},{""key"":""warehouse"",""label"":""Відділення або поштомат"",""type"":""input"",""required"":true,""isEnabled"":true,""optionsJson"":""""}]', NOW(), NOW(), false),
                    (2, 'Укрпошта',   'ukrposhta',   true, '{}', '[{""key"":""postalCode"",""label"":""Поштовий індекс"",""type"":""input"",""required"":true,""isEnabled"":true,""optionsJson"":""""}]', NOW(), NOW(), false);
                SELECT setval(pg_get_serial_sequence('""DeliveryMethods""', 'Id'), 2);");

            // 3. Add DeliveryMethodId nullable to Orders
            migrationBuilder.AddColumn<long>(
                name: "DeliveryMethodId",
                table: "Orders",
                type: "bigint",
                nullable: true);

            // 4. Map old Delivery_Method string to new FK
            migrationBuilder.Sql(
                @"UPDATE ""Orders"" SET ""DeliveryMethodId"" = CASE ""Delivery_Method""
                    WHEN 'NovaPoshta' THEN 1
                    WHEN 'Ukrposhta'  THEN 2
                    ELSE 1
                END;");

            // 5. Make NOT NULL
            migrationBuilder.AlterColumn<long>(
                name: "DeliveryMethodId",
                table: "Orders",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            // 6. Drop old Delivery_Method column
            migrationBuilder.DropColumn(
                name: "Delivery_Method",
                table: "Orders");

            // 7. Add index + FK
            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryMethodId",
                table: "Orders",
                column: "DeliveryMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_DeliveryMethods_DeliveryMethodId",
                table: "Orders",
                column: "DeliveryMethodId",
                principalTable: "DeliveryMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_DeliveryMethods_DeliveryMethodId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DeliveryMethodId",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "Delivery_Method",
                table: "Orders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                @"UPDATE ""Orders"" SET ""Delivery_Method"" = CASE ""DeliveryMethodId""
                    WHEN 1 THEN 'NovaPoshta'
                    WHEN 2 THEN 'Ukrposhta'
                    ELSE 'NovaPoshta'
                END;");

            migrationBuilder.DropColumn(
                name: "DeliveryMethodId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryMethods_Slug",
                table: "DeliveryMethods");

            migrationBuilder.DropTable(name: "DeliveryMethods");
        }
    }
}
