using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public interface ITenantSettingService : ISaasCrudService<TenantSetting, TenantSettingRequest, int>
{
}
