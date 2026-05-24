namespace CodebookService.Models;

/// <summary>
/// Šifarnik spola (codebook_gender).
/// Nasljeđuje CodebookBase — dobiva sve standardne kolone automatski.
///
/// Seed podaci definirani su u CodebookDbContext.OnModelCreating() putem HasData(),
/// a EF Core ih uključuje u generiranu migraciju kao InsertData() pozive.
/// </summary>
public class Gender : CodebookBase
{
    // Nema dodatnih kolona — sve što Gender treba već je u CodebookBase.
    // Ako bi npr. spol trebao imati BiologicalCode ili sl., dodalo bi se ovdje.
}
