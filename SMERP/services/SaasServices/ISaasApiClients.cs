using Domain.SaasDBModels;

namespace SMERP.services.SaasServices;

public interface ISaasApiClients
{
    ICrudApiClient<Tenant> Tenants { get; }
    ICrudApiClient<TenantSetting> TenantSettings { get; }
    ICrudApiClient<Product> Products { get; }
    ICrudApiClient<Uom> Uoms { get; }
    ICrudApiClient<RolePermission> RolePermissions { get; }
    ICrudApiClient<PosTerminal> PosTerminals { get; }
    ICrudApiClient<Permission> Permissions { get; }
    ICrudApiClient<User> Users { get; }
    ICrudApiClient<ProductUom> ProductUoms { get; }
    ICrudApiClient<UserRole> UserRoles { get; }
    ICrudApiClient<Branch> Branches { get; }
    ICrudApiClient<AuditLog> AuditLogs { get; }
    ICrudApiClient<ProductPrice> ProductPrices { get; }
    ICrudApiClient<TaxCode> TaxCodes { get; }
    ICrudApiClient<Role> Roles { get; }
    ICrudApiClient<RefreshToken> RefreshTokens { get; }
    ICrudApiClient<UserBranch> UserBranches { get; }
}
