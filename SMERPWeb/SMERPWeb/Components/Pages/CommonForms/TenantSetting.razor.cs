using Radzen;
using Domain.SaasDBModels;
using Domain.SaasReqDTO;
using Microsoft.AspNetCore.Components;
using SMERPWeb.Services.SaasServices;

namespace SMERPWeb.Components.Pages.CommonForms;

public partial class TenantSetting : ComponentBase
{
    [Inject] private ITenantManagementApiClient TenantApiClient { get; set; } = default!;
    [Inject] private ITenantSettingManagementApiClient TenantSettingApiClient { get; set; } = default!;
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;

    protected List<Domain.SaasDBModels.Tenant> Tenants { get; set; } = [];
    protected List<Domain.SaasDBModels.TenantSetting> TenantSettings { get; set; } = [];
    protected TenantSettingRequest FormModel { get; set; } = new();
    protected int? EditingTenantId { get; set; }
    protected string? ErrorMessage { get; set; }
    protected string? SuccessMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }

    protected async Task SaveAsync(TenantSettingRequest _)
    {
        ErrorMessage = null;
        SuccessMessage = null;

        var action = EditingTenantId.HasValue ? "update" : "create";
        var confirmed = await DialogService.Confirm(
            $"Are you sure you want to {action} this tenant setting?",
            "Confirm",
            new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });

        if (confirmed != true)
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Cancelled", "Tenant setting save operation cancelled.");
            return;
        }

        try
        {
            if (EditingTenantId.HasValue)
            {
                var updated = await TenantSettingApiClient.UpdateAsync(EditingTenantId.Value, FormModel);
                if (!updated)
                {
                    ErrorMessage = "Unable to update tenant setting.";
                    NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
                    return;
                }

                SuccessMessage = "Tenant setting updated successfully.";
            }
            else
            {
                await TenantSettingApiClient.CreateAsync(FormModel);
                SuccessMessage = "Tenant setting created successfully.";
            }

            NotificationService.Notify(NotificationSeverity.Success, "Success", SuccessMessage);
            await LoadTenantSettingsAsync();
            ResetForm();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
        }
    }

    protected void Edit(Domain.SaasDBModels.TenantSetting item)
    {
        EditingTenantId = item.TenantId;
        FormModel = new TenantSettingRequest
        {
            TenantId = item.TenantId,
            CurrencyCode = item.CurrencyCode,
            MoneyDecimals = item.MoneyDecimals,
            QtyDecimals = item.QtyDecimals,
            VatRateDefault = item.VatRateDefault,
            PricesVatInclusive = item.PricesVatInclusive,
            AllowNegativeStock = item.AllowNegativeStock,
            ReceiptPrefix = item.ReceiptPrefix,
            InvoicePrefix = item.InvoicePrefix
        };
    }

    protected async Task DeleteAsync(int tenantId)
    {
        ErrorMessage = null;
        SuccessMessage = null;

        try
        {
            var deleted = await TenantSettingApiClient.DeleteAsync(tenantId);
            if (!deleted)
            {
                ErrorMessage = "Unable to delete tenant setting.";
                return;
            }

            if (EditingTenantId == tenantId)
            {
                ResetForm();
            }

            SuccessMessage = "Tenant setting deleted successfully.";
            await LoadTenantSettingsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    protected void ResetForm()
    {
        EditingTenantId = null;
        ErrorMessage = null;
        SuccessMessage = null;
        FormModel = new TenantSettingRequest
        {
            CurrencyCode = "SAR",
            MoneyDecimals = 2,
            QtyDecimals = 2,
            VatRateDefault = 15
        };
    }

    private async Task LoadAsync()
    {
        Tenants = await TenantApiClient.GetAllAsync();
        await LoadTenantSettingsAsync();
        ResetForm();
    }

    private async Task LoadTenantSettingsAsync()
    {
        TenantSettings = await TenantSettingApiClient.GetAllAsync();

        foreach (var tenantSetting in TenantSettings)
        {
            tenantSetting.Tenant = Tenants.FirstOrDefault(t => t.Id == tenantSetting.TenantId) ?? new Domain.SaasDBModels.Tenant
            {
                Id = tenantSetting.TenantId,
                Name = $"Tenant #{tenantSetting.TenantId}"
            };
        }
    }
}
