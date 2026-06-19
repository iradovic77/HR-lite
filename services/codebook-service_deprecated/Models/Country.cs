namespace CodebookService.Models;

/// <summary>
/// Šifarnik država (codebook_country).
/// Code = ISO 3166-1 alpha-2 (npr. "BA", "HR", "RS").
/// </summary>
public class Country : CodebookBase
{
    // Vrh lokacijske hijerarhije — nema roditeljske veze.
    // Hijerarhija: Country → County → Municipality → Settlement
}
