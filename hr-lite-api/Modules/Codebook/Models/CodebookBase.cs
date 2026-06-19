using HrLite.Shared.Models;

namespace HrLite.Modules.Codebook.Models;

public abstract class CodebookBase : IAuditable
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int Ordinal { get; set; } = 0;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid UpdatedBy { get; set; }
}
