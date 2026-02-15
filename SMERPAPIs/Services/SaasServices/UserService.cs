using Domain.SaasDBModels;

namespace SMERPAPIs.Services.SaasServices;

public class UserService : SaasCrudService<User, long>, IUserService
{
    public UserService(SmerpContext context) : base(context)
    {
    }
}
