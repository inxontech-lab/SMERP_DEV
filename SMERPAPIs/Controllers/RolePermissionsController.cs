using Domain.SaasDBModels;
using Microsoft.AspNetCore.Mvc;
using SMERPAPIs.Services.SaasServices;

namespace SMERPAPIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolePermissionsController : ControllerBase
{
    private readonly IRolePermissionService _service;

    public RolePermissionsController(IRolePermissionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<RolePermission>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{tenantId:int}/{roleId:int}/{permissionId:int}")]
    public async Task<ActionResult<RolePermission>> GetById(int tenantId, int roleId, int permissionId)
    {
        var item = await _service.GetByIdAsync(tenantId, roleId, permissionId);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<RolePermission>> Create([FromBody] RolePermission entity)
    {
        var created = await _service.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { created.TenantId, created.RoleId, created.PermissionId }, created);
    }

    [HttpPut("{tenantId:int}/{roleId:int}/{permissionId:int}")]
    public async Task<IActionResult> Update(int tenantId, int roleId, int permissionId, [FromBody] RolePermission entity)
    {
        var updated = await _service.UpdateAsync(tenantId, roleId, permissionId, entity);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{tenantId:int}/{roleId:int}/{permissionId:int}")]
    public async Task<IActionResult> Delete(int tenantId, int roleId, int permissionId)
    {
        var deleted = await _service.DeleteAsync(tenantId, roleId, permissionId);
        return deleted ? NoContent() : NotFound();
    }
}
