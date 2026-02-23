using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using SMERPUI;
using SMERPUI.Services.Auth;
using SMERPUI.Services.InventoryServices;
using SMERPUI.Services.Navigation;
using SMERPUI.Services.SaasServices;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddRadzenComponents();
builder.Services.AddScoped<ICrudPermissionService, CrudPermissionService>();
builder.Services.AddScoped<CrudPermissionHandler>();

void ConfigureSaasApiClient(HttpClient client)
{
    var baseUrl = builder.Configuration["SaasApi:BaseUrl"];
    client.BaseAddress = !string.IsNullOrWhiteSpace(baseUrl)
        ? new Uri(baseUrl)
        : new Uri("https://localhost:7029/");
}

void AddSaasClient<TClient, TImplementation>(bool applyCrudPermissionHandler = true)
    where TClient : class
    where TImplementation : class, TClient
{
    var httpClientBuilder = builder.Services.AddHttpClient<TClient, TImplementation>(ConfigureSaasApiClient);

    if (applyCrudPermissionHandler)
    {
        httpClientBuilder.AddHttpMessageHandler<CrudPermissionHandler>();
    }
}

AddSaasClient<ITenantApiClient, TenantApiClient>();
AddSaasClient<ITenantManagementApiClient, TenantManagementApiClient>();
AddSaasClient<ITenantSettingManagementApiClient, TenantSettingManagementApiClient>();
AddSaasClient<IBranchManagementApiClient, BranchManagementApiClient>();
AddSaasClient<IRoleManagementApiClient, RoleManagementApiClient>();
AddSaasClient<IPosTerminalManagementApiClient, PosTerminalManagementApiClient>();
AddSaasClient<IRolePermissionManagementApiClient, RolePermissionManagementApiClient>();
AddSaasClient<IPermissionApiClient, PermissionApiClient>(applyCrudPermissionHandler: false);
AddSaasClient<IModuleApiClient, ModuleApiClient>();
AddSaasClient<IMenuApiClient, MenuApiClient>();
AddSaasClient<IUserRoleApiClient, UserRoleApiClient>(applyCrudPermissionHandler: false);
AddSaasClient<IRolePermissionApiClient, RolePermissionApiClient>(applyCrudPermissionHandler: false);
AddSaasClient<IRoleApiClient, RoleApiClient>();
AddSaasClient<IUomApiClient, UomApiClient>();
AddSaasClient<IUserOnboardingApiClient, UserOnboardingApiClient>();
builder.Services.AddScoped<IUserOnboardingService, UserOnboardingService>();
AddSaasClient<IUserManagementApiClient, UserManagementApiClient>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
AddSaasClient<ISupplierApiClient, SupplierApiClient>();
builder.Services.AddScoped<ISupplierManagementService, SupplierManagementService>();
builder.Services.AddHttpClient<IAuthApiClient, AuthApiClient>(ConfigureSaasApiClient);
builder.Services.AddScoped<IUserSessionService, UserSessionService>();
builder.Services.AddScoped<INavigationCatalogService, NavigationCatalogService>();

await builder.Build().RunAsync();
