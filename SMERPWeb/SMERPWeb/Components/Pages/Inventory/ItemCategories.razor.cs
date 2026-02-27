using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Microsoft.AspNetCore.Components;
using Radzen;
using Shared.Services.Auth;
using Shared.Services.InventoryServices;

namespace SMERPWeb.Components.Pages.Inventory;

public partial class ItemCategories : ComponentBase
{
    [Inject] private IInvItemCategoryManagementService ItemCategoryManagementService { get; set; } = default!;
    [Inject] private ICrudPermissionService CrudPermissionService { get; set; } = default!;
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;

    protected List<InvItemCategory> ItemCategoriesList { get; set; } = [];
    protected List<Tenant> Tenants { get; set; } = [];
    protected bool CanCreateItemCategory { get; set; }
    protected bool CanEditItemCategory { get; set; }
    protected bool CanDeleteItemCategory { get; set; }
    protected int ViewerTenantId { get; set; }
    protected string? ErrorMessage { get; set; }
    protected string SearchText { get; set; } = string.Empty;

    protected IEnumerable<InvItemCategory> FilteredItemCategories =>
        string.IsNullOrWhiteSpace(SearchText)
            ? ItemCategoriesList
            : ItemCategoriesList.Where(x =>
                Contains(x.Code, SearchText) ||
                Contains(x.Name, SearchText) ||
                Contains(x.NameAr, SearchText));

    protected override async Task OnInitializedAsync() => await LoadAsync();

    protected async Task OpenCreateDialogAsync()
    {
        if (!CanCreateItemCategory)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Access denied", "You do not have permission to create item categories.");
            return;
        }

        await OpenDialogAsync(null);
    }

    protected async Task OpenEditDialogAsync(InvItemCategory itemCategory)
    {
        if (!CanEditItemCategory)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Access denied", "You do not have permission to edit item categories.");
            return;
        }

        await OpenDialogAsync(itemCategory);
    }

    protected async Task DeleteAsync(int id)
    {
        if (!CanDeleteItemCategory)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Access denied", "You do not have permission to delete item categories.");
            return;
        }

        var confirmed = await DialogService.Confirm("Are you sure you want to delete this item category?", "Confirm", new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
        if (confirmed != true)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Cancelled", "Item category delete operation cancelled.");
            return;
        }

        var deleted = await ItemCategoryManagementService.DeleteAsync(id, ViewerTenantId);
        if (!deleted)
        {
            NotifyTopRight(NotificationSeverity.Error, "Failed", "Unable to delete item category.");
            return;
        }

        await LoadItemCategoriesAsync();
        NotifyTopRight(NotificationSeverity.Success, "Success", "Item category deleted successfully.");
    }

    protected string GetTenantName(int tenantId) => Tenants.FirstOrDefault(t => t.Id == tenantId)?.Name ?? "Unknown Tenant";

    private async Task OpenDialogAsync(InvItemCategory? editingItemCategory)
    {
        var result = await DialogService.OpenAsync<ItemCategoryDialog>(editingItemCategory is null ? "Create Item Category" : "Edit Item Category", new Dictionary<string, object>
        {
            [nameof(ItemCategoryDialog.EditingItemCategory)] = editingItemCategory,
            [nameof(ItemCategoryDialog.ViewerTenantId)] = ViewerTenantId,
            [nameof(ItemCategoryDialog.Tenants)] = Tenants
        }, new DialogOptions { Width = "400px", Draggable = true, Resizable = true, CloseDialogOnEsc = true });

        if (result is not CreateInvItemCategoryRequest request)
        {
            return;
        }

        var action = editingItemCategory is null ? "create" : "update";
        var confirmed = await DialogService.Confirm($"Are you sure you want to {action} this item category?", "Confirm", new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
        if (confirmed != true)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Cancelled", "Item category save operation cancelled.");
            return;
        }

        try
        {
            var success = editingItemCategory is null
                ? await CreateItemCategoryAsync(request)
                : await ItemCategoryManagementService.UpdateAsync(editingItemCategory.Id, MapToUpdateRequest(editingItemCategory.Id, request), ViewerTenantId);

            if (!success)
            {
                NotifyTopRight(NotificationSeverity.Error, "Failed", $"Unable to {action} item category.");
                return;
            }

            await LoadItemCategoriesAsync();
            NotifyTopRight(NotificationSeverity.Success, "Success", editingItemCategory is null ? "Item category created successfully." : "Item category updated successfully.");
        }
        catch (Exception ex)
        {
            NotifyTopRight(NotificationSeverity.Error, "Failed", ex.Message);
        }
    }

    private async Task<bool> CreateItemCategoryAsync(CreateInvItemCategoryRequest request)
    {
        await ItemCategoryManagementService.CreateAsync(request, ViewerTenantId);
        return true;
    }

    private static UpdateInvItemCategoryRequest MapToUpdateRequest(int id, CreateInvItemCategoryRequest request)
        => new()
        {
            Id = id,
            TenantId = request.TenantId,
            Code = request.Code,
            Name = request.Name,
            NameAr = request.NameAr,
            IsActive = request.IsActive
        };

    private async Task LoadAsync()
    {
        ViewerTenantId = await ItemCategoryManagementService.GetViewerTenantIdAsync();
        var permissions = await CrudPermissionService.GetPermissionsAsync("item_category");
        CanCreateItemCategory = permissions.CanCreate;
        CanEditItemCategory = permissions.CanEdit;
        CanDeleteItemCategory = permissions.CanDelete;

        Tenants = await ItemCategoryManagementService.GetTenantsAsync(ViewerTenantId);
        await LoadItemCategoriesAsync();
    }

    private async Task LoadItemCategoriesAsync()
    {
        ErrorMessage = null;
        try
        {
            ItemCategoriesList = await ItemCategoryManagementService.GetItemCategoriesAsync(ViewerTenantId);
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
