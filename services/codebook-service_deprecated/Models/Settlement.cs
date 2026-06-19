namespace CodebookService.Models;

/// <summary>
/// Šifarnik naselja (codebook_settlement).
/// Najniža razina lokacijske hijerarhije.
/// Hijerarhija: Country → County → Municipality → Settlement
/// </summary>
public class Settlement : CodebookBase
{
    /// <summary>
    /// Logički FK prema tablici codebook_municipality.
    /// Nema DB foreign key constraint — veza se provodi na aplikativnom nivou.
    /// </summary>
    public Guid?   MunicipalityId { get; set; }
    public string? PostalNumber   { get; set; }
}
