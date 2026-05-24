namespace CodebookService.Models;

/// <summary>
/// Apstraktna baza za sve šifarnik entitete.
/// Svaka šifarnik tablica nasljeđuje ovu klasu i time automatski dobiva
/// sve standardne kolone: identifikator, šifru, naziv, soft-delete i redoslijed.
/// </summary>
public abstract class CodebookBase
{
    /// <summary>Primarni ključ — uvijek Guid, nikad int identity.</summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Kratka šifra zapisa (npr. "M", "BA", "FT").
    /// Koristi se za programsku identifikaciju — stabilna i ne mijenja se.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Naziv na lokalnom jeziku (hr/bs).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Naziv na engleskom — za buduću višejezičnost.</summary>
    public string? NameEn { get; set; }

    // ---------------------------------------------------------------
    // Šifarnik-specifične kolone (obavezne po CLAUDE.md pravilima)
    // ---------------------------------------------------------------

    /// <summary>
    /// Soft delete flag. IsActive = false znači da je zapis deaktiviran.
    /// Neaktivni zapisi se ne prikazuju u combo-ima, ali ostaju u bazi.
    /// NIKAD fizički brisati šifarnik zapise.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Custom redoslijed prikaza u combo-ima / dropdown-ima.
    /// Niži broj = više u listi. Zapisi s istim Ordinal sortiraju se po Name.
    /// </summary>
    public int Ordinal { get; set; } = 0;

    // ---------------------------------------------------------------
    // Audit kolone (obavezne za sve tablice po CLAUDE.md pravilima)
    // ---------------------------------------------------------------

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    /// <summary>UserId korisnika koji je kreirao zapis. Guid.Empty = sistem (seed data).</summary>
    public Guid CreatedBy { get; set; }

    /// <summary>UserId korisnika koji je zadnji izmijenio zapis. Guid.Empty = sistem.</summary>
    public Guid UpdatedBy { get; set; }
}
