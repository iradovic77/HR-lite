using HrLite.Modules.Codebook.DTOs;
using HrLite.Modules.Codebook.Models;
using HrLite.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace HrLite.Modules.Codebook.Repositories;

public class GenderRepository : IGenderRepository
{
    private readonly AppDbContext _db;

    private const string Hr = "hr";
    private const string En = "en";

    public GenderRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<GenderResponse>> GetAllAsync(bool includeInactive = false)
    {
        var query = _db.Genders.AsQueryable();

        if (!includeInactive)
            query = query.Where(g => g.IsActive);

        return await query
            .OrderBy(g => g.Ordinal)
            .ThenBy(g => g.Code)
            .Select(g => new GenderResponse
            {
                Id       = g.Id,
                Code     = g.Code,
                IsActive = g.IsActive,
                Ordinal  = g.Ordinal,
                NameHr   = _db.Translations
                    .Where(t => t.EntityType   == "codebook_gender"
                             && t.EntityId     == g.Id
                             && t.LanguageCode == Hr
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault() ?? g.Code,
                NameEn = _db.Translations
                    .Where(t => t.EntityType   == "codebook_gender"
                             && t.EntityId     == g.Id
                             && t.LanguageCode == En
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault()
            })
            .ToListAsync();
    }

    public async Task<GenderResponse?> GetByIdAsync(Guid id)
    {
        return await _db.Genders
            .Where(g => g.Id == id)
            .Select(g => new GenderResponse
            {
                Id       = g.Id,
                Code     = g.Code,
                IsActive = g.IsActive,
                Ordinal  = g.Ordinal,
                NameHr   = _db.Translations
                    .Where(t => t.EntityType   == "codebook_gender"
                             && t.EntityId     == g.Id
                             && t.LanguageCode == Hr
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault() ?? g.Code,
                NameEn = _db.Translations
                    .Where(t => t.EntityType   == "codebook_gender"
                             && t.EntityId     == g.Id
                             && t.LanguageCode == En
                             && t.FieldName    == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Gender?> GetEntityByIdAsync(Guid id)
        => await _db.Genders.FindAsync(id);

    public async Task<Gender> CreateAsync(Gender gender)
    {
        _db.Genders.Add(gender);
        await _db.SaveChangesAsync();
        return gender;
    }

    public async Task SaveChangesAsync()
        => await _db.SaveChangesAsync();

    public Task<bool> HasReferencesAsync(Guid id)
        => Task.FromResult(false);

    public async Task DeleteAsync(Gender gender)
    {
        var translations = await _db.Translations
            .Where(t => t.EntityType == "codebook_gender" && t.EntityId == gender.Id)
            .ToListAsync();

        _db.Translations.RemoveRange(translations);
        _db.Genders.Remove(gender);
        await _db.SaveChangesAsync();
    }
}
