namespace CodebookService.Models;

/// <summary>
/// Interface koji označava da entitet ima standardne audit kolone.
/// DbContext.SaveChanges() automatski popunjava ove kolone za sve
/// entitete koji implementiraju ovaj interface.
/// </summary>
public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }

    /// <summary>UserId koji je kreirao zapis. Guid.Empty = sistem (seed data).</summary>
    Guid CreatedBy { get; set; }

    /// <summary>UserId koji je zadnji izmijenio zapis. Guid.Empty = sistem.</summary>
    Guid UpdatedBy { get; set; }
}
