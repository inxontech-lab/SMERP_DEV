using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public interface IPosTerminalService : ISaasCrudService<PosTerminal, PosTerminalRequest, int>
{
}
