using Domain.SaasDBModels;
using Microsoft.AspNetCore.Mvc;
using SMERPAPIs.Services.SaasServices;

namespace SMERPAPIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _service;

    public TenantsController(ITenantService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<Tenant>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Tenant>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Tenant>> Create([FromBody] Tenant entity)
    {
        var created = await _service.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Tenant entity)
    {
        var updated = await _service.UpdateAsync(id, entity);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
