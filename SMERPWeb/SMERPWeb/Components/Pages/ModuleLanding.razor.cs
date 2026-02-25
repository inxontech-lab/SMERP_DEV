using Microsoft.AspNetCore.Components;
using Shared.Services.Auth;
using Shared.Services.Navigation;

namespace SMERPWeb.Components.Pages;

public partial class ModuleLanding
{
    [Inject] private IUserSessionService UserSessionService { get; set; } = default!;
    [Inject] private INavigationCatalogService NavigationCatalogService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private bool IsLoading { get; set; } = true;
    private string? ErrorMessage { get; set; }
    private List<ModuleCardDefinition> Modules { get; set; } = [];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        var session = await UserSessionService.GetSessionAsync();
        if (session is null)
        {
            NavigationManager.NavigateTo("/", true);
            return;
        }

        try
        {
            var snapshot = await NavigationCatalogService.BuildSnapshotAsync(session);
            Modules = snapshot.Modules.ToList();
        }
        catch (Exception)
        {
            ErrorMessage = "Unable to load modules for your account.";
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private async Task SelectModule(ModuleCardDefinition module)
    {
        await NavigationCatalogService.SetSelectedModuleAsync(module.Key);
        NavigationManager.NavigateTo(module.DefaultPath, true);
    }
}
