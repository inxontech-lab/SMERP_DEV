using Domain.SaasDBModels;

namespace SMERPAPIs.Services.SaasServices;

public class AuditLogService : SaasCrudService<AuditLog, long>, IAuditLogService
{
    public AuditLogService(SmerpContext context) : base(context)
    {
    }
}
