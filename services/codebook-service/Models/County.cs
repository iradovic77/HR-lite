namespace CodebookService.Models;

/// <summary>
/// Šifarnik županija (codebook_county).
/// Administrativna jedinica unutar države.
/// Hijerarhija: Country → County → Municipality → Settlement
/// </summary>
public class County : CodebookBase
{
    /// <summary>
    /// Logički FK prema tablici codebook_country.
    /// Nema DB foreign key constraint — veza se provodi na aplikativnom nivou.
    /// </summary>
    public Guid CountryId { get; set; }
}
