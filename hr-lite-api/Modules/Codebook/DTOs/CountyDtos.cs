using System.ComponentModel.DataAnnotations;

namespace HrLite.Modules.Codebook.DTOs;

public class CountyResponse
{
    public Guid    Id            { get; set; }
    public string  Code          { get; set; } = string.Empty;
    public string  NameHr        { get; set; } = string.Empty;
    public string? NameEn        { get; set; }
    public int     Ordinal       { get; set; }
    public bool    IsActive      { get; set; }
    public Guid?   CountryId     { get; set; }
    public string? CountryNameHr { get; set; }
}

public class CreateCountyRequest
{
    [Required, MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string NameHr { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? NameEn { get; set; }

    public Guid? CountryId { get; set; }
    public int   Ordinal   { get; set; } = 0;
    public bool  IsActive  { get; set; } = true;
}

public class UpdateCountyRequest
{
    [Required, MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string NameHr { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? NameEn { get; set; }

    public Guid? CountryId { get; set; }
    public int   Ordinal   { get; set; }
    public bool  IsActive  { get; set; }
}
