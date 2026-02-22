using Domain.InvReqDTO;
using Microsoft.AspNetCore.Components;
using Radzen;
using SMERPWeb.Services.Auth;
using SMERPWeb.Services.InventoryServices;

namespace SMERPWeb.Components.Pages.Inventory;

public partial class Suppliers : ComponentBase
{
    [Inject] private ISupplierManagementService SupplierManagementService { get; set; } = default!;
    [Inject] private ICrudPermissionService CrudPermissionService { get; set; } = default!;
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;

    protected List<Domain.SaasDBModels.InvSupplier> SuppliersList { get; set; } = [];
    protected List<Domain.SaasDBModels.Tenant> Tenants { get; set; } = [];
    protected bool CanCreateSupplier { get; set; }
    protected bool CanEditSupplier { get; set; }
    protected bool CanDeleteSupplier { get; set; }
    protected int ViewerTenantId { get; set; }
    protected string? ErrorMessage { get; set; }
    protected string SearchText { get; set; } = string.Empty;

    protected IEnumerable<Domain.SaasDBModels.InvSupplier> FilteredSuppliers =>
        string.IsNullOrWhiteSpace(SearchText)
            ? SuppliersList
            : SuppliersList.Where(supplier =>
                Contains(supplier.Code, SearchText) ||
                Contains(supplier.Name, SearchText) ||
                Contains(supplier.ContactPerson, SearchText) ||
                Contains(supplier.Phone, SearchText));

    protected override async Task OnInitializedAsync() => await LoadAsync();

    protected async Task OpenCreateDialogAsync()
    {
        if (!CanCreateSupplier)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Access denied", "You do not have permission to create suppliers.");
            return;
        }

        await OpenDialogAsync(null);
    }

    protected async Task OpenEditDialogAsync(Domain.SaasDBModels.InvSupplier supplier)
    {
        if (!CanEditSupplier)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Access denied", "You do not have permission to edit suppliers.");
            return;
        }

        await OpenDialogAsync(supplier);
    }

    protected async Task DeleteAsync(int id)
    {
        if (!CanDeleteSupplier)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Access denied", "You do not have permission to delete suppliers.");
            return;
        }

        var confirmed = await DialogService.Confirm("Are you sure you want to delete this supplier?", "Confirm", new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
        if (confirmed != true)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Cancelled", "Supplier delete operation cancelled.");
            return;
        }

        var deleted = await SupplierManagementService.DeleteAsync(id, ViewerTenantId);
        if (!deleted)
        {
            NotifyTopRight(NotificationSeverity.Error, "Failed", "Unable to delete supplier.");
            return;
        }

        await LoadSuppliersAsync();
        NotifyTopRight(NotificationSeverity.Success, "Success", "Supplier deleted successfully.");
    }

    protected string GetTenantName(int tenantId) => Tenants.FirstOrDefault(t => t.Id == tenantId)?.Name ?? "Unknown Tenant";

    private async Task OpenDialogAsync(Domain.SaasDBModels.InvSupplier? editingSupplier)
    {
        var result = await DialogService.OpenAsync<SupplierDialog>(editingSupplier is null ? "Create Supplier" : "Edit Supplier", new Dictionary<string, object>
        {
            [nameof(SupplierDialog.EditingSupplier)] = editingSupplier,
            [nameof(SupplierDialog.ViewerTenantId)] = ViewerTenantId,
            [nameof(SupplierDialog.Tenants)] = Tenants
        }, new DialogOptions { Width = "650px", Draggable = true, Resizable = true, CloseDialogOnEsc = true });

        if (result is not CreateInvSupplierRequest request)
        {
            return;
        }

        var action = editingSupplier is null ? "create" : "update";
        var confirmed = await DialogService.Confirm($"Are you sure you want to {action} this supplier?", "Confirm", new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });
        if (confirmed != true)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Cancelled", "Supplier save operation cancelled.");
            return;
        }

        try
        {
            var success = editingSupplier is null
                ? await CreateSupplierAsync(request)
                : await SupplierManagementService.UpdateAsync(editingSupplier.Id, MapToUpdateRequest(editingSupplier.Id, request), ViewerTenantId);

            if (!success)
            {
                NotifyTopRight(NotificationSeverity.Error, "Failed", $"Unable to {action} supplier.");
                return;
            }

            await LoadSuppliersAsync();
            NotifyTopRight(NotificationSeverity.Success, "Success", editingSupplier is null ? "Supplier created successfully." : "Supplier updated successfully.");
        }
        catch (Exception ex)
        {
            NotifyTopRight(NotificationSeverity.Error, "Failed", ex.Message);
        }
    }

    private async Task<bool> CreateSupplierAsync(CreateInvSupplierRequest request)
    {
        await SupplierManagementService.CreateAsync(request, ViewerTenantId);
        return true;
    }

    private static UpdateInvSupplierRequest MapToUpdateRequest(int id, CreateInvSupplierRequest request)
        => new()
        {
            Id = id,
            TenantId = request.TenantId,
            Code = request.Code,
            Name = request.Name,
            NameAr = request.NameAr,
            ContactPerson = request.ContactPerson,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            AddressAr = request.AddressAr,
            CountryCode = request.CountryCode,
            VatregistrationNo = request.VatregistrationNo,
            Crno = request.Crno,
            PaymentTermsDays = request.PaymentTermsDays,
            IsActive = request.IsActive
        };

    private async Task LoadAsync()
    {
        ViewerTenantId = await SupplierManagementService.GetViewerTenantIdAsync();
        var permissions = await CrudPermissionService.GetPermissionsAsync("supplier");
        CanCreateSupplier = permissions.CanCreate;
        CanEditSupplier = permissions.CanEdit;
        CanDeleteSupplier = permissions.CanDelete;

        Tenants = await SupplierManagementService.GetTenantsAsync(ViewerTenantId);
        await LoadSuppliersAsync();
    }

    private async Task LoadSuppliersAsync()
    {
        ErrorMessage = null;
        try
        {
            SuppliersList = await SupplierManagementService.GetSuppliersAsync(ViewerTenantId);
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
