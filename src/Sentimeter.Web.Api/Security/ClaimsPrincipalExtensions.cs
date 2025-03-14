using System.Security.Claims;

namespace Sentimeter.Web.Api.Security;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal principal)
    {
        if (principal == null)
        {
            throw new ArgumentNullException(nameof(principal));
        }

        return principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }
}
