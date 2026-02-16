using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public class UserService : SaasCrudService<User, UserRequest, long>, IUserService
{
    public UserService(SmerpContext context) : base(context, MapRequest)
    {
    }

    private static void MapRequest(User entity, UserRequest request)
    {
        entity.TenantId = request.TenantId;
        entity.Username = request.Username;
        entity.DisplayName = request.DisplayName;
        entity.Email = request.Email;
        entity.Mobile = request.Mobile;
        entity.PasswordHash = request.PasswordHash;
        entity.PasswordSalt = request.PasswordSalt;
        entity.IsActive = request.IsActive;
        entity.CreatedAt = request.CreatedAt;
        entity.LastLoginAt = request.LastLoginAt;
    }
}
