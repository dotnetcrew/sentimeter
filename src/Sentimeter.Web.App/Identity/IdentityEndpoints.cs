using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Sentimeter.Web.App.Identity;

internal static class IdentityEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(
        this IEndpointRouteBuilder builder)
    {
        var identityGroup = builder.MapGroup("/authentication");

        identityGroup.MapGet("/login", Login).AllowAnonymous();
        identityGroup.MapPost("/logout", Logout);

        return builder;
    }

    static ChallengeHttpResult Login() =>
        TypedResults.Challenge(properties: new AuthenticationProperties
        {
            RedirectUri = "/"
        });

    static SignOutHttpResult Logout() =>
        TypedResults.SignOut(properties: new AuthenticationProperties
        {
            RedirectUri = "/"
        },
        [
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme
        ]);
}

