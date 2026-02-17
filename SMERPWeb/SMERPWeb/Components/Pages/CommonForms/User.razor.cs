using Domain.SaasDBModels;
using Domain.SaasReqDTO;
using Microsoft.AspNetCore.Components;
using Radzen;
using SMERPWeb.Services.SaasServices;

namespace SMERPWeb.Components.Pages.CommonForms;

public partial class User : ComponentBase
{
    [Inject] private IUserOnboardingService UserOnboardingService { get; set; } = default!;
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;

    protected List<Tenant> Tenants { get; set; } = [];
    protected List<Role> Roles { get; set; } = [];
    protected List<UserWithRoleResponse> Users { get; set; } = [];
    protected CreateUserWithRoleRequest FormModel { get; set; } = new();
    protected long? EditingId { get; set; }
    protected string? ErrorMessage { get; set; }
    protected string? SuccessMessage { get; set; }

    protected IEnumerable<Role> FilteredRoles => Roles.Where(item => item.TenantId == FormModel.TenantId);

    protected override async Task OnInitializedAsync() => await LoadAsync();

    protected async Task SaveAsync(CreateUserWithRoleRequest _)
    {
        ErrorMessage = null;
        SuccessMessage = null;

        if (!EditingId.HasValue && string.IsNullOrWhiteSpace(FormModel.Password))
        {
            ErrorMessage = "Password is required for creating user.";
            NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
            return;
        }

        var action = EditingId.HasValue ? "update" : "create";
        var confirmed = await DialogService.Confirm(
            $"Are you sure you want to {action} this user?",
            "Confirm",
            new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });

        if (confirmed != true)
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Cancelled", "User save operation cancelled.");
            return;
        }

        try
        {
            if (EditingId.HasValue)
            {
                var updateRequest = new UpdateUserWithRoleRequest
                {
                    TenantId = FormModel.TenantId,
                    RoleId = FormModel.RoleId,
                    Username = FormModel.Username,
                    DisplayName = FormModel.DisplayName,
                    Password = string.IsNullOrWhiteSpace(FormModel.Password) ? null : FormModel.Password,
                    Email = FormModel.Email,
                    Mobile = FormModel.Mobile,
                    IsActive = FormModel.IsActive
                };

                var updated = await UserOnboardingService.UpdateUserWithRoleAsync(EditingId.Value, updateRequest);
                if (!updated)
                {
                    ErrorMessage = "Unable to update user.";
                    NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
                    return;
                }

                SuccessMessage = "User updated successfully.";
            }
            else
            {
                await UserOnboardingService.CreateUserWithRoleAsync(FormModel);
                SuccessMessage = "User created successfully.";
            }

            NotificationService.Notify(NotificationSeverity.Success, "Success", SuccessMessage);
            await LoadUsersAsync();
            ResetForm();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
        }
    }

    protected void Edit(UserWithRoleResponse item)
    {
        EditingId = item.UserId;
        FormModel = new CreateUserWithRoleRequest
        {
            TenantId = item.TenantId,
            RoleId = item.RoleId ?? 0,
            Username = item.Username,
            DisplayName = item.DisplayName,
            Email = item.Email,
            Mobile = item.Mobile,
            IsActive = item.IsActive,
            Password = string.Empty
        };
    }

    protected async Task DeleteAsync(long userId)
    {
        ErrorMessage = null;
        SuccessMessage = null;

        var confirmed = await DialogService.Confirm(
            "Are you sure you want to delete this user?",
            "Confirm",
            new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });

        if (confirmed != true)
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Cancelled", "User delete operation cancelled.");
            return;
        }

        try
        {
            var deleted = await UserOnboardingService.DeleteUserWithRoleAsync(userId);
            if (!deleted)
            {
                ErrorMessage = "Unable to delete user.";
                NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
                return;
            }

            if (EditingId == userId)
            {
                ResetForm();
            }

            SuccessMessage = "User deleted successfully.";
            NotificationService.Notify(NotificationSeverity.Success, "Success", SuccessMessage);
            await LoadUsersAsync();
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

        FormModel = new CreateUserWithRoleRequest
        {
            IsActive = true,
            TenantId = Tenants.FirstOrDefault()?.Id ?? 0
        };

        OnTenantChanged(FormModel.TenantId);
    }

    protected void OnTenantChanged(object value)
    {
        var tenantId = value switch
        {
            int intValue => intValue,
            _ => FormModel.TenantId
        };

        FormModel.TenantId = tenantId;

        if (!FilteredRoles.Any(item => item.Id == FormModel.RoleId))
        {
            FormModel.RoleId = FilteredRoles.FirstOrDefault()?.Id ?? 0;
        }
    }

    private async Task LoadAsync()
    {
        Tenants = await UserOnboardingService.GetTenantsAsync();
        Roles = await UserOnboardingService.GetRolesAsync();
        await LoadUsersAsync();
        ResetForm();
    }

    private async Task LoadUsersAsync() => Users = await UserOnboardingService.GetUsersWithRolesAsync();
}
