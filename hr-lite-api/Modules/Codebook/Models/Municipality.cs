namespace HrLite.Modules.Codebook.Models;

public class Municipality : CodebookBase
{
    public Guid? CountyId { get; set; }
    public string? JOPPDCode { get; set; }
}
