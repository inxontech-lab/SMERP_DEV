using Domain.SaasDBModels;

namespace SMERPAPIs.Services.SaasServices;

public class ProductService : SaasCrudService<Product, long>, IProductService
{
    public ProductService(SmerpContext context) : base(context)
    {
    }
}
