using CodebookService.Data;
using CodebookService.DTOs;
using CodebookService.Models;
using CodebookService.Repositories;

namespace CodebookService.Services;

public class GenderService : IGenderService
{
    private readonly IGenderRepository _repo;
    private readonly CodebookDbContext _db;

    private const string Hr = "hr";
    private const string En = "en";

    public GenderService(IGenderRepository repo, CodebookDbContext db)
    {
        _repo = repo;
        _db   = db;
    }

    public Task<IEnumerable<GenderResponse>> GetAllAsync(bool includeInactive = false)
        => _repo.GetAllAsync(includeInactive);

    public Task<GenderResponse?> GetByIdAsync(Guid id)
        => _repo.GetByIdAsync(id);

    public async Task<GenderResponse> CreateAsync(CreateGenderRequest req)
    {
        var gender = new Gender
        {
            Id        = Guid.NewGuid(),
            Code      = req.Code.ToUpperInvariant(),
            IsActive  = req.IsActive,
            Ordinal   = req.Ordinal,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.Empty,   // TODO: zamijeniti s UserId iz JWT claims
            UpdatedBy = Guid.Empty
        };

        await _repo.CreateAsync(gender);
        await UpsertTranslationsAsync(gender.Id, req.NameHr, req.NameEn);

        return (await _repo.GetByIdAsync(gender.Id))!;
    }

    public async Task<GenderResponse?> UpdateAsync(Guid id, UpdateGenderRequest req)
    {
        var gender = await _repo.GetEntityByIdAsync(id);
        if (gender is null) return null;

        gender.Code      = req.Code.ToUpperInvariant();
        gender.IsActive  = req.IsActive;
        gender.Ordinal   = req.Ordinal;
        gender.UpdatedAt = DateTime.UtcNow;
        gender.UpdatedBy = Guid.Empty;   // TODO: JWT claims

        await _repo.SaveChangesAsync();
        await UpsertTranslationsAsync(id, req.NameHr, req.NameEn);

        return await _repo.GetByIdAsync(id);
    }

    public async Task<GenderResponse?> ToggleActiveAsync(Guid id)
    {
        var gender = await _repo.GetEntityByIdAsync(id);
        if (gender is null) return null;

        gender.IsActive  = !gender.IsActive;
        gender.UpdatedAt = DateTime.UtcNow;
        gender.UpdatedBy = Guid.Empty;

        await _repo.SaveChangesAsync();
        return await _repo.GetByIdAsync(id);
    }

    public async Task<DeleteResult> DeleteAsync(Guid id)
    {
        var gender = await _repo.GetEntityByIdAsync(id);
        if (gender is null) return new DeleteResult(Found: false, HasReferences: false);

        if (await _repo.HasReferencesAsync(id))
            return new DeleteResult(Found: true, HasReferences: true);

        await _repo.DeleteAsync(gender);
        return new DeleteResult(Found: true, HasReferences: false);
    }

    // ── Privatne metode ─────────────────────────────────────────────────

    /// <summary>
    /// Upsert prijevoda: ako postoji zapis za (entitet, jezik, polje) — ažurira ga,
    /// inače ga kreira. Ovo osigurava idempotentnost pri višestrukim pozivima.
    /// </summary>
    private async Task UpsertTranslationsAsync(Guid genderId, string nameHr, string? nameEn)
    {
        await UpsertOneAsync(genderId, Hr, "Name", nameHr);

        if (!string.IsNullOrWhiteSpace(nameEn))
            await UpsertOneAsync(genderId, En, "Name", nameEn);

        await _db.SaveChangesAsync();
    }

    private async Task UpsertOneAsync(Guid entityId, string languageCode, string fieldName, string value)
    {
        var existing = _db.Translations.FirstOrDefault(t =>
            t.EntityType    == "codebook_gender" &&
            t.EntityId      == entityId          &&
            t.LanguageCode  == languageCode      &&
            t.FieldName     == fieldName);

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
                Id           = Guid.NewGuid(),
                EntityType   = "codebook_gender",
                EntityId     = entityId,
                LanguageCode = languageCode,
                FieldName    = fieldName,
                Value        = value,
                CreatedAt    = DateTime.UtcNow,
                UpdatedAt    = DateTime.UtcNow,
                CreatedBy    = Guid.Empty,
                UpdatedBy    = Guid.Empty
            });
        }
    }
}
