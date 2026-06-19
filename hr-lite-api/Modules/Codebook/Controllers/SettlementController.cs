using HrLite.Modules.Codebook.DTOs;
using HrLite.Modules.Codebook.Services;
using Microsoft.AspNetCore.Mvc;

namespace HrLite.Modules.Codebook.Controllers;

/// <summary>CRUD endpointi za šifarnik gradova/naselja (codebook_settlement).</summary>
[ApiController]
[Route("api/codebook/city")]
[Produces("application/json")]
public class SettlementController : ControllerBase
{
    private readonly ISettlementService _service;

    public SettlementController(ISettlementService service) => _service = service;

    /// <summary>Vraća listu svih gradova/naselja s prijevodima.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SettlementResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool includeInactive = false,
        [FromQuery] Guid? municipalityId = null)
        => Ok(await _service.GetAllAsync(includeInactive, municipalityId));

    /// <summary>Vraća jedan grad/naselje prema ID-u.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SettlementResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Kreira novi grad/naselje s prijevodima.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(SettlementResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSettlementRequest request)
    {
        var result = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Ažurira postojeći grad/naselje i prijevode.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SettlementResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSettlementRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Aktivira ili deaktivira grad/naselje (toggle).</summary>
    [HttpPatch("{id:guid}/toggle-active")]
    [ProducesResponseType(typeof(SettlementResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var result = await _service.ToggleActiveAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Fizički briše grad/naselje i sve prijevode.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result.Found) return NotFound();
        return NoContent();
    }
}
