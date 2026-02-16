using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public interface IProductService : ISaasCrudService<Product, ProductRequest, long>
{
}
