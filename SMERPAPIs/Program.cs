using Domain.SaasDBModels;
using Microsoft.EntityFrameworkCore;
using SMERPAPIs.Services.SaasServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<SmerpContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SmerpConnection")));

void ConfigureSaasApiClient(HttpClient client)
{
    var baseUrl = builder.Configuration["SaasApi:BaseUrl"];
    if (!string.IsNullOrWhiteSpace(baseUrl))
    {
        client.BaseAddress = new Uri(baseUrl);
    }
}

builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IPosTerminalService, PosTerminalService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductPriceService, ProductPriceService>();
builder.Services.AddScoped<IProductUomService, ProductUomService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<ITaxCodeService, TaxCodeService>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ITenantSettingService, TenantSettingService>();
builder.Services.AddScoped<IUomService, UomService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserBranchService, UserBranchService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();

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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
