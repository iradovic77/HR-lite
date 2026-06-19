using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using CodebookService.Data;

#nullable disable

// =============================================================================
// MIGRACIJA: Language.Code postaje primarni ključ
// =============================================================================
//
// Što se mijenja:
//   1. language.Id (uuid) se briše — Code (varchar 10) postaje PK
//   2. translation.LanguageId (uuid FK) zamjenjuje se s LanguageCode (varchar 10 FK)
//
// Zašto:
//   Code je ISO 639-1 kod ("hr", "en") — jedinstven, nepromjenjiv, čitljiv.
//   UUID kao PK je bio posredni sloj bez dodane vrijednosti. Direktni FK na
//   Code eliminira JOIN kad treba znati jezik prijevoda.
//
// Data migracija:
//   Korak 4 u Up() radi UPDATE translation SET LanguageCode = l.Code ...
//   Svi prijevodi se migriraju prije brisanja starog stupca.
//
// Primjena: dotnet ef database update  (ili automatski u Program.cs)
// Rollback:  dotnet ef database update 20260524000001_InitialCreate
// =============================================================================

namespace CodebookService.Data.Migrations;

/// <inheritdoc />
[DbContext(typeof(CodebookDbContext))]
[Migration("20260606000001_LanguageCodeAsPrimaryKey")]
public partial class LanguageCodeAsPrimaryKey : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // ------------------------------------------------------------------
        // 1. Ukloni FK i indekse koji referenciraju LanguageId
        // ------------------------------------------------------------------
        migrationBuilder.DropForeignKey(
            name: "FK_translation_language_LanguageId",
            schema: "hr_codebook",
            table: "translation");

        migrationBuilder.DropIndex(
            name: "IX_translation_EntityType_EntityId_LanguageId_FieldName",
            schema: "hr_codebook",
            table: "translation");

        migrationBuilder.DropIndex(
            name: "IX_translation_LanguageId",
            schema: "hr_codebook",
            table: "translation");

        // ------------------------------------------------------------------
        // 2. Dodaj stupac LanguageCode (nullable za početak — popunit ćemo ga)
        // ------------------------------------------------------------------
        migrationBuilder.AddColumn<string>(
            name: "LanguageCode",
            schema: "hr_codebook",
            table: "translation",
            type: "character varying(10)",
            maxLength: 10,
            nullable: true);

        // ------------------------------------------------------------------
        // 3. Migracija podataka: popuni LanguageCode iz language.Code
        // ------------------------------------------------------------------
        migrationBuilder.Sql(@"
            UPDATE hr_codebook.translation t
            SET    ""LanguageCode"" = l.""Code""
            FROM   hr_codebook.language l
            WHERE  t.""LanguageId"" = l.""Id""");

        // ------------------------------------------------------------------
        // 4. Postavi NOT NULL na LanguageCode
        // ------------------------------------------------------------------
        migrationBuilder.AlterColumn<string>(
            name: "LanguageCode",
            schema: "hr_codebook",
            table: "translation",
            type: "character varying(10)",
            maxLength: 10,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(10)",
            oldMaxLength: 10,
            oldNullable: true);

        // ------------------------------------------------------------------
        // 5. Briši stari stupac LanguageId iz translation
        // ------------------------------------------------------------------
        migrationBuilder.DropColumn(
            name: "LanguageId",
            schema: "hr_codebook",
            table: "translation");

        // ------------------------------------------------------------------
        // 6. Promijeni PK na language: ukloni stari PK i Id stupac
        // ------------------------------------------------------------------
        migrationBuilder.DropIndex(
            name: "IX_language_Code",
            schema: "hr_codebook",
            table: "language");

        migrationBuilder.DropPrimaryKey(
            name: "PK_language",
            schema: "hr_codebook",
            table: "language");

        migrationBuilder.DropColumn(
            name: "Id",
            schema: "hr_codebook",
            table: "language");

        // ------------------------------------------------------------------
        // 7. Code postaje PK na language
        // ------------------------------------------------------------------
        migrationBuilder.AddPrimaryKey(
            name: "PK_language",
            schema: "hr_codebook",
            table: "language",
            column: "Code");

        // ------------------------------------------------------------------
        // 8. Dodaj FK i indekse na translation.LanguageCode
        // ------------------------------------------------------------------
        migrationBuilder.AddForeignKey(
            name: "FK_translation_language_LanguageCode",
            schema: "hr_codebook",
            table: "translation",
            column: "LanguageCode",
            principalSchema: "hr_codebook",
            principalTable: "language",
            principalColumn: "Code",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.CreateIndex(
            name: "IX_translation_EntityType_EntityId_LanguageCode_FieldName",
            schema: "hr_codebook",
            table: "translation",
            columns: new[] { "EntityType", "EntityId", "LanguageCode", "FieldName" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_translation_LanguageCode",
            schema: "hr_codebook",
            table: "translation",
            column: "LanguageCode");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Rollback: vraćamo uuid Id na language i LanguageId na translation

        // ------------------------------------------------------------------
        // 1. Ukloni FK i indekse koji referenciraju LanguageCode
        // ------------------------------------------------------------------
        migrationBuilder.DropForeignKey(
            name: "FK_translation_language_LanguageCode",
            schema: "hr_codebook",
            table: "translation");

        migrationBuilder.DropIndex(
            name: "IX_translation_EntityType_EntityId_LanguageCode_FieldName",
            schema: "hr_codebook",
            table: "translation");

        migrationBuilder.DropIndex(
            name: "IX_translation_LanguageCode",
            schema: "hr_codebook",
            table: "translation");

        // ------------------------------------------------------------------
        // 2. Vrati PK na language: dodaj Id stupac i postavi PK
        // ------------------------------------------------------------------
        migrationBuilder.DropPrimaryKey(
            name: "PK_language",
            schema: "hr_codebook",
            table: "language");

        migrationBuilder.AddColumn<Guid>(
            name: "Id",
            schema: "hr_codebook",
            table: "language",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

        // Postavi originalne GUIDs za seed zapise
        migrationBuilder.Sql(@"
            UPDATE hr_codebook.language SET ""Id"" = 'b0000000-0000-0000-0000-000000000001' WHERE ""Code"" = 'hr';
            UPDATE hr_codebook.language SET ""Id"" = 'b0000000-0000-0000-0000-000000000002' WHERE ""Code"" = 'en';");

        migrationBuilder.AddPrimaryKey(
            name: "PK_language",
            schema: "hr_codebook",
            table: "language",
            column: "Id");

        migrationBuilder.CreateIndex(
            name: "IX_language_Code",
            schema: "hr_codebook",
            table: "language",
            column: "Code",
            unique: true);

        // ------------------------------------------------------------------
        // 3. Dodaj LanguageId stupac na translation (nullable za početak)
        // ------------------------------------------------------------------
        migrationBuilder.AddColumn<Guid>(
            name: "LanguageId",
            schema: "hr_codebook",
            table: "translation",
            type: "uuid",
            nullable: true);

        // Migracija podataka: popuni LanguageId iz language.Id via Code
        migrationBuilder.Sql(@"
            UPDATE hr_codebook.translation t
            SET    ""LanguageId"" = l.""Id""
            FROM   hr_codebook.language l
            WHERE  t.""LanguageCode"" = l.""Code""");

        migrationBuilder.AlterColumn<Guid>(
            name: "LanguageId",
            schema: "hr_codebook",
            table: "translation",
            type: "uuid",
            nullable: false,
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldNullable: true);

        // ------------------------------------------------------------------
        // 4. Briši LanguageCode stupac iz translation
        // ------------------------------------------------------------------
        migrationBuilder.DropColumn(
            name: "LanguageCode",
            schema: "hr_codebook",
            table: "translation");

        // ------------------------------------------------------------------
        // 5. Vrati FK i indekse na LanguageId
        // ------------------------------------------------------------------
        migrationBuilder.AddForeignKey(
            name: "FK_translation_language_LanguageId",
            schema: "hr_codebook",
            table: "translation",
            column: "LanguageId",
            principalSchema: "hr_codebook",
            principalTable: "language",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

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
    }
}
