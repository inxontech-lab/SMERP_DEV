using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public class UomService : SaasCrudService<Uom, UomRequest, int>, IUomService
{
    public UomService(SmerpContext context) : base(context, MapRequest)
    {
    }

    private static void MapRequest(Uom entity, UomRequest request)
    {
        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.UomType = request.UomType;
    }
}
