using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using SMERPUI;
using SMERPUI.Services.Auth;
using SMERPUI.Services.SaasServices;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddRadzenComponents();
builder.Services.AddScoped<IUserSessionService, UserSessionService>();

void ConfigureSaasApiClient(HttpClient client)
{
    var baseUrl = builder.Configuration["SaasApi:BaseUrl"];
    client.BaseAddress = !string.IsNullOrWhiteSpace(baseUrl)
        ? new Uri(baseUrl)
        : new Uri(builder.HostEnvironment.BaseAddress);
}

builder.Services.AddHttpClient<IAuthApiClient, AuthApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<IAuditLogApiClient, AuditLogApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<IBranchApiClient, BranchApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<IPermissionApiClient, PermissionApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<IPosTerminalApiClient, PosTerminalApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<IProductApiClient, ProductApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<IProductPriceApiClient, ProductPriceApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<IProductUomApiClient, ProductUomApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<IRefreshTokenApiClient, RefreshTokenApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<IRoleApiClient, RoleApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<ITaxCodeApiClient, TaxCodeApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<ITenantApiClient, TenantApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<ITenantSettingApiClient, TenantSettingApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<IUomApiClient, UomApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<IUserApiClient, UserApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<IUserBranchApiClient, UserBranchApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<IUserRoleApiClient, UserRoleApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<IRolePermissionApiClient, RolePermissionApiClient>(ConfigureSaasApiClient);

await builder.Build().RunAsync();
