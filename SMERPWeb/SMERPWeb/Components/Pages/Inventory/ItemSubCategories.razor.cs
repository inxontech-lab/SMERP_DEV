using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Microsoft.AspNetCore.Components;
using Radzen;
using Shared.Services.Auth;
using Shared.Services.InventoryServices;

namespace SMERPWeb.Components.Pages.Inventory;

public partial class ItemSubCategories : ComponentBase
{
    [Inject] private IInvItemSubCategoryManagementService ItemSubCategoryManagementService { get; set; } = default!;
    [Inject] private ICrudPermissionService CrudPermissionService { get; set; } = default!;
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;

    protected List<InvItemSubCategory> ItemSubCategoriesList { get; set; } = [];
    protected List<InvItemCategory> ItemCategories { get; set; } = [];
    protected List<Tenant> Tenants { get; set; } = [];
    protected bool CanCreateItemSubCategory { get; set; }
    protected bool CanEditItemSubCategory { get; set; }
    protected bool CanDeleteItemSubCategory { get; set; }
    protected int ViewerTenantId { get; set; }
    protected string? ErrorMessage { get; set; }
    protected string SearchText { get; set; } = string.Empty;

    protected IEnumerable<InvItemSubCategory> FilteredItemSubCategories =>
        string.IsNullOrWhiteSpace(SearchText)
            ? ItemSubCategoriesList
            : ItemSubCategoriesList.Where(x =>
                Contains(x.Code, SearchText) ||
                Contains(x.Name, SearchText) ||
                Contains(x.NameAr, SearchText));

    protected override async Task OnInitializedAsync() => await LoadAsync();

    protected async Task OpenCreateDialogAsync()
    {
        if (!CanCreateItemSubCategory)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Access denied", "You do not have permission to create item sub categories.");
            return;
        }

        await OpenDialogAsync(null);
    }

    protected async Task OpenEditDialogAsync(InvItemSubCategory itemSubCategory)
    {
        if (!CanEditItemSubCategory)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Access denied", "You do not have permission to edit item sub categories.");
            return;
        }

        await OpenDialogAsync(itemSubCategory);
    }

    protected async Task DeleteAsync(int id)
    {
        if (!CanDeleteItemSubCategory)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Access denied", "You do not have permission to delete item sub categories.");
            return;
        }

        var confirmed = await DialogService.Confirm("Are you sure you want to delete this item sub category?", "Confirm", new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
        if (confirmed != true)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Cancelled", "Item sub category delete operation cancelled.");
            return;
        }

        var deleted = await ItemSubCategoryManagementService.DeleteAsync(id, ViewerTenantId);
        if (!deleted)
        {
            NotifyTopRight(NotificationSeverity.Error, "Failed", "Unable to delete item sub category.");
            return;
        }

        await LoadItemSubCategoriesAsync();
        NotifyTopRight(NotificationSeverity.Success, "Success", "Item sub category deleted successfully.");
    }

    protected string GetTenantName(int tenantId) => Tenants.FirstOrDefault(t => t.Id == tenantId)?.Name ?? "Unknown Tenant";

    protected string GetCategoryName(int categoryId) => ItemCategories.FirstOrDefault(c => c.Id == categoryId)?.Name ?? $"Category #{categoryId}";

    private async Task OpenDialogAsync(InvItemSubCategory? editingItemSubCategory)
    {
        var result = await DialogService.OpenAsync<ItemSubCategoryDialog>(editingItemSubCategory is null ? "Create Item Sub Category" : "Edit Item Sub Category", new Dictionary<string, object>
        {
            [nameof(ItemSubCategoryDialog.EditingItemSubCategory)] = editingItemSubCategory,
            [nameof(ItemSubCategoryDialog.ViewerTenantId)] = ViewerTenantId,
            [nameof(ItemSubCategoryDialog.Tenants)] = Tenants,
            [nameof(ItemSubCategoryDialog.ItemCategories)] = ItemCategories
        }, new DialogOptions { Width = "900px", Draggable = true, Resizable = true, CloseDialogOnEsc = true });

        if (result is not CreateInvItemSubCategoryRequest request)
        {
            return;
        }

        var action = editingItemSubCategory is null ? "create" : "update";
        var confirmed = await DialogService.Confirm($"Are you sure you want to {action} this item sub category?", "Confirm", new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
        if (confirmed != true)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Cancelled", "Item sub category save operation cancelled.");
            return;
        }

        try
        {
            var success = editingItemSubCategory is null
                ? await CreateItemSubCategoryAsync(request)
                : await ItemSubCategoryManagementService.UpdateAsync(editingItemSubCategory.Id, MapToUpdateRequest(editingItemSubCategory.Id, request), ViewerTenantId);

            if (!success)
            {
                NotifyTopRight(NotificationSeverity.Error, "Failed", $"Unable to {action} item sub category.");
                return;
            }

            await LoadItemSubCategoriesAsync();
            NotifyTopRight(NotificationSeverity.Success, "Success", editingItemSubCategory is null ? "Item sub category created successfully." : "Item sub category updated successfully.");
        }
        catch (Exception ex)
        {
            NotifyTopRight(NotificationSeverity.Error, "Failed", ex.Message);
        }
    }

    private async Task<bool> CreateItemSubCategoryAsync(CreateInvItemSubCategoryRequest request)
    {
        await ItemSubCategoryManagementService.CreateAsync(request, ViewerTenantId);
        return true;
    }

    private static UpdateInvItemSubCategoryRequest MapToUpdateRequest(int id, CreateInvItemSubCategoryRequest request)
        => new()
        {
            Id = id,
            TenantId = request.TenantId,
            CategoryId = request.CategoryId,
            Code = request.Code,
            Name = request.Name,
            NameAr = request.NameAr,
            IsActive = request.IsActive
        };

    private async Task LoadAsync()
    {
        ViewerTenantId = await ItemSubCategoryManagementService.GetViewerTenantIdAsync();
        var permissions = await CrudPermissionService.GetPermissionsAsync("item_sub_category");
        CanCreateItemSubCategory = permissions.CanCreate;
        CanEditItemSubCategory = permissions.CanEdit;
        CanDeleteItemSubCategory = permissions.CanDelete;

        Tenants = await ItemSubCategoryManagementService.GetTenantsAsync(ViewerTenantId);
        ItemCategories = await ItemSubCategoryManagementService.GetItemCategoriesAsync(ViewerTenantId);
        await LoadItemSubCategoriesAsync();
    }

    private async Task LoadItemSubCategoriesAsync()
    {
        ErrorMessage = null;
        try
        {
            ItemSubCategoriesList = await ItemSubCategoryManagementService.GetItemSubCategoriesAsync(ViewerTenantId);
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
