using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VypusknykPlus.Application.Data;

#nullable disable

namespace VypusknykPlus.Application.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260427410000_AddRibbonPrintColorUsageFlags")]
public partial class AddRibbonPrintColorUsageFlags : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            ALTER TABLE ""RibbonPrintColors""
                ADD COLUMN ""IsForMainText""  BOOLEAN NOT NULL DEFAULT TRUE,
                ADD COLUMN ""IsForExtraText"" BOOLEAN NOT NULL DEFAULT FALSE;

            -- white is used in both sections
            UPDATE ""RibbonPrintColors"" SET ""IsForExtraText"" = TRUE WHERE ""Slug"" = 'white';
            -- yellow is only for extra text
            UPDATE ""RibbonPrintColors"" SET ""IsForMainText"" = FALSE, ""IsForExtraText"" = TRUE WHERE ""Slug"" = 'yellow';
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            ALTER TABLE ""RibbonPrintColors""
                DROP COLUMN ""IsForMainText"",
                DROP COLUMN ""IsForExtraText"";
        ");
    }
}
