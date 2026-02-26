using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Microsoft.AspNetCore.Components;
using Radzen;
using Shared.Services.Auth;
using Shared.Services.InventoryServices;

namespace SMERPWeb.Components.Pages.Inventory;

public partial class Warehouses : ComponentBase
{
    [Inject] private IWarehouseManagementService WarehouseManagementService { get; set; } = default!;
    [Inject] private ICrudPermissionService CrudPermissionService { get; set; } = default!;
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;

    protected List<InvWarehouse> WarehousesList { get; set; } = [];
    protected List<Tenant> Tenants { get; set; } = [];
    protected bool CanCreateWarehouse { get; set; }
    protected bool CanEditWarehouse { get; set; }
    protected bool CanDeleteWarehouse { get; set; }
    protected int ViewerTenantId { get; set; }
    protected string? ErrorMessage { get; set; }
    protected string SearchText { get; set; } = string.Empty;

    protected IEnumerable<InvWarehouse> FilteredWarehouses =>
        string.IsNullOrWhiteSpace(SearchText)
            ? WarehousesList
            : WarehousesList.Where(warehouse =>
                Contains(warehouse.Code, SearchText) ||
                Contains(warehouse.Name, SearchText));

    protected override async Task OnInitializedAsync() => await LoadAsync();

    protected async Task OpenCreateDialogAsync() => await OpenDialogAsync(null);

    protected async Task OpenEditDialogAsync(InvWarehouse warehouse) => await OpenDialogAsync(warehouse);

    protected async Task DeleteAsync(int id)
    {
        var confirmed = await DialogService.Confirm("Are you sure you want to delete this warehouse?", "Confirm", new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
        if (confirmed != true)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Cancelled", "Warehouse delete operation cancelled.");
            return;
        }

        var deleted = await WarehouseManagementService.DeleteAsync(id, ViewerTenantId);
        if (!deleted)
        {
            NotifyTopRight(NotificationSeverity.Error, "Failed", "Unable to delete warehouse.");
            return;
        }

        await LoadWarehousesAsync();
        NotifyTopRight(NotificationSeverity.Success, "Success", "Warehouse deleted successfully.");
    }

    protected string GetTenantName(int tenantId) => Tenants.FirstOrDefault(tenant => tenant.Id == tenantId)?.Name ?? "Unknown Tenant";

    private async Task OpenDialogAsync(InvWarehouse? editingWarehouse)
    {
        var result = await DialogService.OpenAsync<WarehouseDialog>(editingWarehouse is null ? "Create Warehouse" : "Edit Warehouse", new Dictionary<string, object>
        {
            [nameof(WarehouseDialog.EditingWarehouse)] = editingWarehouse,
            [nameof(WarehouseDialog.ViewerTenantId)] = ViewerTenantId,
            [nameof(WarehouseDialog.Tenants)] = Tenants
        }, new DialogOptions { Width = "900px", Draggable = true, Resizable = true, CloseDialogOnEsc = true });

        if (result is not CreateInvWarehouseRequest request)
        {
            return;
        }

        var action = editingWarehouse is null ? "create" : "update";
        var confirmed = await DialogService.Confirm($"Are you sure you want to {action} this warehouse?", "Confirm", new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
        if (confirmed != true)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Cancelled", "Warehouse save operation cancelled.");
            return;
        }

        try
        {
            var success = editingWarehouse is null
                ? await CreateWarehouseAsync(request)
                : await WarehouseManagementService.UpdateAsync(editingWarehouse.Id, MapToUpdateRequest(editingWarehouse.Id, request), ViewerTenantId);

            if (!success)
            {
                NotifyTopRight(NotificationSeverity.Error, "Failed", $"Unable to {action} warehouse.");
                return;
            }

            await LoadWarehousesAsync();
            NotifyTopRight(NotificationSeverity.Success, "Success", editingWarehouse is null ? "Warehouse created successfully." : "Warehouse updated successfully.");
        }
        catch (Exception ex)
        {
            NotifyTopRight(NotificationSeverity.Error, "Failed", ex.Message);
        }
    }

    private async Task<bool> CreateWarehouseAsync(CreateInvWarehouseRequest request)
    {
        await WarehouseManagementService.CreateAsync(request, ViewerTenantId);
        return true;
    }

    private static UpdateInvWarehouseRequest MapToUpdateRequest(int id, CreateInvWarehouseRequest request)
        => new()
        {
            Id = id,
            TenantId = request.TenantId,
            Code = request.Code,
            Name = request.Name,
            NameAr = request.NameAr,
            Address = request.Address,
            IsDefault = request.IsDefault,
            IsActive = request.IsActive
        };

    private async Task LoadAsync()
    {
        ViewerTenantId = await WarehouseManagementService.GetViewerTenantIdAsync();
        var permissions = await CrudPermissionService.GetPermissionsAsync("warehouse");
        CanCreateWarehouse = permissions.CanCreate;
        CanEditWarehouse = permissions.CanEdit;
        CanDeleteWarehouse = permissions.CanDelete;

        Tenants = await WarehouseManagementService.GetTenantsAsync(ViewerTenantId);
        await LoadWarehousesAsync();
    }

    private async Task LoadWarehousesAsync()
    {
        ErrorMessage = null;
        try
        {
            WarehousesList = await WarehouseManagementService.GetWarehousesAsync(ViewerTenantId);
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
