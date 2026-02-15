using Domain.SaasDBModels;

namespace SMERPAPIs.Services.SaasServices;

public class ProductUomService : SaasCrudService<ProductUom, long>, IProductUomService
{
    public ProductUomService(SmerpContext context) : base(context)
    {
    }
}
