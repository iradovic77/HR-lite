using HrLite.Modules.Codebook.DTOs;
using HrLite.Modules.Codebook.Services;
using Microsoft.AspNetCore.Mvc;

namespace HrLite.Modules.Codebook.Controllers;

/// <summary>CRUD endpointi za šifarnik spolova (codebook_gender).</summary>
[ApiController]
[Route("api/codebook/gender")]
[Produces("application/json")]
public class GenderController : ControllerBase
{
    private readonly IGenderService _service;

    public GenderController(IGenderService service) => _service = service;

    /// <summary>Vraća listu svih spolova s prijevodima (hr i en).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GenderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        => Ok(await _service.GetAllAsync(includeInactive));

    /// <summary>Vraća jedan spol prema ID-u.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GenderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Kreira novi spol s prijevodima.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(GenderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateGenderRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Ažurira postojeći spol i prijevode.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(GenderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGenderRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Aktivira ili deaktivira spol (toggle).</summary>
    [HttpPatch("{id:guid}/toggle-active")]
    [ProducesResponseType(typeof(GenderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var result = await _service.ToggleActiveAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Fizički briše spol. Vraća 409 ako ima FK reference.</summary>
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
