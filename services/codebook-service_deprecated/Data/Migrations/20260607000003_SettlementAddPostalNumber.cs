using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using CodebookService.Data;

#nullable disable

namespace CodebookService.Data.Migrations;

/// <inheritdoc />
[DbContext(typeof(CodebookDbContext))]
[Migration("20260607000003_SettlementAddPostalNumber")]
public partial class SettlementAddPostalNumber : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "PostalNumber",
            schema: "hr_codebook",
            table: "codebook_settlement",
            type: "character varying(20)",
            maxLength: 20,
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "PostalNumber",
            schema: "hr_codebook",
            table: "codebook_settlement");
    }
}
