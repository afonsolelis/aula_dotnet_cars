using Volkswagen.Dashboard.Domain.Users;
using Volkswagen.Dashboard.Services.Auth;

namespace Volkswagen.Dashboard.Services.Security;

public interface ITokenService
{
    LoginResponse GenerateToken(User user);
}
