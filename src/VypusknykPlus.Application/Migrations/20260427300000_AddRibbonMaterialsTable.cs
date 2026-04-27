using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VypusknykPlus.Application.Data;

#nullable disable

namespace VypusknykPlus.Application.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260427300000_AddRibbonMaterialsTable")]
public partial class AddRibbonMaterialsTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            CREATE TABLE ""RibbonMaterials"" (
                ""Id""             BIGSERIAL PRIMARY KEY,
                ""Name""           VARCHAR(100) NOT NULL,
                ""Slug""           VARCHAR(100) NOT NULL,
                ""PriceModifier""  NUMERIC(10,2) NOT NULL DEFAULT 0,
                ""IsActive""       BOOLEAN NOT NULL DEFAULT TRUE,
                ""SortOrder""      INTEGER NOT NULL DEFAULT 0,
                ""IsDeleted""      BOOLEAN NOT NULL DEFAULT FALSE,
                ""CreatedAt""      TIMESTAMP NOT NULL DEFAULT NOW(),
                ""UpdatedAt""      TIMESTAMP NOT NULL DEFAULT NOW()
            );

            INSERT INTO ""RibbonMaterials"" (""Name"", ""Slug"", ""SortOrder"") VALUES
                ('Атлас', 'atlas', 1),
                ('Шовк',  'silk',  2),
                ('Сатин', 'satin', 3);
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"DROP TABLE IF EXISTS ""RibbonMaterials"";");
    }
}
