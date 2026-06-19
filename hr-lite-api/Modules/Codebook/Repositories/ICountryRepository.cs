using HrLite.Modules.Codebook.DTOs;
using HrLite.Modules.Codebook.Models;

namespace HrLite.Modules.Codebook.Repositories;

public interface ICountryRepository
{
    Task<IEnumerable<CountryResponse>> GetAllAsync(bool includeInactive = false);
    Task<CountryResponse?>             GetByIdAsync(Guid id);
    Task<Country>                      CreateAsync(Country country);
    Task<Country?>                     GetEntityByIdAsync(Guid id);
    Task                               SaveChangesAsync();
    Task<bool>                         HasReferencesAsync(Guid id);
    Task                               DeleteAsync(Country country);
}
