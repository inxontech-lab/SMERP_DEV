using Domain.SaasDBModels;

namespace SMERPAPIs.Services.SaasServices;

public interface IAuditLogService : ISaasCrudService<AuditLog, long>
{
}
