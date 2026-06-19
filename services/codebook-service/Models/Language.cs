namespace CodebookService.Models;

/// <summary>
/// Tablica podržanih jezika u sustavu.
///
/// Code (ISO 639-1) je primarni ključ — jedinstven, nepromjenjiv,
/// čitljiv bez JOIN-a. Dodavanje novog jezika = jedan novi redak.
/// Bez izmjene sheme niti koda šifarnika.
///
/// Seed data: hr (Hrvatski), en (English)
/// </summary>
public class Language : IAuditable
{
    /// <summary>ISO 639-1 kod jezika (npr. "hr", "en", "de") — primarni ključ.</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Naziv jezika na vlastitom jeziku (npr. "Hrvatski", "English").</summary>
    public string Name { get; set; } = string.Empty;

    // Audit kolone
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid UpdatedBy { get; set; }
}
