using Microsoft.AspNetCore.Components;
using SMERPWeb.Services.Auth;
using SMERPWeb.Services.Navigation;

namespace SMERPWeb.Components.Pages;

public partial class ModuleLanding
{
    [Inject] private IUserSessionService UserSessionService { get; set; } = default!;
    [Inject] private INavigationCatalogService NavigationCatalogService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private bool IsLoading { get; set; } = true;
    private string? ErrorMessage { get; set; }
    private List<ModuleCardDefinition> Modules { get; set; } = [];
    private bool _isInitializing;
    private bool _isInitialized;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_isInitializing || _isInitialized)
        {
            return;
        }

        _isInitializing = true;

        try
        {
            var session = await UserSessionService.GetSessionAsync();
            if (session is null)
            {
                if (firstRender)
                {
                    return;
                }

                NavigationManager.NavigateTo("/", true);
                _isInitialized = true;
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
                _isInitialized = true;
                StateHasChanged();
            }
        }
        finally
        {
            _isInitializing = false;
        }
    }

    private async Task SelectModule(ModuleCardDefinition module)
    {
        await NavigationCatalogService.SetSelectedModuleAsync(module.Key);
        NavigationManager.NavigateTo(module.DefaultPath, true);
    }
}
