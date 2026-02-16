using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public interface IPermissionService : ISaasCrudService<Permission, PermissionRequest, int>
{
}
