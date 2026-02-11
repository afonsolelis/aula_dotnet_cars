using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Volkswagen.Dashboard.Repository.Documents
{
    public class EmailWhitelistDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
