using CodebookService.Data;
using CodebookService.DTOs;
using CodebookService.Models;
using Microsoft.EntityFrameworkCore;

namespace CodebookService.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly CodebookDbContext _db;

    private const string Hr = "hr";
    private const string En = "en";

    public CountryRepository(CodebookDbContext db) => _db = db;

    public async Task<IEnumerable<CountryResponse>> GetAllAsync(bool includeInactive = false)
    {
        var query = _db.Countries.AsQueryable();

        if (!includeInactive)
            query = query.Where(c => c.IsActive);

        return await query
            .OrderBy(c => c.Ordinal)
            .ThenBy(c => c.Code)
            .Select(c => new CountryResponse
            {
                Id       = c.Id,
                Code     = c.Code,
                IsActive = c.IsActive,
                Ordinal  = c.Ordinal,
                NameHr   = _db.Translations
                    .Where(t => t.EntityType   == "codebook_country"
                             && t.EntityId     == c.Id
                             && t.LanguageCode == Hr
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault() ?? c.Code,
                NameEn = _db.Translations
                    .Where(t => t.EntityType   == "codebook_country"
                             && t.EntityId     == c.Id
                             && t.LanguageCode == En
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault(),
                CitizenshipHr = _db.Translations
                    .Where(t => t.EntityType   == "codebook_country"
                             && t.EntityId     == c.Id
                             && t.LanguageCode == Hr
                             && t.FieldName    == "Citizenship")
                    .Select(t => t.Value)
                    .FirstOrDefault(),
                CitizenshipEn = _db.Translations
                    .Where(t => t.EntityType   == "codebook_country"
                             && t.EntityId     == c.Id
                             && t.LanguageCode == En
                             && t.FieldName    == "Citizenship")
                    .Select(t => t.Value)
                    .FirstOrDefault()
            })
            .ToListAsync();
    }

    public async Task<CountryResponse?> GetByIdAsync(Guid id)
    {
        return await _db.Countries
            .Where(c => c.Id == id)
            .Select(c => new CountryResponse
            {
                Id       = c.Id,
                Code     = c.Code,
                IsActive = c.IsActive,
                Ordinal  = c.Ordinal,
                NameHr   = _db.Translations
                    .Where(t => t.EntityType   == "codebook_country"
                             && t.EntityId     == c.Id
                             && t.LanguageCode == Hr
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault() ?? c.Code,
                NameEn = _db.Translations
                    .Where(t => t.EntityType   == "codebook_country"
                             && t.EntityId     == c.Id
                             && t.LanguageCode == En
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault(),
                CitizenshipHr = _db.Translations
                    .Where(t => t.EntityType   == "codebook_country"
                             && t.EntityId     == c.Id
                             && t.LanguageCode == Hr
                             && t.FieldName    == "Citizenship")
                    .Select(t => t.Value)
                    .FirstOrDefault(),
                CitizenshipEn = _db.Translations
                    .Where(t => t.EntityType   == "codebook_country"
                             && t.EntityId     == c.Id
                             && t.LanguageCode == En
                             && t.FieldName    == "Citizenship")
                    .Select(t => t.Value)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Country?> GetEntityByIdAsync(Guid id)
        => await _db.Countries.FindAsync(id);

    public async Task<Country> CreateAsync(Country country)
    {
        _db.Countries.Add(country);
        await _db.SaveChangesAsync();
        return country;
    }

    public async Task SaveChangesAsync()
        => await _db.SaveChangesAsync();

    /// <summary>
    /// Provjerava postoje li županije koje referenciraju ovu državu.
    /// </summary>
    public async Task<bool> HasReferencesAsync(Guid id)
        => await _db.Counties.AnyAsync(c => c.CountryId == id);

    public async Task DeleteAsync(Country country)
    {
        var translations = await _db.Translations
            .Where(t => t.EntityType == "codebook_country" && t.EntityId == country.Id)
            .ToListAsync();

        _db.Translations.RemoveRange(translations);
        _db.Countries.Remove(country);
        await _db.SaveChangesAsync();
    }
}
