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
        entity.UomId = request.UomId;
        entity.BranchId = request.BranchId;
        entity.PriceType = request.PriceType;
        entity.Price = request.Price;
        entity.MinQty = request.MinQty;
        entity.EffectiveFrom = request.EffectiveFrom;
        entity.EffectiveTo = request.EffectiveTo;
        entity.IsActive = request.IsActive;
    }
}
