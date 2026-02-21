using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Microsoft.AspNetCore.Mvc;
using SMERPAPIs.Services.InventoryServices;

namespace SMERPAPIs.Controllers.Inventory;

[ApiController]
[Route("api/Inventory/[controller]")]
public class InvWarehousesController(IInvWarehouseService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<InvWarehouse>>> GetAll([FromQuery] int viewerTenantId) => Ok(await service.GetAllAsync(viewerTenantId));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<InvWarehouse>> GetById(int id, [FromQuery] int viewerTenantId)
    {
        var entity = await service.GetByIdAsync(id, viewerTenantId);
        return entity is null ? NotFound() : Ok(entity);
    }

    [HttpPost]
    public async Task<ActionResult<InvWarehouse>> Create([FromBody] CreateInvWarehouseRequest request, [FromQuery] int viewerTenantId)
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
    public async Task<IActionResult> Update(int id, [FromBody] UpdateInvWarehouseRequest request, [FromQuery] int viewerTenantId)
    {
        try
        {
            return await service.UpdateAsync(id, request, viewerTenantId) ? NoContent() : NotFound();
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
