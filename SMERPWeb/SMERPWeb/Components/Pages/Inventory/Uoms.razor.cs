using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Microsoft.AspNetCore.Components;
using Radzen;
using Shared.Services.Auth;
using Shared.Services.InventoryServices;

namespace SMERPWeb.Components.Pages.Inventory;

public partial class Uoms : ComponentBase
{
    [Inject] private IUomManagementService UomManagementService { get; set; } = default!;
    [Inject] private ICrudPermissionService CrudPermissionService { get; set; } = default!;
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;

    protected List<InvUom> UomsList { get; set; } = [];
    protected List<Tenant> Tenants { get; set; } = [];
    protected bool CanCreateUom { get; set; }
    protected bool CanEditUom { get; set; }
    protected bool CanDeleteUom { get; set; }
    protected int ViewerTenantId { get; set; }
    protected string? ErrorMessage { get; set; }
    protected string SearchText { get; set; } = string.Empty;

    protected IEnumerable<InvUom> FilteredUoms =>
        string.IsNullOrWhiteSpace(SearchText)
            ? UomsList
            : UomsList.Where(uom =>
                Contains(uom.Code, SearchText) ||
                Contains(uom.Name, SearchText));

    protected override async Task OnInitializedAsync() => await LoadAsync();

    protected async Task OpenCreateDialogAsync()
    {
        if (!CanCreateUom)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Access denied", "You do not have permission to create UOMs.");
            return;
        }

        await OpenDialogAsync(null);
    }

    protected async Task OpenEditDialogAsync(InvUom uom)
    {
        if (!CanEditUom)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Access denied", "You do not have permission to edit UOMs.");
            return;
        }

        await OpenDialogAsync(uom);
    }

    protected async Task DeleteAsync(int id)
    {
        if (!CanDeleteUom)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Access denied", "You do not have permission to delete UOMs.");
            return;
        }

        var confirmed = await DialogService.Confirm("Are you sure you want to delete this UOM?", "Confirm", new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
        if (confirmed != true)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Cancelled", "UOM delete operation cancelled.");
            return;
        }

        var deleted = await UomManagementService.DeleteAsync(id, ViewerTenantId);
        if (!deleted)
        {
            NotifyTopRight(NotificationSeverity.Error, "Failed", "Unable to delete UOM.");
            return;
        }

        await LoadUomsAsync();
        NotifyTopRight(NotificationSeverity.Success, "Success", "UOM deleted successfully.");
    }

    protected string GetTenantName(int tenantId) => Tenants.FirstOrDefault(tenant => tenant.Id == tenantId)?.Name ?? "Unknown Tenant";

    private async Task OpenDialogAsync(InvUom? editingUom)
    {
        var result = await DialogService.OpenAsync<UomDialog>(editingUom is null ? "Create UOM" : "Edit UOM", new Dictionary<string, object>
        {
            [nameof(UomDialog.EditingUom)] = editingUom,
            [nameof(UomDialog.ViewerTenantId)] = ViewerTenantId,
            [nameof(UomDialog.Tenants)] = Tenants
        }, new DialogOptions { Width = "900px", Draggable = true, Resizable = true, CloseDialogOnEsc = true });

        if (result is not CreateInvUomRequest request)
        {
            return;
        }

        var action = editingUom is null ? "create" : "update";
        var confirmed = await DialogService.Confirm($"Are you sure you want to {action} this UOM?", "Confirm", new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
        if (confirmed != true)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Cancelled", "UOM save operation cancelled.");
            return;
        }

        try
        {
            var success = editingUom is null
                ? await CreateUomAsync(request)
                : await UomManagementService.UpdateAsync(editingUom.Id, MapToUpdateRequest(editingUom.Id, request), ViewerTenantId);

            if (!success)
            {
                NotifyTopRight(NotificationSeverity.Error, "Failed", $"Unable to {action} UOM.");
                return;
            }

            await LoadUomsAsync();
            NotifyTopRight(NotificationSeverity.Success, "Success", editingUom is null ? "UOM created successfully." : "UOM updated successfully.");
        }
        catch (Exception ex)
        {
            NotifyTopRight(NotificationSeverity.Error, "Failed", ex.Message);
        }
    }

    private async Task<bool> CreateUomAsync(CreateInvUomRequest request)
    {
        await UomManagementService.CreateAsync(request, ViewerTenantId);
        return true;
    }

    private static UpdateInvUomRequest MapToUpdateRequest(int id, CreateInvUomRequest request)
        => new()
        {
            Id = id,
            TenantId = request.TenantId,
            Code = request.Code,
            Name = request.Name,
            NameAr = request.NameAr,
            IsBase = request.IsBase,
            IsActive = request.IsActive
        };

    private async Task LoadAsync()
    {
        ViewerTenantId = await UomManagementService.GetViewerTenantIdAsync();
        var permissions = await CrudPermissionService.GetPermissionsAsync("uom");
        CanCreateUom = permissions.CanCreate;
        CanEditUom = permissions.CanEdit;
        CanDeleteUom = permissions.CanDelete;

        Tenants = await UomManagementService.GetTenantsAsync(ViewerTenantId);
        await LoadUomsAsync();
    }

    private async Task LoadUomsAsync()
    {
        ErrorMessage = null;
        try
        {
            UomsList = await UomManagementService.GetUomsAsync(ViewerTenantId);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    private static bool Contains(string? value, string term)
        => !string.IsNullOrWhiteSpace(value) && value.Contains(term, StringComparison.OrdinalIgnoreCase);

    private void NotifyTopRight(NotificationSeverity severity, string summary, string detail)
    {
        NotificationService.Notify(new NotificationMessage
        {
            Severity = severity,
            Summary = summary,
            Detail = detail,
            Duration = 4000,
            CloseOnClick = true
        });
    }
}
