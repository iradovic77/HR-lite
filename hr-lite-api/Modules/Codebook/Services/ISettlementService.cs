using HrLite.Modules.Codebook.DTOs;

namespace HrLite.Modules.Codebook.Services;

public interface ISettlementService
{
    Task<IEnumerable<SettlementResponse>> GetAllAsync(bool includeInactive = false, Guid? municipalityId = null);
    Task<SettlementResponse?>             GetByIdAsync(Guid id);
    Task<SettlementResponse>              CreateAsync(CreateSettlementRequest request);
    Task<SettlementResponse?>             UpdateAsync(Guid id, UpdateSettlementRequest request);
    Task<SettlementResponse?>             ToggleActiveAsync(Guid id);
    Task<DeleteResult>                    DeleteAsync(Guid id);
}
