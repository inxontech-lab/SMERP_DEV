using Domain.SaasDBModels;
using Domain.SaasReqDTO;
using Microsoft.AspNetCore.Mvc;
using SMERPAPIs.Services.InventoryServices;

namespace SMERPAPIs.Controllers.Inventory;

[ApiController]
[Route("api/Items")]
public class ItemsController(IItemService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetAll([FromQuery] int viewerTenantId)
        => Ok(await service.GetAllAsync(viewerTenantId));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<Product>> GetById(long id, [FromQuery] int viewerTenantId)
    {
        var item = await service.GetByIdAsync(id, viewerTenantId);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create([FromBody] ProductRequest request, [FromQuery] int viewerTenantId)
    {
        try
        {
            var created = await service.CreateAsync(request, viewerTenantId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id, viewerTenantId }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] ProductRequest request, [FromQuery] int viewerTenantId)
    {
        try
        {
            var updated = await service.UpdateAsync(id, request, viewerTenantId);
            return updated ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, [FromQuery] int viewerTenantId)
    {
        var deleted = await service.DeleteAsync(id, viewerTenantId);
        return deleted ? NoContent() : NotFound();
    }
}
