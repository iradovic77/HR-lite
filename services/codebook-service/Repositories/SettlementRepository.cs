using CodebookService.Data;
using CodebookService.DTOs;
using CodebookService.Models;
using Microsoft.EntityFrameworkCore;

namespace CodebookService.Repositories;

public class SettlementRepository : ISettlementRepository
{
    private readonly CodebookDbContext _db;

    private const string Hr = "hr";
    private const string En = "en";

    public SettlementRepository(CodebookDbContext db) => _db = db;

    public async Task<IEnumerable<SettlementResponse>> GetAllAsync(bool includeInactive = false, Guid? municipalityId = null)
    {
        var query = _db.Settlements.AsQueryable();

        if (!includeInactive)
            query = query.Where(s => s.IsActive);

        if (municipalityId.HasValue)
            query = query.Where(s => s.MunicipalityId == municipalityId.Value);

        return await (
            from s in query
            join municipality in _db.Municipalities on s.MunicipalityId equals municipality.Id into mg
            from municipality in mg.DefaultIfEmpty()
            join county in _db.Counties on municipality.CountyId equals county.Id into cg
            from county in cg.DefaultIfEmpty()
            join country in _db.Countries on county.CountryId equals country.Id into crg
            from country in crg.DefaultIfEmpty()
            orderby country.Ordinal, county.Ordinal, municipality.Ordinal, s.Ordinal, s.Code
            select new SettlementResponse
            {
                Id             = s.Id,
                Code           = s.Code,
                IsActive       = s.IsActive,
                Ordinal        = s.Ordinal,
                MunicipalityId = s.MunicipalityId,
                NameHr         = _db.Translations
                    .Where(t => t.EntityType   == "codebook_settlement"
                             && t.EntityId     == s.Id
                             && t.LanguageCode == Hr
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault() ?? s.Code,
                NameEn = _db.Translations
                    .Where(t => t.EntityType   == "codebook_settlement"
                             && t.EntityId     == s.Id
                             && t.LanguageCode == En
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault(),
                MunicipalityNameHr = _db.Translations
                    .Where(t => t.EntityType   == "codebook_municipality"
                             && t.EntityId     == s.MunicipalityId
                             && t.LanguageCode == Hr
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault()
            }
        ).ToListAsync();
    }

    public async Task<SettlementResponse?> GetByIdAsync(Guid id)
    {
        return await _db.Settlements
            .Where(s => s.Id == id)
            .Select(s => new SettlementResponse
            {
                Id             = s.Id,
                Code           = s.Code,
                IsActive       = s.IsActive,
                Ordinal        = s.Ordinal,
                MunicipalityId = s.MunicipalityId,
                NameHr         = _db.Translations
                    .Where(t => t.EntityType   == "codebook_settlement"
                             && t.EntityId     == s.Id
                             && t.LanguageCode == Hr
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault() ?? s.Code,
                NameEn = _db.Translations
                    .Where(t => t.EntityType   == "codebook_settlement"
                             && t.EntityId     == s.Id
                             && t.LanguageCode == En
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault(),
                MunicipalityNameHr = _db.Translations
                    .Where(t => t.EntityType   == "codebook_municipality"
                             && t.EntityId     == s.MunicipalityId
                             && t.LanguageCode == Hr
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Settlement?> GetEntityByIdAsync(Guid id)
        => await _db.Settlements.FindAsync(id);

    public async Task<Settlement> CreateAsync(Settlement settlement)
    {
        _db.Settlements.Add(settlement);
        await _db.SaveChangesAsync();
        return settlement;
    }

    public async Task SaveChangesAsync()
        => await _db.SaveChangesAsync();

    /// <summary>
    /// Naselja su najniža razina hijerarhije — nema entiteta koji ih referenciraju.
    /// </summary>
    public Task<bool> HasReferencesAsync(Guid id)
        => Task.FromResult(false);

    public async Task DeleteAsync(Settlement settlement)
    {
        var translations = await _db.Translations
            .Where(t => t.EntityType == "codebook_settlement" && t.EntityId == settlement.Id)
            .ToListAsync();

        _db.Translations.RemoveRange(translations);
        _db.Settlements.Remove(settlement);
        await _db.SaveChangesAsync();
    }
}
