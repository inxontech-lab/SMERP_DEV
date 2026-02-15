using Domain.SaasDBModels;

namespace SMERPAPIs.Services.SaasServices;

public class TaxCodeService : SaasCrudService<TaxCode, int>, ITaxCodeService
{
    public TaxCodeService(SmerpContext context) : base(context)
    {
    }
}
