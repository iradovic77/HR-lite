using CodebookService.Data;
using CodebookService.DTOs;
using CodebookService.Models;
using Microsoft.EntityFrameworkCore;

namespace CodebookService.Repositories;

public class CountyRepository : ICountyRepository
{
    private readonly CodebookDbContext _db;

    private const string Hr = "hr";
    private const string En = "en";

    public CountyRepository(CodebookDbContext db) => _db = db;

    public async Task<IEnumerable<CountyResponse>> GetAllAsync(bool includeInactive = false, Guid? countryId = null)
    {
        var query = _db.Counties.AsQueryable();

        if (!includeInactive)
            query = query.Where(c => c.IsActive);

        if (countryId.HasValue)
            query = query.Where(c => c.CountryId == countryId.Value);

        return await query
            .OrderBy(c => c.Ordinal)
            .ThenBy(c => c.Code)
            .Select(c => new CountyResponse
            {
                Id       = c.Id,
                Code     = c.Code,
                IsActive = c.IsActive,
                Ordinal  = c.Ordinal,
                CountryId = c.CountryId,
                NameHr   = _db.Translations
                    .Where(t => t.EntityType   == "codebook_county"
                             && t.EntityId     == c.Id
                             && t.LanguageCode == Hr
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault() ?? c.Code,
                NameEn = _db.Translations
                    .Where(t => t.EntityType   == "codebook_county"
                             && t.EntityId     == c.Id
                             && t.LanguageCode == En
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault(),
                CountryNameHr = _db.Translations
                    .Where(t => t.EntityType   == "codebook_country"
                             && t.EntityId     == c.CountryId
                             && t.LanguageCode == Hr
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault()
            })
            .ToListAsync();
    }

    public async Task<CountyResponse?> GetByIdAsync(Guid id)
    {
        return await _db.Counties
            .Where(c => c.Id == id)
            .Select(c => new CountyResponse
            {
                Id        = c.Id,
                Code      = c.Code,
                IsActive  = c.IsActive,
                Ordinal   = c.Ordinal,
                CountryId = c.CountryId,
                NameHr    = _db.Translations
                    .Where(t => t.EntityType   == "codebook_county"
                             && t.EntityId     == c.Id
                             && t.LanguageCode == Hr
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault() ?? c.Code,
                NameEn = _db.Translations
                    .Where(t => t.EntityType   == "codebook_county"
                             && t.EntityId     == c.Id
                             && t.LanguageCode == En
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault(),
                CountryNameHr = _db.Translations
                    .Where(t => t.EntityType   == "codebook_country"
                             && t.EntityId     == c.CountryId
                             && t.LanguageCode == Hr
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<County?> GetEntityByIdAsync(Guid id)
        => await _db.Counties.FindAsync(id);

    public async Task<County> CreateAsync(County county)
    {
        _db.Counties.Add(county);
        await _db.SaveChangesAsync();
        return county;
    }

    public async Task SaveChangesAsync()
        => await _db.SaveChangesAsync();

    /// <summary>
    /// Provjerava postoje li općine koje referenciraju ovu županiju.
    /// </summary>
    public async Task<bool> HasReferencesAsync(Guid id)
        => await _db.Municipalities.AnyAsync(m => m.CountyId == id);

    public async Task DeleteAsync(County county)
    {
        var translations = await _db.Translations
            .Where(t => t.EntityType == "codebook_county" && t.EntityId == county.Id)
            .ToListAsync();

        _db.Translations.RemoveRange(translations);
        _db.Counties.Remove(county);
        await _db.SaveChangesAsync();
    }
}
