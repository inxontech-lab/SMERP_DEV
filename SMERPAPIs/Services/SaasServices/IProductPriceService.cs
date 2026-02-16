using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public interface IProductPriceService : ISaasCrudService<ProductPrice, ProductPriceRequest, long>
{
}
