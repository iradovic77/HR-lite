using HrLite.Modules.Codebook.DTOs;

namespace HrLite.Modules.Codebook.Services;

public interface ICountryService
{
    Task<IEnumerable<CountryResponse>> GetAllAsync(bool includeInactive = false);
    Task<CountryResponse?>             GetByIdAsync(Guid id);
    Task<CountryResponse>              CreateAsync(CreateCountryRequest request);
    Task<CountryResponse?>             UpdateAsync(Guid id, UpdateCountryRequest request);
    Task<CountryResponse?>             ToggleActiveAsync(Guid id);
    Task<DeleteResult>                 DeleteAsync(Guid id);
}
