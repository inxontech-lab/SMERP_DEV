using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public interface IRoleService : ISaasCrudService<Role, RoleRequest, int>
{
}
