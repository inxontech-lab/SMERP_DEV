using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public class ProductService : SaasCrudService<Product, ProductRequest, long>, IProductService
{
    public ProductService(SmerpContext context) : base(context, MapRequest)
    {
    }

    private static void MapRequest(Product entity, ProductRequest request)
    {
        entity.TenantId = request.TenantId;
        entity.Sku = request.Sku;
        entity.Barcode = request.Barcode;
        entity.Name = request.Name;
        entity.BaseUomId = request.BaseUomId;
        entity.IsActive = request.IsActive;
        entity.CreatedAt = request.CreatedAt;
    }
}
