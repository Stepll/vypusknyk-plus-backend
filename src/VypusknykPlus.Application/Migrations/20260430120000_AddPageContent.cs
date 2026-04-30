using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VypusknykPlus.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddPageContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""PageContents"" (
                    ""Slug"" character varying(50) NOT NULL,
                    ""Data"" text NOT NULL,
                    ""UpdatedAt"" timestamp with time zone NOT NULL,
                    CONSTRAINT ""PK_PageContents"" PRIMARY KEY (""Slug"")
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PageContents");
        }
    }
}
