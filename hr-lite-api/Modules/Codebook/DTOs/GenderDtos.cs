using System.ComponentModel.DataAnnotations;

namespace HrLite.Modules.Codebook.DTOs;

public class GenderResponse
{
    public Guid   Id       { get; set; }
    public string Code     { get; set; } = string.Empty;
    public string NameHr   { get; set; } = string.Empty;
    public string? NameEn  { get; set; }
    public int    Ordinal  { get; set; }
    public bool   IsActive { get; set; }
}

public class CreateGenderRequest
{
    [Required, MaxLength(10)]
    public string Code { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string NameHr { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? NameEn { get; set; }

    public int  Ordinal  { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}

public class UpdateGenderRequest
{
    [Required, MaxLength(10)]
    public string Code { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string NameHr { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? NameEn { get; set; }

    public int  Ordinal  { get; set; }
    public bool IsActive { get; set; }
}
