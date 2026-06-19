namespace CodebookService.Models;

/// <summary>
/// Apstraktna baza za sve šifarnik entitete.
///
/// Svaka šifarnik tablica nasljeđuje ovu klasu i time automatski dobiva
/// sve standardne kolone: Id, Code, IsActive, Ordinal i audit kolone.
///
/// VIŠEJEZIČNOST: Nazivi (Name) više nisu u ovoj klasi — čuvaju se u
/// Translation tablici. Svaki naziv se dohvaća putem upita:
///   SELECT Value FROM translation
///   WHERE EntityType = 'codebook_gender'
///     AND EntityId   = '{id}'
///     AND LanguageId = '{languageId}'
///     AND FieldName  = 'Name'
/// </summary>
public abstract class CodebookBase : IAuditable
{
    /// <summary>Primarni ključ — uvijek Guid, nikad int identity.</summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Kratka šifra zapisa (npr. "M", "BA", "FT").
    /// Neutralan identifikator koji se NIKAD ne prevodi.
    /// Stabilan — ne mijenja se nakon kreiranja.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Soft delete flag. false = zapis deaktiviran.
    /// Neaktivni zapisi se ne prikazuju u combo-ima, ali ostaju u bazi.
    /// NIKAD fizički brisati šifarnik zapise.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Custom redoslijed prikaza u combo-ima / dropdown-ima.
    /// Niži broj = više u listi. Zapisi s istim Ordinal sortiraju se po Code.
    /// </summary>
    public int Ordinal { get; set; } = 0;

    // Audit kolone — automatski postavljaju se u DbContext.SaveChanges()
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid UpdatedBy { get; set; }
}
