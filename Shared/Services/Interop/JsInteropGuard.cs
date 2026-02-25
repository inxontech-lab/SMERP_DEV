using Microsoft.JSInterop;

namespace Shared.Services.Interop;

internal static class JsInteropGuard
{
    public static bool IsUnavailable(Exception exception)
    {
        if (exception is JSDisconnectedException)
        {
            return true;
        }

        if (exception is InvalidOperationException invalidOperationException)
        {
            return invalidOperationException.Message.Contains("statically rendered", StringComparison.OrdinalIgnoreCase)
                || invalidOperationException.Message.Contains("prerender", StringComparison.OrdinalIgnoreCase)
                || invalidOperationException.Message.Contains("JavaScript interop calls cannot be issued", StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }
}
