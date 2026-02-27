using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace SMERPWeb.Components.Pages.Inventory;

public partial class ItemCategoryDialog : ComponentBase
{
    [Parameter] public InvItemCategory? EditingItemCategory { get; set; }
    [Parameter] public int ViewerTenantId { get; set; }
    [Parameter] public List<Tenant> Tenants { get; set; } = [];

    [Inject] private DialogService DialogService { get; set; } = default!;

    protected CreateInvItemCategoryRequest FormModel { get; set; } = new();

    protected override void OnInitialized()
    {
        FormModel = EditingItemCategory is null
            ? new CreateInvItemCategoryRequest
            {
                TenantId = ViewerTenantId <= 1 ? Tenants.FirstOrDefault()?.Id ?? 0 : ViewerTenantId,
                IsActive = true
            }
            : new CreateInvItemCategoryRequest
            {
                TenantId = EditingItemCategory.TenantId,
                Code = EditingItemCategory.Code,
                Name = EditingItemCategory.Name,
                NameAr = EditingItemCategory.NameAr,
                IsActive = EditingItemCategory.IsActive
            };
    }

    protected void HandleSubmit(CreateInvItemCategoryRequest _)
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
