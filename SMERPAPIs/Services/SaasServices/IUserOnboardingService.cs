using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public interface IUserOnboardingService
{
    Task<long> CreateUserWithRoleAsync(CreateUserWithRoleRequest request);
}
