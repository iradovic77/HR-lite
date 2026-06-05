using CodebookService.DTOs;
using CodebookService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodebookService.Controllers;

/// <summary>CRUD endpointi za šifarnik općina (codebook_municipality).</summary>
[ApiController]
[Route("api/codebook/municipality")]
[Produces("application/json")]
public class MunicipalityController : ControllerBase
{
    private readonly IMunicipalityService _service;

    public MunicipalityController(IMunicipalityService service) => _service = service;

    /// <summary>Vraća listu svih općina s prijevodima.</summary>
    /// <param name="includeInactive">Uključi neaktivne zapise. Default: false.</param>
    /// <param name="countyId">Filter po županiji. Opcionalno.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MunicipalityResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool includeInactive = false,
        [FromQuery] Guid? countyId = null)
    {
        var result = await _service.GetAllAsync(includeInactive, countyId);
        return Ok(result);
    }

    /// <summary>Vraća jednu općinu prema ID-u.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MunicipalityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Kreira novu općinu s prijevodima.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(MunicipalityResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMunicipalityRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Ažurira postojeću općinu i prijevode.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(MunicipalityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMunicipalityRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Aktivira ili deaktivira općinu (toggle).</summary>
    [HttpPatch("{id:guid}/toggle-active")]
    [ProducesResponseType(typeof(MunicipalityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var result = await _service.ToggleActiveAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Fizički briše općinu i sve njene prijevode.
    /// Vraća 409 Conflict ako postoje naselja koja je referenciraju.
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
