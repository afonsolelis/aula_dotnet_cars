using System.Net.Http.Json;
using Volkswagen.Dashboard.Web.Models;

namespace Volkswagen.Dashboard.Web.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/user/login", request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }

        return null;
    }

    public async Task<bool> RegisterAsync(string username, string email, string password)
    {
        var request = new { Username = username, Email = email, Password = password };
        var response = await _httpClient.PostAsJsonAsync("api/user/register", request);
        return response.IsSuccessStatusCode;
    }
}
