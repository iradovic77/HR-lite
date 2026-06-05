using CodebookService.DTOs;
using CodebookService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodebookService.Controllers;

/// <summary>CRUD endpointi za šifarnik županija (codebook_county).</summary>
[ApiController]
[Route("api/codebook/county")]
[Produces("application/json")]
public class CountyController : ControllerBase
{
    private readonly ICountyService _service;

    public CountyController(ICountyService service) => _service = service;

    /// <summary>Vraća listu svih županija s prijevodima.</summary>
    /// <param name="includeInactive">Uključi neaktivne zapise. Default: false.</param>
    /// <param name="countryId">Filter po državi. Opcionalno.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CountyResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool includeInactive = false,
        [FromQuery] Guid? countryId = null)
    {
        var result = await _service.GetAllAsync(includeInactive, countryId);
        return Ok(result);
    }

    /// <summary>Vraća jednu županiju prema ID-u.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CountyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Kreira novu županiju s prijevodima.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CountyResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCountyRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Ažurira postojeću županiju i prijevode.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CountyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCountyRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Aktivira ili deaktivira županiju (toggle).</summary>
    [HttpPatch("{id:guid}/toggle-active")]
    [ProducesResponseType(typeof(CountyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var result = await _service.ToggleActiveAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Fizički briše županiju i sve njene prijevode.
    /// Vraća 409 Conflict ako postoje općine koje je referenciraju.
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
