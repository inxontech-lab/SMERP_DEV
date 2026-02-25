using System.Net;
using System.Net.Http;

namespace SMERPWeb.Services.Auth;

public sealed class CrudPermissionHandler(
    ICrudPermissionService crudPermissionService,
    IUserSessionService userSessionService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri is null || request.Method == HttpMethod.Get)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var resourceKey = ResolveResourceKey(request.RequestUri);
        if (string.IsNullOrWhiteSpace(resourceKey))
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var session = await userSessionService.GetSessionAsync();
        if (session is null)
        {
            // The HTTP client handler pipeline can run outside a Blazor circuit scope where JS-backed session data is unavailable.
            // In that case, skip client-side permission short-circuiting and let the API enforce authorization.
            return await base.SendAsync(request, cancellationToken);
        }

        var permissions = await crudPermissionService.GetPermissionsAsync(resourceKey, cancellationToken);
        var isAllowed = request.Method.Method.ToUpperInvariant() switch
        {
            "POST" => permissions.CanCreate,
            "PUT" => permissions.CanEdit,
            "PATCH" => permissions.CanEdit,
            "DELETE" => permissions.CanDelete,
            _ => true
        };

        if (!isAllowed)
        {
            return new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                RequestMessage = request,
                Content = new StringContent($"Missing permission for {request.Method.Method} on {resourceKey}.")
            };
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private static string? ResolveResourceKey(Uri uri)
    {
        var segments = uri.AbsolutePath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length < 2 || !segments[0].Equals("api", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var resource = segments[1].ToLowerInvariant();
        return resource switch
        {
            "tenants" => "tenant",
            "tenantsettings" => "tenant_setting",
            "branches" => "branch",
            "roles" => "role",
            "posterminals" => "pos_terminal",
            "users" => "user",
            "permissions" => "permission",
            "rolepermissions" => "role_permission",
            "userroles" => "user_role",
            "userbranches" => "user_branch",
            _ => resource.EndsWith("s", StringComparison.Ordinal) ? resource[..^1] : resource
        };
    }
}
