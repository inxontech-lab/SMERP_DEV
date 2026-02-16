using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public class ProductUomService : SaasCrudService<ProductUom, ProductUomRequest, long>, IProductUomService
{
    public ProductUomService(SmerpContext context) : base(context, MapRequest)
    {
    }

    private static void MapRequest(ProductUom entity, ProductUomRequest request)
    {
        entity.TenantId = request.TenantId;
        entity.ProductId = request.ProductId;
        entity.UomId = request.UomId;
        entity.FactorToBase = request.FactorToBase;
        entity.QtyStep = request.QtyStep;
        entity.MaxDecimals = request.MaxDecimals;
        entity.IsSellable = request.IsSellable;
        entity.IsPurchasable = request.IsPurchasable;
        entity.IsDefaultSell = request.IsDefaultSell;
        entity.IsDefaultBuy = request.IsDefaultBuy;
    }
}
