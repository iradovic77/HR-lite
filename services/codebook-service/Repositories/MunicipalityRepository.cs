using CodebookService.Data;
using CodebookService.DTOs;
using CodebookService.Models;
using Microsoft.EntityFrameworkCore;

namespace CodebookService.Repositories;

public class MunicipalityRepository : IMunicipalityRepository
{
    private readonly CodebookDbContext _db;

    private static readonly Guid HrId = new("b0000000-0000-0000-0000-000000000001");
    private static readonly Guid EnId = new("b0000000-0000-0000-0000-000000000002");

    public MunicipalityRepository(CodebookDbContext db) => _db = db;

    public async Task<IEnumerable<MunicipalityResponse>> GetAllAsync(bool includeInactive = false, Guid? countyId = null)
    {
        var query = _db.Municipalities.AsQueryable();

        if (!includeInactive)
            query = query.Where(m => m.IsActive);

        if (countyId.HasValue)
            query = query.Where(m => m.CountyId == countyId.Value);

        return await query
            .OrderBy(m => m.Ordinal)
            .ThenBy(m => m.Code)
            .Select(m => new MunicipalityResponse
            {
                Id       = m.Id,
                Code     = m.Code,
                IsActive = m.IsActive,
                Ordinal  = m.Ordinal,
                CountyId = m.CountyId,
                NameHr   = _db.Translations
                    .Where(t => t.EntityType == "codebook_municipality"
                             && t.EntityId   == m.Id
                             && t.LanguageId == HrId
                             && t.FieldName  == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault() ?? m.Code,
                NameEn = _db.Translations
                    .Where(t => t.EntityType == "codebook_municipality"
                             && t.EntityId   == m.Id
                             && t.LanguageId == EnId
                             && t.FieldName  == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault(),
                CountyNameHr = _db.Translations
                    .Where(t => t.EntityType == "codebook_county"
                             && t.EntityId   == m.CountyId
                             && t.LanguageId == HrId
                             && t.FieldName  == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault()
            })
            .ToListAsync();
    }

    public async Task<MunicipalityResponse?> GetByIdAsync(Guid id)
    {
        return await _db.Municipalities
            .Where(m => m.Id == id)
            .Select(m => new MunicipalityResponse
            {
                Id       = m.Id,
                Code     = m.Code,
                IsActive = m.IsActive,
                Ordinal  = m.Ordinal,
                CountyId = m.CountyId,
                NameHr   = _db.Translations
                    .Where(t => t.EntityType == "codebook_municipality"
                             && t.EntityId   == m.Id
                             && t.LanguageId == HrId
                             && t.FieldName  == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault() ?? m.Code,
                NameEn = _db.Translations
                    .Where(t => t.EntityType == "codebook_municipality"
                             && t.EntityId   == m.Id
                             && t.LanguageId == EnId
                             && t.FieldName  == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault(),
                CountyNameHr = _db.Translations
                    .Where(t => t.EntityType == "codebook_county"
                             && t.EntityId   == m.CountyId
                             && t.LanguageId == HrId
                             && t.FieldName  == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();
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
