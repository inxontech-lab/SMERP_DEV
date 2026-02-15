using Domain.SaasDBModels;

namespace SMERP.services.SaasServices;

public sealed class SaasApiClients : ISaasApiClients
{
    public SaasApiClients(HttpClient httpClient)
    {
        Tenants = new CrudApiClient<Tenant>(httpClient, "Tenants");
        TenantSettings = new CrudApiClient<TenantSetting>(httpClient, "TenantSettings");
        Products = new CrudApiClient<Product>(httpClient, "Products");
        Uoms = new CrudApiClient<Uom>(httpClient, "Uoms");
        RolePermissions = new CrudApiClient<RolePermission>(httpClient, "RolePermissions");
        PosTerminals = new CrudApiClient<PosTerminal>(httpClient, "PosTerminals");
        Permissions = new CrudApiClient<Permission>(httpClient, "Permissions");
        Users = new CrudApiClient<User>(httpClient, "Users");
        ProductUoms = new CrudApiClient<ProductUom>(httpClient, "ProductUoms");
        UserRoles = new CrudApiClient<UserRole>(httpClient, "UserRoles");
        Branches = new CrudApiClient<Branch>(httpClient, "Branches");
        AuditLogs = new CrudApiClient<AuditLog>(httpClient, "AuditLogs");
        ProductPrices = new CrudApiClient<ProductPrice>(httpClient, "ProductPrices");
        TaxCodes = new CrudApiClient<TaxCode>(httpClient, "TaxCodes");
        Roles = new CrudApiClient<Role>(httpClient, "Roles");
        RefreshTokens = new CrudApiClient<RefreshToken>(httpClient, "RefreshTokens");
        UserBranches = new CrudApiClient<UserBranch>(httpClient, "UserBranches");
    }

    public ICrudApiClient<Tenant> Tenants { get; }
    public ICrudApiClient<TenantSetting> TenantSettings { get; }
    public ICrudApiClient<Product> Products { get; }
    public ICrudApiClient<Uom> Uoms { get; }
    public ICrudApiClient<RolePermission> RolePermissions { get; }
    public ICrudApiClient<PosTerminal> PosTerminals { get; }
    public ICrudApiClient<Permission> Permissions { get; }
    public ICrudApiClient<User> Users { get; }
    public ICrudApiClient<ProductUom> ProductUoms { get; }
    public ICrudApiClient<UserRole> UserRoles { get; }
    public ICrudApiClient<Branch> Branches { get; }
    public ICrudApiClient<AuditLog> AuditLogs { get; }
    public ICrudApiClient<ProductPrice> ProductPrices { get; }
    public ICrudApiClient<TaxCode> TaxCodes { get; }
    public ICrudApiClient<Role> Roles { get; }
    public ICrudApiClient<RefreshToken> RefreshTokens { get; }
    public ICrudApiClient<UserBranch> UserBranches { get; }
}
