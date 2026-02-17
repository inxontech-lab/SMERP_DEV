using Domain.SaasDBModels;
using Domain.SaasReqDTO;
using Microsoft.AspNetCore.Components;
using SMERPWeb.Services.SaasServices;

namespace SMERPWeb.Components.Pages.CommonForms;

public partial class Tenant : ComponentBase
{
    [Inject] private ITenantManagementApiClient TenantApiClient { get; set; } = default!;

    protected List<Domain.SaasDBModels.Tenant> Tenants { get; set; } = [];
    protected TenantRequest FormModel { get; set; } = BuildDefaultForm();
    protected int? EditingId { get; set; }
    protected string? ErrorMessage { get; set; }
    protected string? SuccessMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }

    protected async Task SaveAsync(TenantRequest _)
    {
        ErrorMessage = null;
        SuccessMessage = null;

        try
        {
            if (EditingId.HasValue)
            {
                var updated = await TenantApiClient.UpdateAsync(EditingId.Value, FormModel);
                if (!updated)
                {
                    ErrorMessage = "Unable to update tenant.";
                    return;
                }

                SuccessMessage = "Tenant updated successfully.";
            }
            else
            {
                await TenantApiClient.CreateAsync(FormModel);
                SuccessMessage = "Tenant created successfully.";
            }

            await LoadAsync();
            ResetForm();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    protected void Edit(Domain.SaasDBModels.Tenant item)
    {
        EditingId = item.Id;
        FormModel = new TenantRequest
        {
            Code = item.Code,
            Name = item.Name,
            LegalName = item.LegalName,
            VatNo = item.VatNo,
            CrNo = item.CrNo,
            CountryCode = item.CountryCode,
            TimeZone = item.TimeZone,
            IsActive = item.IsActive,
            CreatedAt = item.CreatedAt
        };
    }

    protected async Task DeleteAsync(int id)
    {
        ErrorMessage = null;
        SuccessMessage = null;

        try
        {
            var deleted = await TenantApiClient.DeleteAsync(id);
            if (!deleted)
            {
                ErrorMessage = "Unable to delete tenant.";
                return;
            }

            if (EditingId == id)
            {
                ResetForm();
            }

            SuccessMessage = "Tenant deleted successfully.";
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    protected void ResetForm()
    {
        EditingId = null;
        ErrorMessage = null;
        SuccessMessage = null;
        FormModel = BuildDefaultForm();
    }

    private async Task LoadAsync()
    {
        Tenants = await TenantApiClient.GetAllAsync();
    }

    private static TenantRequest BuildDefaultForm() => new()
    {
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };
}
