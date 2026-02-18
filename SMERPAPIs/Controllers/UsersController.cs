using Domain.SaasDBModels;
using Domain.SaasReqDTO;
using Microsoft.AspNetCore.Mvc;
using SMERPAPIs.Services.SaasServices;

namespace SMERPAPIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;
    private readonly IUserOnboardingService _onboardingService;

    public UsersController(IUserService service, IUserOnboardingService onboardingService)
    {
        _service = service;
        _onboardingService = onboardingService;
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("with-role")]
    public async Task<ActionResult<List<UserWithRoleResponse>>> GetAllWithRole([FromQuery] int? viewerTenantId = null)
    {
        return Ok(await _onboardingService.GetUsersWithRolesAsync(viewerTenantId));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<User>> GetById(long id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<User>> Create([FromBody] UserRequest request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost("with-role")]
    public async Task<ActionResult<long>> CreateWithRole([FromBody] CreateUserWithRoleRequest request)
    {
        try
        {
            var userId = await _onboardingService.CreateUserWithRoleAsync(request);
            return Ok(userId);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("with-role/{id:long}")]
    public async Task<IActionResult> UpdateWithRole(long id, [FromBody] UpdateUserWithRoleRequest request)
    {
        try
        {
            var updated = await _onboardingService.UpdateUserWithRoleAsync(id, request);
            return updated ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("with-role/{id:long}")]
    public async Task<IActionResult> DeleteWithRole(long id)
    {
        var deleted = await _onboardingService.DeleteUserWithRoleAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UserRequest request)
    {
        var updated = await _service.UpdateAsync(id, request);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
