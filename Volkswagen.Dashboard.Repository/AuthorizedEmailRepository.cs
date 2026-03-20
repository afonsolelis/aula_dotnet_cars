using MongoDB.Driver;
using Volkswagen.Dashboard.Domain.Repositories;
using Volkswagen.Dashboard.Domain.Users;
using Volkswagen.Dashboard.Repository.Documents;

namespace Volkswagen.Dashboard.Repository;

public sealed class AuthorizedEmailRepository : IAuthorizedEmailRepository
{
    private readonly IMongoCollection<EmailWhitelistDocument> _whitelist;

    public AuthorizedEmailRepository(IMongoDatabase database)
    {
        _whitelist = database.GetCollection<EmailWhitelistDocument>("email_whitelist");
    }

    public async Task<AuthorizedEmail?> GetAuthorizedEmailAsync(EmailAddress email)
    {
        var document = await _whitelist.Find(x => x.Email == email.Value).FirstOrDefaultAsync();
        if (document is null)
        {
            return null;
        }

        return new AuthorizedEmail(document.Id.ToString(), new EmailAddress(document.Email), document.CreatedAt);
    }
}
