using CodebookService.Data;
using CodebookService.DTOs;
using CodebookService.Models;
using CodebookService.Repositories;

namespace CodebookService.Services;

public class MunicipalityService : IMunicipalityService
{
    private readonly IMunicipalityRepository _repo;
    private readonly CodebookDbContext       _db;

    private static readonly Guid HrId = new("b0000000-0000-0000-0000-000000000001");
    private static readonly Guid EnId = new("b0000000-0000-0000-0000-000000000002");

    public MunicipalityService(IMunicipalityRepository repo, CodebookDbContext db)
    {
        _repo = repo;
        _db   = db;
    }

    public Task<IEnumerable<MunicipalityResponse>> GetAllAsync(bool includeInactive = false, Guid? countyId = null)
        => _repo.GetAllAsync(includeInactive, countyId);

    public Task<MunicipalityResponse?> GetByIdAsync(Guid id)
        => _repo.GetByIdAsync(id);

    public async Task<MunicipalityResponse> CreateAsync(CreateMunicipalityRequest req)
    {
        var municipality = new Municipality
        {
            Id        = Guid.NewGuid(),
            Code      = req.Code.ToUpperInvariant(),
            IsActive  = req.IsActive,
            Ordinal   = req.Ordinal,
            CountyId  = req.CountyId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.Empty,
            UpdatedBy = Guid.Empty
        };

        await _repo.CreateAsync(municipality);
        await UpsertTranslationsAsync(municipality.Id, req.NameHr, req.NameEn);

        return (await _repo.GetByIdAsync(municipality.Id))!;
    }

    public async Task<MunicipalityResponse?> UpdateAsync(Guid id, UpdateMunicipalityRequest req)
    {
        var municipality = await _repo.GetEntityByIdAsync(id);
        if (municipality is null) return null;

        municipality.Code      = req.Code.ToUpperInvariant();
        municipality.IsActive  = req.IsActive;
        municipality.Ordinal   = req.Ordinal;
        municipality.CountyId  = req.CountyId;
        municipality.UpdatedAt = DateTime.UtcNow;
        municipality.UpdatedBy = Guid.Empty;

        await _repo.SaveChangesAsync();
        await UpsertTranslationsAsync(id, req.NameHr, req.NameEn);

        return await _repo.GetByIdAsync(id);
    }

    public async Task<MunicipalityResponse?> ToggleActiveAsync(Guid id)
    {
        var municipality = await _repo.GetEntityByIdAsync(id);
        if (municipality is null) return null;

        municipality.IsActive  = !municipality.IsActive;
        municipality.UpdatedAt = DateTime.UtcNow;
        municipality.UpdatedBy = Guid.Empty;

        await _repo.SaveChangesAsync();
        return await _repo.GetByIdAsync(id);
    }

    public async Task<DeleteResult> DeleteAsync(Guid id)
    {
        var municipality = await _repo.GetEntityByIdAsync(id);
        if (municipality is null) return new DeleteResult(Found: false, HasReferences: false);

        if (await _repo.HasReferencesAsync(id))
            return new DeleteResult(Found: true, HasReferences: true);

        await _repo.DeleteAsync(municipality);
        return new DeleteResult(Found: true, HasReferences: false);
    }

    private async Task UpsertTranslationsAsync(Guid municipalityId, string nameHr, string? nameEn)
    {
        await UpsertOneAsync(municipalityId, HrId, "Name", nameHr);

        if (!string.IsNullOrWhiteSpace(nameEn))
            await UpsertOneAsync(municipalityId, EnId, "Name", nameEn);

        await _db.SaveChangesAsync();
    }

    private async Task UpsertOneAsync(Guid entityId, Guid languageId, string fieldName, string value)
    {
        var existing = _db.Translations.FirstOrDefault(t =>
            t.EntityType == "codebook_municipality" &&
            t.EntityId   == entityId               &&
            t.LanguageId == languageId             &&
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
                EntityType = "codebook_municipality",
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
