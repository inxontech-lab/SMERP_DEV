using Domain.InvReqDTO;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace SMERPWeb.Components.Pages.Inventory;

public partial class SupplierDialog : ComponentBase
{
    [Parameter] public Domain.SaasDBModels.InvSupplier? EditingSupplier { get; set; }
    [Parameter] public int ViewerTenantId { get; set; }
    [Parameter] public List<Domain.SaasDBModels.Tenant> Tenants { get; set; } = [];

    [Inject] private DialogService DialogService { get; set; } = default!;

    protected CreateInvSupplierRequest FormModel { get; set; } = new();

    protected override void OnInitialized()
    {
        FormModel = EditingSupplier is null
            ? new CreateInvSupplierRequest
            {
                TenantId = ViewerTenantId <= 1 ? Tenants.FirstOrDefault()?.Id ?? 0 : ViewerTenantId,
                IsActive = true
            }
            : new CreateInvSupplierRequest
            {
                TenantId = EditingSupplier.TenantId,
                Code = EditingSupplier.Code,
                Name = EditingSupplier.Name,
                NameAr = EditingSupplier.NameAr,
                ContactPerson = EditingSupplier.ContactPerson,
                Phone = EditingSupplier.Phone,
                Email = EditingSupplier.Email,
                Address = EditingSupplier.Address,
                AddressAr = EditingSupplier.AddressAr,
                CountryCode = EditingSupplier.CountryCode,
                VatregistrationNo = EditingSupplier.VatregistrationNo,
                Crno = EditingSupplier.Crno,
                PaymentTermsDays = EditingSupplier.PaymentTermsDays,
                IsActive = EditingSupplier.IsActive
            };
    }

    protected void HandleSubmit(CreateInvSupplierRequest _)
    {
        if (ViewerTenantId > 1)
        {
            FormModel.TenantId = ViewerTenantId;
        }

        DialogService.Close(FormModel);
    }

    protected void Cancel() => DialogService.Close();

    protected string GetTenantName(int tenantId)
        => Tenants.FirstOrDefault(t => t.Id == tenantId)?.Name ?? "Unknown Tenant";
}
