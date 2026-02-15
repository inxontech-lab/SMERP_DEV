using Domain.SaasDBModels;

namespace SMERPAPIs.Services.SaasServices;

public class ProductPriceService : SaasCrudService<ProductPrice, long>, IProductPriceService
{
    public ProductPriceService(SmerpContext context) : base(context)
    {
    }
}
