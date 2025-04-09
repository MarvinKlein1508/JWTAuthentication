using Microsoft.AspNetCore.Components;
using System.Text.Json;
using WebApi.Contracts.Responses;

namespace BlazorServer.Services;

public class AuthService
{
    private readonly AccessTokenService _accessTokenService;
    private readonly NavigationManager _navigationManager;
    private readonly RefreshTokenService _refreshTokenService;
    private readonly HttpClient _client;

    public AuthService(AccessTokenService accessTokenService, NavigationManager navigationManager, IHttpClientFactory httpClientFactory, RefreshTokenService refreshTokenService)
    {
        _accessTokenService = accessTokenService;
        _navigationManager = navigationManager;
        _refreshTokenService = refreshTokenService;
        _client = httpClientFactory.CreateClient("ApiClient");
    }

    public async Task<bool> Login(string email, string password)
    {
        var status = await _client.PostAsJsonAsync("auth", new { email, password });

        if (status.IsSuccessStatusCode)
        {
            var token = await status.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthResponse>(token)!;

            await _accessTokenService.RemoveToken();
            await _accessTokenService.SetToken(result.AccessToken);
            await _refreshTokenService.SetToken(result.RefreshToken);
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> RefreshTokenAsync()
    {
        var refreshToken = await _refreshTokenService.GetToken();
        _client.DefaultRequestHeaders.Add("Cookie", $"refreshToken={refreshToken}");
        var responseMessage = await _client.PostAsync("auth/refresh", null);
        if (responseMessage.IsSuccessStatusCode)
        {
            var token = await responseMessage.Content.ReadAsStringAsync();

            if (!string.IsNullOrEmpty(token))
            {
                var result = JsonSerializer.Deserialize<AuthResponse>(token)!;
                await _accessTokenService.SetToken(result.AccessToken);
                await _refreshTokenService.SetToken(result.RefreshToken);
                return true;
            }
        }

        return false;
    }
    public async Task Logout()
    {
        var refreshToken = await _refreshTokenService.GetToken();
        _client.DefaultRequestHeaders.Add("Cookie", $"refreshToken={refreshToken}");
        var responseMessage = await _client.PostAsync("auth/logout", null);
        if (responseMessage.IsSuccessStatusCode)
        {
            await _accessTokenService.RemoveToken();
            await _refreshTokenService.Remove();
            _navigationManager.NavigateTo("/login", forceLoad: true);
        }
    }
}
