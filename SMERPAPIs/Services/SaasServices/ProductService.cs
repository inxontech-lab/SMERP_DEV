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
        entity.NameArabic = request.NameArabic;
        entity.ShortName = request.ShortName;
        entity.Description = request.Description;
        entity.DescriptionArabic = request.DescriptionArabic;
        entity.BaseUomId = request.BaseUomId;
        entity.TaxCodeId = request.TaxCodeId;
        entity.IsVatApplicable = request.IsVatApplicable;
        entity.VatPricingMethod = request.VatPricingMethod;
        entity.IsBatchTracked = request.IsBatchTracked;
        entity.IsSerialTracked = request.IsSerialTracked;
        entity.IsStockItem = request.IsStockItem;
        entity.IsActive = request.IsActive;
        entity.CreatedAt = request.CreatedAt;
    }
}
