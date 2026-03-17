namespace Volkswagen.Dashboard.Services.Auth;

public interface IAuthService
{
    Task<LoginResponse> Login(LoginRequest request);
    Task<bool> Register(RegisterRequest request);
}
