using HrLite.Shared.Models;

namespace HrLite.Modules.Codebook.Models;

public class Language : IAuditable
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid UpdatedBy { get; set; }
}
