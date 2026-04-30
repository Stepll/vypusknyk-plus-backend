using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VypusknykPlus.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailMessage",
                table: "NotificationTriggerConfigs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailSubject",
                table: "NotificationTriggerConfigs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SystemMessage",
                table: "NotificationTriggerConfigs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SystemTitle",
                table: "NotificationTriggerConfigs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelegramMessage",
                table: "NotificationTriggerConfigs",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailMessage",
                table: "NotificationTriggerConfigs");

            migrationBuilder.DropColumn(
                name: "EmailSubject",
                table: "NotificationTriggerConfigs");

            migrationBuilder.DropColumn(
                name: "SystemMessage",
                table: "NotificationTriggerConfigs");

            migrationBuilder.DropColumn(
                name: "SystemTitle",
                table: "NotificationTriggerConfigs");

            migrationBuilder.DropColumn(
                name: "TelegramMessage",
                table: "NotificationTriggerConfigs");
        }
    }
}
