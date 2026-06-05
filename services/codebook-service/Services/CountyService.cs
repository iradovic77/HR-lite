using CodebookService.Data;
using CodebookService.DTOs;
using CodebookService.Models;
using CodebookService.Repositories;

namespace CodebookService.Services;

public class CountyService : ICountyService
{
    private readonly ICountyRepository _repo;
    private readonly CodebookDbContext _db;

    private static readonly Guid HrId = new("b0000000-0000-0000-0000-000000000001");
    private static readonly Guid EnId = new("b0000000-0000-0000-0000-000000000002");

    public CountyService(ICountyRepository repo, CodebookDbContext db)
    {
        _repo = repo;
        _db   = db;
    }

    public Task<IEnumerable<CountyResponse>> GetAllAsync(bool includeInactive = false, Guid? countryId = null)
        => _repo.GetAllAsync(includeInactive, countryId);

    public Task<CountyResponse?> GetByIdAsync(Guid id)
        => _repo.GetByIdAsync(id);

    public async Task<CountyResponse> CreateAsync(CreateCountyRequest req)
    {
        var county = new County
        {
            Id        = Guid.NewGuid(),
            Code      = req.Code.ToUpperInvariant(),
            IsActive  = req.IsActive,
            Ordinal   = req.Ordinal,
            CountryId = req.CountryId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.Empty,
            UpdatedBy = Guid.Empty
        };

        await _repo.CreateAsync(county);
        await UpsertTranslationsAsync(county.Id, req.NameHr, req.NameEn);

        return (await _repo.GetByIdAsync(county.Id))!;
    }

    public async Task<CountyResponse?> UpdateAsync(Guid id, UpdateCountyRequest req)
    {
        var county = await _repo.GetEntityByIdAsync(id);
        if (county is null) return null;

        county.Code      = req.Code.ToUpperInvariant();
        county.IsActive  = req.IsActive;
        county.Ordinal   = req.Ordinal;
        county.CountryId = req.CountryId;
        county.UpdatedAt = DateTime.UtcNow;
        county.UpdatedBy = Guid.Empty;

        await _repo.SaveChangesAsync();
        await UpsertTranslationsAsync(id, req.NameHr, req.NameEn);

        return await _repo.GetByIdAsync(id);
    }

    public async Task<CountyResponse?> ToggleActiveAsync(Guid id)
    {
        var county = await _repo.GetEntityByIdAsync(id);
        if (county is null) return null;

        county.IsActive  = !county.IsActive;
        county.UpdatedAt = DateTime.UtcNow;
        county.UpdatedBy = Guid.Empty;

        await _repo.SaveChangesAsync();
        return await _repo.GetByIdAsync(id);
    }

    public async Task<DeleteResult> DeleteAsync(Guid id)
    {
        var county = await _repo.GetEntityByIdAsync(id);
        if (county is null) return new DeleteResult(Found: false, HasReferences: false);

        if (await _repo.HasReferencesAsync(id))
            return new DeleteResult(Found: true, HasReferences: true);

        await _repo.DeleteAsync(county);
        return new DeleteResult(Found: true, HasReferences: false);
    }

    private async Task UpsertTranslationsAsync(Guid countyId, string nameHr, string? nameEn)
    {
        await UpsertOneAsync(countyId, HrId, "Name", nameHr);

        if (!string.IsNullOrWhiteSpace(nameEn))
            await UpsertOneAsync(countyId, EnId, "Name", nameEn);

        await _db.SaveChangesAsync();
    }

    private async Task UpsertOneAsync(Guid entityId, Guid languageId, string fieldName, string value)
    {
        var existing = _db.Translations.FirstOrDefault(t =>
            t.EntityType == "codebook_county" &&
            t.EntityId   == entityId          &&
            t.LanguageId == languageId        &&
            t.FieldName  == fieldName);

        if (existing is not null)
        {
            existing.Value     = value;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedBy = Guid.Empty;
        }
        else
        {
            _db.Translations.Add(new Translation
            {
                Id         = Guid.NewGuid(),
                EntityType = "codebook_county",
                EntityId   = entityId,
                LanguageId = languageId,
                FieldName  = fieldName,
                Value      = value,
                CreatedAt  = DateTime.UtcNow,
                UpdatedAt  = DateTime.UtcNow,
                CreatedBy  = Guid.Empty,
                UpdatedBy  = Guid.Empty
            });
        }
    }
}
