namespace Volkswagen.Dashboard.Services.Auth;

public interface IRegistrationService
{
    Task<bool> Register(RegisterRequest request);
}
