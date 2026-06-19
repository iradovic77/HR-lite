namespace HrLite.Modules.Codebook.Models;

public class Settlement : CodebookBase
{
    public Guid? MunicipalityId { get; set; }
    public string? PostalNumber { get; set; }
}
