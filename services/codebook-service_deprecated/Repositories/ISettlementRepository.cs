using CodebookService.DTOs;
using CodebookService.Models;

namespace CodebookService.Repositories;

public interface ISettlementRepository
{
    Task<IEnumerable<SettlementResponse>> GetAllAsync(bool includeInactive = false, Guid? municipalityId = null);
    Task<SettlementResponse?>             GetByIdAsync(Guid id);
    Task<Settlement>                      CreateAsync(Settlement settlement);
    Task<Settlement?>                     GetEntityByIdAsync(Guid id);
    Task                                  SaveChangesAsync();
    Task<bool>                            HasReferencesAsync(Guid id);
    Task                                  DeleteAsync(Settlement settlement);
}
