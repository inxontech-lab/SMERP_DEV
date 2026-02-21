namespace SMERPAPIs.Services.InventoryServices;

internal static class TenantAccessGuard
{
    public static bool CanAccess(int viewerTenantId, int recordTenantId)
        => viewerTenantId <= 1 || viewerTenantId == recordTenantId;

    public static void EnsureAccess(int viewerTenantId, int requestTenantId, string entityName)
    {
        if (!CanAccess(viewerTenantId, requestTenantId))
        {
            throw new InvalidOperationException($"You can only manage {entityName} for your tenant.");
        }
    }
}
