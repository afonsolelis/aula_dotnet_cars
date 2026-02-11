using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Volkswagen.Dashboard.Repository.Documents
{
    public class UserDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("profile")]
        public UserProfile Profile { get; set; } = new();

        [BsonElement("credentials")]
        public UserCredentials Credentials { get; set; } = new();

        [BsonElement("whitelistEntryId")]
        [BsonIgnoreIfNull]
        public ObjectId? WhitelistEntryId { get; set; }

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class UserProfile
    {
        [BsonElement("username")]
        public string Username { get; set; } = string.Empty;
    }

    public class UserCredentials
    {
        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; } = string.Empty;
    }
}
