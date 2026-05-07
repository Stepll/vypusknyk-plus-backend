using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VypusknykPlus.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddCertificateTemplateLayout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasAdditionalText",
                table: "CertificateTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasSecondSigner",
                table: "CertificateTemplates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LayoutJson",
                table: "CertificateTemplates",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NativeOrientation",
                table: "CertificateTemplates",
                type: "text",
                nullable: false,
                defaultValue: "portrait");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasAdditionalText",
                table: "CertificateTemplates");

            migrationBuilder.DropColumn(
                name: "HasSecondSigner",
                table: "CertificateTemplates");

            migrationBuilder.DropColumn(
                name: "LayoutJson",
                table: "CertificateTemplates");

            migrationBuilder.DropColumn(
                name: "NativeOrientation",
                table: "CertificateTemplates");
        }
    }
}
