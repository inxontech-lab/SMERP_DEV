using Domain.SaasDBModels;

namespace SMERPAPIs.Services.SaasServices;

public class UomService : SaasCrudService<Uom, int>, IUomService
{
    public UomService(SmerpContext context) : base(context)
    {
    }
}
