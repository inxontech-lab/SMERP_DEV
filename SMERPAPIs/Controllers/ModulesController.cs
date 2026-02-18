using Microsoft.AspNetCore.Mvc;
using SMERPAPIs.Services.SaasServices;

namespace SMERPAPIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModulesController(IModuleService moduleService) : ControllerBase
{
    [HttpGet("permitted/{tenantId:int}/{userId:long}")]
    public async Task<ActionResult<List<PermittedModuleDto>>> GetPermittedModules(int tenantId, long userId, CancellationToken cancellationToken)
    {
        var modules = await moduleService.GetPermittedModulesAsync(tenantId, userId, cancellationToken);
        return Ok(modules);
    }
}
