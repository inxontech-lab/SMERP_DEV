using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace SMERPWeb.Components.Pages.Inventory;

public partial class ItemUomConversionDialog : ComponentBase
{
    [Parameter] public InvItemUomconversion? EditingConversion { get; set; }
    [Parameter] public int ViewerTenantId { get; set; }
    [Parameter] public List<Tenant> Tenants { get; set; } = [];
    [Parameter] public List<InvItem> Items { get; set; } = [];
    [Parameter] public List<InvUom> Uoms { get; set; } = [];

    [Inject] private DialogService DialogService { get; set; } = default!;

    protected CreateInvItemUomconversionRequest FormModel { get; set; } = new();
    protected IEnumerable<LookupOption> ItemOptions => Items.Select(item => new LookupOption(item.Id, $"{item.Code} - {item.Name}"));
    protected IEnumerable<LookupOption> UomOptions => Uoms.Select(uom => new LookupOption(uom.Id, $"{uom.Code} - {uom.Name}"));

    protected override void OnInitialized()
    {
        FormModel = EditingConversion is null
            ? new CreateInvItemUomconversionRequest
            {
                TenantId = ViewerTenantId <= 1 ? Tenants.FirstOrDefault()?.Id ?? 0 : ViewerTenantId,
                Factor = 1
            }
            : new CreateInvItemUomconversionRequest
            {
                TenantId = EditingConversion.TenantId,
                ItemId = EditingConversion.ItemId,
                FromUomid = EditingConversion.FromUomid,
                ToUomid = EditingConversion.ToUomid,
                Factor = EditingConversion.Factor
            };
    }

    protected void HandleSubmit(CreateInvItemUomconversionRequest _)
    {
        if (ViewerTenantId > 1)
        {
            FormModel.TenantId = ViewerTenantId;
        }

        if (FormModel.TenantId <= 0 || FormModel.ItemId <= 0 || FormModel.FromUomid <= 0 || FormModel.ToUomid <= 0 || FormModel.Factor <= 0)
        {
            return;
        }

        DialogService.Close(FormModel);
    }

    protected void Cancel() => DialogService.Close();

    protected sealed record LookupOption(int Id, string DisplayName);
}
