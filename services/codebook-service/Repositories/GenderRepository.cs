using CodebookService.Data;
using CodebookService.DTOs;
using CodebookService.Models;
using Microsoft.EntityFrameworkCore;

namespace CodebookService.Repositories;

public class GenderRepository : IGenderRepository
{
    private readonly CodebookDbContext _db;

    // Seed GUIDs za jezike — definirani u CodebookDbContext
    private static readonly Guid HrId = new("b0000000-0000-0000-0000-000000000001");
    private static readonly Guid EnId = new("b0000000-0000-0000-0000-000000000002");

    public GenderRepository(CodebookDbContext db) => _db = db;

    /// <summary>
    /// Vraća sve Gender zapise s prijevodima (hr i en).
    /// Prijevodi se dohvaćaju iz translation tablice — subquery po entitetu.
    /// Fallback logika: prijevod → hr → Code (implementirana u projekciji).
    /// </summary>
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
                // Fallback: prijevod → Code
                NameHr = _db.Translations
                    .Where(t => t.EntityType == "codebook_gender"
                             && t.EntityId   == g.Id
                             && t.LanguageId == HrId
                             && t.FieldName  == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault() ?? g.Code,
                NameEn = _db.Translations
                    .Where(t => t.EntityType == "codebook_gender"
                             && t.EntityId   == g.Id
                             && t.LanguageId == EnId
                             && t.FieldName  == "Name")
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
                    .Where(t => t.EntityType == "codebook_gender"
                             && t.EntityId   == g.Id
                             && t.LanguageId == HrId
                             && t.FieldName  == "Name")
                    .Select(t => t.Value)
                    .FirstOrDefault() ?? g.Code,
                NameEn = _db.Translations
                    .Where(t => t.EntityType == "codebook_gender"
                             && t.EntityId   == g.Id
                             && t.LanguageId == EnId
                             && t.FieldName  == "Name")
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
}
