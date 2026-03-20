using MongoDB.Bson;
using MongoDB.Driver;
using Volkswagen.Dashboard.Domain.Repositories;
using Volkswagen.Dashboard.Domain.Users;
using Volkswagen.Dashboard.Repository.Documents;

namespace Volkswagen.Dashboard.Repository;

public sealed class UserRepository : IUserRepository
{
    private readonly IMongoCollection<UserDocument> _users;

    public UserRepository(IMongoDatabase database)
    {
        _users = database.GetCollection<UserDocument>("users");
    }

    public async Task<bool> ExistsByEmailAsync(EmailAddress email)
    {
        var count = await _users.CountDocumentsAsync(x => x.Credentials.Email == email.Value);
        return count > 0;
    }

    public async Task<User?> GetByEmailAsync(EmailAddress email)
    {
        var user = await _users.Find(x => x.Credentials.Email == email.Value).FirstOrDefaultAsync();
        return user is null ? null : ToDomain(user);
    }

    public async Task AddAsync(User user)
    {
        await _users.InsertOneAsync(ToDocument(user));
    }

    private static User ToDomain(UserDocument document)
    {
        var whitelistEntryId = document.WhitelistEntryId?.ToString();
        return User.Register(
            document.Profile.Username,
            new EmailAddress(document.Credentials.Email),
            document.Credentials.PasswordHash,
            document.CreatedAt,
            whitelistEntryId);
    }

    private static UserDocument ToDocument(User user)
    {
        ObjectId? whitelistEntryId = string.IsNullOrWhiteSpace(user.WhitelistEntryId)
            ? null
            : ObjectId.Parse(user.WhitelistEntryId);

        return new UserDocument
        {
            Profile = new UserProfile { Username = user.Username },
            Credentials = new UserCredentials
            {
                Email = user.Email.Value,
                PasswordHash = user.PasswordHash
            },
            WhitelistEntryId = whitelistEntryId,
            CreatedAt = user.CreatedAt
        };
    }
}
