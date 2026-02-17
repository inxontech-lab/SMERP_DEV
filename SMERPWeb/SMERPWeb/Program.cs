using Radzen;
using SMERPWeb.Client.Pages;
using SMERPWeb.Components;
using SMERPWeb.Services.SaasServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddRadzenComponents();

void ConfigureSaasApiClient(HttpClient client)
{
    var baseUrl = builder.Configuration["SaasApi:BaseUrl"];
    client.BaseAddress = !string.IsNullOrWhiteSpace(baseUrl)
        ? new Uri(baseUrl)
        : new Uri("https://localhost:7029/");
}

builder.Services.AddHttpClient<ITenantApiClient, TenantApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<ITenantManagementApiClient, TenantManagementApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<ITenantSettingManagementApiClient, TenantSettingManagementApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<IBranchManagementApiClient, BranchManagementApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<IRoleManagementApiClient, RoleManagementApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<IPosTerminalManagementApiClient, PosTerminalManagementApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<IRolePermissionManagementApiClient, RolePermissionManagementApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<IRoleApiClient, RoleApiClient>(ConfigureSaasApiClient);
builder.Services.AddHttpClient<IUserOnboardingApiClient, UserOnboardingApiClient>(ConfigureSaasApiClient);
builder.Services.AddScoped<IUserOnboardingService, UserOnboardingService>();
builder.Services.AddHttpClient<IUserManagementApiClient, UserManagementApiClient>(ConfigureSaasApiClient);
builder.Services.AddScoped<IUserManagementService, UserManagementService>();

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
