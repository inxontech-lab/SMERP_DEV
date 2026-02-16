using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public class AuditLogService : SaasCrudService<AuditLog, AuditLogRequest, long>, IAuditLogService
{
    public AuditLogService(SmerpContext context) : base(context, MapRequest)
    {
    }

    private static void MapRequest(AuditLog entity, AuditLogRequest request)
    {
        entity.TenantId = request.TenantId;
        entity.UserId = request.UserId;
        entity.Action = request.Action;
        entity.Entity = request.Entity;
        entity.EntityId = request.EntityId;
        entity.Details = request.Details;
        entity.CreatedAt = request.CreatedAt;
    }
}
