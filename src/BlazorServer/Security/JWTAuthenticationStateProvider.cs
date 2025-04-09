using BlazorServer.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlazorServer.Security;

public class JWTAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly AccessTokenService _accessTokenService;

    public JWTAuthenticationStateProvider(AccessTokenService accessTokenService)
    {
        _accessTokenService = accessTokenService;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _accessTokenService.GetToken();

            if (string.IsNullOrEmpty(token))
            {
                return await MarkAsUnauthorized();
            }

            var readJWT = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var identity = new ClaimsIdentity(readJWT.Claims, "JWT");
            var prinicipal = new ClaimsPrincipal(identity);
            return await Task.FromResult(new AuthenticationState(prinicipal));
        }
        catch (Exception)
        {
            return await MarkAsUnauthorized();
        }
    }

    private async Task<AuthenticationState> MarkAsUnauthorized()
    {
        var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        try
        {
            NotifyAuthenticationStateChanged(Task.FromResult(state));
            return state;
        }
        catch (Exception)
        {

            return state;
        }
    }
}
