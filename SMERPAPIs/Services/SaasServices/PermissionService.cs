using Domain.SaasDBModels;

namespace SMERPAPIs.Services.SaasServices;

public class PermissionService : SaasCrudService<Permission, int>, IPermissionService
{
    public PermissionService(SmerpContext context) : base(context)
    {
    }
}
