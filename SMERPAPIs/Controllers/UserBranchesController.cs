using Domain.SaasDBModels;
using Microsoft.AspNetCore.Mvc;
using SMERPAPIs.Services.SaasServices;

namespace SMERPAPIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserBranchesController : ControllerBase
{
    private readonly IUserBranchService _service;

    public UserBranchesController(IUserBranchService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserBranch>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{tenantId:int}/{userId:long}/{branchId:int}")]
    public async Task<ActionResult<UserBranch>> GetById(int tenantId, long userId, int branchId)
    {
        var item = await _service.GetByIdAsync(tenantId, userId, branchId);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<UserBranch>> Create([FromBody] UserBranch entity)
    {
        var created = await _service.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { created.TenantId, created.UserId, created.BranchId }, created);
    }

    [HttpPut("{tenantId:int}/{userId:long}/{branchId:int}")]
    public async Task<IActionResult> Update(int tenantId, long userId, int branchId, [FromBody] UserBranch entity)
    {
        var updated = await _service.UpdateAsync(tenantId, userId, branchId, entity);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{tenantId:int}/{userId:long}/{branchId:int}")]
    public async Task<IActionResult> Delete(int tenantId, long userId, int branchId)
    {
        var deleted = await _service.DeleteAsync(tenantId, userId, branchId);
        return deleted ? NoContent() : NotFound();
    }
}
