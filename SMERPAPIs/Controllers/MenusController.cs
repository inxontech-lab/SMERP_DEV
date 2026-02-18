using Microsoft.AspNetCore.Mvc;
using SMERPAPIs.Services.SaasServices;

namespace SMERPAPIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenusController(IMenuService menuService) : ControllerBase
{
    [HttpGet("navigation/{tenantId:int}/{userId:long}")]
    public async Task<ActionResult<NavigationMenuSnapshotDto>> GetNavigationMenu(
        int tenantId,
        long userId,
        [FromQuery] string? moduleKey,
        CancellationToken cancellationToken)
    {
        var result = await menuService.GetNavigationMenuAsync(tenantId, userId, moduleKey, cancellationToken);
        return Ok(result);
    }
}
