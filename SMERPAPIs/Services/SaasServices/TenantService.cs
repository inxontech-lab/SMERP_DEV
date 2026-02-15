using Domain.SaasDBModels;

namespace SMERPAPIs.Services.SaasServices;

public class TenantService : SaasCrudService<Tenant, int>, ITenantService
{
    public TenantService(SmerpContext context) : base(context)
    {
    }
}
