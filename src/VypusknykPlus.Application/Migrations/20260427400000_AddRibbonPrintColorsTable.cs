using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VypusknykPlus.Application.Data;

#nullable disable

namespace VypusknykPlus.Application.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260427400000_AddRibbonPrintColorsTable")]
public partial class AddRibbonPrintColorsTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            CREATE TABLE ""RibbonPrintColors"" (
                ""Id""             BIGSERIAL PRIMARY KEY,
                ""Name""           VARCHAR(100) NOT NULL,
                ""Slug""           VARCHAR(100) NOT NULL,
                ""Hex""            VARCHAR(20)  NOT NULL,
                ""PriceModifier""  NUMERIC(10,2) NOT NULL DEFAULT 0,
                ""IsActive""       BOOLEAN NOT NULL DEFAULT TRUE,
                ""SortOrder""      INTEGER NOT NULL DEFAULT 0,
                ""IsDeleted""      BOOLEAN NOT NULL DEFAULT FALSE,
                ""CreatedAt""      TIMESTAMP NOT NULL DEFAULT NOW(),
                ""UpdatedAt""      TIMESTAMP NOT NULL DEFAULT NOW()
            );

            INSERT INTO ""RibbonPrintColors"" (""Name"", ""Slug"", ""Hex"", ""SortOrder"") VALUES
                ('Білий',    'white',  '#e8e8e8', 1),
                ('Чорний',   'black',  '#1a1a2e', 2),
                ('Золотий',  'gold',   '#c9a84c', 3),
                ('Жовтий',   'yellow', '#FFD700', 4);
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""RibbonPrintColors"";");
    }
}
