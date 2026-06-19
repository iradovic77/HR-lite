using CodebookService.DTOs;

namespace CodebookService.Services;

public interface IMunicipalityService
{
    Task<IEnumerable<MunicipalityResponse>> GetAllAsync(bool includeInactive = false, Guid? countyId = null);
    Task<MunicipalityResponse?>             GetByIdAsync(Guid id);
    Task<MunicipalityResponse>              CreateAsync(CreateMunicipalityRequest request);
    Task<MunicipalityResponse?>             UpdateAsync(Guid id, UpdateMunicipalityRequest request);
    Task<MunicipalityResponse?>             ToggleActiveAsync(Guid id);
    Task<DeleteResult>                      DeleteAsync(Guid id);
}
