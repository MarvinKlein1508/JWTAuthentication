using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace BlazorServer.Services;

public class RefreshTokenService
{
    private readonly ProtectedLocalStorage _protectedLocalStorage;
    private const string KEY = "refresh_token";
    public RefreshTokenService(ProtectedLocalStorage protectedLocalStorage)
    {
        _protectedLocalStorage = protectedLocalStorage;
    }

    public async Task SetToken(string value)
    {
        await _protectedLocalStorage.SetAsync(KEY, value);
    }

    public async Task<string> GetToken()
    {
        var result = await _protectedLocalStorage.GetAsync<string>(KEY);

        if (result.Success)
        {
            return result.Value ?? string.Empty;
        }

        return string.Empty;
    }

    internal async Task Remove()
    {
        await _protectedLocalStorage.DeleteAsync(KEY);
    }
}
