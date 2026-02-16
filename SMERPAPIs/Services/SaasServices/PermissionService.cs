using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public class PermissionService : SaasCrudService<Permission, PermissionRequest, int>, IPermissionService
{
    public PermissionService(SmerpContext context) : base(context, MapRequest)
    {
    }

    private static void MapRequest(Permission entity, PermissionRequest request)
    {
        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.Module = request.Module;
    }
}
