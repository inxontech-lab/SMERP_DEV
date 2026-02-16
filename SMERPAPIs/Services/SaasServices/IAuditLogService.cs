using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public interface IAuditLogService : ISaasCrudService<AuditLog, AuditLogRequest, long>
{
}
