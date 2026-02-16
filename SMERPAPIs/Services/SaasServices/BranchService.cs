using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public class BranchService : SaasCrudService<Branch, BranchRequest, int>, IBranchService
{
    public BranchService(SmerpContext context) : base(context, MapRequest)
    {
    }

    private static void MapRequest(Branch entity, BranchRequest request)
    {
        entity.TenantId = request.TenantId;
        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.Address = request.Address;
        entity.IsActive = request.IsActive;
    }
}
