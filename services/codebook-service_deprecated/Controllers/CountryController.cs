using CodebookService.DTOs;
using CodebookService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodebookService.Controllers;

/// <summary>CRUD endpointi za šifarnik država (codebook_country).</summary>
[ApiController]
[Route("api/codebook/country")]
[Produces("application/json")]
public class CountryController : ControllerBase
{
    private readonly ICountryService _service;

    public CountryController(ICountryService service) => _service = service;

    /// <summary>Vraća listu svih država s prijevodima.</summary>
    /// <param name="includeInactive">Uključi neaktivne zapise. Default: false.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CountryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        var result = await _service.GetAllAsync(includeInactive);
        return Ok(result);
    }

    /// <summary>Vraća jednu državu prema ID-u.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CountryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Kreira novu državu s prijevodima.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CountryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCountryRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Ažurira postojeću državu i prijevode.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CountryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCountryRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Aktivira ili deaktivira državu (toggle).</summary>
    [HttpPatch("{id:guid}/toggle-active")]
    [ProducesResponseType(typeof(CountryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var result = await _service.ToggleActiveAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Fizički briše državu i sve njene prijevode.
    /// Vraća 409 Conflict ako postoje županije koje je referenciraju.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);

        if (!result.Found)        return NotFound();
        if (result.HasReferences) return Conflict(new { message = "Zapis se ne može obrisati jer se koristi u drugim dijelovima sustava." });

        return NoContent();
    }
}
