using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VypusknykPlus.Application.Data;

#nullable disable

namespace VypusknykPlus.Application.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260428100000_AddConstructorRulesTables")]
public partial class AddConstructorRulesTables : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            CREATE TABLE ""ConstructorIncompatibilities"" (
                ""Id""        BIGSERIAL PRIMARY KEY,
                ""TypeA""     VARCHAR(50)  NOT NULL,
                ""SlugA""     VARCHAR(100) NOT NULL,
                ""TypeB""     VARCHAR(50)  NOT NULL,
                ""IsWarning"" BOOLEAN      NOT NULL DEFAULT FALSE,
                ""Message""   TEXT,
                ""IsDeleted"" BOOLEAN      NOT NULL DEFAULT FALSE,
                ""CreatedAt"" TIMESTAMP    NOT NULL DEFAULT NOW(),
                ""UpdatedAt"" TIMESTAMP    NOT NULL DEFAULT NOW()
            );

            CREATE TABLE ""ConstructorIncompatibilityTargets"" (
                ""Id""     BIGSERIAL PRIMARY KEY,
                ""RuleId"" BIGINT       NOT NULL REFERENCES ""ConstructorIncompatibilities""(""Id"") ON DELETE CASCADE,
                ""SlugB""  VARCHAR(100) NOT NULL
            );

            CREATE TABLE ""ConstructorForcedTexts"" (
                ""Id""          BIGSERIAL PRIMARY KEY,
                ""TriggerType"" VARCHAR(50)  NOT NULL,
                ""TriggerSlug"" VARCHAR(100) NOT NULL,
                ""TargetField"" VARCHAR(50)  NOT NULL,
                ""Message""     TEXT,
                ""IsDeleted""   BOOLEAN      NOT NULL DEFAULT FALSE,
                ""CreatedAt""   TIMESTAMP    NOT NULL DEFAULT NOW(),
                ""UpdatedAt""   TIMESTAMP    NOT NULL DEFAULT NOW()
            );

            CREATE TABLE ""ConstructorForcedTextValues"" (
                ""Id""     BIGSERIAL PRIMARY KEY,
                ""RuleId"" BIGINT NOT NULL REFERENCES ""ConstructorForcedTexts""(""Id"") ON DELETE CASCADE,
                ""Value""  TEXT   NOT NULL
            );
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            DROP TABLE IF EXISTS ""ConstructorForcedTextValues"";
            DROP TABLE IF EXISTS ""ConstructorForcedTexts"";
            DROP TABLE IF EXISTS ""ConstructorIncompatibilityTargets"";
            DROP TABLE IF EXISTS ""ConstructorIncompatibilities"";
        ");
    }
}
