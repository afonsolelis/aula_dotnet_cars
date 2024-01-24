

using Volkswagen.Dashboard.Repository;

namespace Volkswagen.Dashboard.Services
{
    public interface ITokenService
    {
        Task<string> GenerateToken(UserModel user);
    }
}
