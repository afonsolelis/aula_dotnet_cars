using Volkswagen.Dashboard.Domain.Users;

namespace Volkswagen.Dashboard.Domain.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(EmailAddress email);
    Task<User?> GetByEmailAsync(EmailAddress email);
    Task AddAsync(User user);
}
