using BlazorServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace BlazorServer.Security;

public class JWTAuthenticationHandler : AuthenticationHandler<CustomOption>
{
    public JWTAuthenticationHandler(IOptionsMonitor<CustomOption> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            var token = Request.Cookies[AccessTokenService.TOKEN_KEY];
            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.NoResult();
            }

            var readJWT = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var identity = new ClaimsIdentity(readJWT.Claims, "JWT");
            var prinicipal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(prinicipal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
        catch (Exception)
        {
            return AuthenticateResult.NoResult();
        }
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Redirect("/login");
        return Task.CompletedTask;  
    }

    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.Redirect("/access-denied");
        return Task.CompletedTask;
    }
}

public class CustomOption : AuthenticationSchemeOptions
{

}
