using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace SMERPWeb.Components.Pages.Inventory;

public partial class ItemSubCategoryDialog : ComponentBase
{
    [Parameter] public InvItemSubCategory? EditingItemSubCategory { get; set; }
    [Parameter] public int ViewerTenantId { get; set; }
    [Parameter] public List<Tenant> Tenants { get; set; } = [];
    [Parameter] public List<InvItemCategory> ItemCategories { get; set; } = [];

    [Inject] private DialogService DialogService { get; set; } = default!;

    protected CreateInvItemSubCategoryRequest FormModel { get; set; } = new();

    protected IEnumerable<InvItemCategory> FilteredCategories =>
        FormModel.TenantId <= 0 ? [] : ItemCategories.Where(category => category.TenantId == FormModel.TenantId || FormModel.TenantId <= 1);

    protected override void OnInitialized()
    {
        FormModel = EditingItemSubCategory is null
            ? new CreateInvItemSubCategoryRequest
            {
                TenantId = ViewerTenantId <= 1 ? Tenants.FirstOrDefault()?.Id ?? 0 : ViewerTenantId,
                IsActive = true
            }
            : new CreateInvItemSubCategoryRequest
            {
                TenantId = EditingItemSubCategory.TenantId,
                CategoryId = EditingItemSubCategory.CategoryId,
                Code = EditingItemSubCategory.Code,
                Name = EditingItemSubCategory.Name,
                NameAr = EditingItemSubCategory.NameAr,
                IsActive = EditingItemSubCategory.IsActive
            };

        EnsureValidCategorySelection();
    }

    protected void HandleSubmit(CreateInvItemSubCategoryRequest _)
    {
        if (ViewerTenantId > 1)
        {
            FormModel.TenantId = ViewerTenantId;
        }

        if (FormModel.TenantId <= 0 || FormModel.CategoryId <= 0)
        {
            return;
        }

        FormModel.Code = FormModel.Code?.Trim() ?? string.Empty;
        FormModel.Name = FormModel.Name?.Trim() ?? string.Empty;

        DialogService.Close(FormModel);
    }

    protected void Cancel() => DialogService.Close();

    protected void OnTenantChanged(object _)
    {
        EnsureValidCategorySelection();
    }

    private void EnsureValidCategorySelection()
    {
        var availableCategoryIds = FilteredCategories.Select(category => category.Id).ToHashSet();
        if (!availableCategoryIds.Contains(FormModel.CategoryId))
        {
            FormModel.CategoryId = availableCategoryIds.FirstOrDefault();
        }
    }
}
