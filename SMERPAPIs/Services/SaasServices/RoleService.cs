using Domain.SaasDBModels;

namespace SMERPAPIs.Services.SaasServices;

public class RoleService : SaasCrudService<Role, int>, IRoleService
{
    public RoleService(SmerpContext context) : base(context)
    {
    }
}
