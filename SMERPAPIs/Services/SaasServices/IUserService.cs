using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public interface IUserService : ISaasCrudService<User, UserRequest, long>
{
}
