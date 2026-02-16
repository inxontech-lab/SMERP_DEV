using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public class TenantSettingService : SaasCrudService<TenantSetting, TenantSettingRequest, int>, ITenantSettingService
{
    public TenantSettingService(SmerpContext context) : base(context, MapRequest)
    {
    }

    private static void MapRequest(TenantSetting entity, TenantSettingRequest request)
    {
        entity.TenantId = request.TenantId;
        entity.CurrencyCode = request.CurrencyCode;
        entity.MoneyDecimals = request.MoneyDecimals;
        entity.QtyDecimals = request.QtyDecimals;
        entity.VatRateDefault = request.VatRateDefault;
        entity.PricesVatInclusive = request.PricesVatInclusive;
        entity.AllowNegativeStock = request.AllowNegativeStock;
        entity.ReceiptPrefix = request.ReceiptPrefix;
        entity.InvoicePrefix = request.InvoicePrefix;
    }
}
