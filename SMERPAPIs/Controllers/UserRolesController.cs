using Domain.SaasDBModels;
using Domain.SaasReqDTO;
using Microsoft.AspNetCore.Mvc;
using SMERPAPIs.Services.SaasServices;

namespace SMERPAPIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserRolesController : ControllerBase
{
    private readonly IUserRoleService _service;

    public UserRolesController(IUserRoleService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserRole>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{tenantId:int}/{userId:long}/{roleId:int}")]
    public async Task<ActionResult<UserRole>> GetById(int tenantId, long userId, int roleId)
    {
        var item = await _service.GetByIdAsync(tenantId, userId, roleId);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<UserRole>> Create([FromBody] UserRoleRequest request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { created.TenantId, created.UserId, created.RoleId }, created);
    }

    [HttpPut("{tenantId:int}/{userId:long}/{roleId:int}")]
    public async Task<IActionResult> Update(int tenantId, long userId, int roleId, [FromBody] UserRoleRequest request)
    {
        var updated = await _service.UpdateAsync(tenantId, userId, roleId, request);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{tenantId:int}/{userId:long}/{roleId:int}")]
    public async Task<IActionResult> Delete(int tenantId, long userId, int roleId)
    {
        var deleted = await _service.DeleteAsync(tenantId, userId, roleId);
        return deleted ? NoContent() : NotFound();
    }
}
