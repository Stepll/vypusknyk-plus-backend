using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VypusknykPlus.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTasksSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PromoCodes_Code",
                table: "PromoCodes");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "PromoCodes",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateTable(
                name: "UserTasks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TaskType = table.Column<int>(type: "integer", nullable: false),
                    TargetValue = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    TargetCategoryId = table.Column<long>(type: "bigint", nullable: true),
                    RewardPromoCodeId = table.Column<long>(type: "bigint", nullable: false),
                    IsVisibleToGuests = table.Column<bool>(type: "boolean", nullable: false),
                    EndsAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTasks_ProductCategories_TargetCategoryId",
                        column: x => x.TargetCategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UserTasks_PromoCodes_RewardPromoCodeId",
                        column: x => x.RewardPromoCodeId,
                        principalTable: "PromoCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserTaskProgresses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaskId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Progress = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AwardedCardId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTaskProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTaskProgresses_UserPromoCodeCards_AwardedCardId",
                        column: x => x.AwardedCardId,
                        principalTable: "UserPromoCodeCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UserTaskProgresses_UserTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "UserTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTaskProgresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodes_Code",
                table: "PromoCodes",
                column: "Code",
                unique: true,
                filter: "\"Code\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserTaskProgresses_AwardedCardId",
                table: "UserTaskProgresses",
                column: "AwardedCardId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTaskProgresses_TaskId_UserId",
                table: "UserTaskProgresses",
                columns: new[] { "TaskId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTaskProgresses_UserId",
                table: "UserTaskProgresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_RewardPromoCodeId",
                table: "UserTasks",
                column: "RewardPromoCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_TargetCategoryId",
                table: "UserTasks",
                column: "TargetCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTaskProgresses");

            migrationBuilder.DropTable(
                name: "UserTasks");

            migrationBuilder.DropIndex(
                name: "IX_PromoCodes_Code",
                table: "PromoCodes");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "PromoCodes",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodes_Code",
                table: "PromoCodes",
                column: "Code",
                unique: true);
        }
    }
}
