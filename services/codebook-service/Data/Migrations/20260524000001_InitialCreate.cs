using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using CodebookService.Data;

#nullable disable

// =============================================================================
// EF CORE MIGRACIJA — Edukativni komentar
// =============================================================================
//
// Ova migracija se primjenjuje naredbom:
//   dotnet ef database update
// ili automatski pri pokretanju servisa (vidi Program.cs → db.Database.Migrate()).
//
// EF Core čita OnModelCreating() iz CodebookDbContext i generira:
//   CreateTable()  → za svaki DbSet
//   CreateIndex()  → za svaki HasIndex()
//   InsertData()   → za svaki HasData()
//   AddForeignKey()→ za svaki HasForeignKey() s pravim constraint-om
//
// ZAŠTO SU SEED ID-EVI HARDKODIRANI:
//   EF Core koristi ID-eve iz HasData() da prati koji su zapisi "seed".
//   Ako bismo koristili Guid.NewGuid(), svaka nova migracija bi generirala
//   nove INSERT-e jer bi stare ne mogla prepoznati — duplicirali bi se podaci.
//
// NAPOMENA O OVOJ MIGRACIJI:
//   Migracija je ručno ažurirana prije prvog deployementa (baza nije bila
//   pokrenuta) kako bi reflektirala novi Translation sustav višejezičnosti.
//   U produkcijskom okruženju s postojećom bazom koristili bismo novu
//   migraciju umjesto editiranja postojeće.
// =============================================================================

namespace CodebookService.Data.Migrations;

