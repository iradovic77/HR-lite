using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

// =============================================================================
// EF CORE MIGRACIJA — Edukativni komentar
// =============================================================================
//
// Ovaj fajl je generiran naredbom:
//   dotnet ef migrations add InitialCreate
//
// EF Core čita model iz CodebookDbContext.OnModelCreating() i generira:
//   - CreateTable()  → za svaki DbSet koji ne postoji u bazi
//   - InsertData()   → za svaki HasData() poziv u OnModelCreating()
//   - CreateIndex()  → za svaki HasIndex() poziv
//
// Migracija se primjenjuje naredbom:
//   dotnet ef database update
// ili automatski pri pokretanju servisa (vidi Program.cs → app.MigrateDatabase()).
//
// NIKAD ručno mijenjati ovaj fajl — svaka izmjena modela treba novu migraciju.
// =============================================================================

namespace CodebookService.Data.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Kreiranje sheme ako ne postoji
        migrationBuilder.EnsureSchema(name: "hr_codebook");

        // ------------------------------------------------------------------
        // Tablica: codebook_gender
        // ------------------------------------------------------------------
        migrationBuilder.CreateTable(
            name: "codebook_gender",
            schema: "hr_codebook",
            columns: table => new
            {
                Id        = table.Column<Guid>(type: "uuid", nullable: false),
                Code      = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                Name      = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                NameEn    = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                IsActive  = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                Ordinal   = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_codebook_gender", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_codebook_gender_Code",
            schema: "hr_codebook",
            table: "codebook_gender",
            column: "Code",
            unique: true);

        // ------------------------------------------------------------------
        // Tablica: codebook_country
        // ------------------------------------------------------------------
        migrationBuilder.CreateTable(
            name: "codebook_country",
            schema: "hr_codebook",
            columns: table => new
            {
                Id        = table.Column<Guid>(type: "uuid", nullable: false),
                Code      = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                Name      = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                NameEn    = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                IsActive  = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                Ordinal   = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_codebook_country", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_codebook_country_Code",
            schema: "hr_codebook",
            table: "codebook_country",
            column: "Code",
            unique: true);

        // ------------------------------------------------------------------
        // Tablica: codebook_county (Županija)
        // ------------------------------------------------------------------
        migrationBuilder.CreateTable(
            name: "codebook_county",
            schema: "hr_codebook",
            columns: table => new
            {
                Id        = table.Column<Guid>(type: "uuid", nullable: false),
                Code      = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                Name      = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                NameEn    = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                CountryId = table.Column<Guid>(type: "uuid", nullable: false),   // logički FK, bez constraint-a
                IsActive  = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                Ordinal   = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_codebook_county", x => x.Id);
                // Nema ForeignKey prema codebook_country — logička veza, ne DB constraint
            });

        // ------------------------------------------------------------------
        // Tablica: codebook_municipality (Općina)
        // ------------------------------------------------------------------
        migrationBuilder.CreateTable(
            name: "codebook_municipality",
            schema: "hr_codebook",
            columns: table => new
            {
                Id           = table.Column<Guid>(type: "uuid", nullable: false),
                Code         = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                Name         = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                NameEn       = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                CountyId     = table.Column<Guid>(type: "uuid", nullable: false),   // logički FK
                IsActive     = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                Ordinal      = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                CreatedAt    = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt    = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy    = table.Column<Guid>(type: "uuid", nullable: false),
                UpdatedBy    = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_codebook_municipality", x => x.Id);
            });

        // ------------------------------------------------------------------
        // Tablica: codebook_settlement (Naselje)
        // ------------------------------------------------------------------
        migrationBuilder.CreateTable(
            name: "codebook_settlement",
            schema: "hr_codebook",
            columns: table => new
            {
                Id               = table.Column<Guid>(type: "uuid", nullable: false),
                Code             = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                Name             = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                NameEn           = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                MunicipalityId   = table.Column<Guid>(type: "uuid", nullable: false),   // logički FK
                IsActive         = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                Ordinal          = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                CreatedAt        = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt        = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy        = table.Column<Guid>(type: "uuid", nullable: false),
                UpdatedBy        = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_codebook_settlement", x => x.Id);
            });

        // ==================================================================
        // SEED DATA — InsertData() pozivi
        //
        // EF Core generira InsertData() za svaki HasData() poziv u DbContext-u.
        //
        // Kako funkcionira:
        //  - Pri prvoj migraciji: INSERT zapisi ako ne postoje
        //  - Ako se seed promijeni u DbContext: nova migracija generira UPDATE
        //  - Ako se seed izbriše iz HasData(): nova migracija generira DELETE
        //
        // Zato su ID-evi hardkodirani — EF Core ih koristi za praćenje
        // koji je seed zapis koji, između migracija.
        // ==================================================================
        migrationBuilder.InsertData(
            schema: "hr_codebook",
            table: "codebook_gender",
            columns: new[] { "Id", "Code", "Name", "NameEn", "IsActive", "Ordinal", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy" },
            values: new object[,]
            {
                {
                    new Guid("a1000000-0000-0000-0000-000000000001"),
                    "M", "Muško", "Male", true, 1,
                    new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Guid.Empty, Guid.Empty
                },
                {
                    new Guid("a1000000-0000-0000-0000-000000000002"),
                    "F", "Žensko", "Female", true, 2,
                    new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Guid.Empty, Guid.Empty
                },
                {
                    new Guid("a1000000-0000-0000-0000-000000000003"),
                    "O", "Ostalo", "Other", true, 3,
                    new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Guid.Empty, Guid.Empty
                }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Down() se poziva pri: dotnet ef database update <PreviousMigration>
        // Briše sve što je Up() kreirao — u obrnutom redoslijedu.
        migrationBuilder.DropTable(name: "codebook_gender",       schema: "hr_codebook");
        migrationBuilder.DropTable(name: "codebook_settlement",   schema: "hr_codebook");
        migrationBuilder.DropTable(name: "codebook_municipality", schema: "hr_codebook");
        migrationBuilder.DropTable(name: "codebook_county",       schema: "hr_codebook");
        migrationBuilder.DropTable(name: "codebook_country",      schema: "hr_codebook");
    }
}
