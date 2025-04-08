using BlazorServer.DTO;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorServer.Services;

public class AuthService
{
    private readonly AccessTokenService _accessTokenService;
    private readonly NavigationManager _navigationManager;
    private readonly HttpClient _client;

    public AuthService(AccessTokenService accessTokenService, NavigationManager navigationManager, IHttpClientFactory httpClientFactory)
    {
        _accessTokenService = accessTokenService;
        _navigationManager = navigationManager;
        _client = httpClientFactory.CreateClient("ApiClient");
    }

    public async Task<bool> Login(string email, string password)
    {
        var status = await _client.PostAsJsonAsync("auth", new { email, password });

        if(status.IsSuccessStatusCode)
        {
            var token = await status.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthResponse>(token)!;
            await _accessTokenService.SetToken(result.AccessToken);
            return true;
        }
        else
        {
            return false;
        }
    }
}
