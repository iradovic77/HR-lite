namespace CodebookService.Models;

/// <summary>
/// Šifarnik općina (codebook_municipality).
/// Administrativna jedinica unutar županije.
/// Hijerarhija: Country → County → Municipality → Settlement
/// </summary>
public class Municipality : CodebookBase
{
    /// <summary>
    /// Logički FK prema tablici codebook_county.
    /// Nema DB foreign key constraint — veza se provodi na aplikativnom nivou.
    /// </summary>
    public Guid? CountyId { get; set; }
}
