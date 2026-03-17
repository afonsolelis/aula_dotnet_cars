using Volkswagen.Dashboard.Domain.Users;

namespace Volkswagen.Dashboard.Domain.Repositories;

public interface IAuthorizedEmailRepository
{
    Task<AuthorizedEmail?> GetAuthorizedEmailAsync(EmailAddress email);
}
