using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VypusknykPlus.Application.Data;

#nullable disable

namespace VypusknykPlus.Application.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260427500000_AddRibbonFontsTable")]
public partial class AddRibbonFontsTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            CREATE TABLE ""RibbonFonts"" (
                ""Id""          BIGSERIAL PRIMARY KEY,
                ""Name""        VARCHAR(100) NOT NULL,
                ""Slug""        VARCHAR(100) NOT NULL,
                ""FontFamily""  VARCHAR(200) NOT NULL,
                ""ImportUrl""   TEXT         NULL,
                ""IsActive""    BOOLEAN NOT NULL DEFAULT TRUE,
                ""SortOrder""   INTEGER NOT NULL DEFAULT 0,
                ""IsDeleted""   BOOLEAN NOT NULL DEFAULT FALSE,
                ""CreatedAt""   TIMESTAMP NOT NULL DEFAULT NOW(),
                ""UpdatedAt""   TIMESTAMP NOT NULL DEFAULT NOW()
            );

            INSERT INTO ""RibbonFonts"" (""Name"", ""Slug"", ""FontFamily"", ""SortOrder"") VALUES
                ('Класичний',   'classic', 'Georgia, serif',              1),
                ('Курсив',      'italic',  '''Times New Roman'', serif',  2),
                ('Друкований',  'print',   '''Arial'', sans-serif',       3);
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""RibbonFonts"";");
    }
}
