using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public interface IRefreshTokenService : ISaasCrudService<RefreshToken, RefreshTokenRequest, long>
{
}
