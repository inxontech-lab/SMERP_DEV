using Domain.SaasReqDTO;
using Microsoft.AspNetCore.Components;
using Radzen;
using SMERPWeb.Services.SaasServices;

namespace SMERPWeb.Components.Pages.CommonForms;

public partial class Role : ComponentBase
{
    [Inject] private ITenantManagementApiClient TenantApiClient { get; set; } = default!;
    [Inject] private IRoleManagementApiClient RoleApiClient { get; set; } = default!;
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;

    protected List<Domain.SaasDBModels.Tenant> Tenants { get; set; } = [];
    protected List<Domain.SaasDBModels.Role> Roles { get; set; } = [];
    protected RoleRequest FormModel { get; set; } = new();
    protected int? EditingId { get; set; }
    protected string? ErrorMessage { get; set; }
    protected string? SuccessMessage { get; set; }

    protected override async Task OnInitializedAsync() => await LoadAsync();

    protected async Task SaveAsync(RoleRequest _)
    {
        ErrorMessage = null;
        SuccessMessage = null;

        var action = EditingId.HasValue ? "update" : "create";
        var confirmed = await DialogService.Confirm(
            $"Are you sure you want to {action} this role?",
            "Confirm",
            new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });

        if (confirmed != true)
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Cancelled", "Role save operation cancelled.");
            return;
        }

        try
        {
            if (EditingId.HasValue)
            {
                var updated = await RoleApiClient.UpdateAsync(EditingId.Value, FormModel);
                if (!updated)
                {
                    ErrorMessage = "Unable to update role.";
                    NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
                    return;
                }

                SuccessMessage = "Role updated successfully.";
            }
            else
            {
                await RoleApiClient.CreateAsync(FormModel);
                SuccessMessage = "Role created successfully.";
            }

            NotificationService.Notify(NotificationSeverity.Success, "Success", SuccessMessage);
            await LoadRolesAsync();
            ResetForm();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
        }
    }

    protected void Edit(Domain.SaasDBModels.Role item)
    {
        EditingId = item.Id;
        FormModel = new RoleRequest
        {
            TenantId = item.TenantId,
            Name = item.Name,
            IsActive = item.IsActive
        };
    }

    protected async Task DeleteAsync(int id)
    {
        ErrorMessage = null;
        SuccessMessage = null;

        var confirmed = await DialogService.Confirm(
            "Are you sure you want to delete this role?",
            "Confirm",
            new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });

        if (confirmed != true)
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Cancelled", "Role delete operation cancelled.");
            return;
        }

        try
        {
            var deleted = await RoleApiClient.DeleteAsync(id);
            if (!deleted)
            {
                ErrorMessage = "Unable to delete role.";
                NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
                return;
            }

            if (EditingId == id)
            {
                ResetForm();
            }

            SuccessMessage = "Role deleted successfully.";
            NotificationService.Notify(NotificationSeverity.Success, "Success", SuccessMessage);
            await LoadRolesAsync();
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
        FormModel = new RoleRequest { IsActive = true };
    }

    private async Task LoadAsync()
    {
        Tenants = await TenantApiClient.GetAllAsync();
        await LoadRolesAsync();
        ResetForm();
    }

    private async Task LoadRolesAsync() => Roles = await RoleApiClient.GetAllAsync();
}
