using Domain.SaasDBModels;

namespace SMERPAPIs.Services.SaasServices;

public class RefreshTokenService : SaasCrudService<RefreshToken, long>, IRefreshTokenService
{
    public RefreshTokenService(SmerpContext context) : base(context)
    {
    }
}
