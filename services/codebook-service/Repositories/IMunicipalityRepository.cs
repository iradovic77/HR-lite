using CodebookService.DTOs;
using CodebookService.Models;

namespace CodebookService.Repositories;

public interface IMunicipalityRepository
{
    Task<IEnumerable<MunicipalityResponse>> GetAllAsync(bool includeInactive = false, Guid? countyId = null);
    Task<MunicipalityResponse?>             GetByIdAsync(Guid id);
    Task<Municipality>                      CreateAsync(Municipality municipality);
    Task<Municipality?>                     GetEntityByIdAsync(Guid id);
    Task                                    SaveChangesAsync();
    Task<bool>                              HasReferencesAsync(Guid id);
    Task                                    DeleteAsync(Municipality municipality);
}
