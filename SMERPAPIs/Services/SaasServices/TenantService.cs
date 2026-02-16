using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public class TenantService : SaasCrudService<Tenant, TenantRequest, int>, ITenantService
{
    public TenantService(SmerpContext context) : base(context, MapRequest)
    {
    }

    private static void MapRequest(Tenant entity, TenantRequest request)
    {
        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.LegalName = request.LegalName;
        entity.VatNo = request.VatNo;
        entity.CrNo = request.CrNo;
        entity.CountryCode = request.CountryCode;
        entity.TimeZone = request.TimeZone;
        entity.IsActive = request.IsActive;
        entity.CreatedAt = request.CreatedAt;
    }
}
