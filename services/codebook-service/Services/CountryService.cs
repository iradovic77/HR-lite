using CodebookService.Data;
using CodebookService.DTOs;
using CodebookService.Models;
using CodebookService.Repositories;

namespace CodebookService.Services;

public class CountryService : ICountryService
{
    private readonly ICountryRepository _repo;
    private readonly CodebookDbContext  _db;

    private static readonly Guid HrId = new("b0000000-0000-0000-0000-000000000001");
    private static readonly Guid EnId = new("b0000000-0000-0000-0000-000000000002");

    public CountryService(ICountryRepository repo, CodebookDbContext db)
    {
        _repo = repo;
        _db   = db;
    }

    public Task<IEnumerable<CountryResponse>> GetAllAsync(bool includeInactive = false)
        => _repo.GetAllAsync(includeInactive);

    public Task<CountryResponse?> GetByIdAsync(Guid id)
        => _repo.GetByIdAsync(id);

    public async Task<CountryResponse> CreateAsync(CreateCountryRequest req)
    {
        var country = new Country
        {
            Id        = Guid.NewGuid(),
            Code      = req.Code.ToUpperInvariant(),
            IsActive  = req.IsActive,
            Ordinal   = req.Ordinal,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.Empty,
            UpdatedBy = Guid.Empty
        };

        await _repo.CreateAsync(country);
        await UpsertTranslationsAsync(country.Id, req.NameHr, req.NameEn);

        return (await _repo.GetByIdAsync(country.Id))!;
    }

    public async Task<CountryResponse?> UpdateAsync(Guid id, UpdateCountryRequest req)
    {
        var country = await _repo.GetEntityByIdAsync(id);
        if (country is null) return null;

        country.Code      = req.Code.ToUpperInvariant();
        country.IsActive  = req.IsActive;
        country.Ordinal   = req.Ordinal;
        country.UpdatedAt = DateTime.UtcNow;
        country.UpdatedBy = Guid.Empty;

        await _repo.SaveChangesAsync();
        await UpsertTranslationsAsync(id, req.NameHr, req.NameEn);

        return await _repo.GetByIdAsync(id);
    }

    public async Task<CountryResponse?> ToggleActiveAsync(Guid id)
    {
        var country = await _repo.GetEntityByIdAsync(id);
        if (country is null) return null;

        country.IsActive  = !country.IsActive;
        country.UpdatedAt = DateTime.UtcNow;
        country.UpdatedBy = Guid.Empty;

        await _repo.SaveChangesAsync();
        return await _repo.GetByIdAsync(id);
    }

    public async Task<DeleteResult> DeleteAsync(Guid id)
    {
        var country = await _repo.GetEntityByIdAsync(id);
        if (country is null) return new DeleteResult(Found: false, HasReferences: false);

        if (await _repo.HasReferencesAsync(id))
            return new DeleteResult(Found: true, HasReferences: true);

        await _repo.DeleteAsync(country);
        return new DeleteResult(Found: true, HasReferences: false);
    }

    private async Task UpsertTranslationsAsync(Guid countryId, string nameHr, string? nameEn)
    {
        await UpsertOneAsync(countryId, HrId, "Name", nameHr);

        if (!string.IsNullOrWhiteSpace(nameEn))
            await UpsertOneAsync(countryId, EnId, "Name", nameEn);

        await _db.SaveChangesAsync();
    }

    private async Task UpsertOneAsync(Guid entityId, Guid languageId, string fieldName, string value)
    {
        var existing = _db.Translations.FirstOrDefault(t =>
            t.EntityType == "codebook_country" &&
            t.EntityId   == entityId           &&
            t.LanguageId == languageId         &&
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
                EntityType = "codebook_country",
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
