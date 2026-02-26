using Radzen;
using SMERPWeb.Client.Pages;
using Shared.Services.Auth;
using SMERPWeb.Components;
using Shared.Services.SaasServices;
using Shared.Services.Navigation;
using Shared.Services.InventoryServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();


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
AddSaasClient<IWarehouseApiClient, WarehouseApiClient>();
builder.Services.AddScoped<IWarehouseManagementService, WarehouseManagementService>();
AddSaasClient<IInvUomApiClient, InvUomApiClient>();
AddSaasClient<IInvItemApiClient, InvItemApiClient>();
builder.Services.AddScoped<IUomManagementService, UomManagementService>();
AddSaasClient<IItemUomConversionApiClient, ItemUomConversionApiClient>();
builder.Services.AddScoped<IItemUomConversionManagementService, ItemUomConversionManagementService>();
builder.Services.AddHttpClient<IAuthApiClient, AuthApiClient>(ConfigureSaasApiClient);
builder.Services.AddScoped<IUserSessionService, UserSessionService>();
builder.Services.AddScoped<INavigationCatalogService, NavigationCatalogService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(SMERPWeb.Client._Imports).Assembly);

app.Run();