/// <inheritdoc />
[DbContext(typeof(CodebookDbContext))]
[Migration("20260524000001_InitialCreate")]
public partial class InitialCreate : Migration
{
    // Seed GUIDs — isti kao u DbContext-u, hardkodirani
    private static readonly Guid LangHrId  = new("b0000000-0000-0000-0000-000000000001");
    private static readonly Guid LangEnId  = new("b0000000-0000-0000-0000-000000000002");
    private static readonly Guid GenderMId = new("a1000000-0000-0000-0000-000000000001");
    private static readonly Guid GenderFId = new("a1000000-0000-0000-0000-000000000002");
    private static readonly Guid GenderOId = new("a1000000-0000-0000-0000-000000000003");
    private static readonly DateTime SeedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "hr_codebook");

        // ------------------------------------------------------------------
        // Tablica: language
        // Podržani jezici sustava. Novi jezik = novi redak, bez izmjene sheme.
        // ------------------------------------------------------------------
        migrationBuilder.CreateTable(
            name: "language",
            schema: "hr_codebook",
            columns: table => new
            {
                Id        = table.Column<Guid>(type: "uuid", nullable: false),
                Code      = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                Name      = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_language", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_language_Code",
            schema: "hr_codebook",
            table: "language",
            column: "Code",
            unique: true);

        // ------------------------------------------------------------------
        // Tablica: codebook_gender
        // Nema Name/NameEn — nazivi su u translation tablici.
        // ------------------------------------------------------------------
        migrationBuilder.CreateTable(
            name: "codebook_gender",
            schema: "hr_codebook",
            columns: table => new
            {
                Id        = table.Column<Guid>(type: "uuid", nullable: false),
                Code      = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
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
        // CountryId je logički FK — bez DB constraint-a između šifarnika.
        // ------------------------------------------------------------------
        migrationBuilder.CreateTable(
            name: "codebook_county",
            schema: "hr_codebook",
            columns: table => new
            {
                Id        = table.Column<Guid>(type: "uuid", nullable: false),
                Code      = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                CountryId = table.Column<Guid>(type: "uuid", nullable: false),
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
                // Nema ForeignKey → CountryId je logički FK, ne DB constraint
            });

        // ------------------------------------------------------------------
        // Tablica: codebook_municipality (Općina)
        // ------------------------------------------------------------------
        migrationBuilder.CreateTable(
            name: "codebook_municipality",
            schema: "hr_codebook",
            columns: table => new
            {
                Id       = table.Column<Guid>(type: "uuid", nullable: false),
                Code     = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                CountyId = table.Column<Guid>(type: "uuid", nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                Ordinal  = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
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
                Id             = table.Column<Guid>(type: "uuid", nullable: false),
                Code           = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                MunicipalityId = table.Column<Guid>(type: "uuid", nullable: false),
                IsActive       = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                Ordinal        = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                CreatedAt      = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt      = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy      = table.Column<Guid>(type: "uuid", nullable: false),
                UpdatedBy      = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_codebook_settlement", x => x.Id);
            });

        // ------------------------------------------------------------------
        // Tablica: translation
        // Generička tablica prijevoda za sve entitete.
        // LanguageId ima pravi FK constraint (ista shema — dozvoljeno).
        // ------------------------------------------------------------------
        migrationBuilder.CreateTable(
            name: "translation",
            schema: "hr_codebook",
            columns: table => new
            {
                Id         = table.Column<Guid>(type: "uuid", nullable: false),
                EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                EntityId   = table.Column<Guid>(type: "uuid", nullable: false),
                LanguageId = table.Column<Guid>(type: "uuid", nullable: false),
                FieldName  = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Value      = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                CreatedAt  = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt  = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy  = table.Column<Guid>(type: "uuid", nullable: false),
                UpdatedBy  = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_translation", x => x.Id);
                // FK prema language — pravi DB constraint (ista shema)
                table.ForeignKey(
                    name: "FK_translation_language_LanguageId",
                    column: x => x.LanguageId,
                    principalSchema: "hr_codebook",
                    principalTable: "language",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        // Unique index: jedan prijevod po (entitet, zapis, jezik, polje)
        migrationBuilder.CreateIndex(
            name: "IX_translation_EntityType_EntityId_LanguageId_FieldName",
            schema: "hr_codebook",
            table: "translation",
            columns: new[] { "EntityType", "EntityId", "LanguageId", "FieldName" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_translation_LanguageId",
            schema: "hr_codebook",
            table: "translation",
            column: "LanguageId");

        // ==================================================================
        // SEED DATA
        // ==================================================================

        // Jezici
        migrationBuilder.InsertData(
            schema: "hr_codebook",
            table: "language",
            columns: new[] { "Id", "Code", "Name", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy" },
            columnTypes: new[] { "uuid", "character varying(10)", "character varying(100)", "timestamp with time zone", "timestamp with time zone", "uuid", "uuid" },
            values: new object[,]
            {
                { LangHrId, "hr", "Hrvatski", SeedDate, SeedDate, Guid.Empty, Guid.Empty },
                { LangEnId, "en", "English",  SeedDate, SeedDate, Guid.Empty, Guid.Empty }
            });

        // Gender zapisi (samo Code/IsActive/Ordinal — bez Name)
        migrationBuilder.InsertData(
            schema: "hr_codebook",
            table: "codebook_gender",
            columns: new[] { "Id", "Code", "IsActive", "Ordinal", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy" },
            columnTypes: new[] { "uuid", "character varying(10)", "boolean", "integer", "timestamp with time zone", "timestamp with time zone", "uuid", "uuid" },
            values: new object[,]
            {
                { GenderMId, "M", true, 1, SeedDate, SeedDate, Guid.Empty, Guid.Empty },
                { GenderFId, "F", true, 2, SeedDate, SeedDate, Guid.Empty, Guid.Empty },
                { GenderOId, "O", true, 3, SeedDate, SeedDate, Guid.Empty, Guid.Empty }
            });

        // Prijevodi za Gender
        // Fallback logika (u servisnom sloju): traženi jezik → hr → Code
        // Hrvatski prijevod je OBAVEZAN. Engleski i ostali su opcionalni.
        migrationBuilder.InsertData(
            schema: "hr_codebook",
            table: "translation",
            columns: new[] { "Id", "EntityType", "EntityId", "LanguageId", "FieldName", "Value", "CreatedAt", "UpdatedAt", "CreatedBy", "UpdatedBy" },
            columnTypes: new[] { "uuid", "character varying(100)", "uuid", "uuid", "character varying(100)", "character varying(500)", "timestamp with time zone", "timestamp with time zone", "uuid", "uuid" },
            values: new object[,]
            {
                { new Guid("c0000000-0000-0000-0000-000000000001"), "codebook_gender", GenderMId, LangHrId, "Name", "Muško",   SeedDate, SeedDate, Guid.Empty, Guid.Empty },
                { new Guid("c0000000-0000-0000-0000-000000000002"), "codebook_gender", GenderMId, LangEnId, "Name", "Male",    SeedDate, SeedDate, Guid.Empty, Guid.Empty },
                { new Guid("c0000000-0000-0000-0000-000000000003"), "codebook_gender", GenderFId, LangHrId, "Name", "Žensko",  SeedDate, SeedDate, Guid.Empty, Guid.Empty },
                { new Guid("c0000000-0000-0000-0000-000000000004"), "codebook_gender", GenderFId, LangEnId, "Name", "Female",  SeedDate, SeedDate, Guid.Empty, Guid.Empty },
                { new Guid("c0000000-0000-0000-0000-000000000005"), "codebook_gender", GenderOId, LangHrId, "Name", "Ostalo",  SeedDate, SeedDate, Guid.Empty, Guid.Empty },
                { new Guid("c0000000-0000-0000-0000-000000000006"), "codebook_gender", GenderOId, LangEnId, "Name", "Other",   SeedDate, SeedDate, Guid.Empty, Guid.Empty }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Down() se poziva pri rollbacku: dotnet ef database update 0
        // Redoslijed: prvo tablice koje imaju FK-ove, zadnje parent tablice
        migrationBuilder.DropTable(name: "translation",           schema: "hr_codebook");
        migrationBuilder.DropTable(name: "codebook_gender",       schema: "hr_codebook");
        migrationBuilder.DropTable(name: "codebook_settlement",   schema: "hr_codebook");
        migrationBuilder.DropTable(name: "codebook_municipality", schema: "hr_codebook");
        migrationBuilder.DropTable(name: "codebook_county",       schema: "hr_codebook");
        migrationBuilder.DropTable(name: "codebook_country",      schema: "hr_codebook");
        migrationBuilder.DropTable(name: "language",              schema: "hr_codebook");
    }
}
