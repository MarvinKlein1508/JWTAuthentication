namespace BlazorServer.Services;

public class AccessTokenService
{
    private readonly CookieService _cookieService;
    public const string TOKEN_KEY = "access_token";
    public AccessTokenService(CookieService cookieService)
    {
        _cookieService = cookieService;
    }

    public async Task SetToken(string accessToken)
    {
        await _cookieService.Set(TOKEN_KEY, accessToken, 1);
    }

    public async Task<string> GetToken()
    {
        return await _cookieService.Get(TOKEN_KEY);
    }

    public async Task RemoveToken()
    {
        await _cookieService.Remove(TOKEN_KEY);
    }
}
