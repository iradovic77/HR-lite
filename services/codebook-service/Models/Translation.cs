namespace CodebookService.Models;

/// <summary>
/// Generička tablica prijevoda za SVE entitete u sustavu.
///
/// Umjesto da svaka tablica ima Name, NameEn, NameDe... kolone,
/// prijevodi se čuvaju centralno ovdje. Ovo omogućava dodavanje
/// novog jezika bez ikakve izmjene sheme ostalih tablica.
///
/// Primjer retka:
///   EntityType = "codebook_gender"
///   EntityId   = "a1000000-0000-0000-0000-000000000001"  ← Gender M
///   LanguageId = "b0000000-0000-0000-0000-000000000001"  ← hr
///   FieldName  = "Name"
///   Value      = "Muško"
///
/// Fallback logika (implementira se u servisnom sloju):
///   1. Traženi jezik (npr. "de") → ako ne postoji:
///   2. Hrvatski ("hr")           → ako ne postoji:
///   3. Code kolona entiteta      ← uvijek postoji
/// </summary>
public class Translation : IAuditable
{
    /// <summary>Primarni ključ.</summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Naziv entiteta koji se prevodi — odgovara nazivu tablice.
    /// Primjeri: "codebook_gender", "codebook_country", "codebook_leave_type"
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>Id retka koji se prevodi (PK entiteta).</summary>
    public Guid EntityId { get; set; }

    /// <summary>FK prema tablici language. Pravi DB constraint — u istoj shemi.</summary>
    public Guid LanguageId { get; set; }

    /// <summary>
    /// Naziv polja koje se prevodi.
    /// Trenutno uvijek "Name", ali dizajn dozvoljava prijevod više polja istog entiteta.
    /// </summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>Prevedeni tekst.</summary>
    public string Value { get; set; } = string.Empty;

    // Audit kolone
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid UpdatedBy { get; set; }
}
