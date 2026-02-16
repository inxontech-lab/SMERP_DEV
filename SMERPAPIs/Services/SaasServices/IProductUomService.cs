using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public interface IProductUomService : ISaasCrudService<ProductUom, ProductUomRequest, long>
{
}
