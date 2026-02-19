using Domain.SaasReqDTO;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace SMERPWeb.Components.Pages.Inventory;

public partial class ItemDialog : ComponentBase
{
    [Parameter] public Domain.SaasDBModels.Product? EditingItem { get; set; }
    [Parameter] public int ViewerTenantId { get; set; }
    [Parameter] public List<Domain.SaasDBModels.Tenant> Tenants { get; set; } = [];
    [Parameter] public List<Domain.SaasDBModels.Uom> Uoms { get; set; } = [];

    [Inject] private DialogService DialogService { get; set; } = default!;

    protected ProductRequest FormModel { get; set; } = new();

    protected override void OnInitialized()
    {
        FormModel = EditingItem is null
            ? new ProductRequest
            {
                TenantId = ViewerTenantId == 1 ? Tenants.FirstOrDefault()?.Id ?? 1 : ViewerTenantId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
            : new ProductRequest
            {
                TenantId = EditingItem.TenantId,
                Sku = EditingItem.Sku,
                Barcode = EditingItem.Barcode,
                Name = EditingItem.Name,
                BaseUomId = EditingItem.BaseUomId,
                IsActive = EditingItem.IsActive,
                CreatedAt = EditingItem.CreatedAt
            };
    }

    protected void HandleSubmit(ProductRequest _)
    {
        if (ViewerTenantId > 1)
        {
            FormModel.TenantId = ViewerTenantId;
        }

        DialogService.Close(FormModel);
    }

    protected void Cancel() => DialogService.Close();

    protected string GetTenantName(int tenantId)
        => Tenants.FirstOrDefault(t => t.Id == tenantId)?.Name ?? $"Tenant #{tenantId}";
}
