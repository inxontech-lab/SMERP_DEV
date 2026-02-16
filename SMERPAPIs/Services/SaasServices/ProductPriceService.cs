using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public class ProductPriceService : SaasCrudService<ProductPrice, ProductPriceRequest, long>, IProductPriceService
{
    public ProductPriceService(SmerpContext context) : base(context, MapRequest)
    {
    }

    private static void MapRequest(ProductPrice entity, ProductPriceRequest request)
    {
        entity.TenantId = request.TenantId;
        entity.ProductId = request.ProductId;
        entity.TaxCodeId = request.TaxCodeId;
        entity.SellPrice = request.SellPrice;
        entity.IsVatInclusive = request.IsVatInclusive;
        entity.DefaultSellUomId = request.DefaultSellUomId;
    }
}
