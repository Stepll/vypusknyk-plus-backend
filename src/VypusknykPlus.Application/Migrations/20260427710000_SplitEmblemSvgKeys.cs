using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VypusknykPlus.Application.Data;

#nullable disable

namespace VypusknykPlus.Application.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260427710000_SplitEmblemSvgKeys")]
public partial class SplitEmblemSvgKeys : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            ALTER TABLE ""RibbonEmblems""
                DROP COLUMN IF EXISTS ""SvgKey"",
                ADD COLUMN ""SvgKeyLeft""  VARCHAR(500),
                ADD COLUMN ""SvgKeyRight"" VARCHAR(500);
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            ALTER TABLE ""RibbonEmblems""
                DROP COLUMN IF EXISTS ""SvgKeyLeft"",
                DROP COLUMN IF EXISTS ""SvgKeyRight"",
                ADD COLUMN ""SvgKey"" VARCHAR(500);
        ");
    }
}
