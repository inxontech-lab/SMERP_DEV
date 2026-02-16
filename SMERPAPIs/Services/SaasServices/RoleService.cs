using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public class RoleService : SaasCrudService<Role, RoleRequest, int>, IRoleService
{
    public RoleService(SmerpContext context) : base(context, MapRequest)
    {
    }

    private static void MapRequest(Role entity, RoleRequest request)
    {
        entity.TenantId = request.TenantId;
        entity.Name = request.Name;
        entity.IsActive = request.IsActive;
    }
}
