using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace SMERPWeb.Components.Pages.Inventory;

public partial class UomDialog : ComponentBase
{
    [Parameter] public InvUom? EditingUom { get; set; }
    [Parameter] public int ViewerTenantId { get; set; }
    [Parameter] public List<Tenant> Tenants { get; set; } = [];

    [Inject] private DialogService DialogService { get; set; } = default!;

    protected CreateInvUomRequest FormModel { get; set; } = new();

    protected override void OnInitialized()
    {
        FormModel = EditingUom is null
            ? new CreateInvUomRequest
            {
                TenantId = ViewerTenantId <= 1 ? Tenants.FirstOrDefault()?.Id ?? 0 : ViewerTenantId,
                IsActive = true
            }
            : new CreateInvUomRequest
            {
                TenantId = EditingUom.TenantId,
                Code = EditingUom.Code,
                Name = EditingUom.Name,
                NameAr = EditingUom.NameAr,
                IsBase = EditingUom.IsBase,
                IsActive = EditingUom.IsActive
            };
    }

    protected void HandleSubmit(CreateInvUomRequest _)
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
