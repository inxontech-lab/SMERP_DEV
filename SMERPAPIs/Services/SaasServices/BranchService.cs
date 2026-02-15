using Domain.SaasDBModels;

namespace SMERPAPIs.Services.SaasServices;

public class BranchService : SaasCrudService<Branch, int>, IBranchService
{
    public BranchService(SmerpContext context) : base(context)
    {
    }
}
