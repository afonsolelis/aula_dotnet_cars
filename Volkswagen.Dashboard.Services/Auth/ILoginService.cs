namespace Volkswagen.Dashboard.Services.Auth;

public interface ILoginService
{
    Task<LoginResponse> Login(LoginRequest request);
}
