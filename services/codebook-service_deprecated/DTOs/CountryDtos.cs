using System.ComponentModel.DataAnnotations;

namespace CodebookService.DTOs;

/// <summary>Odgovor API-ja za jedan Country zapis (uključuje prijevode).</summary>
public class CountryResponse
{
    public Guid    Id              { get; set; }
    public string  Code            { get; set; } = string.Empty;
    public string  NameHr          { get; set; } = string.Empty;
    public string? NameEn          { get; set; }
    public string? CitizenshipHr   { get; set; }
    public string? CitizenshipEn   { get; set; }
    public int     Ordinal         { get; set; }
    public bool    IsActive        { get; set; }
}

/// <summary>Zahtjev za kreiranje novog Country zapisa.</summary>
public class CreateCountryRequest
{
    [Required, MaxLength(10)]
    public string Code { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string NameHr { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? NameEn { get; set; }

    [MaxLength(200)]
    public string? CitizenshipHr { get; set; }

    [MaxLength(200)]
    public string? CitizenshipEn { get; set; }

    public int  Ordinal  { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}

/// <summary>Zahtjev za izmjenu Country zapisa.</summary>
public class UpdateCountryRequest
{
    [Required, MaxLength(10)]
    public string Code { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string NameHr { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? NameEn { get; set; }

    [MaxLength(200)]
    public string? CitizenshipHr { get; set; }

    [MaxLength(200)]
    public string? CitizenshipEn { get; set; }

    public int  Ordinal  { get; set; }
    public bool IsActive { get; set; }
}
