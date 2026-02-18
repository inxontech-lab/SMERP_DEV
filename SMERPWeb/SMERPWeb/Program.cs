using Radzen;
using SMERPWeb.Client.Pages;
using SMERPWeb.Services.Auth;
using SMERPWeb.Components;
using SMERPWeb.Services.SaasServices;
using SMERPWeb.Services.Navigation;

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

void AddSaasClient<TClient, TImplementation>()
    where TClient : class
    where TImplementation : class, TClient
{
    builder.Services.AddHttpClient<TClient, TImplementation>(ConfigureSaasApiClient)
        .AddHttpMessageHandler<CrudPermissionHandler>();
}

AddSaasClient<ITenantApiClient, TenantApiClient>();
AddSaasClient<ITenantManagementApiClient, TenantManagementApiClient>();
AddSaasClient<ITenantSettingManagementApiClient, TenantSettingManagementApiClient>();
AddSaasClient<IBranchManagementApiClient, BranchManagementApiClient>();
AddSaasClient<IRoleManagementApiClient, RoleManagementApiClient>();
AddSaasClient<IPosTerminalManagementApiClient, PosTerminalManagementApiClient>();
AddSaasClient<IRolePermissionManagementApiClient, RolePermissionManagementApiClient>();
AddSaasClient<IPermissionApiClient, PermissionApiClient>();
AddSaasClient<IModuleApiClient, ModuleApiClient>();
AddSaasClient<IMenuApiClient, MenuApiClient>();
AddSaasClient<IUserRoleApiClient, UserRoleApiClient>();
AddSaasClient<IRolePermissionApiClient, RolePermissionApiClient>();
AddSaasClient<IRoleApiClient, RoleApiClient>();
AddSaasClient<IUserOnboardingApiClient, UserOnboardingApiClient>();
builder.Services.AddScoped<IUserOnboardingService, UserOnboardingService>();
AddSaasClient<IUserManagementApiClient, UserManagementApiClient>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
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
