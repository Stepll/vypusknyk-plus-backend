using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VypusknykPlus.Application.Data;

#nullable disable

namespace VypusknykPlus.Application.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260427700000_AddRibbonEmblemsTable")]
public partial class AddRibbonEmblemsTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            CREATE TABLE ""RibbonEmblems"" (
                ""Id""         BIGSERIAL PRIMARY KEY,
                ""Name""       VARCHAR(100) NOT NULL,
                ""Slug""       VARCHAR(100) NOT NULL,
                ""SvgKey""     VARCHAR(500),
                ""IsActive""   BOOLEAN NOT NULL DEFAULT TRUE,
                ""SortOrder""  INTEGER NOT NULL DEFAULT 0,
                ""IsDeleted""  BOOLEAN NOT NULL DEFAULT FALSE,
                ""CreatedAt""  TIMESTAMP NOT NULL DEFAULT NOW(),
                ""UpdatedAt""  TIMESTAMP NOT NULL DEFAULT NOW()
            );

            INSERT INTO ""RibbonEmblems"" (""Name"", ""Slug"", ""SortOrder"") VALUES
                ('Дзвіночок', 'bell',    0),
                ('Зірка',     'star',    1),
                ('Диплом',    'diploma', 2),
                ('Серце',     'heart',   3),
                ('Факел',     'torch',   4),
                ('Зірка 3Д',  'star-3d', 5);
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""RibbonEmblems"";");
    }
}
