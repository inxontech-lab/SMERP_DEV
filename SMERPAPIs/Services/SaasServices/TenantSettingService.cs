using Domain.SaasDBModels;

namespace SMERPAPIs.Services.SaasServices;

public class TenantSettingService : SaasCrudService<TenantSetting, int>, ITenantSettingService
{
    public TenantSettingService(SmerpContext context) : base(context)
    {
    }
}
