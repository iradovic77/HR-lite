using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using CodebookService.Data;

#nullable disable

namespace CodebookService.Data.Migrations;

/// <inheritdoc />
[DbContext(typeof(CodebookDbContext))]
[Migration("20260607000002_MunicipalityAddJOPPDCode")]
public partial class MunicipalityAddJOPPDCode : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "JOPPDCode",
            schema: "hr_codebook",
            table: "codebook_municipality",
            type: "character varying(20)",
            maxLength: 20,
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "JOPPDCode",
            schema: "hr_codebook",
            table: "codebook_municipality");
    }
}
