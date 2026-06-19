using HrLite.Modules.Codebook.DTOs;
using HrLite.Modules.Codebook.Models;

namespace HrLite.Modules.Codebook.Repositories;

public interface ICountyRepository
{
    Task<IEnumerable<CountyResponse>> GetAllAsync(bool includeInactive = false, Guid? countryId = null);
    Task<CountyResponse?>             GetByIdAsync(Guid id);
    Task<County>                      CreateAsync(County county);
    Task<County?>                     GetEntityByIdAsync(Guid id);
    Task                              SaveChangesAsync();
    Task<bool>                        HasReferencesAsync(Guid id);
    Task                              DeleteAsync(County county);
}
