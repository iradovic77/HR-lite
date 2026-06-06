using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using CodebookService.Data;

#nullable disable

// =============================================================================
// MIGRACIJA: Normalizacija EntityType vrijednosti u translation tablici
// =============================================================================
//
// Što se mijenja:
//   Ispravlja EntityType vrijednosti koje ne odgovaraju nazivu DB tablice.
//   Pravilo: EntityType mora biti identičan PostgreSQL nazivu tablice entiteta.
//
// Poznati slučajevi pogrešnih vrijednosti:
//   'country' → 'codebook_country'
//   Ostale varijante (bez prefiksa 'codebook_') za sve šifarnike.
//
// Metoda:
//   JOIN translation s odgovarajućom entity tablicom po EntityId.
//   Svaki translation red koji pripada entitetu, a ima krivi EntityType,
//   dobiva ispravnu vrijednost.
//
// Primjena: dotnet ef database update  (ili automatski u Program.cs)
// Rollback:  nije podržan — vraćanje pogrešnih vrijednosti nema smisla.
// =============================================================================

namespace CodebookService.Data.Migrations;

/// <inheritdoc />
[DbContext(typeof(CodebookDbContext))]
[Migration("20260607000001_NormalizeEntityTypes")]
public partial class NormalizeEntityTypes : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            UPDATE hr_codebook.translation t
            SET    ""EntityType"" = 'codebook_country'
            FROM   hr_codebook.codebook_country c
            WHERE  t.""EntityId"" = c.""Id""
              AND  t.""EntityType"" != 'codebook_country'");

        migrationBuilder.Sql(@"
            UPDATE hr_codebook.translation t
            SET    ""EntityType"" = 'codebook_gender'
            FROM   hr_codebook.codebook_gender g
            WHERE  t.""EntityId"" = g.""Id""
              AND  t.""EntityType"" != 'codebook_gender'");

        migrationBuilder.Sql(@"
            UPDATE hr_codebook.translation t
            SET    ""EntityType"" = 'codebook_county'
            FROM   hr_codebook.codebook_county c
            WHERE  t.""EntityId"" = c.""Id""
              AND  t.""EntityType"" != 'codebook_county'");

        migrationBuilder.Sql(@"
            UPDATE hr_codebook.translation t
            SET    ""EntityType"" = 'codebook_municipality'
            FROM   hr_codebook.codebook_municipality m
            WHERE  t.""EntityId"" = m.""Id""
              AND  t.""EntityType"" != 'codebook_municipality'");

        migrationBuilder.Sql(@"
            UPDATE hr_codebook.translation t
            SET    ""EntityType"" = 'codebook_settlement'
            FROM   hr_codebook.codebook_settlement s
            WHERE  t.""EntityId"" = s.""Id""
              AND  t.""EntityType"" != 'codebook_settlement'");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Rollback nije podržan — vraćanje pogrešnih EntityType vrijednosti nije smisleno.
    }
}
