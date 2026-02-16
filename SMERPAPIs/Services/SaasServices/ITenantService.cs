using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public interface ITenantService : ISaasCrudService<Tenant, TenantRequest, int>
{
}
