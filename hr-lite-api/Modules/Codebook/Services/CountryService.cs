using HrLite.Modules.Codebook.DTOs;
using HrLite.Modules.Codebook.Models;
using HrLite.Modules.Codebook.Repositories;
using HrLite.Shared.Data;

namespace HrLite.Modules.Codebook.Services;

public class CountryService : ICountryService
{
    private readonly ICountryRepository _repo;
    private readonly AppDbContext _db;

    private const string Hr = "hr";
    private const string En = "en";

    public CountryService(ICountryRepository repo, AppDbContext db)
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
            Id       = Guid.NewGuid(),
            Code     = req.Code.ToUpperInvariant(),
            IsActive = req.IsActive,
            Ordinal  = req.Ordinal,
            CreatedBy = Guid.Empty,
            UpdatedBy = Guid.Empty
        };

        await _repo.CreateAsync(country);
        await UpsertTranslationsAsync(country.Id, req.NameHr, req.NameEn, req.CitizenshipHr, req.CitizenshipEn);

        return (await _repo.GetByIdAsync(country.Id))!;
    }

    public async Task<CountryResponse?> UpdateAsync(Guid id, UpdateCountryRequest req)
    {
        var country = await _repo.GetEntityByIdAsync(id);
        if (country is null) return null;

        country.Code      = req.Code.ToUpperInvariant();
        country.IsActive  = req.IsActive;
        country.Ordinal   = req.Ordinal;
        country.UpdatedBy = Guid.Empty;

        await _repo.SaveChangesAsync();
        await UpsertTranslationsAsync(id, req.NameHr, req.NameEn, req.CitizenshipHr, req.CitizenshipEn);

        return await _repo.GetByIdAsync(id);
    }

    public async Task<CountryResponse?> ToggleActiveAsync(Guid id)
    {
        var country = await _repo.GetEntityByIdAsync(id);
        if (country is null) return null;

        country.IsActive  = !country.IsActive;
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

    private async Task UpsertTranslationsAsync(Guid countryId, string nameHr, string? nameEn,
        string? citizenshipHr, string? citizenshipEn)
    {
        await UpsertOneAsync(countryId, Hr, "Name", nameHr);

        if (!string.IsNullOrWhiteSpace(nameEn))
            await UpsertOneAsync(countryId, En, "Name", nameEn);

        if (!string.IsNullOrWhiteSpace(citizenshipHr))
            await UpsertOneAsync(countryId, Hr, "Citizenship", citizenshipHr);

        if (!string.IsNullOrWhiteSpace(citizenshipEn))
            await UpsertOneAsync(countryId, En, "Citizenship", citizenshipEn);

        await _db.SaveChangesAsync();
    }

    private async Task UpsertOneAsync(Guid entityId, string languageCode, string fieldName, string value)
    {
        var existing = _db.Translations.FirstOrDefault(t =>
            t.EntityType   == "codebook_country" &&
            t.EntityId     == entityId           &&
            t.LanguageCode == languageCode       &&
            t.FieldName    == fieldName);

        if (existing is not null)
        {
            existing.Value    = value;
            existing.UpdatedBy = Guid.Empty;
        }
        else
        {
            _db.Translations.Add(new Translation
            {
                Id           = Guid.NewGuid(),
                EntityType   = "codebook_country",
                EntityId     = entityId,
                LanguageCode = languageCode,
                FieldName    = fieldName,
                Value        = value,
                CreatedBy    = Guid.Empty,
                UpdatedBy    = Guid.Empty
            });
        }
    }
}
