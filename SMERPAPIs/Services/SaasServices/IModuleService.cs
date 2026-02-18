namespace SMERPAPIs.Services.SaasServices;

public interface IModuleService
{
    Task<List<PermittedModuleDto>> GetPermittedModulesAsync(int tenantId, long userId, CancellationToken cancellationToken = default);
}

public sealed record PermittedModuleDto(string ModuleKey, string ModuleName);
