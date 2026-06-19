using CodebookService.Data;
using CodebookService.DTOs;
using CodebookService.Models;
using Microsoft.EntityFrameworkCore;

namespace CodebookService.Repositories;

public class MunicipalityRepository : IMunicipalityRepository
{
    private readonly CodebookDbContext _db;

    private const string Hr = "hr";
    private const string En = "en";

    public MunicipalityRepository(CodebookDbContext db) => _db = db;

    public async Task<IEnumerable<MunicipalityResponse>> GetAllAsync(bool includeInactive = false, Guid? countyId = null)
    {
        var query = _db.Municipalities.AsQueryable();

        if (!includeInactive)
            query = query.Where(m => m.IsActive);

        if (countyId.HasValue)
            query = query.Where(m => m.CountyId == countyId.Value);

        return await (
            from m in query
            join county in _db.Counties on m.CountyId equals county.Id into cg
            from county in cg.DefaultIfEmpty()
            join country in _db.Countries on county.CountryId equals country.Id into crg
            from country in crg.DefaultIfEmpty()
            orderby country.Ordinal, county.Ordinal, m.Ordinal, m.Code
            select new MunicipalityResponse
            {
                Id        = m.Id,
                Code      = m.Code,
                IsActive  = m.IsActive,
                Ordinal   = m.Ordinal,
                CountyId  = m.CountyId,
                JOPPDCode = m.JOPPDCode,
                NameHr    = _db.Translations
                    .Where(t => t.EntityType   == "codebook_municipality"
                             && t.EntityId     == m.Id
                             && t.LanguageCode == Hr
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault() ?? m.Code,
                NameEn = _db.Translations
                    .Where(t => t.EntityType   == "codebook_municipality"
                             && t.EntityId     == m.Id
                             && t.LanguageCode == En
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault(),
                CountyNameHr = _db.Translations
                    .Where(t => t.EntityType   == "codebook_county"
                             && t.EntityId     == m.CountyId
                             && t.LanguageCode == Hr
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault(),
                CountryNameHr = _db.Translations
                    .Where(t => t.EntityType   == "codebook_country"
                             && t.EntityId     == county.CountryId
                             && t.LanguageCode == Hr
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault()
            }
        ).ToListAsync();
    }

    public async Task<MunicipalityResponse?> GetByIdAsync(Guid id)
    {
        return await (
            from m in _db.Municipalities
            where m.Id == id
            join county in _db.Counties on m.CountyId equals county.Id into cg
            from county in cg.DefaultIfEmpty()
            select new MunicipalityResponse
            {
                Id        = m.Id,
                Code      = m.Code,
                IsActive  = m.IsActive,
                Ordinal   = m.Ordinal,
                CountyId  = m.CountyId,
                JOPPDCode = m.JOPPDCode,
                NameHr    = _db.Translations
                    .Where(t => t.EntityType   == "codebook_municipality"
                             && t.EntityId     == m.Id
                             && t.LanguageCode == Hr
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault() ?? m.Code,
                NameEn = _db.Translations
                    .Where(t => t.EntityType   == "codebook_municipality"
                             && t.EntityId     == m.Id
                             && t.LanguageCode == En
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault(),
                CountyNameHr = _db.Translations
                    .Where(t => t.EntityType   == "codebook_county"
                             && t.EntityId     == m.CountyId
                             && t.LanguageCode == Hr
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault(),
                CountryNameHr = _db.Translations
                    .Where(t => t.EntityType   == "codebook_country"
                             && t.EntityId     == county.CountryId
                             && t.LanguageCode == Hr
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault()
            }
        ).FirstOrDefaultAsync();
    }

    public async Task<Municipality?> GetEntityByIdAsync(Guid id)
        => await _db.Municipalities.FindAsync(id);

    public async Task<Municipality> CreateAsync(Municipality municipality)
    {
        _db.Municipalities.Add(municipality);
        await _db.SaveChangesAsync();
        return municipality;
    }

    public async Task SaveChangesAsync()
        => await _db.SaveChangesAsync();

    /// <summary>
    /// Provjerava postoje li naselja koja referenciraju ovu općinu.
    /// </summary>
    public async Task<bool> HasReferencesAsync(Guid id)
        => await _db.Settlements.AnyAsync(s => s.MunicipalityId == id);

    public async Task DeleteAsync(Municipality municipality)
    {
        var translations = await _db.Translations
            .Where(t => t.EntityType == "codebook_municipality" && t.EntityId == municipality.Id)
            .ToListAsync();

        _db.Translations.RemoveRange(translations);
        _db.Municipalities.Remove(municipality);
        await _db.SaveChangesAsync();
    }
}
