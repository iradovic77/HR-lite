using HrLite.Modules.Codebook.DTOs;
using HrLite.Modules.Codebook.Models;

namespace HrLite.Modules.Codebook.Repositories;

public interface IGenderRepository
{
    Task<IEnumerable<GenderResponse>> GetAllAsync(bool includeInactive = false);
    Task<GenderResponse?>             GetByIdAsync(Guid id);
    Task<Gender>                      CreateAsync(Gender gender);
    Task<Gender?>                     GetEntityByIdAsync(Guid id);
    Task                              SaveChangesAsync();
    Task<bool>                        HasReferencesAsync(Guid id);
    Task                              DeleteAsync(Gender gender);
}
