namespace CodebookService.Models;

/// <summary>
/// Tablica podržanih jezika u sustavu.
///
/// Dodavanje novog jezika = jedan novi redak u ovoj tablici.
/// Ne zahtijeva izmjenu sheme niti koda šifarnika.
///
/// Seed data: hr (Hrvatski), en (English)
/// </summary>
public class Language : IAuditable
{
    /// <summary>Primarni ključ.</summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ISO 639-1 kod jezika (npr. "hr", "en", "de").
    /// Jedinstven, nepromjenjiv nakon kreiranja.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Naziv jezika na vlastitom jeziku (npr. "Hrvatski", "English").</summary>
    public string Name { get; set; } = string.Empty;

    // Audit kolone
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid UpdatedBy { get; set; }
}
