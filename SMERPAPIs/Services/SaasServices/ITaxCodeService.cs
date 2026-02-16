using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public interface ITaxCodeService : ISaasCrudService<TaxCode, TaxCodeRequest, int>
{
}
