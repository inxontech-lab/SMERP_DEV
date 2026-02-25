using Domain.SaasReqDTO;
using Microsoft.AspNetCore.Components;
using Radzen;
using Shared.Services.SaasServices;

namespace SMERPWeb.Components.Pages.CommonForms;

public partial class Branch : ComponentBase
{
    [Inject] private ITenantManagementApiClient TenantApiClient { get; set; } = default!;
    [Inject] private IBranchManagementApiClient BranchApiClient { get; set; } = default!;
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;

    protected List<Domain.SaasDBModels.Tenant> Tenants { get; set; } = [];
    protected List<Domain.SaasDBModels.Branch> Branches { get; set; } = [];
    protected BranchRequest FormModel { get; set; } = new();
    protected int? EditingId { get; set; }
    protected string? ErrorMessage { get; set; }
    protected string? SuccessMessage { get; set; }

    protected override async Task OnInitializedAsync() => await LoadAsync();

    protected async Task SaveAsync(BranchRequest _)
    {
        ErrorMessage = null;
        SuccessMessage = null;

        var action = EditingId.HasValue ? "update" : "create";
        var confirmed = await DialogService.Confirm(
            $"Are you sure you want to {action} this branch?",
            "Confirm",
            new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });

        if (confirmed != true)
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Cancelled", "Branch save operation cancelled.");
            return;
        }

        try
        {
            if (EditingId.HasValue)
            {
                var updated = await BranchApiClient.UpdateAsync(EditingId.Value, FormModel);
                if (!updated)
                {
                    ErrorMessage = "Unable to update branch.";
                    NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
                    return;
                }

                SuccessMessage = "Branch updated successfully.";
            }
            else
            {
                await BranchApiClient.CreateAsync(FormModel);
                SuccessMessage = "Branch created successfully.";
            }

            NotificationService.Notify(NotificationSeverity.Success, "Success", SuccessMessage);
            await LoadBranchesAsync();
            ResetForm();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
        }
    }

    protected void Edit(Domain.SaasDBModels.Branch item)
    {
        EditingId = item.Id;
        FormModel = new BranchRequest
        {
            TenantId = item.TenantId,
            Code = item.Code,
            Name = item.Name,
            Address = item.Address,
            IsActive = item.IsActive
        };
    }

    protected async Task DeleteAsync(int id)
    {
        ErrorMessage = null;
        SuccessMessage = null;

        var confirmed = await DialogService.Confirm(
            "Are you sure you want to delete this branch?",
            "Confirm",
            new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });

        if (confirmed != true)
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Cancelled", "Branch delete operation cancelled.");
            return;
        }

        try
        {
            var deleted = await BranchApiClient.DeleteAsync(id);
            if (!deleted)
            {
                ErrorMessage = "Unable to delete branch.";
                NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
                return;
            }

            if (EditingId == id)
            {
                ResetForm();
            }

            SuccessMessage = "Branch deleted successfully.";
            NotificationService.Notify(NotificationSeverity.Success, "Success", SuccessMessage);
            await LoadBranchesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
        }
    }

    protected void ResetForm()
    {
        EditingId = null;
        ErrorMessage = null;
        SuccessMessage = null;
        FormModel = new BranchRequest { IsActive = true };
    }

    private async Task LoadAsync()
    {
        Tenants = await TenantApiClient.GetAllAsync();
        await LoadBranchesAsync();
        ResetForm();
    }


    protected string GetTenantName(int tenantId) =>
        Tenants.FirstOrDefault(t => t.Id == tenantId)?.Name ?? $"Tenant #{tenantId}";

    private async Task LoadBranchesAsync() => Branches = await BranchApiClient.GetAllAsync();
}
