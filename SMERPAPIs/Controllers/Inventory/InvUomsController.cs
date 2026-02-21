using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Microsoft.AspNetCore.Mvc;
using SMERPAPIs.Services.InventoryServices;

namespace SMERPAPIs.Controllers.Inventory;

[ApiController]
[Route("api/Inventory/[controller]")]
public class InvUomsController(IInvUomService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<InvUom>>> GetAll([FromQuery] int viewerTenantId)
        => Ok(await service.GetAllAsync(viewerTenantId));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<InvUom>> GetById(int id, [FromQuery] int viewerTenantId)
    {
        var entity = await service.GetByIdAsync(id, viewerTenantId);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<InvUom>> Create([FromBody] CreateInvUomRequest request, [FromQuery] int viewerTenantId)
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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateInvUomRequest request, [FromQuery] int viewerTenantId)
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, [FromQuery] int viewerTenantId)
        => await service.DeleteAsync(id, viewerTenantId) ? NoContent() : NotFound();
}
