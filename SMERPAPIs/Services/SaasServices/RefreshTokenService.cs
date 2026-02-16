using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public class RefreshTokenService : SaasCrudService<RefreshToken, RefreshTokenRequest, long>, IRefreshTokenService
{
    public RefreshTokenService(SmerpContext context) : base(context, MapRequest)
    {
    }

    private static void MapRequest(RefreshToken entity, RefreshTokenRequest request)
    {
        entity.TenantId = request.TenantId;
        entity.UserId = request.UserId;
        entity.TokenHash = request.TokenHash;
        entity.ExpiresAt = request.ExpiresAt;
        entity.RevokedAt = request.RevokedAt;
        entity.CreatedAt = request.CreatedAt;
        entity.CreatedIp = request.CreatedIp;
        entity.UserAgent = request.UserAgent;
    }
}
