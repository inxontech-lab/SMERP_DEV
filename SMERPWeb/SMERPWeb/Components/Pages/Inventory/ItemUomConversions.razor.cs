using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Microsoft.AspNetCore.Components;
using Radzen;
using Shared.Services.Auth;
using Shared.Services.InventoryServices;

namespace SMERPWeb.Components.Pages.Inventory;

public partial class ItemUomConversions : ComponentBase
{
    [Inject] private IItemUomConversionManagementService ItemUomConversionManagementService { get; set; } = default!;
    [Inject] private ICrudPermissionService CrudPermissionService { get; set; } = default!;
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;

    protected List<InvItemUomconversion> ItemUomConversionsList { get; set; } = [];
    protected List<Tenant> Tenants { get; set; } = [];
    protected List<InvItem> Items { get; set; } = [];
    protected List<InvUom> Uoms { get; set; } = [];
    protected bool CanCreateItemUomConversion { get; set; }
    protected bool CanEditItemUomConversion { get; set; }
    protected bool CanDeleteItemUomConversion { get; set; }
    protected int ViewerTenantId { get; set; }
    protected string? ErrorMessage { get; set; }
    protected string SearchText { get; set; } = string.Empty;

    protected IEnumerable<InvItemUomconversion> FilteredItemUomConversions =>
        string.IsNullOrWhiteSpace(SearchText)
            ? ItemUomConversionsList
            : ItemUomConversionsList.Where(x =>
                Contains(x.ItemId.ToString(), SearchText) ||
                Contains(x.FromUomid.ToString(), SearchText) ||
                Contains(x.ToUomid.ToString(), SearchText) ||
                Contains(x.Factor.ToString(), SearchText));

    protected override async Task OnInitializedAsync() => await LoadAsync();

    protected async Task OpenCreateDialogAsync()
    {
        if (!CanCreateItemUomConversion)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Access denied", "You do not have permission to create item UOM conversions.");
            return;
        }

        await OpenDialogAsync(null);
    }

    protected async Task OpenEditDialogAsync(InvItemUomconversion conversion)
    {
        if (!CanEditItemUomConversion)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Access denied", "You do not have permission to edit item UOM conversions.");
            return;
        }

        await OpenDialogAsync(conversion);
    }

    protected async Task DeleteAsync(int id)
    {
        if (!CanDeleteItemUomConversion)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Access denied", "You do not have permission to delete item UOM conversions.");
            return;
        }

        var confirmed = await DialogService.Confirm("Are you sure you want to delete this conversion?", "Confirm", new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
        if (confirmed != true)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Cancelled", "Item UOM conversion delete operation cancelled.");
            return;
        }

        var deleted = await ItemUomConversionManagementService.DeleteAsync(id, ViewerTenantId);
        if (!deleted)
        {
            NotifyTopRight(NotificationSeverity.Error, "Failed", "Unable to delete item UOM conversion.");
            return;
        }

        await LoadItemUomConversionsAsync();
        NotifyTopRight(NotificationSeverity.Success, "Success", "Item UOM conversion deleted successfully.");
    }

    protected string GetTenantName(int tenantId) => Tenants.FirstOrDefault(tenant => tenant.Id == tenantId)?.Name ?? "Unknown Tenant";

    private async Task OpenDialogAsync(InvItemUomconversion? editingConversion)
    {
        var result = await DialogService.OpenAsync<ItemUomConversionDialog>(editingConversion is null ? "Create Conversion" : "Edit Conversion", new Dictionary<string, object>
        {
            [nameof(ItemUomConversionDialog.EditingConversion)] = editingConversion,
            [nameof(ItemUomConversionDialog.ViewerTenantId)] = ViewerTenantId,
            [nameof(ItemUomConversionDialog.Tenants)] = Tenants,
            [nameof(ItemUomConversionDialog.Items)] = Items,
            [nameof(ItemUomConversionDialog.Uoms)] = Uoms
        }, new DialogOptions { Width = "900px", Draggable = true, Resizable = true, CloseDialogOnEsc = true });

        if (result is not CreateInvItemUomconversionRequest request)
        {
            return;
        }

        var action = editingConversion is null ? "create" : "update";
        var confirmed = await DialogService.Confirm($"Are you sure you want to {action} this conversion?", "Confirm", new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
        if (confirmed != true)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Cancelled", "Item UOM conversion save operation cancelled.");
            return;
        }

        try
        {
            var success = editingConversion is null
                ? await CreateConversionAsync(request)
                : await ItemUomConversionManagementService.UpdateAsync(editingConversion.Id, MapToUpdateRequest(editingConversion.Id, request), ViewerTenantId);

            if (!success)
            {
                NotifyTopRight(NotificationSeverity.Error, "Failed", $"Unable to {action} item UOM conversion.");
                return;
            }

            await LoadItemUomConversionsAsync();
            NotifyTopRight(NotificationSeverity.Success, "Success", editingConversion is null ? "Item UOM conversion created successfully." : "Item UOM conversion updated successfully.");
        }
        catch (Exception ex)
        {
            NotifyTopRight(NotificationSeverity.Error, "Failed", ex.Message);
        }
    }

    private async Task<bool> CreateConversionAsync(CreateInvItemUomconversionRequest request)
    {
        await ItemUomConversionManagementService.CreateAsync(request, ViewerTenantId);
        return true;
    }

    private static UpdateInvItemUomconversionRequest MapToUpdateRequest(int id, CreateInvItemUomconversionRequest request)
        => new()
        {
            Id = id,
            TenantId = request.TenantId,
            ItemId = request.ItemId,
            FromUomid = request.FromUomid,
            ToUomid = request.ToUomid,
            Factor = request.Factor
        };

    private async Task LoadAsync()
    {
        ViewerTenantId = await ItemUomConversionManagementService.GetViewerTenantIdAsync();
        var permissions = await CrudPermissionService.GetPermissionsAsync("uom_conversion");
        CanCreateItemUomConversion = permissions.CanCreate;
        CanEditItemUomConversion = permissions.CanEdit;
        CanDeleteItemUomConversion = permissions.CanDelete;

        Tenants = await ItemUomConversionManagementService.GetTenantsAsync(ViewerTenantId);
        Items = await ItemUomConversionManagementService.GetItemsAsync(ViewerTenantId);
        Uoms = await ItemUomConversionManagementService.GetUomsAsync(ViewerTenantId);
        await LoadItemUomConversionsAsync();
    }

    private async Task LoadItemUomConversionsAsync()
    {
        ErrorMessage = null;
        try
        {
            ItemUomConversionsList = await ItemUomConversionManagementService.GetItemUomConversionsAsync(ViewerTenantId);
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
