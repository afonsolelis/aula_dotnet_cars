using Volkswagen.Dashboard.Web.Models;

namespace Volkswagen.Dashboard.Web.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<bool> RegisterAsync(string username, string email, string password);
}
