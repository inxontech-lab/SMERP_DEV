using Domain.SaasDBModels;

namespace SMERPAPIs.Services.SaasServices;

public class PosTerminalService : SaasCrudService<PosTerminal, int>, IPosTerminalService
{
    public PosTerminalService(SmerpContext context) : base(context)
    {
    }
}
