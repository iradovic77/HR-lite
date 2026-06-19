using HrLite.Shared.Models;

namespace HrLite.Modules.Codebook.Models;

public class Translation : IAuditable
{
    public Guid Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid UpdatedBy { get; set; }
}
