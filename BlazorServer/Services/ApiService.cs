using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;

namespace BlazorServer.Services;

public class ApiService
{

    private HttpClient _client;
    private readonly AccessTokenService _accessTokenService;
    private readonly AuthService _authService;
    private readonly NavigationManager _navigationManager;

    public ApiService(IHttpClientFactory httpClientFactory,
        AccessTokenService accessTokenService,
        AuthService authService,
        NavigationManager navigationManager)
    {

        _client = httpClientFactory.CreateClient("ApiClient");
        _accessTokenService = accessTokenService;
        _authService = authService;
        _navigationManager = navigationManager;
    }

    public async Task<HttpResponseMessage> GetAsync(string endpoint)
    {
        var token = await _accessTokenService.GetToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var responseMessage = await _client.GetAsync(endpoint);
        if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // call refresh token
            var refreshTokenResult = await _authService.RefreshTokenAsync();
            if (!refreshTokenResult)
            {
                await _authService.Logout();
            }

            var newToken = await _accessTokenService.GetToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newToken);

            var newResponse = await _client.GetAsync(endpoint);
            return newResponse;
        }

        return responseMessage;
    }

    public async Task<HttpResponseMessage> PostDataAsync(string endpoint, object obj)
    {
        var token = await _accessTokenService.GetToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var responseMessage = await _client.PostAsJsonAsync(endpoint, obj);
        if (responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // call refresh token
            var refreshTokenResult = await _authService.RefreshTokenAsync();
            if (!refreshTokenResult)
            {
                await _authService.Logout();
            }

            var newToken = await _accessTokenService.GetToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newToken);

            var newResponse = await _client.PostAsJsonAsync(endpoint, obj);
            return newResponse;
        }

        return responseMessage;
    }
}
