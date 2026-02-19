using Domain.SaasReqDTO;
using Microsoft.AspNetCore.Components;
using Radzen;
using SMERPWeb.Services.Auth;
using SMERPWeb.Services.InventoryServices;

namespace SMERPWeb.Components.Pages.Inventory;

public partial class Items : ComponentBase
{
    [Inject] private IItemManagementService ItemManagementService { get; set; } = default!;
    [Inject] private ICrudPermissionService CrudPermissionService { get; set; } = default!;
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;

    protected List<Domain.SaasDBModels.Product> ItemsList { get; set; } = [];
    protected List<Domain.SaasDBModels.Tenant> Tenants { get; set; } = [];
    protected List<Domain.SaasDBModels.Uom> Uoms { get; set; } = [];
    protected bool CanCreateItem { get; set; }
    protected bool CanEditItem { get; set; }
    protected bool CanDeleteItem { get; set; }
    protected int ViewerTenantId { get; set; }
    protected string? ErrorMessage { get; set; }
    protected string SearchText { get; set; } = string.Empty;

    protected IEnumerable<Domain.SaasDBModels.Product> FilteredItems =>
        string.IsNullOrWhiteSpace(SearchText)
            ? ItemsList
            : ItemsList.Where(item =>
                Contains(item.Sku, SearchText) ||
                Contains(item.Name, SearchText) ||
                Contains(item.Barcode, SearchText));

    protected override async Task OnInitializedAsync() => await LoadAsync();

    protected async Task OpenCreateDialogAsync()
    {
        if (!CanCreateItem)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Access denied", "You do not have permission to create items.");
            return;
        }

        await OpenDialogAsync(null);
    }

    protected async Task OpenEditDialogAsync(Domain.SaasDBModels.Product item)
    {
        if (!CanEditItem)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Access denied", "You do not have permission to edit items.");
            return;
        }

        await OpenDialogAsync(item);
    }

    protected async Task DeleteAsync(long id)
    {
        if (!CanDeleteItem)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Access denied", "You do not have permission to delete items.");
            return;
        }

        var confirmed = await DialogService.Confirm("Are you sure you want to delete this item?", "Confirm", new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
        if (confirmed != true)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Cancelled", "Item delete operation cancelled.");
            return;
        }

        var deleted = await ItemManagementService.DeleteAsync(id, ViewerTenantId);
        if (!deleted)
        {
            NotifyTopRight(NotificationSeverity.Error, "Failed", "Unable to delete item.");
            return;
        }

        await LoadItemsAsync();
        NotifyTopRight(NotificationSeverity.Success, "Success", "Item deleted successfully.");
    }

    protected string GetTenantName(int tenantId) => Tenants.FirstOrDefault(t => t.Id == tenantId)?.Name ?? $"Tenant #{tenantId}";
    protected string GetUomName(int uomId) => Uoms.FirstOrDefault(u => u.Id == uomId)?.Name ?? $"UOM #{uomId}";

    private async Task OpenDialogAsync(Domain.SaasDBModels.Product? editingItem)
    {
        var result = await DialogService.OpenAsync<ItemDialog>(editingItem is null ? "Create Item" : "Edit Item", new Dictionary<string, object>
        {
            [nameof(ItemDialog.EditingItem)] = editingItem,
            [nameof(ItemDialog.ViewerTenantId)] = ViewerTenantId,
            [nameof(ItemDialog.Tenants)] = Tenants,
            [nameof(ItemDialog.Uoms)] = Uoms
        }, new DialogOptions { Width = "700px", Draggable = true, Resizable = true, CloseDialogOnEsc = true });

        if (result is not ProductRequest request)
        {
            return;
        }

        var action = editingItem is null ? "create" : "update";
        var confirmed = await DialogService.Confirm($"Are you sure you want to {action} this item?", "Confirm", new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
        if (confirmed != true)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Cancelled", "Item save operation cancelled.");
            return;
        }

        try
        {
            var success = editingItem is null
                ? await CreateItemAsync(request)
                : await ItemManagementService.UpdateAsync(editingItem.Id, request, ViewerTenantId);

            if (!success)
            {
                NotifyTopRight(NotificationSeverity.Error, "Failed", $"Unable to {action} item.");
                return;
            }

            await LoadItemsAsync();
            NotifyTopRight(NotificationSeverity.Success, "Success", editingItem is null ? "Item created successfully." : "Item updated successfully.");
        }
        catch (Exception ex)
        {
            NotifyTopRight(NotificationSeverity.Error, "Failed", ex.Message);
        }
    }

    private async Task<bool> CreateItemAsync(ProductRequest request)
    {
        await ItemManagementService.CreateAsync(request, ViewerTenantId);
        return true;
    }

    private async Task LoadAsync()
    {
        ViewerTenantId = await ItemManagementService.GetViewerTenantIdAsync();
        var permissions = await CrudPermissionService.GetPermissionsAsync("product");
        CanCreateItem = permissions.CanCreate;
        CanEditItem = permissions.CanEdit;
        CanDeleteItem = permissions.CanDelete;

        Tenants = await ItemManagementService.GetTenantsAsync(ViewerTenantId);
        Uoms = await ItemManagementService.GetUomsAsync();
        await LoadItemsAsync();
    }

    private async Task LoadItemsAsync()
    {
        ErrorMessage = null;
        try
        {
            ItemsList = await ItemManagementService.GetItemsAsync(ViewerTenantId);
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
