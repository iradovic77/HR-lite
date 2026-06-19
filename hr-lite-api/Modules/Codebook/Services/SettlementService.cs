using HrLite.Modules.Codebook.DTOs;
using HrLite.Modules.Codebook.Models;
using HrLite.Modules.Codebook.Repositories;
using HrLite.Shared.Data;

namespace HrLite.Modules.Codebook.Services;

public class SettlementService : ISettlementService
{
    private readonly ISettlementRepository _repo;
    private readonly AppDbContext _db;

    private const string Hr = "hr";
    private const string En = "en";

    public SettlementService(ISettlementRepository repo, AppDbContext db)
    {
        _repo = repo;
        _db   = db;
    }

    public Task<IEnumerable<SettlementResponse>> GetAllAsync(bool includeInactive = false, Guid? municipalityId = null)
        => _repo.GetAllAsync(includeInactive, municipalityId);

    public Task<SettlementResponse?> GetByIdAsync(Guid id)
        => _repo.GetByIdAsync(id);

    public async Task<SettlementResponse> CreateAsync(CreateSettlementRequest req)
    {
        var settlement = new Settlement
        {
            Id             = Guid.NewGuid(),
            Code           = req.Code.ToUpperInvariant(),
            IsActive       = req.IsActive,
            Ordinal        = req.Ordinal,
            MunicipalityId = req.MunicipalityId,
            PostalNumber   = req.PostalNumber,
            CreatedBy      = Guid.Empty,
            UpdatedBy      = Guid.Empty
        };

        await _repo.CreateAsync(settlement);
        await UpsertTranslationsAsync(settlement.Id, req.NameHr, req.NameEn);

        return (await _repo.GetByIdAsync(settlement.Id))!;
    }

    public async Task<SettlementResponse?> UpdateAsync(Guid id, UpdateSettlementRequest req)
    {
        var settlement = await _repo.GetEntityByIdAsync(id);
        if (settlement is null) return null;

        settlement.Code           = req.Code.ToUpperInvariant();
        settlement.IsActive       = req.IsActive;
        settlement.Ordinal        = req.Ordinal;
        settlement.MunicipalityId = req.MunicipalityId;
        settlement.PostalNumber   = req.PostalNumber;
        settlement.UpdatedBy      = Guid.Empty;

        await _repo.SaveChangesAsync();
        await UpsertTranslationsAsync(id, req.NameHr, req.NameEn);

        return await _repo.GetByIdAsync(id);
    }

    public async Task<SettlementResponse?> ToggleActiveAsync(Guid id)
    {
        var settlement = await _repo.GetEntityByIdAsync(id);
        if (settlement is null) return null;

        settlement.IsActive  = !settlement.IsActive;
        settlement.UpdatedBy = Guid.Empty;

        await _repo.SaveChangesAsync();
        return await _repo.GetByIdAsync(id);
    }

    public async Task<DeleteResult> DeleteAsync(Guid id)
    {
        var settlement = await _repo.GetEntityByIdAsync(id);
        if (settlement is null) return new DeleteResult(Found: false, HasReferences: false);

        if (await _repo.HasReferencesAsync(id))
            return new DeleteResult(Found: true, HasReferences: true);

        await _repo.DeleteAsync(settlement);
        return new DeleteResult(Found: true, HasReferences: false);
    }

    private async Task UpsertTranslationsAsync(Guid settlementId, string nameHr, string? nameEn)
    {
        await UpsertOneAsync(settlementId, Hr, "Name", nameHr);

        if (!string.IsNullOrWhiteSpace(nameEn))
            await UpsertOneAsync(settlementId, En, "Name", nameEn);

        await _db.SaveChangesAsync();
    }

    private async Task UpsertOneAsync(Guid entityId, string languageCode, string fieldName, string value)
    {
        var existing = _db.Translations.FirstOrDefault(t =>
            t.EntityType   == "codebook_settlement" &&
            t.EntityId     == entityId              &&
            t.LanguageCode == languageCode          &&
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
                EntityType   = "codebook_settlement",
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
