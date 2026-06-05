using CodebookService.DTOs;

namespace CodebookService.Services;

public interface IGenderService
{
    Task<IEnumerable<GenderResponse>> GetAllAsync(bool includeInactive = false);
    Task<GenderResponse?>             GetByIdAsync(Guid id);
    Task<GenderResponse>              CreateAsync(CreateGenderRequest request);
    Task<GenderResponse?>             UpdateAsync(Guid id, UpdateGenderRequest request);
    Task<GenderResponse?>             ToggleActiveAsync(Guid id);
    Task<DeleteResult>                DeleteAsync(Guid id);
}
