using CodebookService.DTOs;
using CodebookService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodebookService.Controllers;

/// <summary>CRUD endpointi za šifarnik spolova (codebook_gender).</summary>
[ApiController]
[Route("api/codebook/gender")]
[Produces("application/json")]
public class GenderController : ControllerBase
{
    private readonly IGenderService _service;

    public GenderController(IGenderService service) => _service = service;

    /// <summary>Vraća listu svih spolova s prijevodima (hr i en).</summary>
    /// <param name="includeInactive">Uključi neaktivne zapise. Default: false.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GenderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        var result = await _service.GetAllAsync(includeInactive);
        return Ok(result);
    }

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

    /// <summary>Aktivira ili deaktivira spol (toggle). Ne briše zapis.</summary>
    [HttpPatch("{id:guid}/toggle-active")]
    [ProducesResponseType(typeof(GenderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var result = await _service.ToggleActiveAsync(id);
        return result is null ? NotFound() : Ok(result);
    }
}
