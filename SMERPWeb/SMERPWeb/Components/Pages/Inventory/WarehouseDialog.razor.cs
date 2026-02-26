using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace SMERPWeb.Components.Pages.Inventory;

public partial class WarehouseDialog : ComponentBase
{
    [Parameter] public InvWarehouse? EditingWarehouse { get; set; }
    [Parameter] public int ViewerTenantId { get; set; }
    [Parameter] public List<Tenant> Tenants { get; set; } = [];

    [Inject] private DialogService DialogService { get; set; } = default!;

    protected CreateInvWarehouseRequest FormModel { get; set; } = new();

    protected override void OnInitialized()
    {
        FormModel = EditingWarehouse is null
            ? new CreateInvWarehouseRequest
            {
                TenantId = ViewerTenantId <= 1 ? Tenants.FirstOrDefault()?.Id ?? 0 : ViewerTenantId,
                IsActive = true
            }
            : new CreateInvWarehouseRequest
            {
                TenantId = EditingWarehouse.TenantId,
                Code = EditingWarehouse.Code,
                Name = EditingWarehouse.Name,
                NameAr = EditingWarehouse.NameAr,
                Address = EditingWarehouse.Address,
                IsDefault = EditingWarehouse.IsDefault,
                IsActive = EditingWarehouse.IsActive
            };
    }

    protected void HandleSubmit(CreateInvWarehouseRequest _)
    {
        if (ViewerTenantId > 1)
        {
            FormModel.TenantId = ViewerTenantId;
        }

        if (FormModel.TenantId <= 0)
        {
            return;
        }

        FormModel.Code = FormModel.Code?.Trim() ?? string.Empty;
        FormModel.Name = FormModel.Name?.Trim() ?? string.Empty;

        DialogService.Close(FormModel);
    }

    protected void Cancel() => DialogService.Close();
}
