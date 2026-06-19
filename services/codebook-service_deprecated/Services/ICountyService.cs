using CodebookService.DTOs;

namespace CodebookService.Services;

public interface ICountyService
{
    Task<IEnumerable<CountyResponse>> GetAllAsync(bool includeInactive = false, Guid? countryId = null);
    Task<CountyResponse?>             GetByIdAsync(Guid id);
    Task<CountyResponse>              CreateAsync(CreateCountyRequest request);
    Task<CountyResponse?>             UpdateAsync(Guid id, UpdateCountyRequest request);
    Task<CountyResponse?>             ToggleActiveAsync(Guid id);
    Task<DeleteResult>                DeleteAsync(Guid id);
}
