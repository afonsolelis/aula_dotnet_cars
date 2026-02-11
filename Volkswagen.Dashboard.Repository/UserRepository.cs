using MongoDB.Bson;
using MongoDB.Driver;
using Volkswagen.Dashboard.Repository.Documents;

namespace Volkswagen.Dashboard.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserDocument> _users;
        private readonly IMongoCollection<EmailWhitelistDocument> _whitelist;

        public UserRepository(IMongoDatabase database)
        {
            _users = database.GetCollection<UserDocument>("users");
            _whitelist = database.GetCollection<EmailWhitelistDocument>("email_whitelist");
        }

        public async Task<bool> ExistWithEmail(string email)
        {
            var count = await _users.CountDocumentsAsync(x => x.Credentials.Email == email);
            return count > 0;
        }

        public async Task<UserModel?> GetUserByEmail(string email)
        {
            var user = await _users.Find(x => x.Credentials.Email == email).FirstOrDefaultAsync();
            if (user is null)
            {
                return null;
            }

            return new UserModel
            {
                Username = user.Profile.Username,
                Email = user.Credentials.Email,
                Password = user.Credentials.PasswordHash
            };
        }

        public async Task InsertUser(string email, string username, string password)
        {
            var whitelistEntry = await _whitelist.Find(x => x.Email == email).FirstOrDefaultAsync();

            var user = new UserDocument
            {
                Profile = new UserProfile { Username = username },
                Credentials = new UserCredentials
                {
                    Email = email,
                    PasswordHash = password
                },
                WhitelistEntryId = whitelistEntry?.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _users.InsertOneAsync(user);
        }

        public async Task<bool> IsEmailInWhitelist(string email)
        {
            var count = await _whitelist.CountDocumentsAsync(x => x.Email == email);
            return count > 0;
        }
    }
}
