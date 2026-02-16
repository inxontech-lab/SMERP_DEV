using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public class TaxCodeService : SaasCrudService<TaxCode, TaxCodeRequest, int>, ITaxCodeService
{
    public TaxCodeService(SmerpContext context) : base(context, MapRequest)
    {
    }

    private static void MapRequest(TaxCode entity, TaxCodeRequest request)
    {
        entity.TenantId = request.TenantId;
        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.Rate = request.Rate;
        entity.IsZeroRated = request.IsZeroRated;
        entity.IsExempt = request.IsExempt;
        entity.IsActive = request.IsActive;
    }
}
