using Domain.SaasDBModels;
using Microsoft.EntityFrameworkCore;
using SMERPAPIs.Services.SaasServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<SmerpContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SmerpConnection")));

builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IModuleService, ModuleService>();
builder.Services.AddScoped<IMenuService, MenuService>();
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
builder.Services.AddScoped<IUserOnboardingService, UserOnboardingService>();
builder.Services.AddScoped<IUserBranchService, UserBranchService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();


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
