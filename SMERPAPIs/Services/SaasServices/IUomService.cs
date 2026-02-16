using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public interface IUomService : ISaasCrudService<Uom, UomRequest, int>
{
}
