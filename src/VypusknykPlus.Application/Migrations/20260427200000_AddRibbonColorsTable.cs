using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VypusknykPlus.Application.Data;

#nullable disable

namespace VypusknykPlus.Application.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260427200000_AddRibbonColorsTable")]
public partial class AddRibbonColorsTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            CREATE TABLE ""RibbonColors"" (
                ""Id""             BIGSERIAL PRIMARY KEY,
                ""Name""           VARCHAR(100) NOT NULL,
                ""Slug""           VARCHAR(100) NOT NULL,
                ""Hex""            VARCHAR(20)  NOT NULL,
                ""SecondaryHex""   VARCHAR(20)  NULL,
                ""PriceModifier""  NUMERIC(10,2) NOT NULL DEFAULT 0,
                ""IsActive""       BOOLEAN NOT NULL DEFAULT TRUE,
                ""SortOrder""      INTEGER NOT NULL DEFAULT 0,
                ""IsDeleted""      BOOLEAN NOT NULL DEFAULT FALSE,
                ""CreatedAt""      TIMESTAMP NOT NULL DEFAULT NOW(),
                ""UpdatedAt""      TIMESTAMP NOT NULL DEFAULT NOW()
            );

            INSERT INTO ""RibbonColors"" (""Name"", ""Slug"", ""Hex"", ""SecondaryHex"", ""SortOrder"") VALUES
                ('Синьо-жовтий', 'blue-yellow', '#1a56a0', '#FFD700', 1),
                ('Синій',        'blue',         '#1d4ed8', NULL,      2),
                ('Червоний',     'red',          '#dc2626', NULL,      3),
                ('Білий',        'white',        '#e8e8e8', NULL,      4),
                ('Бордовий',     'burgundy',     '#7f1d1d', NULL,      5),
                ('Айворі',       'ivory',        '#f5f0e8', NULL,      6),
                ('Золотий',      'gold',         '#c9a84c', NULL,      7),
                ('Срібний',      'silver',       '#9ca3af', NULL,      8);
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""RibbonColors"";");
    }
}
