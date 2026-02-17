using Domain.SaasReqDTO;
using Microsoft.AspNetCore.Components;
using Radzen;
using SMERPWeb.Services.SaasServices;

namespace SMERPWeb.Components.Pages.CommonForms;

public partial class PosTerminal : ComponentBase
{
    [Inject] private ITenantManagementApiClient TenantApiClient { get; set; } = default!;
    [Inject] private IBranchManagementApiClient BranchApiClient { get; set; } = default!;
    [Inject] private IPosTerminalManagementApiClient PosTerminalApiClient { get; set; } = default!;
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;

    protected List<Domain.SaasDBModels.Tenant> Tenants { get; set; } = [];
    protected List<Domain.SaasDBModels.Branch> Branches { get; set; } = [];
    protected List<Domain.SaasDBModels.Branch> TenantBranches { get; set; } = [];
    protected List<Domain.SaasDBModels.PosTerminal> PosTerminals { get; set; } = [];
    protected PosTerminalRequest FormModel { get; set; } = new();
    protected int? EditingId { get; set; }
    protected string? ErrorMessage { get; set; }
    protected string? SuccessMessage { get; set; }

    protected override async Task OnInitializedAsync() => await LoadAsync();

    protected async Task SaveAsync(PosTerminalRequest _)
    {
        ErrorMessage = null;
        SuccessMessage = null;

        var action = EditingId.HasValue ? "update" : "create";
        var confirmed = await DialogService.Confirm(
            $"Are you sure you want to {action} this POS terminal?",
            "Confirm",
            new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });

        if (confirmed != true)
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Cancelled", "POS terminal save operation cancelled.");
            return;
        }

        try
        {
            if (EditingId.HasValue)
            {
                var updated = await PosTerminalApiClient.UpdateAsync(EditingId.Value, FormModel);
                if (!updated)
                {
                    ErrorMessage = "Unable to update POS terminal.";
                    NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
                    return;
                }

                SuccessMessage = "POS terminal updated successfully.";
            }
            else
            {
                await PosTerminalApiClient.CreateAsync(FormModel);
                SuccessMessage = "POS terminal created successfully.";
            }

            NotificationService.Notify(NotificationSeverity.Success, "Success", SuccessMessage);
            await LoadPosTerminalsAsync();
            ResetForm();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
        }
    }

    protected void Edit(Domain.SaasDBModels.PosTerminal item)
    {
        EditingId = item.Id;
        FormModel = new PosTerminalRequest
        {
            TenantId = item.TenantId,
            BranchId = item.BranchId,
            Code = item.Code,
            Name = item.Name,
            IsKiosk = item.IsKiosk,
            IsActive = item.IsActive
        };

        SetTenantBranches(item.TenantId);
    }

    protected async Task DeleteAsync(int id)
    {
        ErrorMessage = null;
        SuccessMessage = null;

        var confirmed = await DialogService.Confirm(
            "Are you sure you want to delete this POS terminal?",
            "Confirm",
            new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });

        if (confirmed != true)
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Cancelled", "POS terminal delete operation cancelled.");
            return;
        }

        try
        {
            var deleted = await PosTerminalApiClient.DeleteAsync(id);
            if (!deleted)
            {
                ErrorMessage = "Unable to delete POS terminal.";
                NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
                return;
            }

            if (EditingId == id)
            {
                ResetForm();
            }

            SuccessMessage = "POS terminal deleted successfully.";
            NotificationService.Notify(NotificationSeverity.Success, "Success", SuccessMessage);
            await LoadPosTerminalsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
        }
    }

    protected void OnTenantChanged(object _)
    {
        SetTenantBranches(FormModel.TenantId);

        if (!TenantBranches.Any(branch => branch.Id == FormModel.BranchId))
        {
            FormModel.BranchId = 0;
        }
    }

    protected void ResetForm()
    {
        EditingId = null;
        ErrorMessage = null;
        SuccessMessage = null;
        FormModel = new PosTerminalRequest { IsActive = true };
        TenantBranches = [];
    }

    private async Task LoadAsync()
    {
        Tenants = await TenantApiClient.GetAllAsync();
        Branches = await BranchApiClient.GetAllAsync();
        await LoadPosTerminalsAsync();
        ResetForm();
    }

    protected string GetTenantName(int tenantId) =>
        Tenants.FirstOrDefault(t => t.Id == tenantId)?.Name ?? $"Tenant #{tenantId}";

    protected string GetBranchName(int branchId) =>
        Branches.FirstOrDefault(b => b.Id == branchId)?.Name ?? $"Branch #{branchId}";

    private async Task LoadPosTerminalsAsync() => PosTerminals = await PosTerminalApiClient.GetAllAsync();

    private void SetTenantBranches(int tenantId) => TenantBranches = Branches.Where(b => b.TenantId == tenantId).ToList();
}
