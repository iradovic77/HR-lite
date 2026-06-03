using CodebookService.DTOs;
using CodebookService.Models;

namespace CodebookService.Repositories;

public interface IGenderRepository
{
    Task<IEnumerable<GenderResponse>> GetAllAsync(bool includeInactive = false);
    Task<GenderResponse?>             GetByIdAsync(Guid id);
    Task<Gender>                      CreateAsync(Gender gender);
    Task<Gender?>                     GetEntityByIdAsync(Guid id);
    Task                              SaveChangesAsync();
}
