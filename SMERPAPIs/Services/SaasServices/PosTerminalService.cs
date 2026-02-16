using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public class PosTerminalService : SaasCrudService<PosTerminal, PosTerminalRequest, int>, IPosTerminalService
{
    public PosTerminalService(SmerpContext context) : base(context, MapRequest)
    {
    }

    private static void MapRequest(PosTerminal entity, PosTerminalRequest request)
    {
        entity.TenantId = request.TenantId;
        entity.BranchId = request.BranchId;
        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.IsKiosk = request.IsKiosk;
        entity.IsActive = request.IsActive;
    }
}
