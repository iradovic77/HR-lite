using System.ComponentModel.DataAnnotations;

namespace HrLite.Modules.Codebook.DTOs;

public class SettlementResponse
{
    public Guid    Id                 { get; set; }
    public string  Code               { get; set; } = string.Empty;
    public string  NameHr             { get; set; } = string.Empty;
    public string? NameEn             { get; set; }
    public int     Ordinal            { get; set; }
    public bool    IsActive           { get; set; }
    public Guid?   MunicipalityId     { get; set; }
    public string? MunicipalityNameHr { get; set; }
    public string? CountyNameHr       { get; set; }
    public string? CountryNameHr      { get; set; }
    public string? PostalNumber       { get; set; }
}

public class CreateSettlementRequest
{
    [Required, MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string NameHr { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? NameEn { get; set; }

    public Guid?   MunicipalityId { get; set; }
    public int     Ordinal        { get; set; } = 0;
    public bool    IsActive       { get; set; } = true;

    [MaxLength(20)]
    public string? PostalNumber   { get; set; }
}

public class UpdateSettlementRequest
{
    [Required, MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string NameHr { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? NameEn { get; set; }

    public Guid?   MunicipalityId { get; set; }
    public int     Ordinal        { get; set; }
    public bool    IsActive       { get; set; }

    [MaxLength(20)]
    public string? PostalNumber   { get; set; }
}
